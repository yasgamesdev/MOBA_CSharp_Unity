using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOBA_CSharp_Server.Library.DataReader;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class Effect : Combat
    {
        public bool IsSendDataToClient;

        public Effect(CombatType type, Unit unitRoot, Entity root) : base(type, unitRoot, root)
        {
            AddInheritedType(typeof(Effect));
            AddAttribute(CombatAttribute.Effect);

            IsSendDataToClient = GetYAMLObject().GetData<bool>("IsSendDataToClient");
        }
    }
}
