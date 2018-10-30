using Math2D;

namespace Collision2D
{
    public class Body
    {
        protected UniformGridWorld world;
        public Shape Shape { get; protected set; }
        public bool IsStatic { get; protected set; }

        public Body(Shape shape, UniformGridWorld world)
        {
            this.world = world;
            Shape = shape;
            IsStatic = true;
        }

        public void Destroy()
        {
            world.Destroy(this);
        }
    }
}