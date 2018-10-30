using Collision2D;
using ECS;
using Math2D;
using System;
using System.Numerics;

namespace MOBA_CSharp_Server.Game
{
    public class BodyComponent : Component
    {
        public Body Body { get; private set; }
        public float Angle { get; private set; }
        public bool Warped { get; private set; }
        AnimationComponent animation;
        //Move
        Vector2 dest;
        Vector2[] path;
        int currentPath;

        public BodyComponent(Body body, float angle, RootEntity root) : base(root)
        {
            Body = body;
            Angle = angle;
        }

        public void Destroy()
        {
            Body.Destroy();
        }

        public Vector2 Position
        {
            get
            {
                return ((DynamicBody)Body).GetPosition();
            }
        }

        public void SetAngle(float angle)
        {
            Angle = angle;
        }

        DynamicBody GetDynamicBody()
        {
            return (DynamicBody)Body;
        }

        public override void Step(float deltaTime)
        {
            PathfindingMove(deltaTime);
        }

        public bool GetWarpedAndReset()
        {
            bool result = Warped;
            Warped = false;
            return result;
        }

        public void SetAnimationComponent(AnimationComponent animation)
        {
            this.animation = animation;
        }

        public void Move(Vector2 dest)
        {
            this.dest = dest;

            path = root.GetChild<PathfindingEntity>().GetPath(Position, dest);
            currentPath = 0;
            if (path.Length > 0)
            {
                if (animation != null)
                {
                    animation.SetLoopAnimation(AnimationType.Walk);
                }
            }
            else
            {
                path = null;
                if (animation != null)
                {
                    animation.SetLoopAnimation(AnimationType.Idle);
                }
            }
        }

        void PathfindingMove(float deltaTime)
        {
            if(path != null)
            {
                float speed = 2.0f;
                float remainMoveDistance = speed * deltaTime;

                while(path != null && remainMoveDistance > 0)
                {
                    if(currentPath >= path.Length - 1)
                    {
                        Vector2 direction = dest - Position;
                        if(direction.Length() <= remainMoveDistance)
                        {
                            GetDynamicBody().MoveTo(dest);
                            if (animation != null)
                            {
                                animation.SetLoopAnimation(AnimationType.Idle);
                            }
                            path = null;
                        }
                        else
                        {
                            Vector2 next = Position + direction / direction.Length() * remainMoveDistance;
                            GetDynamicBody().MoveTo(next);
                            remainMoveDistance = 0;
                        }

                        SetAngle((float)(Math.Atan2(direction.X, direction.Y) / Math.PI * 180.0));
                    }
                    else
                    {
                        Vector2 direction = new Vector2(path[currentPath].X, path[currentPath].Y) - Position;
                        if (direction.Length() <= remainMoveDistance)
                        {
                            GetDynamicBody().MoveTo(new Vector2(path[currentPath].X, path[currentPath].Y));
                            currentPath++;
                            remainMoveDistance -= direction.Length();
                        }
                        else
                        {
                            Vector2 next = Position + direction / direction.Length() * remainMoveDistance;
                            GetDynamicBody().MoveTo(next);
                            remainMoveDistance = 0;
                        }

                        SetAngle((float)(Math.Atan2(direction.X, direction.Y) / Math.PI * 180.0));
                    }
                }
            }
            else
            {
                GetDynamicBody().Idle();
            }
        }
    }
}
