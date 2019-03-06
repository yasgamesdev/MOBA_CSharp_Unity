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
    public class Actor : Unit
    {
        public Actor(Vector2 position, float height, float rotation, CollisionType collisionType, float radius, UnitType type, Team team, Entity root) : base(position, height, rotation, collisionType, radius, type, team, 0, root)
        {
            AddInheritedType(typeof(Actor));

            AddChild(new Sight(false, this, Root));
            AddChild(new Untargetable(this, Root));
        }

        public Actor(Vector2 position, float height, float rotation, CollisionType collisionType, float radius, UnitType type, int ownerUnitID, Team team, Entity root) : base(position, height, rotation, collisionType, radius, type, ownerUnitID, team, 0, root)
        {
            AddInheritedType(typeof(Actor));

            AddChild(new Sight(false, this, Root));
            AddChild(new Untargetable(this, Root));
        }

        public ActorObj GetActorObj()
        {
            return new ActorObj()
            {
                UnitID = UnitID,
                Type = Type,
                Team = Team,
                Position = new Vector3Obj() { X = GetChild<Transform>().GetPositionWith3D().X, Y = GetChild<Transform>().GetPositionWith3D().Y, Z = GetChild<Transform>().GetPositionWith3D().Z },
                Rotation = GetChild<Transform>().Rotation,
                Warped = GetChild<Transform>().Warped
            };
        }
    }
}
