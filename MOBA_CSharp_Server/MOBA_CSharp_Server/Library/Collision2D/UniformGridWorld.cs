using Math2D;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace Collision2D
{
    public class UniformGridWorld
    {
        public int width { get; private set; }
        public int height { get; private set; }

        List<Body>[,] uniformGrid;
        Dictionary<Body, List<Vector2Int>> bodies = new Dictionary<Body, List<Vector2Int>>();

        public void CreateWorld(int width, int height)
        {
            this.width = width;
            this.height = height;

            uniformGrid = new List<Body>[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    uniformGrid[x, y] = new List<Body>();
                }
            }
        }

        public Body GenerateStaticBody(Vector2 p0, Vector2 p1)
        {
            Body body = new Body(new Edge(p0, p1), this);
            List<Vector2Int> list = new List<Vector2Int>();

            bodies.Add(body, list);

            SetUniformGrid(body);

            return body;
        }

        public Body GenerateStaticBody(Vector2 center, float radius)
        {
            Body body = new Body(new Circle(center, radius), this);
            List<Vector2Int> list = new List<Vector2Int>();

            bodies.Add(body, list);

            SetUniformGrid(body);

            return body;
        }

        public Body GenerateStaticBody(Vector2[] points)
        {
            Body body = new Body(new Poly(points), this);
            List<Vector2Int> list = new List<Vector2Int>();

            bodies.Add(body, list);

            SetUniformGrid(body);

            return body;
        }

        public DynamicBody GenerateDynamicBody(Vector2 center, float radius)
        {
            DynamicBody body = new DynamicBody(new Circle(center, radius), this);
            List<Vector2Int> list = new List<Vector2Int>();

            bodies.Add(body, list);

            SetUniformGrid(body);

            return body;
        }

        void SetUniformGrid(Body body)
        {
            foreach (Vector2Int item in bodies[body])
            {
                uniformGrid[item.X, item.Y].Remove(body);
            }

            var list = bodies[body];
            list.Clear();

            foreach (Vector2Int item in body.Shape.GetGrid())
            {
                if (IsInside(item.X, item.Y))
                {
                    uniformGrid[item.X, item.Y].Add(body);
                    list.Add(item);
                }
            }
        }

        public void Destroy(Body body)
        {
            foreach (Vector2Int item in bodies[body])
            {
                uniformGrid[item.X, item.Y].Remove(body);
            }

            bodies.Remove(body);
        }

        public Vector2 MoveTo(DynamicBody body, Vector2 position)
        {
            Circle destCircle = new Circle(position, ((Circle)body.Shape).Radius);

            List<Body> overlapBodies = new List<Body>();
            foreach (Vector2Int coordinate in destCircle.GetGrid())
            {
                if (IsInside(coordinate.X, coordinate.Y))
                {
                    foreach (Body overlapBody in uniformGrid[coordinate.X, coordinate.Y])
                    {
                        if (overlapBody != body)
                        {
                            overlapBodies.Add(overlapBody);
                        }
                    }
                }
            }

            overlapBodies = overlapBodies.Distinct().ToList();

            if (overlapBodies.Count == 0)
            {
                body.SetCircle(destCircle);
                SetUniformGrid(body);
                return ((Circle)body.Shape).Center;
            }

            Vector2 max = Vector2.Zero;
            bool isStatic = false;
            foreach (Body overlapBody in overlapBodies)
            {
                Vector2 notOverlap = overlapBody.Shape.GetNotOverlapVector(destCircle.Center, destCircle.Radius);
                if(notOverlap != Vector2.Zero)
                {
                    if(overlapBody.IsStatic && !isStatic)
                    {
                        max = notOverlap;
                        isStatic = true;
                    }
                    else if(overlapBody.IsStatic == isStatic && notOverlap.LengthSquared() > max.LengthSquared())
                    {
                        max = notOverlap;
                    }
                }
            }

            if (max != Vector2.Zero)
            {
                Circle finalCircle = new Circle(position + max, ((Circle)body.Shape).Radius);
                body.SetCircle(finalCircle);
                SetUniformGrid(body);
                return ((Circle)body.Shape).Center;
            }
            else
            {
                body.SetCircle(destCircle);
                SetUniformGrid(body);
                return ((Circle)body.Shape).Center;
            }
        }

        bool IsInside(int x, int y)
        {
            return 0 <= x && x < width && 0 <= y && y < height;
        }

        public Body[] GetCircleBodies(Vector2 center, float radius)
        {
            Circle circle0 = new Circle(center, radius);

            List<Body> results = new List<Body>();

            foreach (Vector2Int item in circle0.GetGrid())
            {
                if (IsInside(item.X, item.Y))
                {
                    foreach(Body body in uniformGrid[item.X, item.Y])
                    {
                        Circle circle1 = body.Shape as Circle;
                        if(circle1 != null)
                        {
                            if ((circle1.Center - circle0.Center).Length() <= radius)
                            {
                                results.Add(body);
                            }
                        }
                    }
                }
            }

            return results.Distinct().ToArray();
        }

        public bool CheckLineOfSight(Vector2 start, Vector2 end)
        {
            Edge edge = new Edge(start, end);

            List<Body> staticBodies = new List<Body>();

            foreach (Vector2Int item in edge.GetGrid())
            {
                if (IsInside(item.X, item.Y))
                {
                    foreach(Body body in uniformGrid[item.X, item.Y])
                    {
                        if(body.IsStatic)
                        {
                            staticBodies.Add(body);
                        }
                    }
                }
            }

            staticBodies = staticBodies.Distinct().ToList();
            return !staticBodies.Any(x => x.Shape.IntersectsLine(start, end));
        }
    }
}