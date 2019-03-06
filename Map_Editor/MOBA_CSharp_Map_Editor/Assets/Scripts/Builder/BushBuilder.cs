using UnityEngine;

public class BushBuilder : MonoBehaviour {
    [SerializeField] Transform point0, point1, point2, point3;

    public BushInfo GetInfo()
    {
        return new BushInfo() {
            x0 = point0.position.x,
            x1 = point1.position.x,
            x2 = point2.position.x,
            x3 = point3.position.x,
            y0 = point0.position.z,
            y1 = point1.position.z,
            y2 = point2.position.z,
            y3 = point3.position.z
        };
    }
}
