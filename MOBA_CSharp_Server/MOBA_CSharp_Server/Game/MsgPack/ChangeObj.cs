using MessagePack;

[MessagePackObject]
public class ChangeObj 
{
    [Key(0)]
    public Team Team { get; set; }
    [Key(1)]
    public string Name { get; set; }
    [Key(2)]
    public UnitType Type { get; set; }
}