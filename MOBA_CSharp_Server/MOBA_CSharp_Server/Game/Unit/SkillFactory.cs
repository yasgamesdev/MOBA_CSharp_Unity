using Microsoft.Xna.Framework;
using MOBA_CSharp_Server.Library.ECS;
using System;

namespace MOBA_CSharp_Server.Game
{
    public static class SkillFactory
    {
        public static Ability CreateSkill(UnitType unitType, CombatAttribute attribute, Unit unitRoot, Entity root)
        {
            string skillName = root.GetChild<DataReaderEntity>().GetYAMLObject(unitType).GetData<string>(attribute.ToString());

            if(skillName == "")
            {
                return null;
            }

            CombatType type;
            Enum.TryParse(skillName, out type);

            switch(type)
            {
                case CombatType.FireBall:
                    return new ImmediateDirection(attribute, type, unitRoot, root);
                case CombatType.FireBreath:
                    return new ImmediateCone(attribute, type, unitRoot, root);
                case CombatType.Meteor:
                    return new ImmediateCircle(attribute, type, unitRoot, root);
                case CombatType.BigBang:
                    return new ImmediateDirection(attribute, type, unitRoot, root);
                case CombatType.EarthShatter:
                    return new ImmediateDirection(attribute, type, unitRoot, root);
                case CombatType.PoisonGas:
                    return new ImmediateCircle(attribute, type, unitRoot, root);
                case CombatType.PressurisedSteam:
                    return new ImmediateCone(attribute, type, unitRoot, root);
                default:
                    return null;
            }
        }
    }
}
