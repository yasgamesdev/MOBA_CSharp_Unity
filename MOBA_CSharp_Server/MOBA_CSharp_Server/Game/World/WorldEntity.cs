using Microsoft.Xna.Framework;
using MOBA_CSharp_Server.Library.ECS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOBA_CSharp_Server.Game
{
    public class WorldEntity : Entity
    {
        int unitID = 0;

        Dictionary<int, Unit> units = new Dictionary<int, Unit>();

        public WorldEntity(Entity root) : base(root)
        {
            AddInheritedType(typeof(WorldEntity));
        }

        public override void AddChild(Entity entity)
        {
            if (isDuringStep)
            {
                addEntities.Add(entity);
            }
            else
            {
                foreach (Type type in entity.InheritedTypes)
                {
                    if (!children.ContainsKey(type))
                    {
                        children.Add(type, new List<Entity>());
                    }
                    children[type].Add(entity);
                }

                if (entity is Unit)
                {
                    Unit unit = (Unit)entity;
                    units.Add(unit.UnitID, unit);
                }
            }
        }

        public override void RemoveChild(Entity entity)
        {
            if (isDuringStep)
            {
                removeEntities.Add(entity);
            }
            else
            {
                foreach (Type type in entity.InheritedTypes)
                {
                    children[type].Remove(entity);
                    if (children[type].Count == 0)
                    {
                        children.Remove(type);
                    }
                }

                if (entity is Unit)
                {
                    Unit unit = (Unit)entity;
                    units.Remove(unit.UnitID);
                }
            }
        }

        public Unit GetUnit(int unitID)
        {
            
            if(units.ContainsKey(unitID))
            {
                return units[unitID];
            }
            else
            {
                return addEntities.FirstOrDefault(x => x is Unit && ((Unit)x).UnitID == unitID) as Unit;
            }
        }

        public int GenerateUnitID()
        {
            return unitID++;
        }

        public override void Step(float deltaTime)
        {
            SetStatus(deltaTime);

            ConfirmDamage();

            base.Step(deltaTime);

            Destroy();
        }

        void SetStatus(float deltaTime)
        {
            foreach (Unit unit in GetChildren<Unit>())
            {
                unit.SetStatus(deltaTime);
            }
        }

        void ConfirmDamage()
        {
            foreach (Unit unit in GetChildren<Unit>())
            {
                unit.ConfirmDamage();
            }
        }

        public ChampionObj[] GetChampionObjs(bool blueTeam)
        {
            List<ChampionObj> ret = new List<ChampionObj>();

            foreach(var unit in GetChildren<Champion>())
            {
                if ((blueTeam && unit.Status.GetValue(Team.Blue)) || (!blueTeam && unit.Status.GetValue(Team.Red)))
                {
                    ret.Add(unit.GetChampionObj());
                }
            }

            return ret.ToArray();
        }

        public BuildingObj[] GetBuildingObj()
        {
            List<BuildingObj> ret = new List<BuildingObj>();

            foreach (var unit in GetChildren<Building>())
            {
                if(unit.Type != UnitType.Fountain)
                {
                    ret.Add(unit.GetBuildingObj());
                }
            }

            return ret.ToArray();
        }

        public ActorObj[] GetActorObjs(bool blueTeam)
        {
            List<ActorObj> ret = new List<ActorObj>();

            foreach (var unit in GetChildren<Actor>())
            {
                if((blueTeam && unit.Status.GetValue(Team.Blue)) || (!blueTeam && unit.Status.GetValue(Team.Red)))
                {
                    ret.Add(unit.GetActorObj());
                }
            }

            return ret.ToArray();
        }

        public UnitObj[] GetUnitObjs(bool blueTeam)
        {
            List<UnitObj> ret = new List<UnitObj>();

            foreach (var unit in GetChildren<Unit>())
            {
                if (UnitType.Minion <= unit.Type && unit.Type <= UnitType.UltraMonster && ((blueTeam && unit.Status.GetValue(Team.Blue)) || (!blueTeam && unit.Status.GetValue(Team.Red))))
                {
                    ret.Add(unit.GetUnitObj());
                }
            }

            return ret.ToArray();
        }

        public Vector2 GetFountainPosition(Team team)
        {
            return GetChildren<Fountain>().ToList().First(x => x.Team == team).GetChild<Transform>().Position;
        }

        public void RemoveAllEntity()
        {
            List<Entity> allEntities = GetChildren<Entity>().ToList();
            allEntities.ForEach(x => x.ClearReference());
            allEntities.ForEach(x => RemoveChild(x));
        }
    }
}
