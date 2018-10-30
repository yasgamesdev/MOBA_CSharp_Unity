namespace ECS
{
    public abstract class Component
    {
        protected RootEntity root { get; private set; }

        public Component(RootEntity root)
        {
            this.root = root;
        }

        public abstract void Step(float deltaTime);
    }
}
