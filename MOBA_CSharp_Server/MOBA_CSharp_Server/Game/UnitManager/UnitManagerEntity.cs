using ECS;
using MessagePack;
using Network.MsgPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace MOBA_CSharp_Server.Game
{
    public class UnitManagerEntity : Entity
    {
        Dictionary<UnitType, Dictionary<int, UnitEntity>> unitEntities = new Dictionary<UnitType, Dictionary<int, UnitEntity>>();

        public UnitManagerEntity(RootEntity root) : base(root)
        {
            foreach(UnitType type in Enum.GetValues(typeof(UnitType)))
            {
                unitEntities.Add(type, new Dictionary<int, UnitEntity>());
            }
        }

        public void AddUnit(UnitType type, UnitEntity unit)
        {
            unitEntities[type].Add(unit.EntityID, unit);
        }

        public void RemoveUnit(UnitType type, UnitEntity unit)
        {
            UnitEntity removeEntity = unitEntities[type][unit.EntityID];
            unitEntities[type].Remove(removeEntity.EntityID);
            removeEntity.Destroy();
        }

        public void RemoveChampion(uint peerID)
        {
            RemoveUnit(UnitType.Champion, GetChampionEntity(peerID));
        }

        ChampionEntity GetChampionEntity(uint peerID)
        {
            UnitEntity champion = unitEntities[UnitType.Champion].Values.FirstOrDefault(x => ((ChampionEntity)x).PeerID == peerID);
            if(champion != null)
            {
                return (ChampionEntity)champion;
            }
            else
            {
                return null;
            }
        }

        public void Move(Vector2 dest, uint peerID)
        {
            ChampionEntity champion = GetChampionEntity(peerID);

            if (champion != null)
            {
                champion.GetComponent<BodyComponent>().Move(dest);
            }
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            foreach(var dict in unitEntities.Values)
            {
                foreach(var unit in dict.Values)
                {
                    unit.Step(deltaTime);
                }
            }

            SendSnapshot();
        }

        void SendSnapshot()
        {
            var forBlue = MessagePackSerializer.Serialize(GetSnapshotData(Team.Blue));
            var forRed = MessagePackSerializer.Serialize(GetSnapshotData(Team.Red));

            foreach (ChampionEntity champion in unitEntities[UnitType.Champion].Values.Cast<ChampionEntity>())
            {
                if (champion.Team == Team.Blue)
                {
                    root.GetChild<NetworkEntity>().Send(MessageType.Snapshot, champion.PeerID, forBlue, ENet.PacketFlags.None);
                }
                else if (champion.Team == Team.Red)
                {
                    root.GetChild<NetworkEntity>().Send(MessageType.Snapshot, champion.PeerID, forRed, ENet.PacketFlags.None);
                }
            }
        }

        public SnapshotData GetSnapshotData(Team playerTeam)
        {
            List<PlayerData> playerDatas = new List<PlayerData>();
            foreach (ChampionEntity champion in unitEntities[UnitType.Champion].Values.Where(x => x.GetComponent<VisionComponent>().GetVision(playerTeam)))
            {
                playerDatas.Add(champion.GetPlayerData());
            }

            return new SnapshotData() { PlayerDatas = playerDatas.ToArray() };
        }
    }
}
