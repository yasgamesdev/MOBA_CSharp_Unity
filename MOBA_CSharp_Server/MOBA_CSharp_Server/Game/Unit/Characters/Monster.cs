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
    public class Monster : Character
    {
        enum MonsterAttackState
        {
            Idle,
            Attack,
            Back,
        }
        MonsterAttackState state;

        Vector2 spawnPosition;
        float spawnRotation;
        float chaseRadius;
        float respawnTime;

        float timer;

        public Monster(float chaseRadius, float respawnTime, Vector2 position, float rotation, float radius, UnitType type, Entity root) : base(position, rotation, radius, type, Team.Yellow, 0, root)
        {
            AddInheritedType(typeof(Monster));

            spawnPosition = position;
            spawnRotation = rotation;
            this.chaseRadius = chaseRadius;
            this.respawnTime = respawnTime;

            state = MonsterAttackState.Idle;

            timer = respawnTime;
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            if (HP <= 0)
            {
                Cancel(CombatAttribute.Move);
                Cancel(CombatAttribute.Attack);
                state = MonsterAttackState.Idle;

                timer -= deltaTime;
                if(timer <= 0)
                {
                    GetChild<Transform>().Warp(spawnPosition, 0, spawnRotation);
                    Revive();

                    timer = respawnTime;
                }
            }
            else
            {
                if (state == MonsterAttackState.Idle)
                {
                    if(Status.AttackedUnitIDs.Count > 0)
                    {
                        Execute(CombatAttribute.Attack, Status.AttackedUnitIDs[0]);
                        state = MonsterAttackState.Attack;
                    }
                }
                else if (state == MonsterAttackState.Attack)
                {
                    Attack attack = (Attack)GetCombat(CombatAttribute.Attack);
                    if ((attack.IsExecute && (GetChild<Transform>().Position - spawnPosition).Length() >= chaseRadius) || !attack.IsExecute)
                    {
                        Cancel(CombatAttribute.Attack);
                        Execute(CombatAttribute.Move, spawnPosition);
                        state = MonsterAttackState.Back;
                    }
                }
                else
                {
                    if (Status.AttackedUnitIDs.Count > 0)
                    {
                        Cancel(CombatAttribute.Move);
                        Execute(CombatAttribute.Attack, Status.AttackedUnitIDs[0]);
                        state = MonsterAttackState.Attack;
                    }
                    else
                    {
                        Move move = (Move)GetCombat(CombatAttribute.Move);
                        if(!move.IsExecute)
                        {
                            float distance = (GetChild<Transform>().Position - spawnPosition).Length();
                            if(distance >= 0.1f)
                            {
                                Execute(CombatAttribute.Move, spawnPosition);
                            }
                        }
                    }
                }
            }
        }
    }
}
