using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCamera : MonoBehaviour {
    Transform target;
    Vector3 position = new Vector3(0, 10f, -10f);
    Quaternion rotation;
	// Use this for initialization
	void Start () {
        rotation = Quaternion.Euler(45f, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
        if(target != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + position, 0.2f);
            transform.rotation = rotation;
        }
	}

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
