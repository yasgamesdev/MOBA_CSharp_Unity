using MessagePack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmediateCircleUI : MonoBehaviour
{
    [SerializeField] Transform circleRoot, circle, bigCircle;
    PlayerController playerController;
    Network network;
    int skillSlotNum;
    // Start is called before the first frame update
    float distance;
    void Start()
    {
        
    }

    public void Init(PlayerController playerController, Network network, int skillSlotNum, float distance, float radius)
    {
        this.playerController = playerController;
        this.network = network;
        this.skillSlotNum = skillSlotNum;

        this.distance = distance;

        circle.localScale = new Vector3(radius, radius, radius);
        bigCircle.localScale = new Vector3(distance, distance, distance);
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int groundLayerMask = LayerMask.GetMask("Ground");
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 10000f, groundLayerMask))
        {
            Vector3 worldPos = hitInfo.point;
            float angle = Mathf.Atan2(worldPos.z - transform.position.z, worldPos.x - transform.position.x);

            if((worldPos - transform.position).magnitude > distance)
            {
                Vector3 center = transform.position + (worldPos - transform.position) / (worldPos - transform.position).magnitude * distance + new Vector3(0, 0.01f, 0);
                circleRoot.position = center;
            }
            else
            {
                circleRoot.position = worldPos + new Vector3(0, 0.01f, 0);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            CastObj castObj = new CastObj()
            {
                SkillSlotNum = (byte)skillSlotNum,
                IntArgs = new int[0],
                FloatArgs = new float[] { circleRoot.position.x, circleRoot.position.z }
            };
            network.Send(MessageType.Cast, MessagePackSerializer.Serialize(castObj), ENet.PacketFlags.Reliable);
            playerController.ResetFromInstance();
        }
    }
}
