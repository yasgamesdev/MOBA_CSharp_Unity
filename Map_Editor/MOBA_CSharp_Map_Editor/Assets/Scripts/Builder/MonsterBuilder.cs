using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBuilder : MonoBehaviour
{
    [SerializeField] UnitType type;
    [SerializeField] float chaseRadius;
    [SerializeField] float respawnTime;

    public MonsterInfo GetInfo()
    {
        return new MonsterInfo()
        {
            type = type,
            x = transform.position.x,
            y = transform.position.z,
            angle = transform.eulerAngles.y,
            chaseRadius = chaseRadius,
            respawnTime = respawnTime
        };
    }
}
