using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MOBA_CSharp_Server.Library.DataReader;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class Fountain : Building
    {
        float regainRadius;
        float regainRate;

        public Fountain(Vector2 position, float rotation, float radius, Team team, Entity root) : base(position, rotation, Library.Physics.CollisionType.None, radius, UnitType.Fountain, team, root)
        {
            AddInheritedType(typeof(Fountain));

            regainRadius = radius;
            regainRate = GetYAMLObject().GetData<float>("RegainRate");

            AddChild(new Untargetable(this, Root));
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            foreach (int unitID in Root.GetChild<PhysicsEntity>().GetUnit(regainRadius, GetChild<Transform>().Position))
            {
                Unit unit = Root.GetChild<WorldEntity>().GetUnit(unitID);
                if (unit != null && unit.Type >= UnitType.HatsuneMiku && unit.Team == Team && unit.HP > 0)
                {
                    OnBase onBase = unit.GetChild<OnBase>();
                    if(onBase != null)
                    {
                        onBase.UpdateTimer();
                    }
                    else
                    {
                        unit.AddChild(new OnBase(unit, Root));
                    }

                    unit.Damage(unitID, false, unit.Status.GetValue(FloatStatus.MaxHP) * regainRate * deltaTime);
                    unit.DamageMP(unitID, false, unit.Status.GetValue(FloatStatus.MaxMP) * regainRate * deltaTime);
                }
            }
        }
    }
}
