using MOBA_CSharp_Server.Library.DataReader;
using MOBA_CSharp_Server.Library.ECS;
using System.Collections.Generic;

namespace MOBA_CSharp_Server.Game
{
    public class Combat : UnitComponent
    {
        public CombatType Type { get; private set; }
        //Attributes
        public List<CombatAttribute> attributes { get; private set; } = new List<CombatAttribute>();
        //Display
        public float Timer { get; protected set; }
        public int Stack { get; protected set; }
        public bool IsActive { get; protected set; }

        public bool StackDisplayCount { get; protected set; }
        public bool ExecuteConsumeCount { get; protected set; }
        public float Cooldown { get; protected set; }
        public int Charge { get; protected set; }
        public int MaxCharge { get; protected set; }
        public int Count { get; protected set; }
        public float Cost { get; protected set; }

        //Params
        public Dictionary<FloatStatus, FloatParam> floatParams = new Dictionary<FloatStatus, FloatParam>();
        public Dictionary<BoolStatus, BoolParam> boolParams = new Dictionary<BoolStatus, BoolParam>();
        public Dictionary<Team, VisionParam> visionParams = new Dictionary<Team, VisionParam>();
        public AnimParam animParam = null;

        //Execute
        public bool IsExecute { get; private set; }

        public Combat(CombatType type, Unit unitRoot, Entity root) : base(unitRoot, root)
        {
            AddInheritedType(typeof(Combat));

            Type = type;

            StackDisplayCount = GetYAMLObject().GetData<bool>("StackDisplayCount");
            ExecuteConsumeCount = GetYAMLObject().GetData<bool>("ExecuteConsumeCount");
            Cooldown = GetYAMLObject().GetData<float>("Cooldown");
            Charge = GetYAMLObject().GetData<int>("Charge");
            MaxCharge = GetYAMLObject().GetData<int>("MaxCharge");
            Count = GetYAMLObject().GetData<int>("Count");
            Cost = GetYAMLObject().GetData<float>("Cost");

            if (!StackDisplayCount && Charge < MaxCharge)
            {
                Timer = Cooldown;
            }

            SetStackAndIsActive();
        }

        void SetStackAndIsActive()
        {
            Stack = StackDisplayCount ? Count : Charge;
            IsActive = StackDisplayCount ? Count > 0 && unitRoot.MP >= Cost : Charge > 0 && unitRoot.MP >= Cost;
        }

        public void AddAttribute(CombatAttribute attribute)
        {
            attributes.Add(attribute);
        }

        public virtual void Execute(object args)
        {
            if(IsExecutable(args))
            {
                IsExecute = true;
            }
        }

        public virtual void Cancel()
        {
            IsExecute = false;
        }

        public virtual bool IsExecutable(object args)
        {
            if(StackDisplayCount)
            {
                return unitRoot.HP > 0 && unitRoot.MP >= Cost && Count > 0 && (Cooldown == 0 || (Cooldown > 0 && Timer <= 0));
            }
            else
            {
                return unitRoot.HP > 0 && unitRoot.MP >= Cost && Charge > 0;
            }
        }

        protected virtual void ConsumeMPAndReduceStack()
        {
            unitRoot.DamageMP(unitRoot.UnitID, true, Cost);
            if(StackDisplayCount)
            {
                Count--;
                if(Cooldown > 0)
                {
                    Timer = Cooldown;
                }
            }
            else
            {
                Charge--;
                if(Timer <= 0)
                {
                    Timer = Cooldown;
                }
            }
        }

        protected virtual bool ContinueExecution()
        {
            return true;
        }

        protected virtual void ExecuteProcess(float deltaTime)
        {

        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            //Timer
            if(StackDisplayCount)
            {
                if(Cooldown > 0 && Timer > 0)
                {
                    Timer -= deltaTime;
                    if(Timer < 0)
                    {
                        Timer = 0;
                    }
                }
            }
            else
            {
                if (Charge < MaxCharge)
                {
                    Timer -= deltaTime;
                    if (Timer <= 0)
                    {
                        Charge++;

                        if (Charge < MaxCharge)
                        {
                            Timer = Cooldown;
                        }
                        else
                        {
                            Timer = 0;
                        }
                    }
                }
            }
            //Stack&&IsActive
            SetStackAndIsActive();

            if (IsExecute)
            {
                if(ContinueExecution())
                {
                    ExecuteProcess(deltaTime);
                }
                else
                {
                    Cancel();
                }
            }
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
                return Root.GetChild<DataReaderEntity>().GetYAMLObject(@"YAML\Combats\Default.yml");
            }
        }

        protected void SetFloatParam(FloatStatus type, float value, bool isAdd)
        {
            if(!floatParams.ContainsKey(type))
            {
                floatParams.Add(type, new FloatParam(type, value, isAdd));
            }
            else
            {
                floatParams[type] = new FloatParam(type, value, isAdd);
            }
        }

        protected void UnSetFloatParam(FloatStatus type)
        {
            if(floatParams.ContainsKey(type))
            {
                floatParams.Remove(type);
            }
        }

        protected void SetBoolParam(BoolStatus type, bool value, int priority)
        {
            if (!boolParams.ContainsKey(type))
            {
                boolParams.Add(type, new BoolParam(type, value, priority));
            }
            else
            {
                boolParams[type] = new BoolParam(type, value, priority);
            }
        }

        protected void UnSetBoolParam(BoolStatus type)
        {
            if (boolParams.ContainsKey(type))
            {
                boolParams.Remove(type);
            }
        }

        protected void SetVisionParam(Team team, bool value, int priority)
        {
            if (!visionParams.ContainsKey(team))
            {
                visionParams.Add(team, new VisionParam(team, value, priority));
            }
            else
            {
                visionParams[team] = new VisionParam(team, value, priority);
            }
        }

        protected void UnSetVisionParam(Team team)
        {
            if (visionParams.ContainsKey(team))
            {
                visionParams.Remove(team);
            }
        }

        protected void SetAnimationParam(AnimationType type, float speedRate, int priority)
        {
            animParam = new AnimParam(type, speedRate, priority);
        }

        protected void UnSetAnimationParam()
        {
            animParam = null;
        }

        public CombatObj GetCombatObj()
        {
            return new CombatObj()
            {
                Type = Type,
                SlotNum = (byte)GetSlotNum(),
                Timer = Timer,
                Stack = (byte)Stack,
                IsActive = IsActive
            };
        }

        protected virtual int GetSlotNum()
        {
            return 0;
        }
    }
}
