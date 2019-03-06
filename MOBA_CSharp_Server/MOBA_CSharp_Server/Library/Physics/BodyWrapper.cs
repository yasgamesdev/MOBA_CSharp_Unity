using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace MOBA_CSharp_Server.Library.Physics
{
    public class BodyWrapper
    {
        public bool IsCollisionBody { get; private set; }
        public Body Body { get; private set; }
        public NoCollisionBody NoCollisionBody { get; private set; }

        public BodyWrapper(Body body)
        {
            IsCollisionBody = true;
            Body = body;
            NoCollisionBody = null;
        }

        public BodyWrapper(NoCollisionBody noCollisionBody)
        {
            IsCollisionBody = false;
            Body = null;
            NoCollisionBody = noCollisionBody;
        }

        public void RemoveBody(World world)
        {
            if(IsCollisionBody)
            {
                world.RemoveBody(Body);
            }
        }

        public void SetUnitVelocity(Vector2 velocity)
        {
            if (IsCollisionBody)
            {
                Body.LinearVelocity = velocity;
            }
        }

        public void ResetVelocity()
        {
            if (IsCollisionBody)
            {
                Body.LinearVelocity = Vector2.Zero;
            }
        }

        public void SetUnitPosition(Vector2 position)
        {
            if(!IsCollisionBody)
            {
                NoCollisionBody.Position = position;
            }
        }

        public Vector2 GetPosition()
        {
            if (IsCollisionBody)
            {
                return Body.Position;
            }
            else
            {
                return NoCollisionBody.Position;
            }
        }
    }
}
