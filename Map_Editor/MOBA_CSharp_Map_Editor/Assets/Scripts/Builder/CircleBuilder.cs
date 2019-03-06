using UnityEngine;

public class CircleBuilder : MonoBehaviour {
	void Update () {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.x);
    }

    public bool IsXZScaleEqual()
    {
        return transform.localScale.x == transform.localScale.z;
    }

    public CircleInfo GetInfo()
    {
        return new CircleInfo()
        {
            x = transform.position.x,
            y = transform.position.z,
            radius = transform.localScale.x * 0.5f
        };
    }

    public void Init(CircleInfo info)
    {
        transform.position = new Vector3(info.x, 0, info.y);
        transform.localScale = new Vector3(info.radius * 2.0f, 1f, info.radius * 2.0f);
    }
}
