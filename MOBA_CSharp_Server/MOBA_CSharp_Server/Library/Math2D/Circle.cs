using System;
using System.Collections.Generic;
using System.Numerics;

namespace Math2D
{
    public class Circle : Shape
    {
        public Vector2 Center;
        public float Radius;

        public Circle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public override Vector2Int[] GetGrid()
        {
            List<Vector2Int> results = new List<Vector2Int>();

            for (int x = (int)Math.Floor(Center.X - Radius); x <= (int)Math.Floor(Center.X + Radius); x++)
            {
                for (int y = (int)Math.Floor(Center.Y - Radius); y <= (int)Math.Floor(Center.Y + Radius); y++)
                {
                    if (GetDistanceFromGrid(new Vector2(x, y), new Vector2(x + 1, y + 1)) < 0)
                    {
                        results.Add(new Vector2Int(x, y));
                    }
                }
            }

            return results.ToArray();
        }

        float GetDistanceFromGrid(Vector2 min, Vector2 max)
        {
            Vector2 point = Vector2.Clamp(Center, min, max);

            float length = (Center - point).Length();

            return length - Radius;
        }

        public override Vector2 GetNotOverlapVector(Vector2 center, float radius)
        {
            float dist = (Center - center).Length();

            if (dist >= Radius + radius)
            {
                return Vector2.Zero;
            }

            if (dist != 0)
            {
                return ((center - Center) / dist) * (Radius + radius - dist);
            }
            else
            {
                Random rand = new Random();
                Vector2 randVector2 = new Vector2((float)(rand.NextDouble() - 0.5), (float)(rand.NextDouble() - 0.5));
                return (randVector2 / randVector2.Length()) * (Radius + radius);
            }
        }

        public override bool IntersectsLine(Vector2 p0, Vector2 p1)
        {
            Vector2 intersection0, intersection1;
            return Math2DHelper.FindLineCircleIntersections(Center.X, Center.Y, Radius, p0, p1, out intersection0, out intersection1) >= 1;
        }
    }
}
