using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOBA_CSharp_Server.Library.DataReader;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class Potion : Item
    {
        public Potion(int slotNum, CombatType type, Unit unitRoot, Entity root) : base(slotNum, type, unitRoot, root)
        {
            AddInheritedType(typeof(Potion));
        }

        public override void Execute(object args)
        {
            unitRoot.Damage(unitRoot.UnitID, false, 100);
            Destroyed = true;
        }
    }
}
