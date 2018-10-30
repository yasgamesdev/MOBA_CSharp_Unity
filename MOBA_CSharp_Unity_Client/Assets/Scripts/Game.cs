using ENet;
using MessagePack;
using MOBA_CSharp_Client.ClientNetwork;
using MOBA_CSharp_Server.Config;
using MOBA_CSharp_Server.MsgPackObjs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance
    {
        get; private set;
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Init();
    }

    ConfigReader config;
    ClientNetwork client;
    [SerializeField] UnitManager unitManager;

    void Init()
    {
        config = new ConfigReader("ClientConfig.txt");
        UnitMovement.SetFrameRate(config.GetInt("FrameRate"));
        PlayerScript.VisionRadius = config.GetFloat("VisionRadius");

        client = new ClientNetwork();
        Connect();
    }

    void OnDestroy()
    {
        client.Shutdown();
    }

    void Connect()
    {
        client.ClearMessageHandlers();
        client.SetMessageHandler(MessageType.Connect, ConnectHandler);

        client.Connect(config.GetString("Host"), (ushort)config.GetInt("Port"));
    }

    void ConnectHandler(byte[] data)
    {
        client.ClearMessageHandlers();

        client.SetMessageHandler(MessageType.Disconnect, DisconnectHandler);
        client.SetMessageHandler(MessageType.Timeout, TimeoutHandler);
        client.SetMessageHandler(MessageType.Snapshot, SnapshotHandler);
    }

    void DisconnectHandler(byte[] data)
    {
        Connect();
    }

    void TimeoutHandler(byte[] data)
    {
        Connect();
    }

    void Update()
    {
        client.Service();
    }

    public uint GetPeerID()
    {
        return client.GetPeerID();
    }

    void SnapshotHandler(byte[] data)
    {
        SnapshotData snapshotData = MessagePackSerializer.Deserialize<SnapshotData>(data);
        unitManager.SetSnapshot(snapshotData);
    }

    public void Send(MessageType type, byte[] data, PacketFlags flags)
    {
        client.Send(type, data, flags);
    }

    public ConfigReader GetConfig()
    {
        return config;
    }
}
