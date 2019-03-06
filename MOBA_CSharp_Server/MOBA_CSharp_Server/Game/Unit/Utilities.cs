using Microsoft.Xna.Framework;

namespace MOBA_CSharp_Server.Game
{
    public static class Utilities
    {
        public static float GetUnitDistance(Unit unit0, Unit unit1)
        {
            return (unit0.GetChild<Transform>().Position - unit1.GetChild<Transform>().Position).Length() - unit0.GetChild<Transform>().Radius - unit1.GetChild<Transform>().Radius;
        }

        public static float CalculateExp(UnitType type, int level)
        {
            if (type == UnitType.Tower)
            {
                return 100f;
            }
            else if (type == UnitType.Minion)
            {
                return 10f;
            }
            else if (type == UnitType.Monster)
            {
                return 30f;
            }
            else if (type == UnitType.SuperMonster)
            {
                return 100f;
            }
            else if (type == UnitType.UltraMonster)
            {
                return 10000f;
            }
            else if (type >= UnitType.HatsuneMiku)
            {
                return 100f;
            }
            else
            {
                return 0;
            }
        }

        public static float CalculateGold(UnitType type, int level)
        {
            if (type == UnitType.Tower)
            {
                return 100f;
            }
            else if (type == UnitType.Minion)
            {
                return 10f;
            }
            else if (type == UnitType.Monster)
            {
                return 30f;
            }
            else if (type == UnitType.SuperMonster)
            {
                return 100f;
            }
            else if (type == UnitType.UltraMonster)
            {
                return 10000f;
            }
            else if (type >= UnitType.HatsuneMiku)
            {
                return 100f;
            }
            else
            {
                return 0;
            }
        }
    }
}
