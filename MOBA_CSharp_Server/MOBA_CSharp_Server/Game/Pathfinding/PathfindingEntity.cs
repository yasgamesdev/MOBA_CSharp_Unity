using ECS;
using Pathfinding;
using System.Numerics;

namespace MOBA_CSharp_Server.Game
{
    public class PathfindingEntity : Entity
    {
        NavMeshPathfinder pathfinder;

        public PathfindingEntity(RootEntity root) : base(root)
        {

        }

        public void Load(string path)
        {
            pathfinder = new NavMeshPathfinder();
            pathfinder.Load(path);
        }

        public Vector2[] GetPath(Vector2 start, Vector2 end)
        {
            return pathfinder.GetPath(start, end);
        }
    }
}
