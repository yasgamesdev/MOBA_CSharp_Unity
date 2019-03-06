using MessagePack;

[MessagePackObject]
public class UnitObj : IGetUnitInfo, IGetAnimInfo
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
    public bool Warped { get; set; }
    [Key(6)]
    public byte AnimationNum { get; set; }
    [Key(7)]
    public float SpeedRate { get; set; }
    [Key(8)]
    public float PlayTime { get; set; }
    [Key(9)]
    public float MaxHP { get; set; }
    [Key(10)]
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
        return Warped;
    }

    public AnimationType GetAnimType()
    {
        return (AnimationType)AnimationNum;
    }

    public float GetSpeedRate()
    {
        return SpeedRate;
    }

    public float GetPlayTime()
    {
        return PlayTime;
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