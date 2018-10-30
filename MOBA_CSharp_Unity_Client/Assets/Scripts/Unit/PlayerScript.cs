using MessagePack;
using MOBA_CSharp_Server.MsgPackObjs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {
    [SerializeField] UnitAnimation anime;
    [SerializeField] UnitMovement movement;
    [SerializeField] UnitTeam team;
    [SerializeField] GameObject vision;

    public int EntityID { get; set; }
    public uint PeerID { get; set; }

    static Team PlayerTeam;
    public static float VisionRadius;
    bool IsPlayer;

    public void Init(PlayerData playerData)
    {
        anime.Init();
        movement.Init(new Vector2(playerData.PosX, playerData.PosZ), playerData.Angle);
        team.Init((Team)playerData.Team);

        EntityID = playerData.EntityID;
        PeerID = playerData.PeerID;

        if(GameObject.Find("Game").GetComponent<Game>().GetPeerID() == PeerID)
        {
            Camera.main.gameObject.GetComponent<RTSCamera>().SetTarget(transform);
            PlayerTeam = (Team)playerData.Team;
            IsPlayer = true;
        }
        else
        {
            IsPlayer = false;
        }

        UpdateData(playerData);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(IsPlayer)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                int layerMask = LayerMask.GetMask("Ground");
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, 10000f, layerMask))
                {
                    Vector2Data vector2Data = new Vector2Data() { X = hitInfo.point.x, Y = hitInfo.point.z };
                    GameObject.Find("Game").GetComponent<Game>().Send(MessageType.Move, MessagePackSerializer.Serialize(vector2Data), ENet.PacketFlags.Reliable);
                }
            }
        }
    }

    public void UpdateData(PlayerData playerData)
    {
        movement.SetPosition(new Vector2(playerData.PosX, playerData.PosZ), playerData.Angle, playerData.Warped);
        anime.SetAnime((AnimationType)playerData.Anime, playerData.Loop);

        if((Team)playerData.Team == PlayerTeam)
        {
            vision.GetComponent<FieldOfView>().viewRadius = VisionRadius;
        }
        else
        {
            vision.GetComponent<FieldOfView>().viewRadius = 3.0f;
        }
        //vision.SetActive((Team)playerData.Team == PlayerTeam);
    }
}
