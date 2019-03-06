using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreBuilder : MonoBehaviour
{
    [SerializeField] public bool blueTeam;
    [SerializeField] float radius;

    public CoreInfo GetInfo()
    {
        return new CoreInfo()
        {
            x = transform.position.x,
            y = transform.position.z,
            angle = transform.eulerAngles.y,
            radius = radius
        };
    }
}
