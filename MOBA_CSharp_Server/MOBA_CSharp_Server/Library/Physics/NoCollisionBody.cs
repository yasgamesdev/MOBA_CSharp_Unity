using Microsoft.Xna.Framework;

namespace MOBA_CSharp_Server.Library.Physics
{
    public class NoCollisionBody
    {
        public Vector2 Position;
        public float Radius;

        public NoCollisionBody(Vector2 position, float radius)
        {
            Position = position;
            Radius = radius;
        }
    }
}
