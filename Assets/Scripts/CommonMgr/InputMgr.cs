using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ColaFramework框架输入管理器
/// </summary>
public class InputMgr : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.Escape))
        {
            ConfirmQuit();
        }
    }

    /// <summary>
    /// 退出游戏前确认对话框
    /// </summary>
    public void ConfirmQuit()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                activity.Call("ShowConfirmDialog");
            }
        }
#endif
    }


}

