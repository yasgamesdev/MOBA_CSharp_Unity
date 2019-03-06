using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOBA_CSharp_Server.Library.DataReader;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class SpecialBoot : Item
    {
        float durationTimer;
        public SpecialBoot(int slotNum, CombatType type, Unit unitRoot, Entity root) : base(slotNum, type, unitRoot, root)
        {
            AddInheritedType(typeof(SpecialBoot));

            durationTimer = 0;
        }

        public override void Execute(object args)
        {
            if (IsExecutable(args))
            {
                ConsumeMPAndReduceStack();

                durationTimer = 3.0f;
            }
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            durationTimer -= deltaTime;
            if(durationTimer > 0)
            {
                SetFloatParam(FloatStatus.MovementSpeed, 2.0f, false);
            }
            else
            {
                UnSetFloatParam(FloatStatus.MovementSpeed);
            }
        }
    }
}
