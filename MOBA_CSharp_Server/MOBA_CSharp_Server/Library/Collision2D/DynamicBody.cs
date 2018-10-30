using Math2D;
using System.Numerics;

namespace Collision2D
{
    public class DynamicBody : Body
    {
        public DynamicBody(Circle circle, UniformGridWorld world) : base(circle, world)
        {
            IsStatic = false;
        }

        public Vector2 MoveTo(Vector2 position)
        {
            return world.MoveTo(this, position);
        }

        public Vector2 Idle()
        {
            return world.MoveTo(this, ((Circle)Shape).Center);
        }

        public Vector2 GetPosition()
        {
            return ((Circle)Shape).Center;
        }

        public void SetCircle(Circle circle)
        {
            Shape = circle;
        }
    }
}