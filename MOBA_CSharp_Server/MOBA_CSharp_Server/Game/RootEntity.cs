using Microsoft.Xna.Framework;
using MOBA_CSharp_Server.Library.ECS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBA_CSharp_Server.Game
{
    public class RootEntity : Entity
    {
        public RootEntity() : base(null)
        {
            AddInheritedType(typeof(RootEntity));

            AddEntities();

            //CreateWorld();

            Listen();
        }

        void AddEntities()
        {
            //DataReader
            DataReaderEntity dataReader = new DataReaderEntity(this);
            AddChild(dataReader);

            //TableReader
            CSVReaderEntity tableReader = new CSVReaderEntity(this);
            AddChild(tableReader);

            //Physics
            PhysicsEntity physics = new PhysicsEntity(this);
            AddChild(physics);

            //Pathfinding
            PathfindingEntity pathfinding = new PathfindingEntity(this);
            AddChild(pathfinding);

            //World
            WorldEntity world = new WorldEntity(this);
            AddChild(world);

            //Network
            NetworkEntity network = new NetworkEntity(this);
            AddChild(network);
        }

        public void CreateWorld()
        {
            PhysicsEntity physics = GetChild<PhysicsEntity>();
            WorldEntity world = GetChild<WorldEntity>();

            //world.RemoveAllEntity();

            MapInfo mapInfo = JsonConvert.DeserializeObject<MapInfo>(File.ReadAllText(GetChild<DataReaderEntity>().GetYAMLObject(@"YAML\ServerConfig.yml").GetData<string>("Map")));

            //Fountain
            SpawnInfo blueSpawnInfo = mapInfo.blueSpawn;
            world.AddChild(new Fountain(new Vector2(blueSpawnInfo.x, blueSpawnInfo.y), 0, blueSpawnInfo.regainRadius, Team.Blue, this));

            SpawnInfo redSpawnInfo = mapInfo.redSpawn;
            world.AddChild(new Fountain(new Vector2(redSpawnInfo.x, redSpawnInfo.y), 0, redSpawnInfo.regainRadius, Team.Red, this));

            //MinionRelayPoint
            Dictionary<Team, Dictionary<int, Dictionary<int, Vector2>>> points = new Dictionary<Team, Dictionary<int, Dictionary<int, Vector2>>>();
            points.Add(Team.Blue, new Dictionary<int, Dictionary<int, Vector2>>());
            points[Team.Blue].Add(0, new Dictionary<int, Vector2>());
            points[Team.Blue].Add(1, new Dictionary<int, Vector2>());
            points[Team.Blue].Add(2, new Dictionary<int, Vector2>());
            points.Add(Team.Red, new Dictionary<int, Dictionary<int, Vector2>>());
            points[Team.Red].Add(0, new Dictionary<int, Vector2>());
            points[Team.Red].Add(1, new Dictionary<int, Vector2>());
            points[Team.Red].Add(2, new Dictionary<int, Vector2>());
            foreach (var minionRelayPoint in mapInfo.minionRelayPoints)
            {
                points[minionRelayPoint.blueTeam ? Team.Blue : Team.Red][minionRelayPoint.laneNum].Add(minionRelayPoint.index, new Vector2(minionRelayPoint.x, minionRelayPoint.y));
            }

            //Core
            CoreInfo blueCoreInfo = mapInfo.blueCore;
            world.AddChild(new Core(points[Team.Blue], new Vector2(blueCoreInfo.x, blueCoreInfo.y), blueCoreInfo.angle, blueCoreInfo.radius, Team.Blue, this));

            CoreInfo redCoreInfo = mapInfo.redCore;
            world.AddChild(new Core(points[Team.Red], new Vector2(redCoreInfo.x, redCoreInfo.y), redCoreInfo.angle, redCoreInfo.radius, Team.Red, this));

            world.GetChildren<Core>().ToList().ForEach(x => x.SetGoal());

            //Tower
            foreach (var towerInfo in mapInfo.towers)
            {
                world.AddChild(new Tower(towerInfo.height, new Vector2(towerInfo.x, towerInfo.y), towerInfo.angle, towerInfo.radius, towerInfo.blueTeam ? Team.Blue : Team.Red, this));
            }

            //Monster
            foreach (var monsterInfo in mapInfo.monsters)
            {
                world.AddChild(new Monster(monsterInfo.chaseRadius, monsterInfo.respawnTime, new Vector2(monsterInfo.x, monsterInfo.y), monsterInfo.angle, 0.3f, monsterInfo.type, this));
            }

            foreach (var edgeInfo in mapInfo.edges)
            {
                physics.CreateEdgeWall(new Vector2(edgeInfo.x0, edgeInfo.y0), new Vector2(edgeInfo.x1, edgeInfo.y1));
            }

            foreach (var circleInfo in mapInfo.circles)
            {
                physics.CreateCircleWall(circleInfo.radius, new Vector2(circleInfo.x, circleInfo.y));
            }

            foreach (var bushInfo in mapInfo.bushes)
            {
                List<Vector2> vertices = new List<Vector2>();
                vertices.Add(new Vector2(bushInfo.x0, bushInfo.y0));
                vertices.Add(new Vector2(bushInfo.x1, bushInfo.y1));
                vertices.Add(new Vector2(bushInfo.x2, bushInfo.y2));
                vertices.Add(new Vector2(bushInfo.x3, bushInfo.y3));
                physics.CreateBush(vertices);
            }

            GetChild<PathfindingEntity>().Load(GetChild<DataReaderEntity>().GetYAMLObject(@"YAML\ServerConfig.yml").GetData<string>("NavMesh"));
        }

        void Listen()
        {
            GetChild<NetworkEntity>().Listen((ushort)GetChild<DataReaderEntity>().GetYAMLObject(@"YAML\ServerConfig.yml").GetData<int>("Port"), 100);
        }

        bool setLobbyFlag = false;
        public void SetLobby()
        {
            setLobbyFlag = true;
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            if(setLobbyFlag)
            {
                setLobbyFlag = false;
                GetChild<NetworkEntity>().SetLobby();
            }
        }
    }
}
