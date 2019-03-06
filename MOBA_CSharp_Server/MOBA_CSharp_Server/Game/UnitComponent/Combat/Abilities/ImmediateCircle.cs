using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class ImmediateCircle : Skill
    {
        float duration;
        float radius;
        float distance;

        public ImmediateCircle(CombatAttribute skillAttribute, CombatType type, Unit unitRoot, Entity root) : base(skillAttribute, type, unitRoot, root)
        {
            AddInheritedType(typeof(ImmediateCircle));

            duration = GetYAMLObject().GetData<float>("Duration");
            radius = GetYAMLObject().GetData<float>("Radius");
            distance = GetYAMLObject().GetData<float>("Distance");
        }

        public override bool IsExecutable(object args)
        {
            return unitRoot.HP > 0 && unitRoot.MP >= Cost && Charge > 0 && !unitRoot.Status.GetValue(BoolStatus.Silenced);
        }

        public override void Execute(object args)
        {
            if (IsExecutable(args))
            {
                Vector2 center = new Vector2(((CastObj)args).FloatArgs[0], ((CastObj)args).FloatArgs[1]);

                ConsumeMPAndReduceStack();

                if(Type == CombatType.Meteor)
                {
                    
                }
                UnitType unitType;
                Enum.TryParse(Type.ToString(), out unitType);

                if(unitType == UnitType.Meteor)
                {
                    Root.GetChild<WorldEntity>().AddChild(new AreaOfEffect(unitRoot.Status.GetValue(FloatStatus.Attack), duration, center, 0, 0, radius, unitType, unitRoot.UnitID, unitRoot.Team, Root));
                }
                else if(unitType == UnitType.PoisonGas)
                {
                    float debuffDuration = GetYAMLObject().GetData<float>("DebuffDuration");
                    float attackRate = GetYAMLObject().GetData<float>("AttackRate");
                    Root.GetChild<WorldEntity>().AddChild(new PoisonGas(debuffDuration, unitRoot.Status.GetValue(FloatStatus.Attack) * attackRate, duration, center, 0, 0, radius, unitType, unitRoot.UnitID, unitRoot.Team, Root));
                }

                Vector2 direction = center - unitRoot.GetChild<Transform>().Position;
                double x = Math.Atan2(direction.Y, direction.X);
                double rotation = (x > 0 ? x : (2 * Math.PI + x)) * 360 / (2 * Math.PI);

                unitRoot.GetChild<Transform>().SetRotation((float)-rotation + 90.0f);
            }
        }
    }
}
