using ECS;
using System;
using System.Collections.Generic;

namespace MOBA_CSharp_Server.Game
{
    public class VisionComponent : Component
    {
        Dictionary<Team, bool> visions = new Dictionary<Team, bool>();

        public VisionComponent(Team team, RootEntity root) : base(root)
        {
            foreach(Team t in Enum.GetValues(typeof(Team)))
            {
                visions.Add(t, t == team);
            }
        }

        public override void Step(float deltaTime)
        {

        }

        public bool GetVision(Team team)
        {
            return visions[team];
        }

        public void SetVision(Team team, bool vision)
        {
            visions[team] = vision;
        }
    }
}
