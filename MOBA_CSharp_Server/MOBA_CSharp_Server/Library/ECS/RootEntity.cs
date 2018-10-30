namespace ECS
{
    public class RootEntity : Entity
    {
        public RootEntity() : base()
        {
            SetEntityID(0);
            SetRoot(this);

            AddChild(new EntityIDGenerator(this));
        }

        public int GenerateEntityID()
        {
            return GetChild<EntityIDGenerator>().GenerateEntityID();
        }
    }
}
