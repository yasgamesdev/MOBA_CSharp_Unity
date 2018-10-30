using System;
using System.Collections.Generic;
using System.Numerics;

namespace Math2D
{
    public class Edge : Shape
    {
        public Vector2 P0;
        public Vector2 P1;

        public Edge(Vector2 p0, Vector2 p1)
        {
            P0 = p0;
            P1 = p1;
        }

        public override Vector2Int[] GetGrid()
        {
            List<Vector2Int> results = new List<Vector2Int>();

            Vector2 min = new Vector2(Math.Min(P0.X, P1.X), Math.Min(P0.Y, P1.Y));
            Vector2 max = new Vector2(Math.Max(P0.X, P1.X), Math.Max(P0.Y, P1.Y));

            for (int x = (int)Math.Floor(min.X); x <= (int)Math.Floor(max.X); x++)
            {
                for (int y = (int)Math.Floor(min.Y); y <= (int)Math.Floor(max.Y); y++)
                {
                    if (Math2DHelper.LineIntersectsRect(P0, P1, new Vector2(x, y), new Vector2(x + 1, y + 1)))
                    {
                        results.Add(new Vector2Int(x, y));
                    }
                }
            }

            return results.ToArray();
        }

        public override Vector2 GetNotOverlapVector(Vector2 center, float radius)
        {
            Vector2 intersection0, intersection1;
            int count = Math2DHelper.FindLineCircleIntersections(center.X, center.Y, radius, P0, P1, out intersection0, out intersection1);

            if (count == 0)
            {
                return Vector2.Zero;
            }
            else if (count == 1)
            {
                if ((intersection0 - P0).LengthSquared() <= (P1 - intersection0).LengthSquared())
                {
                    Vector2 line = P0 - P1;
                    Vector2 cross = new Vector2(line.Y, -line.X);

                    Vector2 up = Vector2.Normalize(line) * (intersection0 - P0).Length();

                    Vector2 right = Vector2.Normalize(cross) * Vector2.Dot(intersection0 - center, Vector2.Normalize(cross));
                    Vector2 right1 = -Vector2.Normalize(right) * (radius - right.Length());

                    if (up.LengthSquared() <= right1.LengthSquared())
                    {
                        return up;
                    }
                    else
                    {
                        return right1;
                    }
                }
                else
                {
                    Vector2 line = P1 - P0;
                    Vector2 cross = new Vector2(line.Y, -line.X);

                    Vector2 up = Vector2.Normalize(line) * (intersection0 - P1).Length();

                    Vector2 right = Vector2.Normalize(cross) * Vector2.Dot(intersection0 - center, Vector2.Normalize(cross));
                    Vector2 right1 = -Vector2.Normalize(right) * (radius - right.Length());

                    if (up.LengthSquared() <= right1.LengthSquared())
                    {
                        return up;
                    }
                    else
                    {
                        return right1;
                    }
                }
            }
            else
            {
                Vector2 middle = (intersection0 + intersection1) * 0.5f;
                if (center == middle)
                {
                    Vector2 line = P0 - P1;
                    Vector2 cross = new Vector2(line.Y, -line.X);
                    return Vector2.Normalize(cross) * radius;
                }
                else
                {
                    return Vector2.Normalize((center - middle)) * (radius - (center - middle).Length());
                }
            }
        }

        public override bool IntersectsLine(Vector2 p0, Vector2 p1)
        {
            return Math2DHelper.LineIntersectsLine(P0, P1, p0, p1);
        }
    }
}
