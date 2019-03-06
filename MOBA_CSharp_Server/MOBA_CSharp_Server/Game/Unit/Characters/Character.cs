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
    public class Character : Unit
    {
        public Character(Vector2 position, float rotation, float radius, UnitType type, Team team, float gold, Entity root) : base(position, 0, rotation, CollisionType.Dynamic, radius, type, team, gold, root)
        {
            AddInheritedType(typeof(Character));

            AddChild(new Sight(false, this, Root));
            AddChild(new Eye(GetYAMLObject().GetData<float>("VisionRadius"), this, Root));

            AddChild(new Move(this, Root));
            AddChild(new Attack(this, Root));
        }
    }
}
