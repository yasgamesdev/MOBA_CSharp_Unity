using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collision2D;
using ECS;

namespace MOBA_CSharp_Server.Game
{
    public abstract class DynamicBodyUnitEntity : UnitEntity
    {
        public DynamicBodyUnitEntity(Team team, DynamicBody body, float angle, RootEntity root) : base(team, body, angle, root)
        {
            AnimationComponent animation = new AnimationComponent(root);
            GetComponent<BodyComponent>().SetAnimationComponent(animation);

            AddComponent(animation);
        }
    }
}
