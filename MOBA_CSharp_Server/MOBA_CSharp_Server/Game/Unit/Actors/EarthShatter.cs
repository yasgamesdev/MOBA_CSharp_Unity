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
    public class EarthShatter : Actor
    {
        float timer = 2.0f;
        Vector2 direction;
        float attack;
        float distance;
        float debuffDuration;

        public EarthShatter(float debuffDuration, Vector2 direction, float attack, float distance, Vector2 position, float height, float rotation, float radius, UnitType type, int ownerUnitID, Team team, Entity root) : base(position, height, rotation, CollisionType.None, radius, type, ownerUnitID, team, root)
        {
            AddInheritedType(typeof(EarthShatter));

            this.debuffDuration = debuffDuration;
            this.direction = direction;
            this.attack = attack;
            this.distance = distance;
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            timer -= deltaTime;
            if(timer <= 0)
            {
                Destroyed = true;
            }

            Vector2 center = GetChild<Transform>().Position + direction / direction.Length() * distance * (2.0f - timer) * 0.5f;

            var unitIDs = Root.GetChild<PhysicsEntity>().GetUnit(GetChild<Transform>().Radius, center);
            List<Unit> enemyUnits = new List<Unit>();
            foreach (int unitID in unitIDs)
            {
                Unit targetUnit = Root.GetChild<WorldEntity>().GetUnit(unitID);
                if (targetUnit != null &&
                    Team != targetUnit.Team &&
                    targetUnit.HP > 0)
                {
                    enemyUnits.Add(targetUnit);
                }
            }

            Hit(enemyUnits, deltaTime);
        }

        protected virtual void Hit(List<Unit> hitUnits, float deltaTime)
        {
            foreach(Unit hitUnit in hitUnits)
            {
                Stun stun = hitUnit.GetChild<Stun>();
                if(stun != null)
                {
                    stun.UpdateTimer(debuffDuration);
                }
                else
                {
                    hitUnit.AddChild(new Stun(debuffDuration, hitUnit, Root));
                }
            }
        }
    }
}
