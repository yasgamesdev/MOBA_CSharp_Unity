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
    public class AreaOfEffect : Actor
    {
        protected float attack;
        float duration;

        float timer;

        public AreaOfEffect(float attack, float duration, Vector2 position, float height, float rotation, float radius, UnitType type, int ownerUnitID, Team team, Entity root) : base(position, height, rotation, CollisionType.None, radius, type, ownerUnitID, team, root)
        {
            AddInheritedType(typeof(AreaOfEffect));

            this.attack = attack;
            this.duration = duration;

            timer = duration;
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            timer -= deltaTime;

            var unitIDs = Root.GetChild<PhysicsEntity>().GetUnit(GetChild<Transform>().Radius, GetChild<Transform>().Position);
            List<Unit> hitUnits = new List<Unit>();
            foreach (int unitID in unitIDs)
            {
                Unit targetUnit = Root.GetChild<WorldEntity>().GetUnit(unitID);
                if (targetUnit != null &&
                    targetUnit.HP > 0)
                {
                    hitUnits.Add(targetUnit);
                }
            }

            Hit(hitUnits, deltaTime);

            if (timer <= 0)
            {
                Destroyed = true;
            }
        }

        protected virtual void Hit(List<Unit> hitUnits, float deltaTime)
        {
            foreach (Unit unit in hitUnits)
            {
                if (Team != unit.Team)
                {
                    unit.Damage(OwnerUnitID, true, attack * deltaTime);
                }
            }
        }
    }
}
