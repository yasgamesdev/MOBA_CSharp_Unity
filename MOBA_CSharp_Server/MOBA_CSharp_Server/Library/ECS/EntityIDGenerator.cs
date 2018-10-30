namespace ECS
{
    public class EntityIDGenerator : Entity
    {
        int idCounter = 2;

        public EntityIDGenerator(RootEntity root) : base()
        {
            SetEntityID(1);
            SetRoot(root);
        }

        public int GenerateEntityID()
        {
            return idCounter++;
        }
    }
}
