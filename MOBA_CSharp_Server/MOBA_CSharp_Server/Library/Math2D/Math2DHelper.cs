using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace Math2D
{
    public static class Math2DHelper
    {
        public static bool LineIntersectsRect(Vector2 p0, Vector2 p1, Vector2 min, Vector2 max)
        {
            return LineIntersectsLine(p0, p1, new Vector2(min.X, min.Y), new Vector2(max.X, min.Y)) ||
                   LineIntersectsLine(p0, p1, new Vector2(min.X, max.Y), new Vector2(min.X, min.Y)) ||
                   LineIntersectsLine(p0, p1, new Vector2(max.X, max.Y), new Vector2(min.X, max.Y)) ||
                   LineIntersectsLine(p0, p1, new Vector2(max.X, min.Y), new Vector2(max.X, max.Y)) ||
                   (RectangleContainsPoint(min, max, p0) && RectangleContainsPoint(min, max, p1));
        }

        public static bool PolyIntersectsRect(Vector2[] points, Vector2 min, Vector2 max)
        {
            GeometryFactory factory = new GeometryFactory();

            List<Coordinate> coordinates = new List<Coordinate>();
            foreach (Vector2 point in points)
            {
                coordinates.Add(new Coordinate(point.X, point.Y));
            }
            coordinates.Add(new Coordinate(points[0].X, points[0].Y));
            var polygon = factory.CreatePolygon(coordinates.ToArray());

            var rect = factory.CreatePolygon(new Coordinate[]
            {
                new Coordinate(min.X, min.Y),
                new Coordinate(min.X, max.Y),
                new Coordinate(max.X, max.Y),
                new Coordinate(max.X, min.Y),
                new Coordinate(min.X, min.Y),
            });

            return polygon.Intersects(rect);
        }

        public static bool LineIntersectsLine(Vector2 a0, Vector2 a1, Vector2 b0, Vector2 b1)
        {
            float q = (a0.Y - b0.Y) * (b1.X - b0.X) - (a0.X - b0.X) * (b1.Y - b0.Y);
            float d = (a1.X - a0.X) * (b1.Y - b0.Y) - (a1.Y - a0.Y) * (b1.X - b0.X);

            if (d == 0)
            {
                return false;
            }

            float r = q / d;

            q = (a0.Y - b0.Y) * (a1.X - a0.X) - (a0.X - b0.X) * (a1.Y - a0.Y);
            float s = q / d;

            if (r <= 0 || r >= 1 || s <= 0 || s >= 1)
            {
                return false;
            }

            return true;
        }

        public static bool RectangleContainsPoint(Vector2 min, Vector2 max, Vector2 p)
        {
            return min.X <= p.X && p.X <= max.X && min.Y <= p.Y && p.Y <= max.Y;
        }

        public static bool PolygonContainsPoint(Vector2[] polygon, Vector2 point)
        {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Y < point.Y && polygon[j].Y >= point.Y || polygon[j].Y < point.Y && polygon[i].Y >= point.Y)
                {
                    if (polygon[i].X + (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < point.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        public static bool IsInPolygon(Vector2[] poly, Vector2 point)
        {
            bool result = false;
            var a = poly.Last();
            foreach (var b in poly)
            {
                if ((b.X == point.X) && (b.Y == point.Y))
                    return true;

                if ((b.Y == a.Y) && (point.Y == a.Y) && (a.X <= point.X) && (point.X <= b.X))
                    return true;

                if ((b.Y < point.Y) && (a.Y >= point.Y) || (a.Y < point.Y) && (b.Y >= point.Y))
                {
                    if (b.X + (point.Y - b.Y) / (a.Y - b.Y) * (a.X - b.X) <= point.X)
                        result = !result;
                }
                a = b;
            }
            return result;
        }

        public static bool IsPointOnLine(Vector2 p0, Vector2 p1, Vector2 point)
        {
            return (point - p0).Length() + (p1 - point).Length() == (p1 - p0).Length();
        }

        public static bool IsInPolygonStrict(Vector2[] poly, Vector2 point)
        {
            if (IsInPolygon(poly, point))
            {
                for (int i = 0; i < poly.Length; i++)
                {
                    Vector2 p0 = poly[i];
                    Vector2 p1 = poly[(i + 1) % poly.Length];
                    if (IsPointOnLine(p0, p1, point))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public static int FindLineCircleIntersections(float cx, float cy, float radius,
        Vector2 point1, Vector2 point2, out Vector2 intersection1, out Vector2 intersection2)
        {
            float dx, dy, A, B, C, det, t;

            dx = point2.X - point1.X;
            dy = point2.Y - point1.Y;

            A = dx * dx + dy * dy;
            B = 2 * (dx * (point1.X - cx) + dy * (point1.Y - cy));
            C = (point1.X - cx) * (point1.X - cx) + (point1.Y - cy) * (point1.Y - cy) - radius * radius;

            det = B * B - 4 * A * C;
            if ((A <= 0.0000001) || (det < 0))
            {
                // No real solutions.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            else if (det == 0)
            {
                if ((float)((-B + Math.Sqrt(det)) / (2 * A)) == (float)((-B - Math.Sqrt(det)) / (2 * A)))
                {
                    t = (float)((-B + Math.Sqrt(det)) / (2 * A));
                    intersection1 = new Vector2(point1.X + t * dx, point1.Y + t * dy);
                    t = (float)((-B - Math.Sqrt(det)) / (2 * A));
                    intersection2 = new Vector2(point1.X + t * dx, point1.Y + t * dy);
                    return 2;
                }
                // One solution.
                t = -B / (2 * A);
                intersection1 = new Vector2(point1.X + t * dx, point1.Y + t * dy);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 1;
            }
            else
            {
                //// Two solutions.
                //t = (float)((-B + Math.Sqrt(det)) / (2 * A));
                //intersection1 = new Vector2(point1.X + t * dx, point1.Y + t * dy);
                //t = (float)((-B - Math.Sqrt(det)) / (2 * A));
                //intersection2 = new Vector2(point1.X + t * dx, point1.Y + t * dy);
                //return 2;

                float t0 = (float)((-B + Math.Sqrt(det)) / (2 * A));
                float t1 = (float)((-B - Math.Sqrt(det)) / (2 * A));
                if((0 <= t0 && t0 <= 1) && (0 <= t1 && t1 <= 1))
                {
                    t = (float)((-B + Math.Sqrt(det)) / (2 * A));
                    intersection1 = new Vector2(point1.X + t * dx, point1.Y + t * dy);
                    t = (float)((-B - Math.Sqrt(det)) / (2 * A));
                    intersection2 = new Vector2(point1.X + t * dx, point1.Y + t * dy);
                    return 2;
                }
                else if ((0 <= t0 && t0 <= 1))
                {
                    t = (float)((-B + Math.Sqrt(det)) / (2 * A));
                    intersection1 = new Vector2(point1.X + t * dx, point1.Y + t * dy);
                    intersection2 = new Vector2(float.NaN, float.NaN);
                    return 1;
                }
                else if ((0 <= t1 && t1 <= 1))
                {
                    t = (float)((-B - Math.Sqrt(det)) / (2 * A));
                    intersection1 = new Vector2(point1.X + t * dx, point1.Y + t * dy);
                    intersection2 = new Vector2(float.NaN, float.NaN);
                    return 1;
                }
                else
                {
                    intersection1 = new Vector2(float.NaN, float.NaN);
                    intersection2 = new Vector2(float.NaN, float.NaN);
                    return 0;
                }
            }
        }

        public static bool PolyIntersectsLine(Vector2[] points, Vector2 p0, Vector2 p1)
        {
            GeometryFactory factory = new GeometryFactory();

            List<Coordinate> coordinates = new List<Coordinate>();
            foreach (Vector2 point in points)
            {
                coordinates.Add(new Coordinate(point.X, point.Y));
            }
            coordinates.Add(new Coordinate(points[0].X, points[0].Y));
            var polygon = factory.CreatePolygon(coordinates.ToArray());

            var line = factory.CreateLineString(new Coordinate[] { new Coordinate(p0.X, p0.Y), new Coordinate(p1.X, p1.Y) });

            return polygon.Intersects(line);
        }
    }
}
