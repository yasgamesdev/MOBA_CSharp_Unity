using MessagePack;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    enum SkillState
    {
        None,
        QActive,
        WActive,
        EActive,
        RActive
    };

    [SerializeField] Network network;

    GameObject playerUnitInstance;

    //Skill
    PlayerObj playerObj;
    SkillState skillState;
    Dictionary<int, bool> actives = new Dictionary<int, bool>();
    Dictionary<int, CombatType> skillTypes = new Dictionary<int, CombatType>();

    [SerializeField] GameObject immediateDirectionPrefab, immediateConePrefab, immediateCirclePrefab;
    GameObject skillInstance;

    //GroundPointer
    [SerializeField] GameObject groundPointerPrefab;

    //MouseIcon
    [SerializeField] Texture2D attackTexture, moveTexture, castTexture;
    CursorMode cursorMode = CursorMode.Auto;
    Vector2 hotSpot = Vector2.zero;

    [SerializeField] ChatUI chatUI;

    void Start()
    {
        network = GameObject.Find("Network").GetComponent<Network>();
    }

    public void SetPlayerObj(PlayerObj playerObj)
    {
        if(this.playerObj == null || this.playerObj.UnitID != playerObj.UnitID)
        {
            ResetSkillState(playerObj);
        }

        this.playerObj = playerObj;
        actives.Clear();
        foreach(CombatObj combatObj in playerObj.Skills)
        {
            actives.Add(combatObj.SlotNum, combatObj.IsActive);
        }
    }

    void ResetSkillState(PlayerObj playerObj)
    {
        skillState = SkillState.None;
        DestroySkillInstance();

        skillTypes.Clear();
        foreach (CombatObj combatObj in playerObj.Skills)
        {
            skillTypes.Add(combatObj.SlotNum, combatObj.Type);
        }

    }

    public void ResetFromInstance()
    {
        skillState = SkillState.None;
        DestroySkillInstance();
    }

    void DestroySkillInstance()
    {
        if(skillInstance != null)
        {
            Destroy(skillInstance);
            skillInstance = null;
        }
    }

    public void SetPlayerUnit(GameObject playerUnitInstance)
    {
        this.playerUnitInstance = playerUnitInstance;
    }

    void Update()
    {
        if (playerUnitInstance != null)
        {
            Team playerTeam = playerUnitInstance.GetComponent<Unit>().Info.GetTeam();
            float playerCurHP = playerUnitInstance.GetComponent<Unit>().Info.GetCurHP();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int unitLayerMask = LayerMask.GetMask("Unit");
            int groundLayerMask = LayerMask.GetMask("Ground");

            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray, 1000f, unitLayerMask);
            Unit attackUnit = null;
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                Unit unit = hit.transform.GetComponentsInParent<Unit>()[0];

                Team team = unit.Info.GetTeam();
                float curHP = unit.Info.GetCurHP();

                if (team != playerTeam && curHP > 0)
                {
                    attackUnit = unit;
                    break;
                }
            }

            RaycastHit hitInfo;
            Vector2 movePosition = Vector2.zero;
            if (Physics.Raycast(ray, out hitInfo, 10000f, groundLayerMask))
            {
                movePosition = new Vector2(hitInfo.point.x, hitInfo.point.z);

                
            }

            if(Input.GetMouseButtonDown(1))
            {
                if(attackUnit != null)
                {
                    network.Send(MessageType.Attack, MessagePackSerializer.Serialize(attackUnit.Info.GetUnitID()), ENet.PacketFlags.Reliable);

                    GameObject groundPointer = Instantiate(groundPointerPrefab);
                    groundPointer.GetComponent<GroundPointer>().Init(attackUnit.transform);
                }
                else
                {
                    Vector2Obj vector2Obj = new Vector2Obj() { X = movePosition.x, Y = movePosition.y };
                    network.Send(MessageType.Move, MessagePackSerializer.Serialize(vector2Obj), ENet.PacketFlags.Reliable);

                    GameObject groundPointer = Instantiate(groundPointerPrefab);
                    groundPointer.GetComponent<GroundPointer>().Init(new Vector2(hitInfo.point.x, hitInfo.point.z));
                }
            }

            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                Cursor.SetCursor(null, hotSpot, cursorMode);
            }
            else if (skillState != SkillState.None)
            {
                Cursor.SetCursor(castTexture, hotSpot, cursorMode);
            }
            else if(attackUnit != null)
            {
                Cursor.SetCursor(attackTexture, hotSpot, cursorMode);
            }
            else
            {
                Cursor.SetCursor(moveTexture, hotSpot, cursorMode);
            }            

            if (Input.GetKeyDown(KeyCode.B))
            {
                network.Send(MessageType.Recall, new byte[0], ENet.PacketFlags.Reliable);
            }
            for(KeyCode keyCode = KeyCode.Alpha1; keyCode<KeyCode.Alpha7; keyCode++)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    IconPressed(keyCode);
                }
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                IconPressed(KeyCode.Q);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                IconPressed(KeyCode.W);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                IconPressed(KeyCode.E);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                IconPressed(KeyCode.R);
            }
        }
    }

    void ActivateSkill(int skillSlotNum, SkillState activeState)
    {
        CastType castType;
        Enum.TryParse(network.GetYAMLObject(skillTypes[skillSlotNum]).GetData<string>("CastType"), out castType);
        if (castType == CastType.ImmediateDirection)
        {
            Transform transform = playerUnitInstance.GetComponentInChildren<Canvas>().transform;
            skillInstance = Instantiate(immediateDirectionPrefab, transform);
            skillInstance.GetComponent<ImmediateDirectionUI>().Init(this, network, skillSlotNum, network.GetYAMLObject(skillTypes[skillSlotNum]).GetData<float>("Distance"));

            skillState = activeState;
        }
        else if (castType == CastType.ImmediateCone)
        {
            Transform transform = playerUnitInstance.GetComponentInChildren<Canvas>().transform;
            skillInstance = Instantiate(immediateConePrefab, transform);
            float angle = network.GetYAMLObject(skillTypes[skillSlotNum]).GetData<float>("Angle");
            float radius = network.GetYAMLObject(skillTypes[skillSlotNum]).GetData<float>("Radius");
            skillInstance.GetComponent<ImmediateConeUI>().Init(this, network, skillSlotNum, radius, angle);

            skillState = activeState;
        }
        else if (castType == CastType.ImmediateCircle)
        {
            Transform transform = playerUnitInstance.GetComponentInChildren<Canvas>().transform;
            skillInstance = Instantiate(immediateCirclePrefab, transform);
            float distance = network.GetYAMLObject(skillTypes[skillSlotNum]).GetData<float>("Distance");
            float radius = network.GetYAMLObject(skillTypes[skillSlotNum]).GetData<float>("Radius");
            skillInstance.GetComponent<ImmediateCircleUI>().Init(this, network, skillSlotNum, distance, radius);

            skillState = activeState;
        }
    }

    public void IconPressed(KeyCode keyCode)
    {
        if(chatUI.InputTextActiveInHierarchy())
        {
            return;
        }

        if (keyCode == KeyCode.Alpha1)
        {
            network.Send(MessageType.UseItem, MessagePackSerializer.Serialize((byte)0), ENet.PacketFlags.Reliable);
        }
        else if (keyCode == KeyCode.Alpha2)
        {
            network.Send(MessageType.UseItem, MessagePackSerializer.Serialize((byte)1), ENet.PacketFlags.Reliable);
        }
        else if (keyCode == KeyCode.Alpha3)
        {
            network.Send(MessageType.UseItem, MessagePackSerializer.Serialize((byte)2), ENet.PacketFlags.Reliable);
        }
        else if (keyCode == KeyCode.Alpha4)
        {
            network.Send(MessageType.UseItem, MessagePackSerializer.Serialize((byte)3), ENet.PacketFlags.Reliable);
        }
        else if (keyCode == KeyCode.Alpha5)
        {
            network.Send(MessageType.UseItem, MessagePackSerializer.Serialize((byte)4), ENet.PacketFlags.Reliable);
        }
        else if (keyCode == KeyCode.Alpha6)
        {
            network.Send(MessageType.UseItem, MessagePackSerializer.Serialize((byte)5), ENet.PacketFlags.Reliable);
        }
        else if (keyCode == KeyCode.Q && actives.ContainsKey(0) && actives[0])
        {
            DestroySkillInstance();

            if (skillState != SkillState.QActive)
            {
                ActivateSkill(0, SkillState.QActive);
            }
            else
            {
                skillState = SkillState.None;
            }
        }
        else if (keyCode == KeyCode.W && actives.ContainsKey(1) && actives[1])
        {
            DestroySkillInstance();

            if (skillState != SkillState.WActive)
            {
                ActivateSkill(1, SkillState.WActive);
            }
            else
            {
                skillState = SkillState.None;
            }
        }
        else if (keyCode == KeyCode.E && actives.ContainsKey(2) && actives[2])
        {
            DestroySkillInstance();

            if (skillState != SkillState.EActive)
            {
                ActivateSkill(2, SkillState.EActive);
            }
            else
            {
                skillState = SkillState.None;
            }
        }
        else if (keyCode == KeyCode.R && actives.ContainsKey(3) && actives[3])
        {
            DestroySkillInstance();

            if (skillState != SkillState.RActive)
            {
                ActivateSkill(3, SkillState.RActive);
            }
            else
            {
                skillState = SkillState.None;
            }
        }
    }
}
