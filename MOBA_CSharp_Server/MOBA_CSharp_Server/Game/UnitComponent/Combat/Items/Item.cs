using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOBA_CSharp_Server.Library.DataReader;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class Item : Combat
    {
        public int SlotNum;

        public Item(int slotNum, CombatType type, Unit unitRoot, Entity root) : base(type, unitRoot, root)
        {
            AddInheritedType(typeof(Item));
            AddAttribute(CombatAttribute.Item);

            SlotNum = slotNum;

            ItemTable itemTable = Root.GetChild<CSVReaderEntity>().GetItemTable(type);

            if (itemTable.AddMaxHP != 0)
            {
                SetFloatParam(FloatStatus.MaxHP, itemTable.AddMaxHP, true);
            }
            if (itemTable.MulMaxHP != 0)
            {
                SetFloatParam(FloatStatus.MaxHP, itemTable.MulMaxHP, false);
            }
            if (itemTable.AddMaxMP != 0)
            {
                SetFloatParam(FloatStatus.MaxMP, itemTable.AddMaxMP, true);
            }
            if (itemTable.MulMaxMP != 0)
            {
                SetFloatParam(FloatStatus.MaxMP, itemTable.MulMaxMP, false);
            }

            if (itemTable.AddAttack != 0)
            {
                SetFloatParam(FloatStatus.Attack, itemTable.AddAttack, true);
            }
            if (itemTable.MulAttack != 0)
            {
                SetFloatParam(FloatStatus.Attack, itemTable.MulAttack, false);
            }
            if (itemTable.AddDefence != 0)
            {
                SetFloatParam(FloatStatus.Defence, itemTable.AddDefence, true);
            }
            if (itemTable.MulDefence != 0)
            {
                SetFloatParam(FloatStatus.Defence, itemTable.MulDefence, false);
            }
            if (itemTable.AddMagicAttack != 0)
            {
                SetFloatParam(FloatStatus.MagicAttack, itemTable.AddMagicAttack, true);
            }
            if (itemTable.MulMagicAttack != 0)
            {
                SetFloatParam(FloatStatus.MagicAttack, itemTable.MulMagicAttack, false);
            }
            if (itemTable.AddMagicDefence != 0)
            {
                SetFloatParam(FloatStatus.MagicDefence, itemTable.AddMagicDefence, true);
            }
            if (itemTable.MulMagicDefence != 0)
            {
                SetFloatParam(FloatStatus.MagicDefence, itemTable.MulMagicDefence, false);
            }

            if (itemTable.AddAttackRate != 0)
            {
                SetFloatParam(FloatStatus.AttackRate, itemTable.AddAttackRate, true);
            }
            if (itemTable.MulAttackRate != 0)
            {
                SetFloatParam(FloatStatus.AttackRate, itemTable.MulAttackRate, false);
            }
            if (itemTable.AddAttackRange != 0)
            {
                SetFloatParam(FloatStatus.AttackRange, itemTable.AddAttackRange, true);
            }
            if (itemTable.MulAttackRange != 0)
            {
                SetFloatParam(FloatStatus.AttackRange, itemTable.MulAttackRange, false);
            }
            if (itemTable.AddMovementSpeed != 0)
            {
                SetFloatParam(FloatStatus.MovementSpeed, itemTable.AddMovementSpeed, true);
            }
            if (itemTable.MulMovementSpeed != 0)
            {
                SetFloatParam(FloatStatus.MovementSpeed, itemTable.MulMovementSpeed, false);
            }

            if (GetType() == typeof(Item))
            {
                StackDisplayCount = true;
                ExecuteConsumeCount = false;
                Cooldown = 0;
                Charge = 0;
                MaxCharge = 0;
                Count = 1;
                Cost = 0;
            }
        }

        protected override int GetSlotNum()
        {
            return SlotNum;
        }
    }
}
