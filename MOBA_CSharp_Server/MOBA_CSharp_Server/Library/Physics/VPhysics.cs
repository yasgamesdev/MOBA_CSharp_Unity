using Microsoft.Xna.Framework;
using System.Collections.Generic;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;
using VelcroPhysics.Shared;

namespace MOBA_CSharp_Server.Library.Physics
{
    public enum CollisionType
    {
        Static,
        Dynamic,
        None
    }

    public class VPhysics
    {
        World collisionWorld = new World(new Vector2());
        World visionWorld = new World(new Vector2());
        Dictionary<int, BodyWrapper> unitBodies = new Dictionary<int, BodyWrapper>();
        Dictionary<int, BodyWrapper> noCollisionUnitBodies = new Dictionary<int, BodyWrapper>();
        Dictionary<Body, Body> bushBodies = new Dictionary<Body, Body>();

        public void CreateEdgeWall(Vector2 start, Vector2 end)
        {
            Body body0 = BodyFactory.CreateEdge(collisionWorld, start, end);
            body0.BodyType = BodyType.Static;

            Body body1 = BodyFactory.CreateEdge(visionWorld, start, end);
            body1.BodyType = BodyType.Static;
        }

        public void CreateCircleWall(float radius, Vector2 position)
        {
            Body body0 = BodyFactory.CreateCircle(collisionWorld, radius, 1f, position, BodyType.Static);
            Body body1 = BodyFactory.CreateCircle(visionWorld, radius, 1f, position, BodyType.Static);
        }

        public void CreateUnit(int unitID, float radius, Vector2 position, CollisionType type)
        {
            if(type != CollisionType.None)
            {
                Body body = BodyFactory.CreateCircle(collisionWorld, radius, 1f, position, type == CollisionType.Dynamic ? BodyType.Dynamic : BodyType.Static, unitID);

                unitBodies.Add(unitID, new BodyWrapper(body));
            }
            else
            {
                NoCollisionBody noCollisionBody = new NoCollisionBody(position, radius);
                BodyWrapper bodyWrapper = new BodyWrapper(noCollisionBody);

                unitBodies.Add(unitID, bodyWrapper);
                noCollisionUnitBodies.Add(unitID, bodyWrapper);
            }
        }

        public void RemoveUnit(int unitID)
        {
            if(unitBodies.ContainsKey(unitID))
            {
                unitBodies[unitID].RemoveBody(collisionWorld);
                unitBodies.Remove(unitID);
            }

            if(noCollisionUnitBodies.ContainsKey(unitID))
            {
                noCollisionUnitBodies.Remove(unitID);
            }
        }

        public void SetUnitVelocity(int unitID, Vector2 velocity)
        {
            if (unitBodies.ContainsKey(unitID))
            {
                unitBodies[unitID].SetUnitVelocity(velocity);
            }
        }

        public void SetUnitPosition(int unitID, Vector2 position)
        {
            if (unitBodies.ContainsKey(unitID))
            {
                unitBodies[unitID].SetUnitPosition(position);
            }
        }

        public void Step(float deltaTime)
        {
            collisionWorld.Step(deltaTime);

            foreach (var body in unitBodies.Values)
            {
                body.ResetVelocity();
            }
        }

        public Vector2 GetPosition(int unitID)
        {
            return unitBodies[unitID].GetPosition();
        }

        public List<int> GetUnit(float radius, Vector2 position)
        {
            List<int> ret = new List<int>();

            AABB aabb = new AABB(position, radius * 2f, radius * 2f);
            var fixtures = collisionWorld.QueryAABB(ref aabb);

            foreach(var fixture in fixtures)
            {
                Body body = fixture.Body;
                if(body.UserData != null)
                {
                    Vector2 unitPosition = body.Position;
                    float unitRadius = fixture.Shape.Radius;

                    if((position - unitPosition).Length() <= (radius - unitRadius))
                    {
                        ret.Add((int)body.UserData);
                    }
                }
            }

            foreach (var keyValue in noCollisionUnitBodies)
            {
                Vector2 unitPosition = keyValue.Value.NoCollisionBody.Position;
                float unitRadius = keyValue.Value.NoCollisionBody.Radius;

                if ((position - unitPosition).Length() <= (radius - unitRadius))
                {
                    ret.Add(keyValue.Key);
                }
            }

            return ret;
        }

        public bool CheckLineOfSight(int unitID_0, int unitID_1)
        {
            Vector2 point0 = unitBodies[unitID_0].GetPosition();
            Vector2 point1 = unitBodies[unitID_1].GetPosition();

            if(point0 == point1)
            {
                return true;
            }
            else
            {
                return visionWorld.RayCast(point0, point1).Count == 0;
            }
        }

        public void CreateBush(IEnumerable<Vector2> vertices)
        {
            Body body = BodyFactory.CreatePolygon(visionWorld, new Vertices(vertices), 1f);
            bushBodies.Add(body, body);
        }

        public Body GetBushBody(Vector2 position)
        {
            AABB aabb = new AABB(position, 0.01f, 0.01f);
            var fixtures = visionWorld.QueryAABB(ref aabb);

            foreach (var fixture in fixtures)
            {
                Body body = fixture.Body;
                if(bushBodies.ContainsKey(body))
                {
                    return body;
                }
            }

            return null;
        }
    }
}
