using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Animation : MonoBehaviour
{
    [SerializeField] List<AnimationTypeClip> clips;

    private AnimatorOverrideController overrideController;
    private Animator animator;

    AnimationType type;

    public void Init(AnimationType type, float speedRate, float playTime)
    {
        var animators = GetComponentsInChildren<Animator>();
        if(animators.Length > 0)
        {
            animator = animators[0];

            overrideController = new AnimatorOverrideController();
            overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;
            animator.runtimeAnimatorController = overrideController;
        }

        ChangeClip(type, playTime);
        animator.speed = GetAnimationClip(type).Speed * speedRate;
    }

    AnimationTypeClip GetAnimationClip(AnimationType type)
    {
        return clips.FirstOrDefault(x => x.Type == type);
    }

    void ChangeClip(AnimationType type, float playTime)
    {
        AnimatorStateInfo[] layerInfo = new AnimatorStateInfo[animator.layerCount];
        for (int i = 0; i < animator.layerCount; i++)
        {
            layerInfo[i] = animator.GetCurrentAnimatorStateInfo(i);
        }
        overrideController["Empty"] = GetAnimationClip(type).Clip;
        animator.Update(0.0f);
        //animator.speed = GetAnimationClip(type).Speed;
        animator.applyRootMotion = GetAnimationClip(type).RootMotion;
        animator.transform.localPosition = Vector3.zero;
        animator.transform.localRotation = Quaternion.identity;

        for (int i = 0; i < animator.layerCount; i++)
        {
            animator.Play(layerInfo[i].nameHash, i, layerInfo[i].normalizedTime);
        }
        animator.Play("Empty", -1, playTime);

        this.type = type;
    }

    public void SetAnime(AnimationType type, float speedRate, float playTime)
    {
        if (this.type != type)
        {
            ChangeClip(type, playTime);
        }
        animator.speed = GetAnimationClip(type).Speed * speedRate;
    }
}
