using Microsoft.Xna.Framework;
using MOBA_CSharp_Server.Library.ECS;
using MOBA_CSharp_Server.Library.Physics;

namespace MOBA_CSharp_Server.Game
{
    public class Transform : UnitComponent
    {
        public Vector2 Position { get; private set; }
        public float Height { get; private set; }
        public float Rotation { get; private set; }
        public bool Warped { get; private set; }
        public CollisionType Type { get; private set; }
        public float Radius { get; private set; }

        public Transform(Vector2 position, float height, float rotation, CollisionType type, float radius, Unit unitRoot, Entity root) : base(unitRoot, root)
        {
            AddInheritedType(typeof(Transform));

            Position = position;
            Height = height;
            Rotation = rotation;
            Warped = false;
            Type = type;
            Radius = radius;

            root.GetChild<PhysicsEntity>().CreateUnit(unitRoot.UnitID, radius, position, type);
        }

        public void SetHeight(float height)
        {
            Height = height;
        }

        public void SetRadius(float radius)
        {
            Radius = radius;

            Root.GetChild<PhysicsEntity>().RemoveUnit(unitRoot.UnitID);
            Root.GetChild<PhysicsEntity>().CreateUnit(unitRoot.UnitID, Radius, Position, Type);
        }

        public override void ClearReference()
        {
            base.ClearReference();

            if(Type != CollisionType.Static)
            {
                Root.GetChild<PhysicsEntity>().RemoveUnit(unitRoot.UnitID);
            }
        }

        public void SetVelocity(Vector2 velocity)
        {
            Root.GetChild<PhysicsEntity>().SetUnitVelocity(unitRoot.UnitID, velocity);
        }

        public void SetRotation(float rotation)
        {
            Rotation = rotation;
        }

        public void SetPositionWith3D(Vector3 position)
        {
            Position = new Vector2(position.X, position.Z);
            Height = position.Y;

            Root.GetChild<PhysicsEntity>().SetUnitPosition(unitRoot.UnitID, Position);
        }

        public Vector3 GetPositionWith3D()
        {
            return new Vector3(Position.X, Height, Position.Y);
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            Position = Root.GetChild<PhysicsEntity>().GetPosition(unitRoot.UnitID);
        }

        public void Warp(Vector2 position, float height, float rotation)
        {
            Root.GetChild<PhysicsEntity>().RemoveUnit(unitRoot.UnitID);
            Root.GetChild<PhysicsEntity>().CreateUnit(unitRoot.UnitID, Radius, position, Type);

            Position = position;
            Height = height;
            Rotation = rotation;
            Warped = true;
        }

        public void ResetWarped()
        {
            Warped = false;
        }
    }
}
