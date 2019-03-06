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
    public class Building : Unit
    {
        public Building(Vector2 position, float rotation, CollisionType collisionType, float radius, UnitType type, Team team, Entity root) : base(position, 0, rotation, collisionType, radius, type, team, 0, root)
        {
            AddInheritedType(typeof(Building));

            AddChild(new Sight(true, this, Root));
            AddChild(new Eye(GetYAMLObject().GetData<float>("VisionRadius"), this, Root));
        }

        public BuildingObj GetBuildingObj()
        {
            return new BuildingObj()
            {
                UnitID = UnitID,
                Type = Type,
                Team = Team,
                Position = new Vector2Obj() { X = GetChild<Transform>().Position.X, Y = GetChild<Transform>().Position.Y },
                Rotation = GetChild<Transform>().Rotation,
                MaxHP = Status.GetValue(FloatStatus.MaxHP),
                CurHP = HP
            };
        }
    }
}
