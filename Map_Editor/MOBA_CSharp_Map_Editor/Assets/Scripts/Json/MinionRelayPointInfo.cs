using System;

[Serializable]
public class MinionRelayPointInfo
{
    public float x, y;
    public bool blueTeam;
    public int laneNum; //Top:0, Mid:1, Bot:2
    public int index;
}