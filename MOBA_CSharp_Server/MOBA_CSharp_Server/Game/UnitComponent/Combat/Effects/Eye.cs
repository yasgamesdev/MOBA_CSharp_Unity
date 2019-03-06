using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class Eye : Effect
    {
        float visionRadius;
        float span;
        PhysicsEntity physics;

        float timer = 0;

        public Eye(float visionRadius, Unit unitRoot, Entity root) : base(CombatType.Eye, unitRoot, root)
        {
            AddInheritedType(typeof(Eye));

            this.visionRadius = visionRadius;
            span = GetYAMLObject().GetData<float>("Span");

            Random rand = new Random(unitRoot.UnitID);
            timer = (float)(span * rand.NextDouble());

            physics = root.GetChild<PhysicsEntity>();
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            timer -= deltaTime;
            if(timer <= 0)
            {
                timer = span;

                List<int> unitIDs = physics.GetUnit(visionRadius, unitRoot.GetChild<Transform>().Position);
                foreach (int unitID in unitIDs)
                {
                    Unit unit = Root.GetChild<WorldEntity>().GetUnit(unitID);
                    if (unit != null && unit.Team != unitRoot.Team)
                    {
                        if (physics.CheckLightOfSight(unitRoot.UnitID, unit.UnitID))
                        {
                            Sight sight = unit.GetChild<Sight>();
                            if (sight != null)
                            {
                                sight.SetSight(unitRoot.Team);
                            }
                        }
                    }
                }
            }
        }
    }
}
