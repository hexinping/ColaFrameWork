using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色的动画播放控制器(Animator版)
/// </summary>
public class CharAnimatorCtrl : IAnimCtrl
{
    private Animator animator;

    public CharAnimatorCtrl(GameObject entity)
    {
        animator = entity.AddSingleComponent<Animator>();
        animator.runtimeAnimatorController = AssetLoader.Load<RuntimeAnimatorController>(GloablDefine.ModelAnimatorPath + entity.name + ".controller");
        animator.updateMode = AnimatorUpdateMode.Normal;
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
    }

    public void PlayAnimation(string animName)
    {
    }

    public void PlayAnimation(string animName, Action<bool> callback)
    {
    }

    public void PlayAnimation(int animState)
    {
        animator.SetInteger(AnimCurveNames.IAnimName, animState);
    }

    public void PlayAnimation(int animState, Action<bool> callback)
    {
        //用一种合适的方式触发回调
        animator.SetInteger(AnimCurveNames.IAnimName, animState);
    }

    public void Release()
    {
        StopPlay();
        animator = null;
    }

    public void StopPlay()
    {
        animator.SetBool("Idle", true);
    }
}
