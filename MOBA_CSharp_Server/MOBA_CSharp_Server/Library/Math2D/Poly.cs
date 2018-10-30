using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Math2D
{
    public class Poly : Shape
    {
        public Vector2[] Points;

        public Poly(Vector2[] points)
        {
            Points = points;
        }

        public override Vector2Int[] GetGrid()
        {
            List<Vector2Int> results = new List<Vector2Int>();

            Vector2 min = new Vector2(Points.Min(x => x.X), Points.Min(x => x.Y));
            Vector2 max = new Vector2(Points.Max(x => x.X), Points.Max(x => x.Y));

            for (int x = (int)Math.Floor(min.X); x <= (int)Math.Floor(max.X); x++)
            {
                for (int y = (int)Math.Floor(min.Y); y <= (int)Math.Floor(max.Y); y++)
                {
                    if (Math2DHelper.PolyIntersectsRect(Points, new Vector2(x, y), new Vector2(x + 1, y + 1)))
                    {
                        results.Add(new Vector2Int(x, y));
                    }
                }
            }

            return results.ToArray();
        }

        public override Vector2 GetNotOverlapVector(Vector2 center, float radius)
        {
            Vector2 max = Vector2.Zero;

            for (int i = 0; i < Points.Length; i++)
            {
                Vector2 p0 = Points[i];
                Vector2 p1 = Points[(i + 1) % Points.Length];
                Edge edge = new Edge(p0, p1);
                Vector2 notOverlap = edge.GetNotOverlapVector(center, radius);
                if (notOverlap.LengthSquared() >= max.LengthSquared())
                {
                    max = notOverlap;
                }
            }

            return max;
        }

        public override bool IntersectsLine(Vector2 p0, Vector2 p1)
        {
            return Math2DHelper.PolyIntersectsLine(Points, p0, p1);
        }
    }
}
