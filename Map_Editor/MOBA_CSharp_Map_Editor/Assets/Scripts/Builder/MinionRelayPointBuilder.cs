using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionRelayPointBuilder : MonoBehaviour
{
    [SerializeField] public bool blueTeam;
    [SerializeField] int laneNum; //Top:0, Mid:1, Bot:2
    [SerializeField] int index;

    public MinionRelayPointInfo GetInfo()
    {
        return new MinionRelayPointInfo()
        {
            x = transform.position.x,
            y = transform.position.z,
            blueTeam = blueTeam,
            laneNum = laneNum,
            index = index
        };
    }
}
