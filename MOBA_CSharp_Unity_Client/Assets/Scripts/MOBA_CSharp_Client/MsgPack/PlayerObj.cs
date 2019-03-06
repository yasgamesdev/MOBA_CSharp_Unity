using MessagePack;

[MessagePackObject]
public class PlayerObj
{
    [Key(0)]
    public int UnitID { get; set; }
    [Key(1)]
    public float Exp { get; set; }
    [Key(2)]
    public float NextExp { get; set; }
    [Key(3)]
    public float Gold { get; set; }
    [Key(4)]
    public CombatObj[] Skills { get; set; }
    [Key(5)]
    public CombatObj[] Effects { get; set; }
    [Key(6)]
    public CombatObj[] Items { get; set; }
}