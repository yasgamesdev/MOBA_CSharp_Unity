using MOBA_CSharp_Server.Library.DataReader;
using MOBA_CSharp_Server.Library.ECS;
using System.Collections.Generic;
using System.Linq;

namespace MOBA_CSharp_Server.Game
{
    public class UnitStatus : Effect
    {
        public int Level { get; private set; }
        public float Exp { get; private set; }
        Dictionary<int, ExpTable> table;

        public UnitStatus(Unit unitRoot, Entity root) : base(CombatType.UnitStatus, unitRoot, root)
        {
            AddInheritedType(typeof(UnitStatus));

            Level = 0;
            Exp = 0;
            table = Root.GetChild<CSVReaderEntity>().GetExpTable(unitRoot.Type);

            SetLevel();
        }

        void SetLevel()
        {
            while(table.ContainsKey(Level + 1) && Exp >= table[Level + 1].Exp)
            {
                Exp -= table[Level + 1].Exp;
                Level++;

                SetFloatParam(FloatStatus.MaxHP, table[Level].MaxHP, true);
                SetFloatParam(FloatStatus.MaxMP, table[Level].MaxMP, true);

                SetFloatParam(FloatStatus.Attack, table[Level].Attack, true);
                SetFloatParam(FloatStatus.Defence, table[Level].Defence, true);
                SetFloatParam(FloatStatus.MagicAttack, table[Level].MagicAttack, true);
                SetFloatParam(FloatStatus.MagicDefence, table[Level].MagicDefence, true);

                SetFloatParam(FloatStatus.AttackRange, table[Level].AttackRange, true);
                SetFloatParam(FloatStatus.AttackRate, table[Level].AttackRate, true);
                SetFloatParam(FloatStatus.MovementSpeed, table[Level].MovementSpeed, true);
            }
        }

        public void AddExp(float amount)
        {
            Exp += amount;
            SetLevel();
        }

        public float GetNextExp()
        {
            if(table.ContainsKey(Level + 1))
            {
                return table[Level + 1].Exp;
            }
            else
            {
                return Exp;
            }
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            if (unitRoot.HP > 0)
            {
                UnSetAnimationParam();
            }
            else
            {
                SetAnimationParam(AnimationType.Death, 1f, (int)AnimationStatusPriority.Death);
            }
        }
    }
}
