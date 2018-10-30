using ECS;
using ENet;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace MOBA_CSharp_Server.Game
{
    public class ClientManagerEntity : Entity
    {
        Dictionary<uint, Peer> peers = new Dictionary<uint, Peer>();

        UnitManagerEntity unitManager;
        CollisionEntity collision;
        VisionManagerEntity visionManager;

        public ClientManagerEntity(RootEntity root): base(root)
        {

        }

        public void SetEntities(UnitManagerEntity unitManager, CollisionEntity collision, VisionManagerEntity visionManager)
        {
            this.unitManager = unitManager;
            this.collision = collision;
            this.visionManager = visionManager;
        }

        int count = 1;
        public void AddClient(Peer peer)
        {
            peers.Add(peer.ID, peer);

            Random rand = new Random();
            var randPos = new Vector2((float)rand.NextDouble() * 5.0f + 1.5f, (float)rand.NextDouble() * 5.0f + 1.5f);

            ChampionEntity champion = new ChampionEntity(peer.ID, count++ % 2 == 0 ? Team.Blue : Team.Red, collision.GenerateDynamicBody(randPos, 0.3f), 45f, root);
            visionManager.AddUnit(champion);

            unitManager.AddUnit(UnitType.Champion, champion);
        }

        public void RemoveClient(uint peerID)
        {
            unitManager.RemoveChampion(peerID);

            peers.Remove(peerID);
        }

        public void Move(Vector2 position, uint peerID)
        {
            unitManager.Move(position, peerID);
        }
    }
}
