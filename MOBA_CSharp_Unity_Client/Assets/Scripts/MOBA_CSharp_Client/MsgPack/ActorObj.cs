using MessagePack;

[MessagePackObject]
public class ActorObj : IGetUnitInfo
{
    [Key(0)]
    public int UnitID { get; set; }
    [Key(1)]
    public UnitType Type { get; set; }
    [Key(2)]
    public Team Team { get; set; }
    [Key(3)]
    public Vector3Obj Position { get; set; }
    [Key(4)]
    public float Rotation { get; set; }
    [Key(5)]
    public bool Warped { get; set; }

    public int GetUnitID()
    {
        return UnitID;
    }

    public UnitType GetUnitType()
    {
        return Type;
    }

    public Team GetTeam()
    {
        return Team;
    }

    public float GetXPos()
    {
        return Position.X;
    }

    public float GetYPos()
    {
        return Position.Y;
    }

    public float GetZPos()
    {
        return Position.Z;
    }

    public float GetRotation()
    {
        return Rotation;
    }

    public bool GetWarped()
    {
        return Warped;
    }

    public float GetMaxHP()
    {
        return 0;
    }

    public float GetCurHP()
    {
        return 0;
    }
}