using MessagePack;

namespace Network.MsgPack
{
    [MessagePackObject]
    public class PlayerData
    {
        [Key(0)]
        public int EntityID { get; set; }
        [Key(1)]
        public uint PeerID { get; set; }
        [Key(2)]
        public byte Team { get; set; }
        [Key(3)]
        public float PosX { get; set; }
        [Key(4)]
        public float PosZ { get; set; }
        [Key(5)]
        public float Angle { get; set; }
        [Key(6)]
        public bool Warped { get; set; }
        [Key(7)]
        public ushort Anime { get; set; }
        [Key(8)]
        public bool Loop { get; set; }
    }
}
