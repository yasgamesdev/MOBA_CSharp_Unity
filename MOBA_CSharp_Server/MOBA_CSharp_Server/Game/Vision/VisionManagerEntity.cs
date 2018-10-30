using Collision2D;
using ECS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOBA_CSharp_Server.Game
{
    public class VisionManagerEntity : Entity
    {
        CollisionEntity collision;
        float visionRadius;
        
        Dictionary<Team, Dictionary<Body, UnitEntity>> units = new Dictionary<Team, Dictionary<Body, UnitEntity>>();

        public VisionManagerEntity(CollisionEntity collision, float visionRadius, RootEntity root) : base(root)
        {
            this.collision = collision;
            this.visionRadius = visionRadius;

            foreach (Team team in Enum.GetValues(typeof(Team)))
            {
                units.Add(team, new Dictionary<Body, UnitEntity>());
            }
        }

        public void AddUnit(UnitEntity unit)
        {
            units[unit.Team].Add(unit.GetComponent<BodyComponent>().Body, unit);
        }

        public void RemoveUnit(UnitEntity unit)
        {
            units[unit.Team].Remove(unit.GetComponent<BodyComponent>().Body);
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            foreach (Team targetTeam in Enum.GetValues(typeof(Team)))
            {
                foreach (Team enemyTeam in Enum.GetValues(typeof(Team)))
                {
                    if(enemyTeam == targetTeam)
                    {
                        continue;
                    }

                    SetVision(targetTeam, enemyTeam);
                }
            }
        }

        void SetVision(Team targetTeam, Team enemyTeam)
        {
            Dictionary<Body, UnitEntity> targetPlayers = new Dictionary<Body, UnitEntity>(units[targetTeam]);
            Dictionary<Body, UnitEntity> enemyPlayers = new Dictionary<Body, UnitEntity>(units[enemyTeam]);

            foreach (var enemyPlayer in enemyPlayers)
            {
                if (targetPlayers.Count == 0)
                {
                    break;
                }

                var bodies = collision.GetCircleBodies(((DynamicBody)enemyPlayer.Key).GetPosition(), visionRadius);
                foreach (var body in bodies)
                {
                    if (targetPlayers.Any(x => x.Key == body))
                    {
                        if (collision.CheckLineOfSight(((DynamicBody)enemyPlayer.Key).GetPosition(), ((DynamicBody)body).GetPosition()))
                        {
                            var item = targetPlayers.First(x => x.Key == body);
                            targetPlayers[item.Key].GetComponent<VisionComponent>().SetVision(enemyTeam, true);
                            targetPlayers.Remove(item.Key);
                        }
                    }
                }
            }

            foreach (var targetPlayer in targetPlayers.Values)
            {
                targetPlayer.GetComponent<VisionComponent>().SetVision(enemyTeam, false);
            }
        }
    }
}
