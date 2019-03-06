using Microsoft.Xna.Framework;
using MOBA_CSharp_Server.Library.DataReader;
using MOBA_CSharp_Server.Library.ECS;
using MOBA_CSharp_Server.Library.Physics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOBA_CSharp_Server.Game
{
    public class Unit : Entity
    {
        public UnitType Type { get; private set; }
        public int UnitID { get; private set; }
        public int OwnerUnitID { get; private set; }
        public Team Team { get; private set; }

        //Gold
        public float Gold { get; private set; }

        //HPMP
        public float HP { get; private set; }
        public float MP { get; private set; }

        List<DamageHistory> hpDamageHistories = new List<DamageHistory>();
        List<DamageHistory> mpDamageHistories = new List<DamageHistory>();

        //Status
        public Status Status { get; private set; } = new Status();

        //Combat
        Dictionary<CombatAttribute, List<Combat>> combats = new Dictionary<CombatAttribute, List<Combat>>();

        int itemSlotNum;

        public Unit(Vector2 position, float height, float rotation, CollisionType collisionType, float radius, UnitType type, Team team, float gold, Entity root) : base(root)
        {
            AddInheritedType(typeof(Unit));

            itemSlotNum = Root.GetChild<DataReaderEntity>().GetYAMLObject(@"YAML\ServerConfig.yml").GetData<int>("ItemSlotNum");

            Type = type;
            UnitID = Root.GetChild<WorldEntity>().GenerateUnitID();
            OwnerUnitID = UnitID;
            Team = team;
            Gold = gold;

            AddChild(new UnitStatus(this, Root));
            AddChild(new Transform(position, height, rotation, collisionType, radius, this, Root));

            SetStatus(0);
            Revive();
        }

        public Unit(Vector2 position, float height, float rotation, CollisionType collisionType, float radius, UnitType type, int ownerUnitID, Team team, float gold, Entity root) : base(root)
        {
            AddInheritedType(typeof(Unit));

            Type = type;
            UnitID = Root.GetChild<WorldEntity>().GenerateUnitID();
            OwnerUnitID = ownerUnitID;
            Team = team;
            Gold = gold;

            AddChild(new UnitStatus(this, Root));
            AddChild(new Transform(position, height, rotation, collisionType, radius, this, Root));

            SetStatus(0);
            Revive();
        }

        protected YAMLObject GetYAMLObject()
        {
            var yaml = Root.GetChild<DataReaderEntity>().GetYAMLObject(Type);
            if(yaml != null)
            {
                return yaml;
            }
            else
            {
                return Root.GetChild<DataReaderEntity>().GetYAMLObject(@"YAML\Units\Default.yml");
            }
        }

        public void SetStatus(float deltaTime)
        {
            //Init
            Dictionary<FloatStatus, List<FloatParam>> floatParams = new Dictionary<FloatStatus, List<FloatParam>>();
            foreach(FloatStatus type in Enum.GetValues(typeof(FloatStatus)))
            {
                floatParams.Add(type, new List<FloatParam>());
            }
            Dictionary<BoolStatus, List<BoolParam>> boolParams = new Dictionary<BoolStatus, List<BoolParam>>();
            foreach (BoolStatus type in Enum.GetValues(typeof(BoolStatus)))
            {
                boolParams.Add(type, new List<BoolParam>());
            }
            Dictionary<Team, List<VisionParam>> visionParams = new Dictionary<Team, List<VisionParam>>();
            foreach(Team team in Enum.GetValues(typeof(Team)))
            {
                visionParams.Add(team, new List<VisionParam>());
            }
            List<AnimParam> animParams = new List<AnimParam>();

            //Add
            foreach (Combat combat in GetChildren<Combat>())
            {
                foreach(FloatParam param in combat.floatParams.Values)
                {
                    floatParams[param.Type].Add(param);
                }
                foreach (BoolParam param in combat.boolParams.Values)
                {
                    boolParams[param.Type].Add(param);
                }
                foreach (VisionParam param in combat.visionParams.Values)
                {
                    visionParams[param.Team].Add(param);
                }
                if(combat.animParam != null)
                {
                    animParams.Add(combat.animParam);
                }
            }

            //Set
            foreach (FloatStatus type in Enum.GetValues(typeof(FloatStatus)))
            {
                if (floatParams[type].Count > 0)
                {
                    float addSum = floatParams[type].Where(x => x.IsAdd).Sum(x => x.Value);
                    float mulSum = floatParams[type].Where(x => !x.IsAdd).Sum(x => x.Value);
                    Status.FloatStatus[(int)type] = addSum * (1.0f + mulSum);
                }
                else
                {
                    Status.FloatStatus[(int)type] = 0;
                }
            }
            foreach (BoolStatus type in Enum.GetValues(typeof(BoolStatus)))
            {
                if (boolParams[type].Count > 0)
                {
                    int minPriority = boolParams[type].Min(x => x.Priority);
                    Status.BoolStatus[(int)type] = boolParams[type].First(x => x.Priority == minPriority).Value;
                }
                else
                {
                    Status.BoolStatus[(int)type] = false;
                }
            }
            foreach (Team team in Enum.GetValues(typeof(Team)))
            {
                if (visionParams[team].Count > 0)
                {
                    int minPriority = visionParams[team].Min(x => x.Priority);
                    Status.VisionStatus[(int)team] = visionParams[team].First(x => x.Priority == minPriority).Value;
                }
                else
                {
                    Status.VisionStatus[(int)team] = false;
                }
            }
            if(animParams.Count > 0)
            {
                int minPriority = animParams.Min(x => x.Priority);
                var animParam = animParams.First(x => x.Priority == minPriority);
                Status.SetAnimationStatus(animParam.Type, animParam.SpeedRate, deltaTime);
            }
            else
            {
                Status.SetAnimationStatus(AnimationType.Idle, 1f, deltaTime);
            }
        }

        public override void AddChild(Entity entity)
        {
            base.AddChild(entity);

            if(entity is Combat)
            {
                Combat combat = (Combat)entity;
                foreach(CombatAttribute attribute in combat.attributes)
                {
                    if(!combats.ContainsKey(attribute))
                    {
                        combats.Add(attribute, new List<Combat>());
                    }
                    combats[attribute].Add(combat);
                }
            }
        }

        public override void RemoveChild(Entity entity)
        {
            base.RemoveChild(entity);

            if(entity is Combat)
            {
                Combat combat = (Combat)entity;
                foreach (CombatAttribute attribute in combat.attributes)
                {
                    combats[attribute].Remove(combat);
                    if(combats[attribute].Count == 0)
                    {
                        combats.Remove(attribute);
                    }
                }
            }
        }

        public Combat GetCombat(CombatAttribute attribute)
        {
            if(combats.ContainsKey(attribute))
            {
                return combats[attribute][0];
            }
            else
            {
                return null;
            }
        }

        public Combat[] GetCombats(CombatAttribute attribute)
        {
            if (combats.ContainsKey(attribute))
            {
                return combats[attribute].ToArray();
            }
            else
            {
                return new Combat[0];
            }
        }

        public void Execute(CombatAttribute attribute, object args)
        {
            foreach(var combat in GetCombats(attribute))
            {
                combat.Execute(args);
            }
        }

        public void Cancel(CombatAttribute attribute)
        {
            foreach (var combat in GetCombats(attribute))
            {
                combat.Cancel();
            }
        }

        public void ConfirmDamage()
        {
            ConfirmHPDamage();
            ConfirmMPDamage();
        }

        void ConfirmHPDamage()
        {
            Status.Damaged = hpDamageHistories.Any(x => x.IsDamage);
            Status.AttackedUnitIDs = hpDamageHistories.Select(x => x.UnitID).ToList();

            float sum = hpDamageHistories.Sum(x => x.IsDamage ? x.Amount : -x.Amount);

            float nextHP = HP - sum;
            if (nextHP > Status.FloatStatus[(int)FloatStatus.MaxHP])
            {
                nextHP = Status.FloatStatus[(int)FloatStatus.MaxHP];
            }
            else if (nextHP < 0)
            {
                nextHP = 0;
            }

            Status.Dead = HP > 0 && nextHP <= 0;
            HP = nextHP;

            if(Status.Dead)
            {
                List<int> attackedUnitIDs = hpDamageHistories.Where(x => x.IsDamage).Select(x => x.UnitID).ToList();
                attackedUnitIDs = attackedUnitIDs.Distinct().ToList();

                if(attackedUnitIDs.Count > 0)
                {
                    int level = GetChild<UnitStatus>().Level;

                    float exp = Utilities.CalculateExp(Type, level) / attackedUnitIDs.Count;
                    float gold = Utilities.CalculateGold(Type, level) / attackedUnitIDs.Count;

                    foreach (int attackedUnitID in attackedUnitIDs)
                    {
                        Unit unit = Root.GetChild<WorldEntity>().GetUnit(attackedUnitID);
                        if (unit != null)
                        {
                            unit.AddExp(exp);
                            unit.AddGold(gold);
                        }
                    }
                }
            }

            hpDamageHistories.Clear();
        }

        public void AddExp(float amount)
        {
            GetChild<UnitStatus>().AddExp(amount);
        }

        public void AddGold(float amount)
        {
            Gold += amount;
        }

        void ConfirmMPDamage()
        {
            float sum = mpDamageHistories.Sum(x => x.IsDamage ? x.Amount : -x.Amount);

            float nextMP = MP - sum;
            if (nextMP > Status.FloatStatus[(int)FloatStatus.MaxMP])
            {
                nextMP = Status.FloatStatus[(int)FloatStatus.MaxMP];
            }
            else if (nextMP < 0)
            {
                nextMP = 0;
            }

            MP = nextMP;

            mpDamageHistories.Clear();
        }

        public void Revive()
        {
            HP = Status.FloatStatus[(int)FloatStatus.MaxHP];
            MP = Status.FloatStatus[(int)FloatStatus.MaxMP];
        }

        public void Damage(int unitID, bool isDamage, float amount)
        {
            hpDamageHistories.Add(new DamageHistory(unitID, isDamage, amount));

            Unit enemyUnit = Root.GetChild<WorldEntity>().GetUnit(unitID);
            if(enemyUnit != null && enemyUnit.Team != Team)
            {
                Sight enemySight = enemyUnit.GetChild<Sight>();
                if(enemySight != null)
                {
                    enemySight.SetSight(Team);
                }

                Sight sight = GetChild<Sight>();
                if(sight != null)
                {
                    sight.SetSight(enemyUnit.Team);
                }
            }
        }

        public void DamageMP(int unitID, bool isDamage, float amount)
        {
            mpDamageHistories.Add(new DamageHistory(unitID, isDamage, amount));
        }

        public UnitObj GetUnitObj()
        {
            return new UnitObj()
            {
                UnitID = UnitID,
                Type = Type,
                Team = Team,
                Position = new Vector2Obj() { X = GetChild<Transform>().Position.X, Y = GetChild<Transform>().Position.Y },
                Rotation = GetChild<Transform>().Rotation,
                Warped = GetChild<Transform>().Warped,
                AnimationNum = (byte)Status.AnimationStatus,
                SpeedRate = Status.SpeedRate,
                PlayTime = Status.PlayTime,
                MaxHP = Status.GetValue(FloatStatus.MaxHP),
                CurHP = HP
            };
        }

        public PlayerObj GetPlayerObj()
        {
            //Effects
            List<CombatObj> effects = new List<CombatObj>();
            foreach(Effect effect in GetChildren<Effect>())
            {
                if(effect.IsSendDataToClient)
                {
                    effects.Add(effect.GetCombatObj());
                }
            }

            //Items
            List<CombatObj> items = new List<CombatObj>();
            foreach (Item item in GetChildren<Item>())
            {
                items.Add(item.GetCombatObj());
            }

            //Skills
            List<CombatObj> skills = new List<CombatObj>();
            foreach (Skill skill in GetChildren<Skill>())
            {
                skills.Add(skill.GetCombatObj());
            }

            return new PlayerObj()
            {
                UnitID = UnitID,
                Exp = GetChild<UnitStatus>().Exp,
                NextExp = GetChild<UnitStatus>().GetNextExp(),
                Gold = Gold,
                Effects = effects.ToArray(),
                Items = items.ToArray(),
                Skills = skills.ToArray()
            };
        }

        public void BuyItem(CombatType type)
        {
            if(GetChild<OnBase>() == null || type < CombatType.Potion)
            {
                return;
            }

            float buyingPrice = Root.GetChild<CSVReaderEntity>().GetItemTable(type).BuyingPrice;
            int slotNum = GetEmptySlotNum();

            if(Gold >= buyingPrice && slotNum != -1)
            {
                AddChild(ItemFactory.CreateItem(type, slotNum, this, Root));
                AddGold(-buyingPrice);
            }
        }

        int GetEmptySlotNum()
        {
            List<Item> items = GetCombats(CombatAttribute.Item).Select(x => (Item)x).ToList();
            for(int i=0; i<itemSlotNum; i++)
            {
                if(items.All(x => x.SlotNum != i))
                {
                    return i;
                }
            }

            return -1;
        }

        public void SellItem(int slotNum)
        {
            if (GetChild<OnBase>() == null || slotNum >= itemSlotNum)
            {
                return;
            }

            Item item = GetItem(slotNum);
            if (item != null)
            {
                item.ClearReference();
                RemoveChild(item);

                AddGold(Root.GetChild<CSVReaderEntity>().GetItemTable(item.Type).SellingPrice * item.Count);
            }
        }

        public void UseItem(int slotNum)
        {
            if (slotNum >= itemSlotNum)
            {
                return;
            }

            Item item = GetItem(slotNum);
            if (item != null)
            {
                item.Execute(null);
            }
        }

        Item GetItem(int slotNum)
        {
            List<Item> items = GetCombats(CombatAttribute.Item).Select(x => (Item)x).ToList();
            return items.FirstOrDefault(x => x.SlotNum == slotNum);
        }
    }
}
