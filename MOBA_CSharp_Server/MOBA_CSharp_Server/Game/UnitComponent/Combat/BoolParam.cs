namespace MOBA_CSharp_Server.Game
{
    public class BoolParam
    {
        public BoolStatus Type { get; private set; }
        public bool Value { get; private set; }
        public int Priority { get; private set; }

        public BoolParam(BoolStatus type, bool value, int priority)
        {
            Type = type;
            Value = value;
            Priority = priority;
        }
    }
}
