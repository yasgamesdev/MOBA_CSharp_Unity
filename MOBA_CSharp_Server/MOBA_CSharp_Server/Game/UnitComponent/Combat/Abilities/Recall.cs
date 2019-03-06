using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class Recall : Ability
    {
        float recallTime;
        float timer;

        public Recall(Unit unitRoot, Entity root) : base(CombatType.Recall, unitRoot, root)
        {
            AddInheritedType(typeof(Recall));
            AddAttribute(CombatAttribute.Recall);

            recallTime = GetYAMLObject().GetData<float>("RecallTime");
        }

        public override bool IsExecutable(object args)
        {
            return unitRoot.HP > 0 && !unitRoot.Status.GetValue(BoolStatus.Unmovable) && !unitRoot.Status.Damaged;
        }

        public override void Execute(object args)
        {
            base.Execute(args);

            timer = recallTime;
            SetAnimationParam(AnimationType.Recall, 1f, (int)AnimationStatusPriority.Recall);
        }

        public override void Cancel()
        {
            base.Cancel();

            UnSetAnimationParam();
        }

        protected override bool ContinueExecution()
        {
            return unitRoot.HP > 0 && !unitRoot.Status.GetValue(BoolStatus.Unmovable) && !unitRoot.Status.Damaged;
        }

        protected override void ExecuteProcess(float deltaTime)
        {
            base.ExecuteProcess(deltaTime);

            timer -= deltaTime;
            if (timer <= 0)
            {
                unitRoot.GetChild<Transform>().Warp(Root.GetChild<WorldEntity>().GetFountainPosition(unitRoot.Team), 0, 0);

                Cancel();
            }
        }
    }
}
