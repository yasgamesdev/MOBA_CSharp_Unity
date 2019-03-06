using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class Skill : Ability
    {
        public CombatAttribute SlotAttribute { get; private set; }

        public Skill(CombatAttribute skillAttribute, CombatType type, Unit unitRoot, Entity root) : base(type, unitRoot, root)
        {
            AddInheritedType(typeof(Skill));
            AddAttribute(CombatAttribute.Skill);
            AddAttribute(skillAttribute);

            SlotAttribute = skillAttribute;
        }

        protected override int GetSlotNum()
        {
            return SlotAttribute - CombatAttribute.QSkill;
        }
    }
}
