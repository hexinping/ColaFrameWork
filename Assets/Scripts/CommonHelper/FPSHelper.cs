using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 显示游戏帧率/占用内存等信息的助手类
/// </summary>
public class FPSHelper : MonoBehaviour
{
    /// <summary>
    /// 计算的更新频率
    /// </summary>
    public float updateInterval = 0.5F;

    /// <summary>
    /// 用来保存时间间隔
    /// </summary>
    private float lastInterval;

    /// <summary>
    /// 记录帧数
    /// </summary>
    private int frames = 0;

    /// <summary>
    /// 记录帧率
    /// </summary>
    private float fps;

    void Start()
    {
        //Application.targetFrameRate=60;

        lastInterval = Time.realtimeSinceStartup;

        frames = 0;
    }

    void OnGUI()
    {
        GUI.color = new Color(0, 1, 0);
        GUI.skin.label.fontSize = 20;
        GUI.Label(new Rect(Screen.width - 330, 0, 330, 300), "MemoryAllocated:" + GetMemoryMB(UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong())
                                                             + "  MemoryReserved:" + GetMemoryMB(UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong())
                                                             + " MemoryUnusedReserved:" + GetMemoryMB(UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong())
                                                             + "  usedHeapSize:" + GetMemoryMB(UnityEngine.Profiling.Profiler.usedHeapSizeLong)
                                                             + "  MonoHeapSize:" + GetMemoryMB(UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong())
                                                             + "  MonoUsedSize:" + GetMemoryMB(UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong())
        );
        if(GUI.Button(new Rect(Screen.width / 2 - 30, 0, 100, 50),"GC")){
            CommonHelper.ResourcesClearAndGC();
        }
        {

        }
        if (fps > 50)
        {
            GUI.color = new Color(0, 1, 0);
        }
        else if (fps > 25)
        {
            GUI.color = new Color(1, 1, 0);
        }
        else
        {
            GUI.color = new Color(1.0f, 0, 0);
        }

        GUI.Label(new Rect(0, 50, 300, 300), "FPS:" + fps.ToString("f2"));

    }

    private string GetMemoryMB(long curSize)
    {
        float mbSize = curSize / (1024f * 1024f);
        return mbSize.ToString("f2") + "MB";
    }

    void Update()
    {
        ++frames;

        if (Time.realtimeSinceStartup > lastInterval + updateInterval)
        {
            fps = frames / (Time.realtimeSinceStartup - lastInterval);

            frames = 0;

            lastInterval = Time.realtimeSinceStartup;
        }
    }
}
