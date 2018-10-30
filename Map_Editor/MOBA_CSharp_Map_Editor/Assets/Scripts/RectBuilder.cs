using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectBuilder : MonoBehaviour {
    [SerializeField] Transform point0, point1, point2, point3;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public PolyJson GetJson()
    {
        return new PolyJson()
        {
            points = new System.Numerics.Vector2[]
            {
                new System.Numerics.Vector2(point0.position.x, point0.position.z),
                new System.Numerics.Vector2(point1.position.x, point1.position.z),
                new System.Numerics.Vector2(point2.position.x, point2.position.z),
                new System.Numerics.Vector2(point3.position.x, point3.position.z),
            }
        };
    }
}
