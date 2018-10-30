using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENet;

namespace MOBA_CSharp_Client.ClientNetwork
{
    public delegate void MessageHandler(byte[] data);

    class ClientNetwork
    {
        MessageHandler[] handlers;

        Host client;
        Address address;
        Peer peer;

        public ClientNetwork()
        {
            handlers = new MessageHandler[Enum.GetNames(typeof(MessageType)).Length];
        }

        public void SetMessageHandler(MessageType type, MessageHandler handler)
        {
            handlers[(int)type] = handler;
        }

        public void ClearMessageHandlers()
        {
            for(int i=0; i<handlers.Length; i++)
            {
                handlers[i] = null;
            }
        }

        public void Connect(string hostName, ushort port)
        {
            Library.Initialize();

            client = new Host();

            address = new Address();
            address.SetHost(hostName);
            address.Port = port;

            client.Create();

            peer = client.Connect(address);
        }

        public void Shutdown()
        {
            if (client != null)
            {
                client.Flush();
                peer.DisconnectNow(0);
                Library.Deinitialize();
            }
        }

        public void Service()
        {
            Event netEvent;
            while (client.Service(0, out netEvent) > 0)
            {
                switch (netEvent.Type)
                {
                    case EventType.None:
                        break;

                    case EventType.Connect:
                        Console.WriteLine("Client connected to server - ID: " + peer.ID);
                        Invoke(MessageType.Connect, new byte[0]);
                        break;

                    case EventType.Disconnect:
                        Console.WriteLine("Client disconnected from server");
                        Invoke(MessageType.Disconnect, new byte[0]);
                        break;

                    case EventType.Timeout:
                        Console.WriteLine("Client connection timeout");
                        Invoke(MessageType.Timeout, new byte[0]);
                        break;

                    case EventType.Receive:
                        Console.WriteLine("Packet received from server - Channel ID: " + netEvent.ChannelID + ", Data length: " + netEvent.Packet.Length);
                        Receive(netEvent);
                        netEvent.Packet.Dispose();
                        break;
                }
            }
        }

        void Invoke(MessageType type, byte[] data)
        {
            handlers[(int)type]?.Invoke(data);
        }

        void Receive(Event netEvent)
        {
            if(netEvent.Packet.Length < MessageConfig.MESSAGE_LEN)
            {
                return;
            }

            byte[] buffer = new byte[netEvent.Packet.Length];
            netEvent.Packet.CopyTo(buffer);

            MessageType type = (MessageType)BitConverter.ToInt16(buffer, 0);

            byte[] data = new byte[netEvent.Packet.Length - MessageConfig.MESSAGE_LEN];
            Array.Copy(buffer, 2, data, 0, data.Length);

            Invoke(type, data);
        }

        public void Send(MessageType type, byte[] data, PacketFlags flags)
        {
            Packet packet = default(Packet);
            byte[] buffer = new byte[MessageConfig.MESSAGE_LEN + data.Length];

            byte[] byteType = BitConverter.GetBytes((ushort)type);
            Array.Copy(byteType, buffer, MessageConfig.MESSAGE_LEN);

            Array.Copy(data, 0, buffer, MessageConfig.MESSAGE_LEN, data.Length);

            packet.Create(buffer, flags);
            peer.Send(0, ref packet);
        }

        public uint GetPeerID()
        {
            return peer.ID;
        }
    }
}
