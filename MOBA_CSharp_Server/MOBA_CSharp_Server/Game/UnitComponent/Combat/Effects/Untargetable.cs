using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class Untargetable : Effect
    {
        public Untargetable(Unit unitRoot, Entity root) : base(CombatType.Untargetable, unitRoot, root)
        {
            AddInheritedType(typeof(Untargetable));

            SetBoolParam(BoolStatus.Untargetable, true, (int)UntargetablePriority.UntargetableEffect);
        }
    }
}
