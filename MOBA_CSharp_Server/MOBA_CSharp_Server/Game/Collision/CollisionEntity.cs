using Collision2D;
using ECS;
using System.Numerics;

namespace MOBA_CSharp_Server.Game
{
    public class CollisionEntity : Entity
    {
        UniformGridWorld world;

        public CollisionEntity(RootEntity root) : base(root)
        {
            
        }

        public void CreateWorld(int width, int height)
        {
            world = new UniformGridWorld();
            world.CreateWorld(width, height);
        }

        public Body GenerateStaticBody(Vector2 p0, Vector2 p1)
        {
            return world.GenerateStaticBody(p0, p1);
        }

        public Body GenerateStaticBody(Vector2 center, float radius)
        {
            return world.GenerateStaticBody(center, radius);
        }

        public Body GenerateStaticBody(Vector2[] points)
        {
            return world.GenerateStaticBody(points);
        }

        public DynamicBody GenerateDynamicBody(Vector2 center, float radius)
        {
            return world.GenerateDynamicBody(center, radius);
        }

        public Body[] GetCircleBodies(Vector2 center, float radius)
        {
            return world.GetCircleBodies(center, radius);
        }

        public bool CheckLineOfSight(Vector2 start, Vector2 end)
        {
            return world.CheckLineOfSight(start, end);
        }
    }
}
