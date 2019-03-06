using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MOBA_CSharp_Server.Library.DataReader;
using MOBA_CSharp_Server.Library.ECS;
using MOBA_CSharp_Server.Library.Physics;

namespace MOBA_CSharp_Server.Game
{
    public class PoisonGas : AreaOfEffect
    {
        float debuffDuration;

        public PoisonGas(float debuffDuration, float attack, float duration, Vector2 position, float height, float rotation, float radius, UnitType type, int ownerUnitID, Team team, Entity root) : base(attack, duration, position, height, rotation, radius, type, ownerUnitID, team, root)
        {
            AddInheritedType(typeof(PoisonGas));

            this.debuffDuration = debuffDuration;
        }

        protected override void Hit(List<Unit> hitUnits, float deltaTime)
        {
            foreach (Unit unit in hitUnits)
            {
                if (Team != unit.Team)
                {
                    var poisons = unit.GetChildren<Poison>();
                    var poison = poisons.FirstOrDefault(x => x.UnitID == OwnerUnitID);
                    if(poison != null)
                    {
                        poison.UpdateTimer(debuffDuration, attack);
                    }
                    else
                    {
                        unit.AddChild(new Poison(OwnerUnitID, debuffDuration, attack, unit, Root));
                    }
                }
            }
        }
    }
}
