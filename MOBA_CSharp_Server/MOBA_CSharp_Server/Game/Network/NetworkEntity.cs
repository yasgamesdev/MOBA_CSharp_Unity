using ECS;
using ENet;
using MessagePack;
using Network;
using Network.MsgPack;
using System.Collections.Generic;
using System.Numerics;

namespace MOBA_CSharp_Server.Game
{
    public class NetworkEntity : Entity
    {
        ClientManagerEntity clientManager;

        ServerNetwork server;
        Dictionary<uint, Peer> peers = new Dictionary<uint, Peer>();
        
        public NetworkEntity(ClientManagerEntity clientManager, ushort port, RootEntity root) : base(root)
        {
            this.clientManager = clientManager;

            server = new ServerNetwork();
            server.Listen(port, 100);

            server.SetMessageHandler(MessageType.Connect, ConnectHandler);
            server.SetMessageHandler(MessageType.Disconnect, DisconnectHandler);
            server.SetMessageHandler(MessageType.Timeout, TimeoutHandler);
            server.SetMessageHandler(MessageType.Move, MoveHandler);
        }

        void ConnectHandler(Peer peer, byte[] data)
        {
            peers.Add(peer.ID, peer);
            clientManager.AddClient(peer);
        }

        void DisconnectHandler(Peer peer, byte[] data)
        {
            clientManager.RemoveClient(peer.ID);
            peers.Remove(peer.ID);
        }

        void TimeoutHandler(Peer peer, byte[] data)
        {
            clientManager.RemoveClient(peer.ID);
            peers.Remove(peer.ID);
        }

        public override void Step(float deltaTime)
        {
            server.Service();
        }

        public void Send(MessageType type, uint peerID, byte[] data, PacketFlags flags)
        {
            server.Send(type, peers[peerID], data, flags);
        }

        public void SendAll(MessageType type, byte[] data, PacketFlags flags)
        {
            foreach(Peer peer in peers.Values)
            {
                server.Send(type, peer, data, flags);
            }
        }

        void MoveHandler(Peer peer, byte[] data)
        {
            Vector2Data vector2Data = MessagePackSerializer.Deserialize<Vector2Data>(data);
            clientManager.Move(new Vector2(vector2Data.X, vector2Data.Y), peer.ID);
        }
    }
}
