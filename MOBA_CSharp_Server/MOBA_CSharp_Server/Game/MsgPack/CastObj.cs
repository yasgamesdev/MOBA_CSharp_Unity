using MessagePack;

[MessagePackObject]
public class CastObj
{
    [Key(0)]
    public byte SkillSlotNum { get; set; }
    [Key(1)]
    public int[] IntArgs { get; set; }
    [Key(2)]
    public float[] FloatArgs { get; set; }
}