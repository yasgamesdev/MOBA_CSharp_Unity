using ENet;

namespace MOBA_CSharp_Server.Game
{
    public class GameClient
    {
        public Peer Peer { get; private set; }
        public string Name;
        public Team Team;
        public UnitType Type;
        public bool Ready;
        public int UnitID;

        public GameClient(Peer peer, string name, Team team, UnitType type, bool ready, int unitID)
        {
            Peer = peer;
            Name = name;
            Team = team;
            Type = type;
            Ready = ready;
            UnitID = unitID;
        }
    }
}
