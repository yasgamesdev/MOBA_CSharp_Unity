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
    public class Champion : Character
    {
        public uint PeerID { get; private set; }

        float championRespawnTime;
        float timer;

        public Champion(uint peerID, Vector2 position, float rotation, float radius, UnitType type, Team team, float gold, Entity root) : base(position, rotation, radius, type, team, gold, root)
        {
            AddInheritedType(typeof(Champion));

            championRespawnTime = Root.GetChild<DataReaderEntity>().GetYAMLObject(@"YAML\ServerConfig.yml").GetData<float>("ChampionRespawnTime");

            PeerID = peerID;

            AddChild(new Recall(this, Root));

            AddQWERSkill(CombatAttribute.QSkill);
            AddQWERSkill(CombatAttribute.WSkill);
            AddQWERSkill(CombatAttribute.ESkill);
            AddQWERSkill(CombatAttribute.RSkill);
        }

        protected void AddQWERSkill(CombatAttribute attribute)
        {
            Ability ability = SkillFactory.CreateSkill(Type, attribute, this, Root);
            if(ability != null)
            {
                AddChild(ability);
            }
        }

        public ChampionObj GetChampionObj()
        {
            return new ChampionObj()
            {
                UnitID = UnitID,
                Type = Type,
                DisplayName = Root.GetChild<NetworkEntity>().GetName(PeerID),
                Team = Team,
                Level = (byte)GetChild<UnitStatus>().Level,
                Position = new Vector2Obj() { X = GetChild<Transform>().Position.X, Y = GetChild<Transform>().Position.Y },
                Rotation = GetChild<Transform>().Rotation,
                Warped = GetChild<Transform>().Warped,
                AnimationNum = (byte)Status.AnimationStatus,
                SpeedRate = Status.SpeedRate,
                PlayTime = Status.PlayTime,
                MaxHP = Status.GetValue(FloatStatus.MaxHP),
                CurHP = HP,
                MaxMP = Status.GetValue(FloatStatus.MaxMP),
                CurMP = MP
            };
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            if(Status.Dead)
            {
                timer = championRespawnTime;
            }

            if(timer > 0)
            {
                timer -= deltaTime;
                if(timer <= 0)
                {
                    timer = 0;

                    GetChild<Transform>().Warp(Root.GetChild<WorldEntity>().GetFountainPosition(Team), 0, 0);
                    Revive();
                }
            }
        }
    }
}
