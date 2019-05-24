using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICopyAssetTest : MonoBehaviour
{

    private int index = 1;
    private int totalCount = 12;
    private int timerID = 0;

    // Use this for initialization
    void Start()
    {
        timerID = TimeHelper.SetRepeatTimer(() =>
        {
            if (index <= totalCount)
            {
                UICopyingAssetHelper.Instance().UpdateUI(index, totalCount, "正在拷贝资源...");
                index++;
            }
            else
            {
                UICopyingAssetHelper.Instance().Close();
                TimeHelper.KillTimer(timerID);
            }
        }, 1.0f);
    }

}
