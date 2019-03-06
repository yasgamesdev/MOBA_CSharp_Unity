using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBA_CSharp_Server.Game
{
    public class DamageHistory
    {
        public int UnitID;
        public bool IsDamage;
        public float Amount;

        public DamageHistory(int unitID, bool isDamage, float amount)
        {
            UnitID = unitID;
            IsDamage = isDamage;
            Amount = amount;
        }
    }
}
