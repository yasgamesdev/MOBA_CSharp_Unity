using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class OnBase : Effect
    {
        float duration;

        public OnBase(Unit unitRoot, Entity root) : base(CombatType.OnBase, unitRoot, root)
        {
            AddInheritedType(typeof(OnBase));

            duration = GetYAMLObject().GetData<float>("Duration");

            UpdateTimer();
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            Timer -= deltaTime;
            if(Timer <= 0)
            {
                Destroyed = true;
            }
        }

        public void UpdateTimer()
        {
            Timer = duration;
            Stack = 1;
            IsActive = true;

            Destroyed = false;
        }
    }
}
