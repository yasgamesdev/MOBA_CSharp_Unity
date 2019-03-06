using System;
using UnityEngine;

[Serializable]
public class AnimationTypeClip
{
    public AnimationType Type;
    public AnimationClip Clip;
    public float Speed;
    public bool RootMotion;
}