using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class Move : Ability
    {
        Vector2 dest;
        Vector2[] path;
        int currentPath;

        public Move(Unit unitRoot, Entity root) : base(CombatType.Move, unitRoot, root)
        {
            AddInheritedType(typeof(Move));
            AddAttribute(CombatAttribute.Move);
        }

        public override bool IsExecutable(object args)
        {
            return unitRoot.HP > 0 && !unitRoot.Status.GetValue(BoolStatus.Unmovable);
        }

        public override void Execute(object args)
        {
            base.Execute(args);

            dest = (Vector2)args;
            try
            {
                path = Root.GetChild<PathfindingEntity>().GetPath(unitRoot.GetChild<Transform>().Position, dest);
            }
            catch
            {
                path = null;
            }
            currentPath = 1;
            SetAnimationParam(AnimationType.Walk, unitRoot.Status.GetValue(FloatStatus.MovementSpeed), (int)AnimationStatusPriority.Walk);
        }

        protected override bool ContinueExecution()
        {
            return unitRoot.HP > 0 && !unitRoot.Status.GetValue(BoolStatus.Unmovable) && path != null;
        }

        public override void Cancel()
        {
            base.Cancel();

            UnSetAnimationParam();
        }

        protected override void ExecuteProcess(float deltaTime)
        {
            base.ExecuteProcess(deltaTime);

            Transform transform = unitRoot.GetChild<Transform>();
            Vector2 tempPosition = transform.Position;

            float speed = unitRoot.Status.GetValue(FloatStatus.MovementSpeed);
            float remainMoveDistance = speed * deltaTime;

            SetAnimationParam(AnimationType.Walk, speed, (int)AnimationStatusPriority.Walk);

            while (path != null && remainMoveDistance > 0)
            {
                if (currentPath >= path.Length - 1)
                {
                    Vector2 direction = dest - tempPosition;
                    if (direction.Length() <= remainMoveDistance)
                    {
                        transform.SetVelocity((dest - transform.Position) / deltaTime);
                        path = null;
                        Cancel();
                    }
                    else
                    {
                        Vector2 next = tempPosition + direction / direction.Length() * remainMoveDistance;
                        transform.SetVelocity((next - transform.Position) / deltaTime);
                        remainMoveDistance = 0;
                    }

                    transform.SetRotation((float)(Math.Atan2(direction.X, direction.Y) / Math.PI * 180.0));
                }
                else
                {
                    Vector2 direction = new Vector2(path[currentPath].X, path[currentPath].Y) - transform.Position;
                    if (direction.Length() <= remainMoveDistance)
                    {
                        tempPosition += direction;
                        currentPath++;
                        remainMoveDistance -= direction.Length();
                    }
                    else
                    {
                        Vector2 next = tempPosition + direction / direction.Length() * remainMoveDistance;
                        transform.SetVelocity((next - transform.Position) / deltaTime);
                        remainMoveDistance = 0;
                    }

                    transform.SetRotation((float)(Math.Atan2(direction.X, direction.Y) / Math.PI * 180.0));
                }
            }
        }
    }
}
