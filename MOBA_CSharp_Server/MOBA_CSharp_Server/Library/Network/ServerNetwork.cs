using ENet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBA_CSharp_Server.Library.Network
{
    public delegate void MessageHandler(Peer peer, byte[] data);

    public class ServerNetwork
    {
        MessageHandler[] handlers;

        Host server;
        Address address;

        public ServerNetwork()
        {
            handlers = new MessageHandler[Enum.GetNames(typeof(MessageType)).Length];
        }

        public void SetMessageHandler(MessageType type, MessageHandler handler)
        {
            handlers[(int)type] = handler;
        }

        public void ClearMessageHandlers()
        {
            for (int i = 0; i < handlers.Length; i++)
            {
                handlers[i] = null;
            }
        }

        public void Listen(ushort port, int peerLimit)
        {
            ENet.Library.Initialize();

            server = new Host();

            address = new Address();
            address.Port = port;

            server.Create(address, peerLimit);
        }

        public void Shutdown()
        {
            if (server != null)
            {
                server.Flush();
                ENet.Library.Deinitialize();
            }
        }

        public void Service()
        {
            Event netEvent;
            while (server.Service(0, out netEvent) > 0)
            {
                switch (netEvent.Type)
                {
                    case EventType.None:
                        break;

                    case EventType.Connect:
                        Console.WriteLine("Client connected - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                        Invoke(MessageType.Connect, netEvent.Peer, new byte[0]);
                        break;

                    case EventType.Disconnect:
                        Console.WriteLine("Client disconnected - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                        Invoke(MessageType.Disconnect, netEvent.Peer, new byte[0]);
                        break;

                    case EventType.Timeout:
                        Console.WriteLine("Client timeout - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                        Invoke(MessageType.Timeout, netEvent.Peer, new byte[0]);
                        break;

                    case EventType.Receive:
                        //Console.WriteLine("Packet received from - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP + ", Channel ID: " + netEvent.ChannelID + ", Data length: " + netEvent.Packet.Length);
                        Receive(netEvent);
                        netEvent.Packet.Dispose();
                        break;
                }
            }
        }

        void Invoke(MessageType type, Peer peer, byte[] data)
        {
            if (handlers[(int)type] != null)
            {
                handlers[(int)type](peer, data);
            }
        }

        void Receive(Event netEvent)
        {
            if (netEvent.Packet.Length < MessageConfig.MESSAGE_LEN)
            {
                return;
            }

            byte[] buffer = new byte[netEvent.Packet.Length];
            netEvent.Packet.CopyTo(buffer);

            MessageType type = (MessageType)BitConverter.ToInt16(buffer, 0);

            byte[] data = new byte[netEvent.Packet.Length - MessageConfig.MESSAGE_LEN];
            Array.Copy(buffer, 2, data, 0, data.Length);

            Invoke(type, netEvent.Peer, data);
        }

        public void Send(MessageType type, Peer peer, byte[] data, PacketFlags flags)
        {
            Packet packet = default(Packet);
            byte[] buffer = new byte[MessageConfig.MESSAGE_LEN + data.Length];

            byte[] byteType = BitConverter.GetBytes((ushort)type);
            Array.Copy(byteType, buffer, MessageConfig.MESSAGE_LEN);

            Array.Copy(data, 0, buffer, MessageConfig.MESSAGE_LEN, data.Length);

            packet.Create(buffer, flags);
            peer.Send(0, ref packet);
        }
    }
}
