using MessagePack;

[MessagePackObject]
public class BuildingObj : IGetUnitInfo
{
    [Key(0)]
    public int UnitID { get; set; }
    [Key(1)]
    public UnitType Type { get; set; }
    [Key(2)]
    public Team Team { get; set; }
    [Key(3)]
    public Vector2Obj Position { get; set; }
    [Key(4)]
    public float Rotation { get; set; }
    [Key(5)]
    public float MaxHP { get; set; }
    [Key(6)]
    public float CurHP { get; set; }

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
        return 0;
    }

    public float GetZPos()
    {
        return Position.Y;
    }

    public float GetRotation()
    {
        return Rotation;
    }

    public bool GetWarped()
    {
        return false;
    }

    public float GetMaxHP()
    {
        return MaxHP;
    }

    public float GetCurHP()
    {
        return CurHP;
    }
}