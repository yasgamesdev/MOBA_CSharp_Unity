namespace MOBA_CSharp_Server.Game
{
    public class AnimParam
    {
        public AnimationType Type { get; private set; }
        public float SpeedRate { get; private set; }
        public int Priority { get; private set; }

        public AnimParam(AnimationType type, float speedRate, int priority)
        {
            Type = type;
            SpeedRate = speedRate;
            Priority = priority;
        }
    }
}
