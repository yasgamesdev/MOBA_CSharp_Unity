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
    public class TowerBullet : Actor
    {
        int targetUnitID;
        float attack;

        float targetHeight;
        float speed;

        public TowerBullet(int targetUnitID, float attack, Vector2 position, float height, float rotation, float radius, int ownerUnitID, Team team, Entity root) : base(position, height, rotation, CollisionType.None, radius, UnitType.TowerBullet, ownerUnitID, team, root)
        {
            AddInheritedType(typeof(TowerBullet));

            this.targetUnitID = targetUnitID;
            this.attack = attack;

            targetHeight = GetYAMLObject().GetData<float>("TargetHeight");
            speed = GetYAMLObject().GetData<float>("Speed");
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            Unit targetUnit = Root.GetChild<WorldEntity>().GetUnit(targetUnitID);
            if (targetUnit != null)
            {
                Vector3 targetPosition = new Vector3(targetUnit.GetChild<Transform>().Position.X, targetHeight, targetUnit.GetChild<Transform>().Position.Y);
                Vector3 position = GetChild<Transform>().GetPositionWith3D();

                if ((targetPosition - position).Length() <= speed * deltaTime)
                {
                    targetUnit.Damage(OwnerUnitID, true, attack);
                    Destroyed = true;
                }
                else
                {
                    Vector3 direction = (targetPosition - position) / (targetPosition - position).Length();
                    GetChild<Transform>().SetPositionWith3D(position + direction * speed * deltaTime);
                }
            }
            else
            {
                Destroyed = true;
            }
        }
    }
}
