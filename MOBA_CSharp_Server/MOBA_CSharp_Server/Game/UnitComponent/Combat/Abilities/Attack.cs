using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class Attack : Ability
    {
        enum AttackState
        {
            Move,
            BeforeAttack,
            AfterAttack
        }
        AttackState state;
        float updatePathTimer;
        float beforeAttackTimer;
        float afterAttackTimer;

        int targetUnitID;

        float updatePathSpan;
        float attackTimeRate;

        public Attack(Unit unitRoot, Entity root) : base(CombatType.Attack, unitRoot, root)
        {
            AddInheritedType(typeof(Attack));
            AddAttribute(CombatAttribute.Attack);

            updatePathSpan = GetYAMLObject().GetData<float>("UpdatePathSpan");
            attackTimeRate = GetYAMLObject().GetData<float>("AttackTimeRate");
        }

        public override bool IsExecutable(object args)
        {
            targetUnitID = (int)args;
            Unit targetUnit = Root.GetChild<WorldEntity>().GetUnit(targetUnitID);
            if (targetUnit != null &&
                unitRoot.Team != targetUnit.Team &&
                targetUnit.Status.GetValue(unitRoot.Team) &&
                targetUnit.HP > 0 &&
                !targetUnit.Status.GetValue(BoolStatus.Untargetable) &&
                unitRoot.HP > 0 &&
                !unitRoot.Status.GetValue(BoolStatus.UnArmed))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Execute(object args)
        {
            base.Execute(args);

            targetUnitID = (int)args;
            Unit targetUnit = Root.GetChild<WorldEntity>().GetUnit(targetUnitID);

            state = AttackState.Move;
            updatePathTimer = updatePathSpan;
            beforeAttackTimer = 0.0f;

            unitRoot.Execute(CombatAttribute.Move, targetUnit.GetChild<Transform>().Position);
        }

        protected override bool ContinueExecution()
        {
            Unit targetUnit = Root.GetChild<WorldEntity>().GetUnit(targetUnitID);
            if (targetUnit != null &&
                unitRoot.Team != targetUnit.Team &&
                targetUnit.Status.GetValue(unitRoot.Team) &&
                targetUnit.HP > 0 &&
                !targetUnit.Status.GetValue(BoolStatus.Untargetable) &&
                unitRoot.HP > 0 &&
                !unitRoot.Status.GetValue(BoolStatus.UnArmed))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Cancel()
        {
            base.Cancel();

            UnSetAnimationParam();

            if (state == AttackState.Move)
            {
                unitRoot.Cancel(CombatAttribute.Move);
            }
        }

        public override void Step(float deltaTime)
        {
            updatePathTimer -= deltaTime;
            afterAttackTimer -= deltaTime;

            base.Step(deltaTime);
        }

        protected override void ExecuteProcess(float deltaTime)
        {
            base.ExecuteProcess(deltaTime);

            Unit targetUnit = Root.GetChild<WorldEntity>().GetUnit(targetUnitID);

            Transform transform = unitRoot.GetChild<Transform>();
            Transform targetTransform = targetUnit.GetChild<Transform>();

            if (state == AttackState.Move)
            {
                Move move = (Move)unitRoot.GetCombat(CombatAttribute.Move);
                float attackRange = unitRoot.Status.GetValue(FloatStatus.AttackRange);
                if ((transform.Position - targetTransform.Position).Length() - transform.Radius - targetTransform.Radius <= attackRange)
                {
                    unitRoot.Cancel(CombatAttribute.Move);
                    state = AttackState.BeforeAttack;
                    beforeAttackTimer = 1f / unitRoot.Status.GetValue(FloatStatus.AttackRate) * attackTimeRate;
                    SetAnimationParam(AnimationType.Attack, unitRoot.Status.GetValue(FloatStatus.AttackRate), (int)AnimationStatusPriority.Attack);
                }
                else
                {
                    if (updatePathTimer <= 0)
                    {
                        updatePathTimer = updatePathSpan;
                        unitRoot.Cancel(CombatAttribute.Move);
                        unitRoot.Execute(CombatAttribute.Move, targetTransform.Position);
                    }
                }
            }
            else if (state == AttackState.BeforeAttack)
            {
                Vector2 direction = targetTransform.Position - transform.Position;
                transform.SetRotation((float)(Math.Atan2(direction.X, direction.Y) / Math.PI * 180.0));

                float attackRange = unitRoot.Status.GetValue(FloatStatus.AttackRange);
                if ((transform.Position - targetTransform.Position).Length() - transform.Radius - targetTransform.Radius <= attackRange)
                {
                    beforeAttackTimer -= deltaTime;
                    if (beforeAttackTimer <= 0)
                    {
                        state = AttackState.AfterAttack;
                        afterAttackTimer = 1f / unitRoot.Status.GetValue(FloatStatus.AttackRate) * (1.0f - attackTimeRate);

                        targetUnit.Damage(unitRoot.UnitID, true, unitRoot.Status.GetValue(FloatStatus.Attack));
                    }
                }
                else
                {
                    state = AttackState.Move;
                    UnSetAnimationParam();
                    unitRoot.Execute(CombatAttribute.Move, targetTransform.Position);
                }
            }
            else
            {
                Vector2 direction = targetTransform.Position - transform.Position;
                transform.SetRotation((float)(Math.Atan2(direction.X, direction.Y) / Math.PI * 180.0));

                float attackRange = unitRoot.Status.GetValue(FloatStatus.AttackRange);
                if ((transform.Position - targetTransform.Position).Length() - transform.Radius - targetTransform.Radius <= attackRange)
                {
                    if (afterAttackTimer <= 0)
                    {
                        state = AttackState.BeforeAttack;
                        beforeAttackTimer = 1f / unitRoot.Status.GetValue(FloatStatus.AttackRate) * attackTimeRate;
                    }
                }
                else
                {
                    state = AttackState.Move;
                    UnSetAnimationParam();
                    unitRoot.Execute(CombatAttribute.Move, targetTransform.Position);
                }
            }
        }
    }
}
