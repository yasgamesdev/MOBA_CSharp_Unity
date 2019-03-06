using Microsoft.Xna.Framework;
using MOBA_CSharp_Server.Library.ECS;
using MOBA_CSharp_Server.Library.Physics;
using System.Collections.Generic;

namespace MOBA_CSharp_Server.Game
{
    public class PhysicsEntity : Entity
    {
        VPhysics physics = new VPhysics();

        public PhysicsEntity(Entity root) : base(root)
        {
            AddInheritedType(typeof(PhysicsEntity));
        }

        public void CreateEdgeWall(Vector2 start, Vector2 end)
        {
            physics.CreateEdgeWall(start, end);
        }

        public void CreateCircleWall(float radius, Vector2 position)
        {
            physics.CreateCircleWall(radius, position);
        }

        public void CreateUnit(int unitID, float radius, Vector2 position, CollisionType type)
        {
            physics.CreateUnit(unitID, radius, position, type);
        }

        public void RemoveUnit(int unitID)
        {
            physics.RemoveUnit(unitID);
        }

        public void SetUnitVelocity(int unitID, Vector2 velocity)
        {
            physics.SetUnitVelocity(unitID, velocity);
        }

        public void SetUnitPosition(int unitID, Vector2 position)
        {
            physics.SetUnitPosition(unitID, position);
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            physics.Step(deltaTime);
        }

        public Vector2 GetPosition(int unitID)
        {
            return physics.GetPosition(unitID);
        }

        public List<int> GetUnit(float radius, Vector2 position)
        {
            return physics.GetUnit(radius, position);
        }

        public void CreateBush(IEnumerable<Vector2> vertices)
        {
            physics.CreateBush(vertices);
        }

        public bool CheckLightOfSight(int unitID_0, int unitID_1)
        {
            return physics.CheckLineOfSight(unitID_0, unitID_1);
        }
    }
}
