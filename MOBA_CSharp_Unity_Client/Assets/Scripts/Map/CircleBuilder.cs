using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleBuilder : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.x);
    }

    public bool IsXZScaleEqual()
    {
        return transform.localScale.x == transform.localScale.z;
    }

    public CircleJson GetJson()
    {
        return new CircleJson()
        {
            center = new System.Numerics.Vector2(transform.position.x, transform.position.z),
            radius = transform.localScale.x * 0.5f
        };
    }

    public void Init(CircleJson circle)
    {
        transform.position = new Vector3(circle.center.X, 0, circle.center.Y);
        transform.localScale = new Vector3(circle.radius * 2.0f, 1f, circle.radius * 2.0f);
    }
}
