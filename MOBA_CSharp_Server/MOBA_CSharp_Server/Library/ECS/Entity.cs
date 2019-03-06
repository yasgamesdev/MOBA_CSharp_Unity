using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBA_CSharp_Server.Library.ECS
{
    public class Entity
    {
        public List<Type> InheritedTypes { get; private set; } = new List<Type>();
        public Entity Root { get; private set; }
        public bool Destroyed { get; set; }

        //children
        protected Dictionary<Type, List<Entity>> children = new Dictionary<Type, List<Entity>>();
        protected bool isDuringStep;
        protected List<Entity> addEntities = new List<Entity>();
        protected List<Entity> removeEntities = new List<Entity>();

        public Entity(Entity root)
        {
            AddInheritedType(typeof(Entity));

            Root = root;
            Destroyed = false;

            isDuringStep = false;
        }

        protected void AddInheritedType(Type type)
        {
            InheritedTypes.Add(type);
        }

        public void Destroy()
        {
            List<Entity> tempRemoveEntities = new List<Entity>();
            foreach (Entity entity in GetChildren<Entity>())
            {
                entity.Destroy();

                if (entity.Destroyed)
                {
                    entity.ClearReference();
                    tempRemoveEntities.Add(entity);
                }
            }
            tempRemoveEntities.ForEach(x => RemoveChild(x));
        }

        public virtual void ClearReference()
        {
            foreach (Entity entity in GetChildren<Entity>())
            {
                entity.ClearReference();
            }
        }

        public virtual void AddChild(Entity entity)
        {
            if(isDuringStep)
            {
                addEntities.Add(entity);
            }
            else
            {
                foreach (Type type in entity.InheritedTypes)
                {
                    if (!children.ContainsKey(type))
                    {
                        children.Add(type, new List<Entity>());
                    }
                    children[type].Add(entity);
                }
            }
        }

        public virtual void RemoveChild(Entity entity)
        {
            if(isDuringStep)
            {
                removeEntities.Add(entity);
            }
            else
            {
                foreach (Type type in entity.InheritedTypes)
                {
                    children[type].Remove(entity);
                    if (children[type].Count == 0)
                    {
                        children.Remove(type);
                    }
                }
            }
        }

        public virtual void Step(float deltaTime)
        {
            isDuringStep = true;

            foreach(Entity entity in GetChildren<Entity>())
            {
                entity.Step(deltaTime);
            }

            isDuringStep = false;

            foreach(Entity addEntity in addEntities)
            {
                AddChild(addEntity);
            }
            addEntities.Clear();
            foreach(Entity removeEntity in removeEntities)
            {
                RemoveChild(removeEntity);
            }
            removeEntities.Clear();
        }

        public T GetChild<T>() where T : Entity
        {
            if(children.ContainsKey(typeof(T)))
            {
                return (T)children[typeof(T)].First();
            }
            else
            {
                return null;
            }
        }

        public T[] GetChildren<T>() where T : Entity
        {
            if (!children.ContainsKey(typeof(T)))
            {
                return new T[0];
            }

            return children[typeof(T)].Cast<T>().ToArray();
        }
    }
}
