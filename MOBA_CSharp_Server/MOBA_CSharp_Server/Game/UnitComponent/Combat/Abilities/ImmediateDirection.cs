using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class ImmediateDirection : Skill
    {
        float distance;
        float speed;
        float radius;

        public ImmediateDirection(CombatAttribute skillAttribute, CombatType type, Unit unitRoot, Entity root) : base(skillAttribute, type, unitRoot, root)
        {
            AddInheritedType(typeof(ImmediateDirection));

            distance = GetYAMLObject().GetData<float>("Distance");
            speed = GetYAMLObject().GetData<float>("Speed");
            radius = GetYAMLObject().GetData<float>("Radius");
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

                if (unitType == UnitType.FireBall)
                {
                    Vector2 direction = new Vector2((float)Math.Cos(rotation * (Math.PI / 180)), (float)Math.Sin(rotation * (Math.PI / 180)));
                    float attack = unitRoot.Status.GetValue(FloatStatus.Attack) * 2f;
                    Root.GetChild<WorldEntity>().AddChild(new Bullet(direction, distance, speed, attack, unitRoot.GetChild<Transform>().Position, 0, -rotation + 90.0f, radius, unitType, unitRoot.UnitID, unitRoot.Team, Root));
                }
                else if(unitType == UnitType.BigBang)
                {
                    Vector2 direction = new Vector2((float)Math.Cos(rotation * (Math.PI / 180)), (float)Math.Sin(rotation * (Math.PI / 180)));
                    float attack = unitRoot.Status.GetValue(FloatStatus.Attack) * 4f;
                    Root.GetChild<WorldEntity>().AddChild(new BigBang(direction, distance, speed, attack, unitRoot.GetChild<Transform>().Position, 0, -rotation + 90.0f, radius, unitType, unitRoot.UnitID, unitRoot.Team, Root));
                }
                else if(unitType == UnitType.EarthShatter)
                {
                    Vector2 direction = new Vector2((float)Math.Cos(rotation * (Math.PI / 180)), (float)Math.Sin(rotation * (Math.PI / 180)));
                    float attack = unitRoot.Status.GetValue(FloatStatus.Attack);
                    float debuffDuration = radius = GetYAMLObject().GetData<float>("DebuffDuration");
                    Root.GetChild<WorldEntity>().AddChild(new EarthShatter(debuffDuration, direction, attack, distance, unitRoot.GetChild<Transform>().Position, 0, -rotation + 90.0f, radius, unitType, unitRoot.UnitID, unitRoot.Team, Root));
                }

                unitRoot.GetChild<Transform>().SetRotation(-rotation + 90.0f);
            }
        }
    }
}
