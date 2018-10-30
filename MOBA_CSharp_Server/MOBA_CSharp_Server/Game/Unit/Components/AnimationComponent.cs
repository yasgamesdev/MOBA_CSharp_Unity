using ECS;

namespace MOBA_CSharp_Server.Game
{
    public class AnimationComponent : Component
    {
        public AnimationType Anime { get; private set; }
        public float PlayTime { get; private set; }
        public bool Loop { get; private set; }

        float time;

        public AnimationComponent(RootEntity root) : base(root)
        {
            SetLoopAnimation(AnimationType.Idle);
        }

        public override void Step(float deltaTime)
        {
            if(Loop)
            {
                time += deltaTime;
            }
            else
            {
                time += deltaTime;
                if(time >= PlayTime)
                {
                    SetLoopAnimation(GetLoopAnimation());
                }
            }
        }

        public void SetAnimation(AnimationType anime, float playTime)
        {
            Anime = anime;
            PlayTime = playTime;
            Loop = false;
            time = 0;
        }

        public void SetLoopAnimation(AnimationType anime)
        {
            Anime = anime;
            PlayTime = 0;
            Loop = true;
            time = 0;
        }

        protected virtual AnimationType GetLoopAnimation()
        {
            return AnimationType.Idle;
        }
    }
}
