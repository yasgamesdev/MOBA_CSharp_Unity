using MessagePack;

namespace Network.MsgPack
{
    [MessagePackObject]
    public class SnapshotData
    {
        [Key(0)]
        public PlayerData[] PlayerDatas { get; set; }
    }
}
