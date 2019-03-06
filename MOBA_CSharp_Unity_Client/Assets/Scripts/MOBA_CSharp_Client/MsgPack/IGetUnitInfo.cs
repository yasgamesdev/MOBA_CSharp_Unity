public interface IGetUnitInfo
{
    int GetUnitID();
    UnitType GetUnitType();
    Team GetTeam();
    float GetXPos();
    float GetYPos();
    float GetZPos();
    float GetRotation();
    bool GetWarped();
    float GetMaxHP();
    float GetCurHP();
}