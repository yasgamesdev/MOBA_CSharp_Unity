using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class Poison : Effect
    {
        public int UnitID { get; private set; }
        float attack;

        public Poison(int unitID, float duration, float attack, Unit unitRoot, Entity root) : base(CombatType.Poison, unitRoot, root)
        {
            AddInheritedType(typeof(Poison));

            UnitID = unitID;
            UpdateTimer(duration, attack);
        }

        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);

            Timer -= deltaTime;
            if(Timer <= 0)
            {
                Destroyed = true;
            }

            if (unitRoot.HP > 0)
            {
                unitRoot.Damage(UnitID, true, attack * deltaTime);
            }
        }

        public void UpdateTimer(float duration, float attack)
        {
            Timer = duration;
            this.attack = attack;
            Stack = 1;
            IsActive = true;

            Destroyed = false;
        }
    }
}
