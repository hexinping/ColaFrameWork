using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 场景角色的接口
/// </summary>
public interface ISceneCharacter
{
    /// <summary>
    /// 角色实际的承载的GameObject
    /// </summary>
    GameObject gameObject { get; set; }

    /// <summary>
    /// 角色的Transform缓存
    /// </summary>
    Transform transform { get; set; }

    /// <summary>
    /// 角色的位置信息
    /// </summary>
    Vector3 Position { get; }

    /// <summary>
    /// 角色的旋转方向
    /// </summary>
    Vector3 Rotation { get; }

    /// <summary>
    /// 设置角色的位置信息
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    void SetPosition2D(float x, float z);

    /// <summary>
    /// 设置角色的旋转信息
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    void SetRotation2D(float x, float z);

    /// <summary>
    /// 角色销毁函数
    /// </summary>
    void Release();
}
