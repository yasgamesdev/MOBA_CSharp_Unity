namespace MOBA_CSharp_Server.Game
{
    public class VisionParam
    {
        public Team Team { get; private set; }
        public bool Value { get; private set; }
        public int Priority { get; private set; }

        public VisionParam(Team team, bool value, int priority)
        {
            Team = team;
            Value = value;
            Priority = priority;
        }
    }
}
