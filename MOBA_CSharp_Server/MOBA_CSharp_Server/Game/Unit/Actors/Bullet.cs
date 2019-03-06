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
    public class Bullet : Actor
    {
        float attack;
        Vector2 direction;
        float distance;
        float speed;

        float remainDistance;

        public Bullet(Vector2 direction, float distance, float speed, float attack, Vector2 position, float height, float rotation, float radius, UnitType type, int ownerUnitID, Team team, Entity root) : base(position, height, rotation, CollisionType.None, radius, type, ownerUnitID, team, root)
        {
            AddInheritedType(typeof(Bullet));

            this.direction = direction;
            this.distance = distance;
            this.speed = speed;
            this.attack = attack;

            remainDistance = distance;
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            float nextDistance = speed * deltaTime;
            if (remainDistance <= nextDistance)
            {
                Vector3 position = GetChild<Transform>().GetPositionWith3D();
                Vector3 nextPosition = position + new Vector3(direction.X, 0, direction.Y) * remainDistance;
                GetChild<Transform>().SetPositionWith3D(nextPosition);

                remainDistance = 0;
            }
            else
            {
                Vector3 position = GetChild<Transform>().GetPositionWith3D();
                Vector3 nextPosition = position + new Vector3(direction.X, 0, direction.Y) * nextDistance;
                GetChild<Transform>().SetPositionWith3D(nextPosition);

                remainDistance -= nextDistance;
            }

            var unitIDs = Root.GetChild<PhysicsEntity>().GetUnit(GetChild<Transform>().Radius, GetChild<Transform>().Position);
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
            if (enemyUnits.Count > 0)
            {
                Hit(enemyUnits, deltaTime);
                Destroyed = true;
            }


            if (remainDistance <= 0)
            {
                Destroyed = true;
            }
        }

        protected virtual void Hit(List<Unit> hitUnits, float deltaTime)
        {
            hitUnits[0].Damage(OwnerUnitID, true, attack);
        }
    }
}
