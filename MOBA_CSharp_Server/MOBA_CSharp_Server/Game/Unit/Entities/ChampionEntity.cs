using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collision2D;
using ECS;
using Network.MsgPack;

namespace MOBA_CSharp_Server.Game
{
    public class ChampionEntity : DynamicBodyUnitEntity
    {
        public uint PeerID { get; private set; }

        public ChampionEntity(uint peerID, Team team, DynamicBody body, float angle, RootEntity root) : base(team, body, angle, root)
        {
            PeerID = peerID;
        }

        public PlayerData GetPlayerData()
        {
            BodyComponent body = GetComponent<BodyComponent>();
            AnimationComponent animation = GetComponent<AnimationComponent>();

            return new PlayerData() {
                EntityID = EntityID,
                PeerID = PeerID,
                Team = (byte)Team,
                PosX = body.Position.X,
                PosZ = body.Position.Y,
                Angle = body.Angle,
                Warped = body.Warped,
                Anime = (ushort)animation.Anime,
                Loop = animation.Loop,
            };
        }
    }
}
