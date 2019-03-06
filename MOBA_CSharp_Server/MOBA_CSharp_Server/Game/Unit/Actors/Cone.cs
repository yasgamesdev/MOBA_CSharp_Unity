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
    public class Cone : Actor
    {
        float duration;

        float timer;

        public Cone(float duration, Vector2 position, float height, float rotation, float radius, UnitType type, int ownerUnitID, Team team, Entity root) : base(position, height, rotation, CollisionType.None, radius, type, ownerUnitID, team, root)
        {
            AddInheritedType(typeof(Cone));

            this.duration = duration;
            timer = duration;
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            timer -= deltaTime;

            Unit ownerUnit = Root.GetChild<WorldEntity>().GetUnit(OwnerUnitID);
            if(ownerUnit != null)
            {
                GetChild<Transform>().SetPositionWith3D(ownerUnit.GetChild<Transform>().GetPositionWith3D());
                GetChild<Transform>().SetRotation(ownerUnit.GetChild<Transform>().Rotation);
            }
            else
            {
                Destroyed = true;
            }

            if (timer <= 0)
            {
                Destroyed = true;
            }
        }
    }
}
