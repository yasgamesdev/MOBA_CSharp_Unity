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
    public class Core : Building
    {
        Dictionary<int, Dictionary<int, Vector2>> relayPoints;

        float respawnTime;
        float spawnRadius;

        float timer;

        float victoryTime;
        bool testMode;

        public Core(Dictionary<int, Dictionary<int, Vector2>> relayPoints, Vector2 position, float rotation, float radius, Team team, Entity root) : base(position, rotation, Library.Physics.CollisionType.Static, radius, UnitType.Core, team, root)
        {
            AddInheritedType(typeof(Core));

            this.relayPoints = relayPoints;

            respawnTime = GetYAMLObject().GetData<float>("RespawnTime");
            spawnRadius = GetYAMLObject().GetData<float>("SpawnRadius");

            timer = respawnTime;

            victoryTime = Root.GetChild<DataReaderEntity>().GetYAMLObject(@"YAML\ServerConfig.yml").GetData<float>("VictoryTime");
            testMode = Root.GetChild<DataReaderEntity>().GetYAMLObject(@"YAML\ServerConfig.yml").GetData<bool>("TestMode");
        }

        public void SetGoal()
        {
            Vector2 goal = Root.GetChild<WorldEntity>().GetChildren<Core>().First(x => x.Team != Team).GetChild<Transform>().Position;
            for (int i = 0; i < 3; i++)
            {
                if (relayPoints[i].Count == 0)
                {
                    relayPoints[i].Add(0, goal);
                }
                else
                {
                    int max = relayPoints[i].Keys.Max();
                    relayPoints[i].Add(max + 1, goal);
                }
            }
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            if (HP > 0)
            {
                timer -= deltaTime;

                if (timer <= 0)
                {
                    timer = respawnTime;

                    Transform transform = GetChild<Transform>();
                    if (Team == Team.Blue)
                    {
                        Spawn(transform.Position + new Vector2(0, spawnRadius), relayPoints[0]);
                        Spawn(transform.Position + (new Vector2(1, 1) * spawnRadius), relayPoints[1]);
                        Spawn(transform.Position + new Vector2(spawnRadius, 0), relayPoints[2]);
                    }
                    else
                    {
                        Spawn(transform.Position + new Vector2(-spawnRadius, 0), relayPoints[0]);
                        Spawn(transform.Position + (new Vector2(1, 1) * -spawnRadius), relayPoints[1]);
                        Spawn(transform.Position + new Vector2(0, -spawnRadius), relayPoints[2]);
                    }
                }
            }
            else
            {
                victoryTime -= deltaTime;
                if(victoryTime <= 0 && !testMode)
                {
                    ((RootEntity)Root).SetLobby();
                }
            }
        }

        void Spawn(Vector2 position, Dictionary<int, Vector2> points)
        {
            Root.GetChild<WorldEntity>().AddChild(new Minion(points, position, 0, 0.3f, Team, Root));
        }
    }
}
