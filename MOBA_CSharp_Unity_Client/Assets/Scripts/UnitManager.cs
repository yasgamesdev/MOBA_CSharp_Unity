using MOBA_CSharp_Server.MsgPackObjs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {
    [SerializeField] GameObject playerPrefab;

    Dictionary<int, GameObject> playerInstances = new Dictionary<int, GameObject>();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetSnapshot(SnapshotData snapshotData)
    {
        Dictionary<int, GameObject> copy = new Dictionary<int, GameObject>(playerInstances);

        foreach(var playerData in snapshotData.playerDatas)
        {
            if(playerInstances.ContainsKey(playerData.EntityID))
            {
                playerInstances[playerData.EntityID].GetComponent<PlayerScript>().UpdateData(playerData);
                copy.Remove(playerData.EntityID);
            }
            else
            {
                GameObject newPlayerInstance = Instantiate(playerPrefab);
                newPlayerInstance.GetComponent<PlayerScript>().Init(playerData);
                playerInstances.Add(playerData.EntityID, newPlayerInstance);
            }
        }

        foreach(var remove in copy)
        {
            GameObject removeGameObject = playerInstances[remove.Key];
            playerInstances.Remove(remove.Key);
            Destroy(removeGameObject);
        }
    }
}
