using MessagePack;

[MessagePackObject]
public class CombatObj
{
    [Key(0)]
    public byte SlotNum { get; set; }
    [Key(1)]
    public CombatType Type { get; set; }
    [Key(2)]
    public float Timer { get; set; }
    [Key(3)]
    public byte Stack { get; set; }
    [Key(4)]
    public bool IsActive { get; set; }
}