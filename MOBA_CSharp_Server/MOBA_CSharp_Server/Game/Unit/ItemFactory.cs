using Microsoft.Xna.Framework;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public static class ItemFactory
    {
        public static Item CreateItem(CombatType type, int slotNum, Unit unitRoot, Entity root)
        {
            switch(type)
            {
                case CombatType.Potion:
                    return new Potion(slotNum, type, unitRoot, root);
                case CombatType.RegenArmor:
                    return new RegenArmor(slotNum, type, unitRoot, root);
                case CombatType.RegenPierce:
                    return new RegenPierce(slotNum, type, unitRoot, root);
                case CombatType.Katana:
                    return new Katana(slotNum, type, unitRoot, root);
                case CombatType.SpecialShield:
                    return new SpecialShield(slotNum, type, unitRoot, root);
                case CombatType.SpecialBoot:
                    return new SpecialBoot(slotNum, type, unitRoot, root);
                default:
                    return new Item(slotNum, type, unitRoot, root);
            }
        }
    }
}
