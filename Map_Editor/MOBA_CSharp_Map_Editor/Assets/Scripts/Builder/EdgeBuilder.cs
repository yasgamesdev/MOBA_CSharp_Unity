using UnityEngine;

public class EdgeBuilder : MonoBehaviour {
    [SerializeField] Transform point0, point1;

    public EdgeInfo GetInfo()
    {
        return new EdgeInfo() {
            x0 = point0.position.x,
            x1 = point1.position.x,
            y0 = point0.position.z,
            y1 = point1.position.z
        };
    }

    public void Init(EdgeInfo info)
    {
        Vector2 p0 = new Vector2(info.x0, info.x1);
        Vector2 p1 = new Vector2(info.y0, info.y1);
        Vector2 center = (p0 + p1) * 0.5f;
        transform.position = new Vector3(center.x, 0, center.y);

        float angle = Mathf.Atan2((p1 - p0).y, (p1 - p0).x);
        transform.eulerAngles = new Vector3(0, -angle * Mathf.Rad2Deg, 0);

        transform.localScale = new Vector3((p1 - p0).magnitude, 1f, 1f);
    }
}
