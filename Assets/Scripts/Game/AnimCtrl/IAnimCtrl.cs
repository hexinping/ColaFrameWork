using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IAnimBehavior
{
    void PlayAnimation(string animName);

    void PlayAnimation(string animName, Action<bool> callback);

    void StopPlay();
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

public enum AnimCurveEnum : byte
{
    Idle = 0,
    Run = 1,
}

public static class AnimCurveNames
{
    public static readonly string IAnimName = "AnimEnum";
    public static readonly string Idle = AnimCurveEnum.Idle.ToString();
    public static readonly string Run = AnimCurveEnum.Run.ToString();
}
