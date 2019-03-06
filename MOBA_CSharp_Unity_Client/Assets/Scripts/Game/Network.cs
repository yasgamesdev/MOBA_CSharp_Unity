using ENet;
using MessagePack;
using MessagePack.Resolvers;
using MOBA_CSharp_Client.ClientNetwork;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Network : MonoBehaviour
{

    enum SceneState
    {
        Title,
        Lobby,
        Main
    }
    //State
    SceneState state;
    bool testMode;

    //Reader
    YAMLReader yamlReader = new YAMLReader();
    CSVReader csvReader = new CSVReader();

    //Network
    ClientNetwork client = new ClientNetwork();

    //Title
    TMP_InputField hostInputField, portInputField;
    GameObject successImage;
    float successTimer;
    Image blackScreen;

    //Lobby&Main
    ChatUI chatUI;

    //Lobby
    [SerializeField] Text lobbyTitle;
    [SerializeField] GameObject championSelectIconPrefab;
    [SerializeField] GameObject playerNodePrefab;
    RectTransform blueContentRectTransform, redContentRectTransform;
    List<GameObject> bluePlayerNodeInstances = new List<GameObject>();
    List<GameObject> redPlayerNodeInstances = new List<GameObject>();
    Image selectedChampionIconImage;
    InputField nameInputField;
    Button setNameButton, teamButton, readyButton;

    SelectObj playerSelectObj = null;
    bool applyPlayerSelectObj = false;
    UnitType type;
    string _name;
    Team _team;
    bool _ready;

    //Main
    UnitManager unitManager;
    
    public static Network Instance
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

    void Init()
    {
        testMode = GetYAMLObject(@"YAML\ClientConfig.yml").GetData<bool>("TestMode");
        Enum.TryParse(SceneManager.GetActiveScene().name, out state);

        MinimapUI.MapWidth = GetYAMLObject(@"YAML\ClientConfig.yml").GetData<float>("MapWidth");
        MinimapUI.MapHeight = GetYAMLObject(@"YAML\ClientConfig.yml").GetData<float>("MapHeight");
        Movement.SetFrameRate(GetYAMLObject(@"YAML\ClientConfig.yml").GetData<int>("FrameRate"));
        RTSCamera.IsScreenEdgeMovement = GetYAMLObject(@"YAML\ClientConfig.yml").GetData<bool>("IsScreenEdgeMovement");
        RecallUI.RecallTime = GetYAMLObject(CombatType.Recall).GetData<float>("RecallTime");

        client.SetMessageHandler(MessageType.Connect, ConnectHandler);
        client.SetMessageHandler(MessageType.Disconnect, DisconnectHandler);
        client.SetMessageHandler(MessageType.Timeout, TimeoutHandler);
        client.SetMessageHandler(MessageType.Lobby, LobbyHandler);
        client.SetMessageHandler(MessageType.Broadcast, BroadcastHandler);
        client.SetMessageHandler(MessageType.Snapshot, SnapshotHandler);

        if (testMode)
        {
            string host = GetYAMLObject(@"YAML\ClientConfig.yml").GetData<string>("Host");
            ushort port = (ushort)GetYAMLObject(@"YAML\ClientConfig.yml").GetData<int>("Port");

            client.Connect(host, port);
        }
        else
        {
            if (state != SceneState.Title)
            {
                string host = GetYAMLObject(@"YAML\ClientConfig.yml").GetData<string>("Host");
                ushort port = (ushort)GetYAMLObject(@"YAML\ClientConfig.yml").GetData<int>("Port");

                client.Connect(host, port);
            }
        }
    }

    public void TriggeredStart()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        Enum.TryParse(SceneManager.GetActiveScene().name, out state);

        if (state == SceneState.Title)
        {
            hostInputField = GameObject.Find("HostInputField").GetComponent<TMP_InputField>();
            portInputField = GameObject.Find("PortInputField").GetComponent<TMP_InputField>();

            hostInputField.text = GetYAMLObject(@"YAML\ClientConfig.yml").GetData<string>("Host");
            portInputField.text = GetYAMLObject(@"YAML\ClientConfig.yml").GetData<int>("Port").ToString();

            GameObject.Find("Button").GetComponent<Button>().onClick.AddListener(ConnectClick);
            successImage = GameObject.Find("Button").transform.Find("Background").Find("SuccessImage").gameObject;
            successTimer = 0;

            blackScreen = GameObject.Find("BlackScreen").GetComponent<Image>();

            applyPlayerSelectObj = false;
            playerSelectObj = null;
        }
        else if (state == SceneState.Lobby)
        {
            RectTransform iconRoot = GameObject.Find("IconRoot").GetComponent<RectTransform>();
            int count = 0;
            Vector2 initPos = new Vector2(32f, -32f);
            foreach (UnitType type in Enum.GetValues(typeof(UnitType)))
            {
                if (type >= UnitType.HatsuneMiku)
                {
                    int x = count % 8;
                    int y = count / 8;

                    GameObject instance = Instantiate(championSelectIconPrefab, iconRoot);
                    instance.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                    instance.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                    instance.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
                    instance.GetComponent<RectTransform>().anchoredPosition = initPos + new Vector2(x * 100, y * -100);

                    instance.GetComponent<ChampionSelectIcon>().SetData(type, this);

                    count++;
                }
            }

            lobbyTitle = GameObject.Find("LobbyTitle").GetComponent<Text>();
            blueContentRectTransform = GameObject.Find("BlueContent").GetComponent<RectTransform>();
            redContentRectTransform = GameObject.Find("RedContent").GetComponent<RectTransform>();
            selectedChampionIconImage = GameObject.Find("SelectedChampionIconImage").GetComponent<Image>();
            nameInputField = GameObject.Find("NameInputField").GetComponent<InputField>();
            setNameButton = GameObject.Find("SetNameButton").GetComponent<Button>();
            GameObject.Find("SetNameButton").GetComponent<Button>().onClick.AddListener(SetNameButtonPressed);
            teamButton = GameObject.Find("TeamButton").GetComponent<Button>();
            GameObject.Find("TeamButton").GetComponent<Button>().onClick.AddListener(SetTeamButtonPressed);
            readyButton = GameObject.Find("ReadyButton").GetComponent<Button>();
            GameObject.Find("ReadyButton").GetComponent<Button>().onClick.AddListener(ReadyButtonPressed);
            chatUI = GameObject.Find("ChatUI").GetComponent<ChatUI>();
            chatUI.SetClientNetwork(client);
            GameObject.Find("SendButton").GetComponent<Button>().onClick.AddListener(chatUI.Send);

            if (!applyPlayerSelectObj && playerSelectObj != null)
            {
                type = playerSelectObj.Type;
                SetUnitTypeIcon(type);

                _name = playerSelectObj.Name;
                nameInputField.text = playerSelectObj.Name;

                _team = playerSelectObj.Team;
                SetTeamButtonColor(_team);

                _ready = playerSelectObj.Ready;
                SetReadyButtonColor(_ready);

                applyPlayerSelectObj = true;
                playerSelectObj = null;
            }
        } 
        else
        {
            unitManager = GameObject.Find("UnitManager").GetComponent<UnitManager>();
            chatUI = GameObject.Find("ChatUI").GetComponent<ChatUI>();
            chatUI.SetClientNetwork(client);

            applyPlayerSelectObj = false;
            playerSelectObj = null;
        }
    }

    void ConnectClick()
    {
        string host = hostInputField.text;
        ushort port = ushort.Parse(portInputField.text);

        client.Connect(host, port);
    }

    //Reader
    public YAMLObject GetYAMLObject(string path)
    {
        return yamlReader.GetYAMLObject(path);
    }

    public YAMLObject GetYAMLObject(UnitType type)
    {
        return yamlReader.GetYAMLObject(@"YAML\Units\" + type.ToString() + ".yml");
    }

    public YAMLObject GetYAMLObject(CombatType type)
    {
        return yamlReader.GetYAMLObject(@"YAML\Combats\" + type.ToString() + ".yml");
    }

    public ItemTable GetItemTable(CombatType type)
    {
        return csvReader.GetItemTable(type);
    }

    //Network
    void OnDestroy()
    {
        client.Shutdown();
    }

    void ConnectHandler(byte[] data)
    {
        if (testMode && state != SceneState.Main)
        {

            SceneManager.LoadScene("Main");
        }
        else if(state == SceneState.Title)
        {
            successImage.SetActive(true);
            successTimer = 1.0f;
            successImage.GetComponent<AudioSource>().Play();
        }
    }

    void DisconnectHandler(byte[] data)
    {
        client.Shutdown();
        SceneManager.LoadScene("Title");
    }

    void TimeoutHandler(byte[] data)
    {
        client.Shutdown();
        SceneManager.LoadScene("Title");
    }

    void Update()
    {
        if(successTimer > 0)
        {
            successTimer -= Time.deltaTime;
            if (successTimer <= 0)
            {
                SceneManager.LoadScene("Lobby");
            }
            if (successTimer <= 0.75f)
            {
                blackScreen.color = new Color(0, 0, 0, 0.75f - successTimer);
            }
        }

        client.Service();
    }

    public uint GetPeerID()
    {
        return client.GetPeerID();
    }

    void SnapshotHandler(byte[] data)
    {
        if (state != SceneState.Main)
        {
            SceneManager.LoadScene("Main");
        }
        else
        {
            SnapshotObj snapshotObj = MessagePackSerializer.Deserialize<SnapshotObj>(data);
            unitManager.SetSnapshot(snapshotObj);
        }
    }

    void BroadcastHandler(byte[] data)
    {
        if(state != SceneState.Title)
        {
            MsgObj msgObj = MessagePackSerializer.Deserialize<MsgObj>(data);
            if (chatUI != null)
            {
                chatUI.Log(msgObj.Team, msgObj.Msg);
            }
        }
    }

    public void Send(MessageType type, byte[] data, PacketFlags flags)
    {
        client.Send(type, data, flags);
    }

    void LobbyHandler(byte[] data)
    {
        LobbyObj lobbyObj = MessagePackSerializer.Deserialize<LobbyObj>(data);

        if (!applyPlayerSelectObj && playerSelectObj == null)
        {
            playerSelectObj = lobbyObj.PeerSelectObj;
            if(state == SceneState.Lobby)
            {
                type = playerSelectObj.Type;
                SetUnitTypeIcon(type);

                _name = playerSelectObj.Name;
                nameInputField.text = playerSelectObj.Name;

                _team = playerSelectObj.Team;
                SetTeamButtonColor(_team);

                _ready = playerSelectObj.Ready;
                SetReadyButtonColor(_ready);

                applyPlayerSelectObj = true;
                playerSelectObj = null;
            }
        }

        if (state == SceneState.Lobby)
        {
            if (lobbyObj.State == 0)
            {
                lobbyTitle.text = "Before Battle";
            }
            else if (lobbyObj.State == 1)
            {
                lobbyTitle.text = lobbyObj.Timer.ToString("F0");
            }
            else
            {
                lobbyTitle.text = "In Battle";
            }

            bluePlayerNodeInstances.ForEach(x => Destroy(x));
            bluePlayerNodeInstances.Clear();
            redPlayerNodeInstances.ForEach(x => Destroy(x));
            redPlayerNodeInstances.Clear();

            foreach (SelectObj selectObj in lobbyObj.SelectObjs)
            {
                if (selectObj.Team == Team.Blue)
                {
                    GameObject instance = Instantiate(playerNodePrefab, blueContentRectTransform);
                    instance.GetComponent<PlayerNode>().SetData(selectObj.Type, selectObj.Team, selectObj.Name, selectObj.Ready);
                    bluePlayerNodeInstances.Add(instance);
                }
                else
                {
                    GameObject instance = Instantiate(playerNodePrefab, redContentRectTransform);
                    instance.GetComponent<PlayerNode>().SetData(selectObj.Type, selectObj.Team, selectObj.Name, selectObj.Ready);
                    redPlayerNodeInstances.Add(instance);
                }
            }
        }
        else if(state == SceneState.Main)
        {
            SceneManager.LoadScene("Lobby");
        }
    }

    public void Set()
    {
        SelectObj selectObj = new SelectObj()
        {
            Type = type,
            Name = nameInputField.text,
            Team = _team,
            Ready = _ready
        };

        Send(MessageType.Select, MessagePackSerializer.Serialize(selectObj), PacketFlags.Reliable);
    }

    public void SetUnitType(UnitType type)
    {
        this.type = type;

        SetUnitTypeIcon(type);

        Set();
    }

    public void ReadyButtonPressed()
    {
        _ready = !_ready;

        SetReadyButtonColor(_ready);

        Set();

        if(_ready)
        {
            readyButton.GetComponent<AudioSource>().Play();
        }
    }

    public void SetNameButtonPressed()
    {
        if(nameInputField.text != "")
        {
            Set();
        }
    }

    public void SetTeamButtonPressed()
    {
        if(_team == Team.Blue)
        {
            _team = Team.Red;
        }
        else
        {
            _team = Team.Blue;
        }

        SetTeamButtonColor(_team);

        Set();
    }

    void SetUnitTypeIcon(UnitType type)
    {
        selectedChampionIconImage.sprite = UnitTable.Instance.GetUnitModel(type).Icon;
    }

    void SetTeamButtonColor(Team team)
    {
        if(team == Team.Blue)
        {
            teamButton.transform.Find("Background").GetComponent<Image>().color = Color.blue;
            teamButton.transform.Find("Background").Find("Label").GetComponent<Text>().text = "Blue";
        }
        else
        {
            teamButton.transform.Find("Background").GetComponent<Image>().color = Color.red;
            teamButton.transform.Find("Background").Find("Label").GetComponent<Text>().text = "Red";
        }
    }

    void SetReadyButtonColor(bool ready)
    {
        if (!ready)
        {
            readyButton.transform.Find("Background").GetComponent<Image>().color = Color.green;
            readyButton.transform.Find("Background").Find("Label").GetComponent<Text>().text = "Ready";
        }
        else
        {
            readyButton.transform.Find("Background").GetComponent<Image>().color = Color.red;
            readyButton.transform.Find("Background").Find("Label").GetComponent<Text>().text = "Cancel";
        }
    }
}
