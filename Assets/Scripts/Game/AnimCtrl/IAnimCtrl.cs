using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IAnimBehavior
{
    void PlayAnimation(string animName);

    void PlayAnimation(string animName, Action<bool> callback);

    void StopPlay(string animName);
}

/// <summary>
/// 动画控制器接口
/// </summary>
public interface IAnimCtrl : IAnimBehavior
{
    void Release();
}

public enum AnimCtrlEnum : byte
{
    CharAnimator = 1,
    CharAnimation = 2,
}
