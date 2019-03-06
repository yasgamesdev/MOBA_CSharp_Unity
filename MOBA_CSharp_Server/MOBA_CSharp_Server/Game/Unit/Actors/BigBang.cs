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
    public class BigBang : Actor
    {
        float attack;
        Vector2 direction;
        float distance;
        float speed;

        float remainDistance;
        float timer = 2.0f;

        bool hitFlag = false;

        public BigBang(Vector2 direction, float distance, float speed, float attack, Vector2 position, float height, float rotation, float radius, UnitType type, int ownerUnitID, Team team, Entity root) : base(position, height, rotation, CollisionType.None, radius, type, ownerUnitID, team, root)
        {
            AddInheritedType(typeof(BigBang));

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

                if (!hitFlag)
                {
                    hitFlag = true;

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
                    }
                }
            }
            else
            {
                Vector3 position = GetChild<Transform>().GetPositionWith3D();
                Vector3 nextPosition = position + new Vector3(direction.X, 0, direction.Y) * nextDistance;
                GetChild<Transform>().SetPositionWith3D(nextPosition);

                remainDistance -= nextDistance;
            }

            if(remainDistance <= 0)
            {
                timer -= deltaTime;
                if(timer <= 0)
                {
                    Destroyed = true;
                }
            }
        }

        protected virtual void Hit(List<Unit> hitUnits, float deltaTime)
        {
            foreach (Unit unit in hitUnits)
            {
                unit.Damage(OwnerUnitID, true, attack);
            }
        }
    }
}
