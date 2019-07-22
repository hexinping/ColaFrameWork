using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ColaFramework框架点击触摸事件工具类
/// 一些在UI界面AttachListener的时候不会自动挂载事件监听器的GameObject
/// 可以在这里手动地添加监听器组件
/// </summary>
public class TouchHelper
{
    /// <summary>
    /// 单独为某一个物体添加点击事件
    /// </summary>
    /// <param name="go"></param>
    /// <param name="onClick"></param>
    public static void AddClickListener(GameObject go, UIEventHandler onClick)
    {

    }

    /// <summary>
    /// 单独为某一个物体添加拖拽事件
    /// </summary>
    /// <param name="go"></param>
    /// <param name="onDrag"></param>
    public static void AddDragListener(GameObject go, UIDragEventHandlerDetail onDrag)
    {

    }
}
