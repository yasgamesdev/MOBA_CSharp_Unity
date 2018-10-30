using ECS;
using System.IO;
using System.Numerics;
using System.Runtime.Serialization.Json;

namespace MOBA_CSharp_Server.Game
{
    public class GameEntity : RootEntity
    {
        public GameEntity(): base()
        {
            AddEntities();

            CreateWorld();
        }

        void AddEntities()
        {
            //Factory
            AddChild(new FactoryEntity(this));

            //Config
            ConfigEntity config = new ConfigEntity(this);
            AddChild(config);

            //Collision
            CollisionEntity collision = new CollisionEntity(this);
            AddChild(collision);

            //Pathfinding
            PathfindingEntity pathfinding = new PathfindingEntity(this);
            AddChild(pathfinding);

            //VisionManager
            VisionManagerEntity visionManager = new VisionManagerEntity(collision, config.GetFloat("VisionRadius"), this);
            AddChild(visionManager);

            //UnitManager
            UnitManagerEntity unitManager = new UnitManagerEntity(this);
            AddChild(unitManager);

            //ClientManager
            ClientManagerEntity clientManager = new ClientManagerEntity(this);
            clientManager.SetEntities(unitManager, collision, visionManager);
            AddChild(clientManager);

            //Network
            AddChild(new NetworkEntity(clientManager, (ushort)config.GetInt("Port"), this));
        }

        void CreateWorld()
        {
            var config = GetChild<ConfigEntity>();

            string mapPath = config.GetString("Map");

            StreamReader reader = new StreamReader(mapPath);
            var serializer = new DataContractJsonSerializer(typeof(MapJson));
            MapJson map = (MapJson)serializer.ReadObject(reader.BaseStream);
            reader.Close();

            GetChild<CollisionEntity>().CreateWorld(map.width, map.height);
            CreateStaticBodies(map);

            GetChild<PathfindingEntity>().Load(config.GetString("NavMesh"));
        }

        void CreateStaticBodies(MapJson map)
        {
            CollisionEntity collision = GetChild<CollisionEntity>();

            Vector2 p0 = Vector2.Zero;
            Vector2 p1 = new Vector2(0, map.height);
            Vector2 p2 = new Vector2(map.width, map.height);
            Vector2 p3 = new Vector2(map.width, 0);
            collision.GenerateStaticBody(p0, p1);
            collision.GenerateStaticBody(p1, p2);
            collision.GenerateStaticBody(p2, p3);
            collision.GenerateStaticBody(p3, p0);

            foreach (EdgeJson edge in map.edges)
            {
                collision.GenerateStaticBody(edge.point0, edge.point1);
            }
            foreach (CircleJson circle in map.circles)
            {
                collision.GenerateStaticBody(circle.center, circle.radius);
            }
            foreach (PolyJson poly in map.polies)
            {
                collision.GenerateStaticBody(poly.points);
            }
        }
    }
}
