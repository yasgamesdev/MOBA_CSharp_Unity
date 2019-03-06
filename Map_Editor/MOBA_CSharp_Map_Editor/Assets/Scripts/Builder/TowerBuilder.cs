using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBuilder : MonoBehaviour
{
    [SerializeField] public bool blueTeam;
    [SerializeField] float radius;
    [SerializeField] float height;

    public TowerInfo GetInfo()
    {
        return new TowerInfo()
        {
            x = transform.position.x,
            y = transform.position.z,
            angle = transform.eulerAngles.y,
            blueTeam = blueTeam,
            radius = radius,
            height = height
        };
    }
}
