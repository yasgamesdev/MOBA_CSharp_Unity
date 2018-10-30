using System;
using System.Collections.Generic;
using System.Linq;

namespace ECS
{
    public abstract class Entity
    {
        public int EntityID { get; private set; }
        public bool Destroyed { get; private set; }

        protected RootEntity root { get; private set; }
        Dictionary<Type, Dictionary<int, Entity>> children = new Dictionary<Type, Dictionary<int, Entity>>();
        Dictionary<Type, Component> components = new Dictionary<Type, Component>();

        public Entity() : base()
        {
            Destroyed = false;
        }

        public Entity(RootEntity root) : base()
        {
            EntityID = root.GenerateEntityID();
            Destroyed = false;
            SetRoot(root);
        }

        public virtual void Destroy()
        {
            Destroyed = true;

            foreach (var dict in children.Values)
            {
                foreach(var entity in dict.Values)
                {
                    entity.Destroy();
                }
            }
        }

        public void SetEntityID(int entityID)
        {
            EntityID = entityID;
        }

        protected void SetRoot(RootEntity root)
        {
            this.root = root;
        }

        public void AddChild(Entity entity)
        {
            if (!children.ContainsKey(entity.GetType()))
            {
                children.Add(entity.GetType(), new Dictionary<int, Entity>());
            }
            children[entity.GetType()].Add(entity.EntityID, entity);
        }

        public void RemoveChild(Entity entity)
        {
            children[entity.GetType()].Remove(entity.EntityID);
        }

        public void AddComponent(Component component)
        {
            components.Add(component.GetType(), component);
        }

        public virtual void Step(float deltaTime)
        {
            foreach(var keyValue in components)
            {
                keyValue.Value.Step(deltaTime);
            }

            foreach(var keyValue in children)
            {
                foreach(var kv in keyValue.Value)
                {
                    kv.Value.Step(deltaTime);
                }
            }
        }

        public T GetChild<T>() where T : Entity
        {
            return (T)children[typeof(T)].First().Value;
        }

        public T[] GetChildren<T>() where T : Entity
        {
            if (!children.ContainsKey(typeof(T)))
            {
                children.Add(typeof(T), new Dictionary<int, Entity>());
            }

            return children[typeof(T)].Values.Cast<T>().ToArray();
        }

        public T GetComponent<T>() where T : Component
        {
            return (T)components[typeof(T)];
        }
    }
}
