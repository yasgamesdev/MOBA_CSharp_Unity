using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOBA_CSharp_Server.Library.DataReader;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class RegenPierce : Item
    {
        public RegenPierce(int slotNum, CombatType type, Unit unitRoot, Entity root) : base(slotNum, type, unitRoot, root)
        {
            AddInheritedType(typeof(RegenPierce));
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            if(unitRoot.HP > 0)
            {
                unitRoot.DamageMP(unitRoot.UnitID, false, unitRoot.Status.GetValue(FloatStatus.MaxMP) * 0.01f * deltaTime);
            }
        }
    }
}
