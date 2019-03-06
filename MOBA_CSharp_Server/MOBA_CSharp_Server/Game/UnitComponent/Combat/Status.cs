using System;
using System.Collections.Generic;

namespace MOBA_CSharp_Server.Game
{
    public class Status
    {
        public float[] FloatStatus;
        public bool[] BoolStatus;
        public bool[] VisionStatus;
        //Animation
        public AnimationType AnimationStatus { get; private set; }
        public float SpeedRate { get; private set; }
        public float PlayTime { get; private set; }
        //Damage
        public bool Damaged, Dead;
        public List<int> AttackedUnitIDs;

        public Status()
        {
            FloatStatus = new float[Enum.GetValues(typeof(FloatStatus)).Length];
            BoolStatus = new bool[Enum.GetValues(typeof(BoolStatus)).Length];
            VisionStatus = new bool[Enum.GetValues(typeof(Team)).Length];
            AnimationStatus = AnimationType.Idle;
            SpeedRate = 1f;
            PlayTime = 0;
            Damaged = false;
            Dead = false;
            AttackedUnitIDs = new List<int>();
        }

        public void SetAnimationStatus(AnimationType type, float speedRate, float deltaTime)
        {
            SpeedRate = speedRate;

            if (AnimationStatus != type)
            {
                AnimationStatus = type;
                PlayTime = 0;
            }
            else
            {
                PlayTime += deltaTime;
            }
        }

        public float GetValue(FloatStatus type)
        {
            return FloatStatus[(int)type];
        }

        public bool GetValue(BoolStatus type)
        {
            return BoolStatus[(int)type];
        }

        public bool GetValue(Team team)
        {
            return VisionStatus[(int)team];
        }
    }
}
