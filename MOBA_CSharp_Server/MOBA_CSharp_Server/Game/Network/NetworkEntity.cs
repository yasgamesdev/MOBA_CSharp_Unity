using ENet;
using MessagePack;
using Microsoft.Xna.Framework;
using MOBA_CSharp_Server.Library.ECS;
using MOBA_CSharp_Server.Library.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOBA_CSharp_Server.Game
{
    public class NetworkEntity : Entity
    {
        enum NetworkState
        {
            Lobby,
            Countdown,
            Battle
        }
        bool testMode;
        NetworkState state;
        float timer;

        ServerNetwork server = new ServerNetwork();
        Dictionary<uint, GameClient> peers = new Dictionary<uint, GameClient>();

        float initGold;

        public NetworkEntity(Entity root) : base(root)
        {
            AddInheritedType(typeof(NetworkEntity));

            testMode = Root.GetChild<DataReaderEntity>().GetYAMLObject(@"YAML\ServerConfig.yml").GetData<bool>("TestMode");
            initGold = Root.GetChild<DataReaderEntity>().GetYAMLObject(@"YAML\ServerConfig.yml").GetData<float>("InitGold");
        }

        public void Listen(ushort port, int peerLimit)
        {
            SetLobby();

            server.Listen(port, peerLimit);
        }

        int count = 0;
        const string DefaultName = "NONAME";
        void ConnectHandler(Peer peer, byte[] data)
        {
            Team team = (count++ % 2) == 0 ? Team.Blue : Team.Red;
            if (testMode)
            {
                Champion champion = new Champion(peer.ID, Root.GetChild<WorldEntity>().GetFountainPosition(team), 0, 0.3f, UnitType.HatsuneMiku, team, initGold, Root);
                Root.GetChild<WorldEntity>().AddChild(champion);

                peers.Add(peer.ID, new GameClient(peer, DefaultName, team, UnitType.HatsuneMiku, true, champion.UnitID));
            }
            else
            {
                peers.Add(peer.ID, new GameClient(peer, DefaultName, team, UnitType.HatsuneMiku, false, -1));
                SendLobby(PacketFlags.Reliable);
            }
        }

        void DisconnectHandler(Peer peer, byte[] data)
        {
            RemoveClient(peer, data);
        }

        void TimeoutHandler(Peer peer, byte[] data)
        {
            RemoveClient(peer, data);
        }

        void RemoveClient(Peer peer, byte[] data)
        {
            Unit unit = Root.GetChild<WorldEntity>().GetUnit(peers[peer.ID].UnitID);
            if(unit != null)
            {
                unit.ClearReference();
                Root.GetChild<WorldEntity>().RemoveChild(unit);
            }

            peers.Remove(peer.ID);
        }

        void SelectHandler(Peer peer, byte[] data)
        {
            SelectObj selectObj = MessagePackSerializer.Deserialize<SelectObj>(data);
            if(selectObj.Type < UnitType.HatsuneMiku || selectObj.Name == "" || selectObj.Team == Team.Yellow)
            {
                return;
            }

            if (state == NetworkState.Lobby)
            {
                peers[peer.ID]= new GameClient(peer, selectObj.Name, selectObj.Team, selectObj.Type, selectObj.Ready, -1);
            }
            else if (state == NetworkState.Countdown)
            {
                peers[peer.ID].Ready = selectObj.Ready;
                if(!selectObj.Ready)
                {
                    state = NetworkState.Lobby;
                }
            }
            else
            {
                peers[peer.ID] = new GameClient(peer, selectObj.Name, selectObj.Team, selectObj.Type, false, -1);
            }

            SendLobby(PacketFlags.Reliable);
        }

        void SendLobby(PacketFlags flags)
        {
            List<SelectObj> selectObjs = new List<SelectObj>();
            foreach(GameClient gameClient in peers.Values)
            {
                selectObjs.Add(new SelectObj()
                {
                    Type = gameClient.Type,
                    Name = gameClient.Name,
                    Team = gameClient.Team,
                    Ready = gameClient.Ready
                });
            }
            LobbyObj lobbyObj = new LobbyObj()
            {
                State = (byte)state,
                Timer = timer,
                SelectObjs = selectObjs.ToArray()
            };

            foreach (var keyValue in peers)
            {
                if(state == NetworkState.Battle && keyValue.Value.Ready)
                {
                    continue;
                }

                lobbyObj.PeerSelectObj = new SelectObj()
                {
                    Type = keyValue.Value.Type,
                    Name = keyValue.Value.Name,
                    Team = keyValue.Value.Team,
                    Ready = keyValue.Value.Ready
                };
                Send(MessageType.Lobby, keyValue.Key, MessagePackSerializer.Serialize(lobbyObj), flags);
            }
        }

        float lobbyTimer = 1.0f;
        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            server.Service();

            lobbyTimer -= deltaTime;
            if (lobbyTimer <= 0)
            {
                lobbyTimer = 1.0f;
                SendLobby(PacketFlags.Reliable);
            }

            if (state == NetworkState.Lobby)
            {
                if(testMode)
                {
                    SetBattle();
                }
                else
                {
                    if(peers.Count > 0 && peers.Values.All(x => x.Ready))
                    {
                        state = NetworkState.Countdown;
                        timer = 5.0f;
                    }
                }
            }
            else if(state == NetworkState.Countdown)
            {
                timer -= deltaTime;
                if(peers.Values.Any(x => !x.Ready))
                {
                    state = NetworkState.Lobby;
                }
                else
                {
                    if(timer <= 0)
                    {
                        SetBattle();
                    }
                }
            }
            else
            {
                if (!testMode && peers.Where(x => x.Value.Ready).Count() == 0)
                {
                    SetLobby();
                }
                else
                {
                    SendSnapshot();

                    foreach (Unit unit in Root.GetChild<WorldEntity>().GetChildren<Unit>())
                    {
                        if (unit.GetChild<Transform>() != null)
                        {
                            unit.GetChild<Transform>().ResetWarped();
                        }
                    }
                }
            }
        }

        void SetBattle()
        {
            state = NetworkState.Battle;

            server.ClearMessageHandlers();

            server.SetMessageHandler(MessageType.Connect, ConnectHandler);
            server.SetMessageHandler(MessageType.Disconnect, DisconnectHandler);
            server.SetMessageHandler(MessageType.Timeout, TimeoutHandler);
            server.SetMessageHandler(MessageType.Move, MoveHandler);
            server.SetMessageHandler(MessageType.Attack, AttackHandler);
            server.SetMessageHandler(MessageType.Recall, RecallHandler);
            server.SetMessageHandler(MessageType.BuyItem, BuyItemHandler);
            server.SetMessageHandler(MessageType.SellItem, SellItemHandler);
            server.SetMessageHandler(MessageType.UseItem, UseItemHandler);
            server.SetMessageHandler(MessageType.Change, ChangeHandler);
            server.SetMessageHandler(MessageType.Cast, CastHandler);
            server.SetMessageHandler(MessageType.Chat, ChatHandler);
            server.SetMessageHandler(MessageType.Select, SelectHandler);

            ((RootEntity)Root).CreateWorld();

            foreach(var keyValue in peers)
            {
                Champion champion = new Champion(keyValue.Key, Root.GetChild<WorldEntity>().GetFountainPosition(keyValue.Value.Team), 0, 0.3f, keyValue.Value.Type, keyValue.Value.Team, initGold, Root);
                Root.GetChild<WorldEntity>().AddChild(champion);

                keyValue.Value.UnitID = champion.UnitID;
            }
        }

        public void SetLobby()
        {
            state = NetworkState.Lobby;

            server.ClearMessageHandlers();

            server.SetMessageHandler(MessageType.Connect, ConnectHandler);
            server.SetMessageHandler(MessageType.Disconnect, DisconnectHandler);
            server.SetMessageHandler(MessageType.Timeout, TimeoutHandler);
            server.SetMessageHandler(MessageType.Chat, ChatHandler);
            server.SetMessageHandler(MessageType.Select, SelectHandler);

            Root.GetChild<WorldEntity>().RemoveAllEntity();
            foreach(var keyValue in peers)
            {
                keyValue.Value.Ready = false;
                keyValue.Value.UnitID = -1;
            }
        }

        void SendSnapshot()
        {
            ClientObj[] clientObjs = GetClientMsgPackObjs();
            ChampionObj[] blueChampionObjs = Root.GetChild<WorldEntity>().GetChampionObjs(true);
            ChampionObj[] redChampionObjs = Root.GetChild<WorldEntity>().GetChampionObjs(false);
            BuildingObj[] buildingObjs = Root.GetChild<WorldEntity>().GetBuildingObj();
            ActorObj[] blueVector3NoAnimObjs = Root.GetChild<WorldEntity>().GetActorObjs(true);
            ActorObj[] redVector3NoAnimObjs = Root.GetChild<WorldEntity>().GetActorObjs(false);
            UnitObj[] blueUnitObjs = Root.GetChild<WorldEntity>().GetUnitObjs(true);
            UnitObj[] redUnitObjs = Root.GetChild<WorldEntity>().GetUnitObjs(false);

            foreach (GameClient gameClient in peers.Values)
            {
                if (gameClient.Ready)
                {
                    Unit unit = Root.GetChild<WorldEntity>().GetUnit(gameClient.UnitID);

                    SnapshotObj snapshotObj = new SnapshotObj()
                    {
                        PlayerObj = Root.GetChild<WorldEntity>().GetUnit(gameClient.UnitID).GetPlayerObj(),
                        ClientObjs = clientObjs,
                        ChampionObjs = unit.Team == Team.Blue ? blueChampionObjs : redChampionObjs,
                        BuildingObjs = buildingObjs,
                        Vector3NoAnimObjs = unit.Team == Team.Blue ? blueVector3NoAnimObjs : redVector3NoAnimObjs,
                        UnitObjs = unit.Team == Team.Blue ? blueUnitObjs : redUnitObjs
                    };

                    Send(MessageType.Snapshot, gameClient.Peer.ID, MessagePackSerializer.Serialize(snapshotObj), PacketFlags.None);
                }
            }
        }

        public string GetName(uint peerID)
        {
            return peers[peerID].Name;
        }

        ClientObj[] GetClientMsgPackObjs()
        {
            List<ClientObj> ret = new List<ClientObj>();

            foreach(var gameClient in peers.Values)
            {
                if (gameClient.Ready)
                {
                    Unit unit = Root.GetChild<WorldEntity>().GetUnit(gameClient.UnitID);
                    ret.Add(new ClientObj()
                    {
                        Name = gameClient.Name,
                        Type = unit.Type,
                        Level = (byte)unit.GetChild<UnitStatus>().Level,
                        Team = unit.Team
                    });
                }
            }

            return ret.ToArray();
        }

        public void Send(MessageType type, uint peerID, byte[] data, PacketFlags flags)
        {
            server.Send(type, peers[peerID].Peer, data, flags);
        }

        public void SendAll(MessageType type, byte[] data, PacketFlags flags)
        {
            foreach (GameClient client in peers.Values)
            {
                server.Send(type, client.Peer, data, flags);
            }
        }

        void MoveHandler(Peer peer, byte[] data)
        {
            Vector2Obj vector2Obj = MessagePackSerializer.Deserialize<Vector2Obj>(data);
            object args = new Vector2(vector2Obj.X, vector2Obj.Y);

            Unit unit = Root.GetChild<WorldEntity>().GetUnit(peers[peer.ID].UnitID);
            if (unit != null && unit.GetCombat(CombatAttribute.Move).IsExecutable(args))
            {
                unit.Cancel(CombatAttribute.Ability);
                unit.Execute(CombatAttribute.Move, args);
            }
            
        }

        void AttackHandler(Peer peer, byte[] data)
        {
            int unitID = MessagePackSerializer.Deserialize<int>(data);
            object args = unitID;

            Unit unit = Root.GetChild<WorldEntity>().GetUnit(peers[peer.ID].UnitID);
            if (unit != null && unit.GetCombat(CombatAttribute.Attack).IsExecutable(args))
            {
                unit.Cancel(CombatAttribute.Ability);
                unit.Execute(CombatAttribute.Attack, args);
            }
        }

        void RecallHandler(Peer peer, byte[] data)
        {
            Unit unit = Root.GetChild<WorldEntity>().GetUnit(peers[peer.ID].UnitID);
            if (unit != null && unit.GetCombat(CombatAttribute.Recall).IsExecutable(null))
            {
                unit.Cancel(CombatAttribute.Ability);
                unit.Execute(CombatAttribute.Recall, null);
            }
        }

        void BuyItemHandler(Peer peer, byte[] data)
        {
            CombatType type = MessagePackSerializer.Deserialize<CombatType>(data);
            Unit unit = Root.GetChild<WorldEntity>().GetUnit(peers[peer.ID].UnitID);
            if(unit != null)
            {
                unit.BuyItem(type);
            }
        }

        void SellItemHandler(Peer peer, byte[] data)
        {
            int slotNum = (int)MessagePackSerializer.Deserialize<byte>(data);
            Unit unit = Root.GetChild<WorldEntity>().GetUnit(peers[peer.ID].UnitID);
            if (unit != null)
            {
                unit.SellItem(slotNum);
            }
        }

        void UseItemHandler(Peer peer, byte[] data)
        {
            int slotNum = (int)MessagePackSerializer.Deserialize<byte>(data);
            Unit unit = Root.GetChild<WorldEntity>().GetUnit(peers[peer.ID].UnitID);
            if (unit != null)
            {
                unit.UseItem(slotNum);
            }
        }

        void ChangeHandler(Peer peer, byte[] data)
        {
            if(!testMode)
            {
                return;
            }

            ChangeObj changeObj = MessagePackSerializer.Deserialize<ChangeObj>(data);
            peers[peer.ID].Name = changeObj.Name;

            Unit unit = Root.GetChild<WorldEntity>().GetUnit(peers[peer.ID].UnitID);
            if(unit != null)
            {
                if(unit.Team != changeObj.Team || unit.Type != changeObj.Type)
                {
                    unit.ClearReference();
                    Root.GetChild<WorldEntity>().RemoveChild(unit);

                    Champion champion = new Champion(peer.ID, Root.GetChild<WorldEntity>().GetFountainPosition(changeObj.Team), 0, 0.3f, changeObj.Type, changeObj.Team, initGold, Root);
                    Root.GetChild<WorldEntity>().AddChild(champion);

                    peers[peer.ID].UnitID = champion.UnitID;
                    peers[peer.ID].Team = changeObj.Team;
                    peers[peer.ID].Type = changeObj.Type;
                }
            }
        }

        void CastHandler(Peer peer, byte[] data)
        {
            CastObj castObj = MessagePackSerializer.Deserialize<CastObj>(data);
            Unit unit = Root.GetChild<WorldEntity>().GetUnit(peers[peer.ID].UnitID);
            if (unit != null)
            {
                if(castObj.SkillSlotNum == 0)
                {
                    unit.Execute(CombatAttribute.QSkill, castObj);
                }
                else if(castObj.SkillSlotNum == 1)
                {
                    unit.Execute(CombatAttribute.WSkill, castObj);
                }
                else if(castObj.SkillSlotNum == 2)
                {
                    unit.Execute(CombatAttribute.ESkill, castObj);
                }
                else
                {
                    unit.Execute(CombatAttribute.RSkill, castObj);
                }
            }
        }

        void ChatHandler(Peer peer, byte[] data)
        {
            string msg = MessagePackSerializer.Deserialize<string>(data);

            Team team = peers[peer.ID].Team;
            string name = peers[peer.ID].Name;
            if (msg.StartsWith("/all "))
            {
                string spaceMsg = msg.Substring(5);
                string trimMsg = spaceMsg.TrimStart();
                string finalMsg = "[" + DateTime.Now.ToString("HH:mm:ss") + " " + name + "] " + trimMsg;

                SendAll(MessageType.Broadcast, MessagePackSerializer.Serialize(new MsgObj() { Team = Team.Yellow, Msg = finalMsg}), PacketFlags.Reliable);
            }
            else if(msg.StartsWith("/team "))
            {
                string spaceMsg = msg.Substring(6);
                string trimMsg = spaceMsg.TrimStart();
                string finalMsg = "[" + DateTime.Now.ToString("HH:mm:ss") + " " + name + "] " + trimMsg;

                foreach(var keyValue in peers)
                {
                    if(keyValue.Value.Team == team)
                    {
                        Send(MessageType.Broadcast, keyValue.Key, MessagePackSerializer.Serialize(new MsgObj() { Team = team, Msg = finalMsg }), PacketFlags.Reliable);
                    }
                }
            }
            else
            {
                string finalMsg = "[" + DateTime.Now.ToString("HH:mm:ss") + " " + name + "] " + msg;

                SendAll(MessageType.Broadcast, MessagePackSerializer.Serialize(new MsgObj() { Team = Team.Yellow, Msg = finalMsg }), PacketFlags.Reliable);
            }
        }
    }
}
