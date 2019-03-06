using MessagePack;

[MessagePackObject]
public class Vector3Obj
{
    [Key(0)]
    public float X { get; set; }
    [Key(1)]
    public float Y { get; set; }
    [Key(2)]
    public float Z { get; set; }
}