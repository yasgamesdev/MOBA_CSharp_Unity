using MessagePack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImmediateConeUI : MonoBehaviour
{
    [SerializeField] Transform coneRoot, rotRoot;
    [SerializeField] Slider slider;
    PlayerController playerController;
    Network network;
    int skillSlotNum;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(PlayerController playerController, Network network, int skillSlotNum, float radius, float angle)
    {
        this.playerController = playerController;
        this.network = network;
        this.skillSlotNum = skillSlotNum;

        slider.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
        slider.value = angle / 360.0f;
        rotRoot.localEulerAngles = new Vector3(0, -angle / 2f, 0);
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
            coneRoot.eulerAngles = new Vector3(0, -angle * Mathf.Rad2Deg, 0);
        }

        if (Input.GetMouseButtonDown(0))
        {
            CastObj castObj = new CastObj()
            {
                SkillSlotNum = (byte)skillSlotNum,
                IntArgs = new int[0],
                FloatArgs = new float[] { 360 - coneRoot.transform.eulerAngles.y }
            };
            network.Send(MessageType.Cast, MessagePackSerializer.Serialize(castObj), ENet.PacketFlags.Reliable);
            playerController.ResetFromInstance();
        }
    }
}
