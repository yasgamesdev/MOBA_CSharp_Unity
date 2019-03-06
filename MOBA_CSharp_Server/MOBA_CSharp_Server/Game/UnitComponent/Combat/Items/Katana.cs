using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOBA_CSharp_Server.Library.DataReader;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class Katana : Item
    {
        public Katana(int slotNum, CombatType type, Unit unitRoot, Entity root) : base(slotNum, type, unitRoot, root)
        {
            AddInheritedType(typeof(Katana));
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            if(unitRoot.HP > 0)
            {
                SetFloatParam(FloatStatus.Attack, unitRoot.Status.GetValue(FloatStatus.MaxHP) / unitRoot.HP * 20.0f, true);
            }
            else
            {
                UnSetFloatParam(FloatStatus.Attack);
            }
        }
    }
}
