using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitAnimation : MonoBehaviour {
    [SerializeField] List<AnimationTypeClip> clips;
    string loopClipName = "Loop";
    string onceClipName = "Once";

    private AnimatorOverrideController overrideController;
    [SerializeField] private Animator animator;

    AnimationType type;
    bool loop;

    //void Start()
    //{
    //    overrideController = new AnimatorOverrideController();
    //    overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;
    //    animator.runtimeAnimatorController = overrideController;

    //    ChangeLoopClip(AnimationType.Idle);
    //}

    public void Init()
    {
        overrideController = new AnimatorOverrideController();
        overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;
        animator.runtimeAnimatorController = overrideController;

        ChangeLoopClip(AnimationType.Idle);
    }

    AnimationClip GetAnimationClip(AnimationType type)
    {
        return clips.FirstOrDefault(x => x.Type == type).Clip;
    }

    void ChangeLoopClip(AnimationType type)
    {
        AnimatorStateInfo[] layerInfo = new AnimatorStateInfo[animator.layerCount];
        for (int i = 0; i < animator.layerCount; i++)
        {
            layerInfo[i] = animator.GetCurrentAnimatorStateInfo(i);
        }
        overrideController[loopClipName] = GetAnimationClip(type);
        animator.Update(0.0f);
        
        for (int i = 0; i < animator.layerCount; i++)
        {
            animator.Play(layerInfo[i].nameHash, i, layerInfo[i].normalizedTime);
        }

        animator.SetTrigger("LoopTrigger");

        this.type = type;
        loop = true;
    }

    void ChangeOnceClip(AnimationType type)
    {
        AnimatorStateInfo[] layerInfo = new AnimatorStateInfo[animator.layerCount];
        for (int i = 0; i < animator.layerCount; i++)
        {
            layerInfo[i] = animator.GetCurrentAnimatorStateInfo(i);
        }

        overrideController[loopClipName] = GetAnimationClip(AnimationType.Idle);
        overrideController[onceClipName] = GetAnimationClip(type);
        animator.Update(0.0f);

        for (int i = 0; i < animator.layerCount; i++)
        {
            animator.Play(layerInfo[i].nameHash, i, layerInfo[i].normalizedTime);
        }

        animator.SetTrigger("OnceTrigger");

        this.type = type;
        loop = false;
    }

    public void SetAnime(AnimationType type, bool loop)
    {
        if(this.type != type || this.loop != loop)
        {
            if(loop)
            {
                ChangeLoopClip(type);
            }
            else
            {
                ChangeOnceClip(type);
            }
        }
    }
}
