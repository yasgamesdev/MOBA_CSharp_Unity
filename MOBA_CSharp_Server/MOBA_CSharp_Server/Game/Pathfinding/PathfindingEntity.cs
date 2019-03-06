using Microsoft.Xna.Framework;
using MOBA_CSharp_Server.Library.ECS;
using MOBA_CSharp_Server.Library.Pathfinding;

namespace MOBA_CSharp_Server.Game
{
    public class PathfindingEntity : Entity
    {
        NavMeshPathfinder pathfinder = new NavMeshPathfinder();

        public PathfindingEntity(Entity root) : base(root)
        {
            AddInheritedType(typeof(PathfindingEntity));
        }

        public void Load(string path)
        {
            pathfinder.Load(path);
        }

        public Vector2[] GetPath(Vector2 start, Vector2 end)
        {
            return pathfinder.GetPath(start, end);
        }
    }
}
