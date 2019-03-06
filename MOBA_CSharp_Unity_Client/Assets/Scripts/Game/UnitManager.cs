using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    [SerializeField] GameObject unitPrefab;
    Dictionary<int, GameObject> unitInstances = new Dictionary<int, GameObject>();

    [SerializeField] PlayerUI playerUI;
    [SerializeField] RecallUI recallUI;
    [SerializeField] FrontUI frontUI;

    SnapshotObj snapshotObj;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public SnapshotObj GetSnapshotObj()
    {
        return snapshotObj;
    }

    public void SetSnapshot(SnapshotObj snapshotObj)
    {
        this.snapshotObj = snapshotObj;

        Dictionary<int, GameObject> removeInstances = new Dictionary<int, GameObject>(unitInstances);

        UpdateData(snapshotObj.ChampionObjs, removeInstances);
        UpdateData(snapshotObj.BuildingObjs, removeInstances);
        UpdateData(snapshotObj.Vector3NoAnimObjs, removeInstances);
        UpdateData(snapshotObj.UnitObjs, removeInstances);

        foreach (var remove in removeInstances)
        {
            GameObject removeGameObject = unitInstances[remove.Key];
            unitInstances.Remove(remove.Key);
            Destroy(removeGameObject);
        }

        if (unitInstances.ContainsKey(snapshotObj.PlayerObj.UnitID))
        {
            GameObject playerUnitInstance = unitInstances[snapshotObj.PlayerObj.UnitID];
            PlayerObj playerObj = snapshotObj.PlayerObj;
            ChampionObj championObj = (ChampionObj)playerUnitInstance.GetComponent<Unit>().Info;

            Camera.main.GetComponent<RTSCamera>().SetTarget(playerUnitInstance.transform);
            playerController.SetPlayerUnit(playerUnitInstance);
            playerController.SetPlayerObj(playerObj);
            playerUI.SetPlayerData(playerObj, championObj);
            recallUI.SetActive(championObj.AnimationNum == (byte)AnimationType.Recall);
            frontUI.SetPlayerData(playerObj, championObj);
            frontUI.SetData(snapshotObj.ClientObjs);
        }
        else
        {
            Camera.main.GetComponent<RTSCamera>().SetTarget(null);
            playerController.SetPlayerUnit(null);
        }
    }

    void UpdateData(IGetUnitInfo[] infos, Dictionary<int, GameObject> removeInstances)
    {
        foreach (var info in infos)
        {
            if (unitInstances.ContainsKey(info.GetUnitID()))
            {
                unitInstances[info.GetUnitID()].GetComponent<Unit>().UpdateData(info);
                removeInstances.Remove(info.GetUnitID());
            }
            else
            {
                GameObject newUnitInstance = Instantiate(unitPrefab);
                newUnitInstance.GetComponent<Unit>().Init(info);
                unitInstances.Add(info.GetUnitID(), newUnitInstance);
            }
        }
    }
}
