using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class ImmediateCone : Skill
    {
        float radius;
        float angle;
        float duration;

        public ImmediateCone(CombatAttribute skillAttribute, CombatType type, Unit unitRoot, Entity root) : base(skillAttribute, type, unitRoot, root)
        {
            AddInheritedType(typeof(ImmediateCone));

            radius = GetYAMLObject().GetData<float>("Radius");
            angle = GetYAMLObject().GetData<float>("Angle");
            duration = GetYAMLObject().GetData<float>("Duration");
        }

        public override bool IsExecutable(object args)
        {
            return unitRoot.HP > 0 && unitRoot.MP >= Cost && Charge > 0 && !unitRoot.Status.GetValue(BoolStatus.Silenced);
        }

        public override void Execute(object args)
        {
            if (IsExecutable(args))
            {
                float rotation = ((CastObj)args).FloatArgs[0];

                ConsumeMPAndReduceStack();

                UnitType unitType;
                Enum.TryParse(Type.ToString(), out unitType);

                if (unitType == UnitType.FireBreath)
                {
                    Root.GetChild<WorldEntity>().AddChild(new Cone(duration, unitRoot.GetChild<Transform>().Position, 0, -rotation + 90.0f, radius, unitType, unitRoot.UnitID, unitRoot.Team, Root));
                }
                else if(unitType == UnitType.PressurisedSteam)
                {
                    Root.GetChild<WorldEntity>().AddChild(new Cone(duration, unitRoot.GetChild<Transform>().Position, 0, -rotation + 90.0f, radius, unitType, unitRoot.UnitID, unitRoot.Team, Root));
                }

                var unitIDs = Root.GetChild<PhysicsEntity>().GetUnit(radius, unitRoot.GetChild<Transform>().Position);
                foreach (int unitID in unitIDs)
                {
                    Unit targetUnit = Root.GetChild<WorldEntity>().GetUnit(unitID);
                    if (targetUnit != null &&
                        targetUnit.Team != unitRoot.Team &&
                        targetUnit.HP > 0)
                    {
                        Vector2 direction = targetUnit.GetChild<Transform>().Position - unitRoot.GetChild<Transform>().Position;
                        double x = Math.Atan2(direction.Y, direction.X);
                        double targetAngle = (x > 0 ? x : (2 * Math.PI + x)) * 360 / (2 * Math.PI);

                        double startAngle = rotation - angle * 0.5;
                        startAngle = (startAngle > 0 ? startAngle : 360 + startAngle);
                        double endAngle = rotation + angle * 0.5;
                        endAngle = (endAngle > 0 ? endAngle : 360 + startAngle);

                        if (endAngle > startAngle)
                        {
                            if (startAngle <= targetAngle && targetAngle <= endAngle)
                            {
                                Hit(unitType, targetUnit);
                            }
                        }
                        else
                        {
                            if ((startAngle <= targetAngle && targetAngle <= 360) || (0 <= targetAngle && targetAngle <= endAngle))
                            {
                                Hit(unitType, targetUnit);
                            }
                        }
                    }
                }

                unitRoot.GetChild<Transform>().SetRotation(-rotation + 90.0f);
            }
        }

        protected virtual void Hit(UnitType unitType, Unit hitUnit)
        {
            if (unitType == UnitType.FireBreath)
            {
                hitUnit.Damage(unitRoot.UnitID, true, unitRoot.Status.GetValue(FloatStatus.Attack));
            }
            else if(unitType == UnitType.PressurisedSteam)
            {
                float slowRate = GetYAMLObject().GetData<float>("SlowRate");
                float debuffDuration = GetYAMLObject().GetData<float>("DebuffDuration");
                hitUnit.AddChild(new Slow(debuffDuration, slowRate, hitUnit, Root));
            }
        }
    }
}
