namespace ECS
{
    public class EntityRef<T> where T : Entity
    {
        Entity entity;

        public EntityRef(Entity entity)
        {
            this.entity = entity;
        }

        public T Get()
        {
            if(entity.Destroyed)
            {
                return null;
            }
            else
            {
                return (T)entity;
            }
        }
    }
}
