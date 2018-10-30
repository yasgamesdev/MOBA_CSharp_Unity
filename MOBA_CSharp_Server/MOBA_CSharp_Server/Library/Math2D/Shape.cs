using System.Numerics;

namespace Math2D
{
    public abstract class Shape
    {
        public abstract Vector2Int[] GetGrid();
        public abstract Vector2 GetNotOverlapVector(Vector2 center, float radius);
        public abstract bool IntersectsLine(Vector2 p0, Vector2 p1);
    }
}
