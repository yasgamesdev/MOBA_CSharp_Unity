using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class Ability : Combat
    {
        public Ability(CombatType type, Unit unitRoot, Entity root) : base(type, unitRoot, root)
        {
            AddInheritedType(typeof(Ability));
            AddAttribute(CombatAttribute.Ability);
        }
    }
}
