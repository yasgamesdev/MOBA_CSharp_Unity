using System;

[Serializable]
public class MapInfo
{
    public SpawnInfo blueSpawn, redSpawn;
    public CoreInfo blueCore, redCore;
    public TowerInfo[] towers;
    public MonsterInfo[] monsters;
    public MinionRelayPointInfo[] minionRelayPoints;
    public EdgeInfo[] edges;
    public CircleInfo[] circles;
    public BushInfo[] bushes;
}