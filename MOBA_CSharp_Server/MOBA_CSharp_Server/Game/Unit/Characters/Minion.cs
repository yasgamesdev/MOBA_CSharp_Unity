using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MOBA_CSharp_Server.Library.DataReader;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class Minion : Character
    {
        enum MinionAttackState
        {
            Start,
            Move,
            Attack,
            Fin
        }
        MinionAttackState state;

        Dictionary<int, Vector2> relayPoints;
        int currentPoint;

        float attackRadius;
        float destroyTime;
        float deathTimer;

        public Minion(Dictionary<int, Vector2> relayPoints, Vector2 position, float rotation, float radius, Team team, Entity root) : base(position, rotation, radius, UnitType.Minion, team, 0, root)
        {
            AddInheritedType(typeof(Minion));

            this.relayPoints = relayPoints;

            attackRadius = GetYAMLObject().GetData<float>("AttackRadius");
            destroyTime = GetYAMLObject().GetData<float>("DestroyTime");

            state = MinionAttackState.Start;
            currentPoint = 0;
            deathTimer = destroyTime;
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            if (HP <= 0 || !relayPoints.ContainsKey(currentPoint))
            {
                Cancel(CombatAttribute.Move);
                Cancel(CombatAttribute.Attack);
                state = MinionAttackState.Fin;

                if (HP <= 0)
                {
                    deathTimer -= deltaTime;
                    if (deathTimer <= 0)
                    {
                        Destroyed = true;
                    }
                }
            }
            else
            {
                if (state == MinionAttackState.Start && relayPoints.ContainsKey(currentPoint))
                {
                    Execute(CombatAttribute.Move, relayPoints[currentPoint]);
                    state = MinionAttackState.Move;
                }
                else if (state == MinionAttackState.Move)
                {
                    if (Status.AttackedUnitIDs.Count > 0)
                    {
                        Cancel(CombatAttribute.Move);
                        Execute(CombatAttribute.Attack, Status.AttackedUnitIDs[0]);
                        state = MinionAttackState.Attack;
                    }
                    else if (!GetCombat(CombatAttribute.Move).IsExecute)
                    {
                        float distance = (GetChild<Transform>().Position - relayPoints[currentPoint]).Length();
                        if(distance >= 0.1f)
                        {
                            Execute(CombatAttribute.Move, relayPoints[currentPoint]);
                        }
                        else
                        {
                            currentPoint++;
                            if (relayPoints.ContainsKey(currentPoint))
                            {
                                Execute(CombatAttribute.Move, relayPoints[currentPoint]);
                            }
                        }
                    }
                    else
                    {
                        var unitIDs = Root.GetChild<PhysicsEntity>().GetUnit(attackRadius, GetChild<Transform>().Position);
                        foreach (int unitID in unitIDs)
                        {
                            Unit targetUnit = Root.GetChild<WorldEntity>().GetUnit(unitID);
                            if (targetUnit != null &&
                                Team != targetUnit.Team &&
                                targetUnit.Status.GetValue(Team) &&
                                targetUnit.HP > 0 &&
                                !targetUnit.Status.GetValue(BoolStatus.Untargetable))
                            {
                                Execute(CombatAttribute.Attack, targetUnit.UnitID);
                                state = MinionAttackState.Attack;
                                break;
                            }
                        }
                    }
                }
                else if (state == MinionAttackState.Attack)
                {
                    if (!GetCombat(CombatAttribute.Attack).IsExecute)
                    {
                        Cancel(CombatAttribute.Attack);
                        Execute(CombatAttribute.Move, relayPoints[currentPoint]);
                        state = MinionAttackState.Move;
                    }
                }
            }
        }
    }
}
