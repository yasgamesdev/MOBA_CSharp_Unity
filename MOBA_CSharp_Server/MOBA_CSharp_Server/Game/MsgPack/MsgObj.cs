using MessagePack;

[MessagePackObject]
public class MsgObj
{
    [Key(0)]
    public Team Team { get; set; }
    [Key(1)]
    public string Msg { get; set; }
}