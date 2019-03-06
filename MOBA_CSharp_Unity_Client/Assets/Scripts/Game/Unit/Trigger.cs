using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Network").GetComponent<Network>().TriggeredStart();
    }
}
