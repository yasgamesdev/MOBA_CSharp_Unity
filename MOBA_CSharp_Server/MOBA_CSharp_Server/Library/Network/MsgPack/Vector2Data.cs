using MessagePack;

namespace Network.MsgPack
{
    [MessagePackObject]
    public class Vector2Data
    {
        [Key(0)]
        public float X { get; set; }
        [Key(1)]
        public float Y { get; set; }
    }
}
