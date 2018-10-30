using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PolyBuilder : MonoBehaviour {
    [SerializeField] GameObject edgePrefab;
    List<GameObject> edgeInstances = new List<GameObject>();
    Vector2[] pointsCopy;
	// Use this for initialization
	void Start () {
        PolygonCollider2D collider = GetComponentInChildren<PolygonCollider2D>();
        pointsCopy = collider.points;
        for(int i=0; i<collider.points.Length; i++)
        {
            Vector3 point0, point1;
            if(i != collider.points.Length -1)
            {
                point0 = collider.points[i];
                point1 = collider.points[i+1];
            }
            else
            {
                point0 = collider.points[i];
                point1 = collider.points[0];
            }

            Vector3 v = point1 - point0;
            float angle = Mathf.Rad2Deg * Mathf.Atan2(v.y, v.x);
            GameObject edgeInstance = Instantiate(edgePrefab, transform);
            edgeInstance.transform.localPosition = new Vector3(((point1 + point0) * 0.5f).x, 0, ((point1 + point0) * 0.5f).y);
            edgeInstance.transform.eulerAngles = new Vector3(0, -angle, 0);
            edgeInstance.transform.localScale = new Vector3(v.magnitude, 1f, 1f);
            edgeInstance.tag = "Untagged";
            edgeInstances.Add(edgeInstance);
        }

        Destroy(collider);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public PolyJson GetJson()
    {
        return new PolyJson()
        {
            //points = pointsCopy.Select(x => new System.Numerics.Vector2(x.x + transform.position.x, x.y + transform.position.z)).ToArray()
            points = pointsCopy.Select(x => new System.Numerics.Vector2(transform.TransformPoint(x.x, 0, x.y).x, transform.TransformPoint(x.x, 0, x.y).z)).ToArray()
        };
    }

    public void Init(PolyJson poly)
    {
        PolygonCollider2D collider = GetComponentInChildren<PolygonCollider2D>();
        collider.points = poly.points.Select(x => new Vector2(x.X, x.Y)).ToArray();
    }
}
