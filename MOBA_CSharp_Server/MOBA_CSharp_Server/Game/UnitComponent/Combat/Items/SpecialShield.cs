using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOBA_CSharp_Server.Library.DataReader;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class SpecialShield : Item
    {
        public SpecialShield(int slotNum, CombatType type, Unit unitRoot, Entity root) : base(slotNum, type, unitRoot, root)
        {
            AddInheritedType(typeof(SpecialShield));
        }

        public override void Execute(object args)
        {
            if (IsExecutable(args))
            {
                ConsumeMPAndReduceStack();

                unitRoot.Damage(unitRoot.UnitID, false, unitRoot.Status.GetValue(FloatStatus.MaxHP) * 0.5f);
            }
        }
    }
}
