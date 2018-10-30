using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeBuilder : MonoBehaviour {
    [SerializeField] Transform point0, point1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public EdgeJson GetJson()
    {
        return new EdgeJson() {
            point0 = new System.Numerics.Vector2(point0.position.x, point0.position.z),
            point1 = new System.Numerics.Vector2(point1.position.x, point1.position.z)
        };
    }

    public void Init(EdgeJson edge)
    {
        System.Numerics.Vector2 center = (edge.point0 + edge.point1) * 0.5f;
        transform.position = new Vector3(center.X, 0, center.Y);

        float angle = Mathf.Atan2((edge.point1 - edge.point0).Y, (edge.point1 - edge.point0).X);
        transform.eulerAngles = new Vector3(0, -angle * Mathf.Rad2Deg, 0);

        transform.localScale = new Vector3((edge.point1 - edge.point0).Length(), 1f, 1f);
    }
}
