using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class Stun : Effect
    {
        public Stun(float duration, Unit unitRoot, Entity root) : base(CombatType.Stun, unitRoot, root)
        {
            AddInheritedType(typeof(Stun));

            UpdateTimer(duration);
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            Timer -= deltaTime;
            if(Timer <= 0)
            {
                Destroyed = true;
            }

            SetBoolParam(BoolStatus.UnArmed, true, (int)UnArmedPriority.Stun);
            SetBoolParam(BoolStatus.Silenced, true, (int)SilencedPriority.Stun);
            SetBoolParam(BoolStatus.Unmovable, true, (int)UnmovablePriority.Stun);
            SetAnimationParam(AnimationType.Stun, 1f, (int)AnimationStatusPriority.Stun);
        }

        public void UpdateTimer(float duration)
        {
            Timer = duration;
            Stack = 1;
            IsActive = true;

            Destroyed = false;
        }
    }
}
