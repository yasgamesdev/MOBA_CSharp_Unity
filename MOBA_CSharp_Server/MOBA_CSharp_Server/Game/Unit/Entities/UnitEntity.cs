using Collision2D;
using ECS;
using System.Numerics;

namespace MOBA_CSharp_Server.Game
{
    public class UnitEntity : Entity
    {
        public Team Team { get; private set; }

        public UnitEntity(Team team, Body body, float angle, RootEntity root) : base(root)
        {
            Team = team;

            AddComponent(new BodyComponent(body, angle, root));
            AddComponent(new VisionComponent(team, root));
        }

        public override void Destroy()
        {
            base.Destroy();

            GetComponent<BodyComponent>().Destroy();
            root.GetChild<VisionManagerEntity>().RemoveUnit(this);
        }
    }
}
