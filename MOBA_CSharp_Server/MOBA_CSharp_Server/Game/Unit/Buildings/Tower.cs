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
    public class Tower : Building
    {
        float towerCoreHeight;
        float timer;

        public Tower(float towerCoreHeight, Vector2 position, float rotation, float radius, Team team, Entity root) : base(position, rotation, Library.Physics.CollisionType.Static, radius, UnitType.Tower, team, root)
        {
            AddInheritedType(typeof(Tower));

            this.towerCoreHeight = towerCoreHeight;
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            timer -= deltaTime;

            if (timer <= 0 && HP > 0 && !Status.GetValue(BoolStatus.UnArmed))
            {
                var unitIDs = Root.GetChild<PhysicsEntity>().GetUnit(Status.GetValue(FloatStatus.AttackRange), GetChild<Transform>().Position);
                SortedDictionary<float, Unit> units = new SortedDictionary<float, Unit>();
                foreach (int unitID in unitIDs)
                {
                    Unit unit = Root.GetChild<WorldEntity>().GetUnit(unitID);
                    if (unit != null &&
                        Team != unit.Team &&
                        unit.Status.GetValue(Team) &&
                        unit.HP > 0 &&
                        !unit.Status.GetValue(BoolStatus.Untargetable))
                    {
                        units.Add(Utilities.GetUnitDistance(unit, this), unit);
                    }
                }

                if (units.Count > 0)
                {
                    Root.GetChild<WorldEntity>().AddChild(new TowerBullet(units.First().Value.UnitID, Status.GetValue(FloatStatus.Attack), GetChild<Transform>().Position, towerCoreHeight, 0, 1f, UnitID, Team, Root));

                    timer = Status.GetValue(FloatStatus.AttackRate);
                }
            }
        }
    }
}
