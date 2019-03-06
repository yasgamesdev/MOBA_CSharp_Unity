using MessagePack;

[MessagePackObject]
public class LobbyObj
{
    [Key(0)]
    public byte State { get; set; }
    [Key(1)]
    public float Timer { get; set; }
    [Key(2)]
    public SelectObj[] SelectObjs { get; set; }
    [Key(3)]
    public SelectObj PeerSelectObj { get; set; }
}