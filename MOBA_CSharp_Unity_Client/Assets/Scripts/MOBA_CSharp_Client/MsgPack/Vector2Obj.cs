using MessagePack;

[MessagePackObject]
public class Vector2Obj
{
    [Key(0)]
    public float X { get; set; }
    [Key(1)]
    public float Y { get; set; }
}