using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOBA_CSharp_Server.Library.DataReader;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class Sight : Effect
    {
        readonly float Duration;
        public bool IsBuilding { get; private set; }
        Dictionary<Team, float> timers = new Dictionary<Team, float>();

        public Sight(bool isBuilding, Unit unitRoot, Entity root) : base(CombatType.Sight, unitRoot, root)
        {
            AddInheritedType(typeof(Sight));

            Duration = GetYAMLObject().GetData<float>("Duration");
            IsBuilding = isBuilding;

            foreach (Team team in Enum.GetValues(typeof(Team)))
            {
                if(isBuilding || team == unitRoot.Team)
                {
                    timers.Add(team, Duration);
                }
                else
                {
                    if(unitRoot.UnitID != unitRoot.OwnerUnitID)
                    {
                        Unit ownerUnit = Root.GetChild<WorldEntity>().GetUnit(unitRoot.OwnerUnitID);
                        if(ownerUnit != null)
                        {
                            Sight sight = ownerUnit.GetChild<Sight>();
                            if (sight != null && sight.GetSight(team))
                            {
                                timers.Add(team, Duration);
                                continue;
                            }
                        }
                    }

                    timers.Add(team, 0);
                }
            }
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            List<Team> keys = new List<Team>(timers.Keys);
            foreach (var team in keys)
            {
                if (team != unitRoot.Team)
                {
                    timers[team] -= IsBuilding ? 0 : deltaTime;
                }

                if (timers[team] > 0)
                {
                    SetVisionParam(team, true, (int)VisionStatusPriority.Sight);
                }
                else
                {
                    UnSetVisionParam(team);
                }
            }
        }

        public bool GetSight(Team team)
        {
            return timers[team] > 0;
        }

        public void SetSight(Team team)
        {
            timers[team] = Duration;
        }
    }
}
