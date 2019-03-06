using MessagePack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmediateDirectionUI : MonoBehaviour
{
    [SerializeField] Transform arrowRoot, rect, arrow;
    PlayerController playerController;
    Network network;
    int skillSlotNum;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(PlayerController playerController, Network network, int skillSlotNum, float distance)
    {
        this.playerController = playerController;
        this.network = network;
        this.skillSlotNum = skillSlotNum;

        rect.localScale = new Vector3(distance, 1, 1);
        rect.localPosition = new Vector3(50 * distance, 0, 0);

        arrow.localPosition = new Vector3(distance * 100 + 50, 0, 0);
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
            arrowRoot.eulerAngles = new Vector3(0, -angle*Mathf.Rad2Deg, 0);
        }

        if (Input.GetMouseButtonDown(0))
        {
            CastObj castObj = new CastObj()
            {
                SkillSlotNum = (byte)skillSlotNum,
                IntArgs = new int[0],
                FloatArgs = new float[] { 360 - arrowRoot.transform.eulerAngles.y }
            };
            network.Send(MessageType.Cast, MessagePackSerializer.Serialize(castObj), ENet.PacketFlags.Reliable);
            playerController.ResetFromInstance();
        }
    }
}
