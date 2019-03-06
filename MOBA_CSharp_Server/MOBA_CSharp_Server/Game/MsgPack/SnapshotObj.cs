using MessagePack;

[MessagePackObject]
public class SnapshotObj
{
    [Key(0)]
    public PlayerObj PlayerObj { get; set; }
    [Key(1)]
    public ClientObj[] ClientObjs { get; set; }
    [Key(2)]
    public ChampionObj[] ChampionObjs { get; set; }
    [Key(3)]
    public BuildingObj[] BuildingObjs { get; set; }
    [Key(4)]
    public ActorObj[] Vector3NoAnimObjs { get; set; }
    [Key(5)]
    public UnitObj[] UnitObjs { get; set; }
}