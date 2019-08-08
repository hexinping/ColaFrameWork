using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColaFramework;

public class UICopyAssetTest : MonoBehaviour
{

    private int index = 1;
    private int totalCount = 12;
    private long timerID = 0;

    // Use this for initialization
    void Start()
    {
        timerID = Timer.RunPerSecond((time) =>
        {
            if (index <= totalCount)
            {
                UICopyingAssetHelper.Instance().UpdateUI(index, totalCount, "正在拷贝资源...");
                index++;
            }
            else
            {
                UICopyingAssetHelper.Instance().Close();
                Timer.Cancel(timerID);
            }
        }, null);
    }

}
