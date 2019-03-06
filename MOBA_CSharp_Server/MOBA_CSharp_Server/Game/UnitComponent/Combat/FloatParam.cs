namespace MOBA_CSharp_Server.Game
{
    public class FloatParam
    {
        public FloatStatus Type { get; private set; }
        public float Value { get; private set; }
        public bool IsAdd { get; private set; }

        public FloatParam(FloatStatus type, float value, bool isAdd)
        {
            Type = type;
            Value = value;
            IsAdd = isAdd;
        }
    }
}
