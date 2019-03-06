using Microsoft.Xna.Framework;
using MOBA_CSharp_Server.Library.ECS;
using MOBA_CSharp_Server.Library.Physics;
using System;

namespace MOBA_CSharp_Server.Game
{
    public static class UnitFactory
    {
        //public static Unit CreateUnit(UnitType type, int unitID, int ownerUnitID, Team team, Vector2 position, float height, float rotation, float radius, Entity root)
        //{
        //    switch(type)
        //    {
        //        case UnitType.Fountain:
        //            return new Fountain(position, rotation, radius, unitID, team, root);
        //        case UnitType.Core:
        //            return new Core(position, rotation, radius, unitID, team, root);
        //        case UnitType.Tower:
        //            return new Tower(position, rotation, radius, unitID, team, root);
        //        case UnitType.FireBall:
        //            return new Bullet(position, height, rotation, radius, type, unitID, ownerUnitID, team, root);
        //        case UnitType.FireBreath:
        //            return new Cone(position, height, rotation, radius, type, unitID, ownerUnitID, team, root);
        //        case UnitType.Meteor:
        //            return new AreaOfEffect(position, height, rotation, radius, type, unitID, ownerUnitID, team, root);
        //        case UnitType.Minion:
        //            return new Minion(position, rotation, radius, unitID, team, root);
        //        case UnitType.Monster:
        //            return new Monster(position, rotation, radius, type, unitID, team, root);
        //        default:
        //            return null;
        //    }
        //}
    }
}
