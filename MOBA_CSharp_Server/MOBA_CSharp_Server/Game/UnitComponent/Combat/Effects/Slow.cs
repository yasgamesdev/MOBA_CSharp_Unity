using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class Slow : Effect
    {
        float rate;

        public Slow(float duration,  float rate, Unit unitRoot, Entity root) : base(CombatType.Slow, unitRoot, root)
        {
            AddInheritedType(typeof(Slow));

            UpdateTimer(duration, rate);
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            Timer -= deltaTime;
            if(Timer <= 0)
            {
                Destroyed = true;
            }

            SetFloatParam(FloatStatus.MovementSpeed, rate, false);
        }

        public void UpdateTimer(float duration, float rate)
        {
            Timer = duration;
            this.rate = rate;
            Stack = 1;
            IsActive = true;

            Destroyed = false;
        }
    }
}
