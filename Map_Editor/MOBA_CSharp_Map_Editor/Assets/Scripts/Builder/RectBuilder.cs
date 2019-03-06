using UnityEngine;

public class RectBuilder : MonoBehaviour {
    [SerializeField] Transform point0, point1, point2, point3;

    public EdgeInfo[] GetInfo()
    {
        return new EdgeInfo[]
        {
            new EdgeInfo {x0 = point0.position.x, y0 = point0.position.z, x1 = point1.position.x, y1 = point1.position.z},
            new EdgeInfo {x0 = point1.position.x, y0 = point1.position.z, x1 = point2.position.x, y1 = point2.position.z},
            new EdgeInfo {x0 = point2.position.x, y0 = point2.position.z, x1 = point3.position.x, y1 = point3.position.z},
            new EdgeInfo {x0 = point3.position.x, y0 = point3.position.z, x1 = point0.position.x, y1 = point0.position.z},
        };
    }
}
