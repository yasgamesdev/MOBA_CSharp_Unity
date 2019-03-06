using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBuilder : MonoBehaviour
{
    [SerializeField] public bool blueTeam;
    [SerializeField] float regainRadius;

    public SpawnInfo GetInfo()
    {
        return new SpawnInfo()
        {
            x = transform.position.x,
            y = transform.position.z,
            regainRadius = regainRadius
        };
    }
}
