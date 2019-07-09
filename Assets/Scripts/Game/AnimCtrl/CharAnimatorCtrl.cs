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
        throw new NotImplementedException();
    }

    public void PlayAnimation(string animName, Action<bool> callback)
    {
        throw new NotImplementedException();
    }

    public void Release()
    {
        throw new NotImplementedException();
    }

    public void StopPlay(string animName)
    {
        throw new NotImplementedException();
    }
}
