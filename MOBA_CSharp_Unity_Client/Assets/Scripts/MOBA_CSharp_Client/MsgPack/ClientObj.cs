using MessagePack;

[MessagePackObject]
public class ClientObj
{
    [Key(0)]
    public UnitType Type { get; set; }
    [Key(1)]
    public string Name { get; set; }
    [Key(2)]
    public Team Team { get; set; }
    [Key(3)]
    public byte Level { get; set; }
}