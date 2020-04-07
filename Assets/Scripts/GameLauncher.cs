//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UnityEngine;
using ColaFramework;

/// <summary>
/// 游戏入口:游戏启动器类
/// </summary>
public class GameLauncher : MonoBehaviour
{
    /// <summary>
    /// 存放资源的可读写路径
    /// </summary>
    private string assetPath;
    /// <summary>
    /// 指示复制资源的游标
    /// </summary>
    private static int resbaseIndex = 0;

    private static GameLauncher instance;
    private GameManager gameManager;
    private GameObject fpsHelperObj;
    private FPSHelper fpsHelper;
    private LogHelper logHelper;
    private InputMgr inputMgr;

    public static GameLauncher Instance
    {
        get { return instance; }
    }

    /// <summary>
    /// LogHelper实例
    /// </summary>
    public LogHelper LogHelper
    {
        get { return this.logHelper; }
    }

    void Awake()
    {
        instance = this;
        gameManager = GameManager.Instance;
        DOTween.Init();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
#if UNITY_STANDALONE_WIN
            Screen.SetResolution(1280, 720, false);
#endif
        Application.targetFrameRate = AppConst.GameFrameRate;

        DontDestroyOnLoad(gameObject);

        //加入输入输出管理器
        inputMgr = gameObject.AddComponent<InputMgr>();

#if SHOW_FPS
        fpsHelperObj = new GameObject("FpsHelperObj");
        fpsHelper = fpsHelperObj.AddComponent<FPSHelper>();
        GameObject.DontDestroyOnLoad(fpsHelperObj);
#endif

        if (AppConst.isLocalGameServer)
        {
            var ServerObj = new GameObject("SimulateServer");
            ServerObj.AddComponent<ColaFramework.NetWork.SimulateServer>();
            GameObject.DontDestroyOnLoad(ServerObj);
        }

#if BUILD_DEBUG_LOG || UNITY_EDITOR
#if UNITY_2017_1_OR_NEWER
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = true;
#endif
#else
#if UNITY_2017_1_OR_NEWER
        Debug.unityLogger.logEnabled = false;
#else
        Debug.logger.logEnabled = false;
#endif
#endif

#if WETEST_SDK
        gameObject.AddComponent<WeTest.U3DAutomation.U3DAutomationBehaviour>();
#endif

#if OUTPUT_LOG
        GameObject logHelperObj = new GameObject("LogHelperObj");
        logHelper = logHelperObj.AddComponent<LogHelper>();
        GameObject.DontDestroyOnLoad(logHelperObj);

        Application.logMessageReceived += logHelper.LogCallback;
#endif
        //初始化多线程工具
        ColaLoom.Initialize();
        StreamingAssetHelper.SetAssetPathDir(assetPath);
    }

    // Use this for initialization
    void Start()
    {
        StartCoroutine(InitGameCore());
    }

    void Update()
    {
        if (null != ColaHelper.Update)
        {
            ColaHelper.Update(Time.deltaTime);
        }
        gameManager.Update(Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (null != ColaHelper.LateUpdate)
        {
            ColaHelper.LateUpdate(Time.deltaTime);
        }
        gameManager.LateUpdate(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (null != ColaHelper.FixedUpdate)
        {
            ColaHelper.FixedUpdate(Time.fixedDeltaTime);
        }
        gameManager.FixedUpdate(Time.fixedDeltaTime);
    }


    private void OnApplicationQuit()
    {
        if (null != ColaHelper.OnApplicationQuit)
        {
            ColaHelper.OnApplicationQuit();
        }
        gameManager.OnApplicationQuit();
    }

    private void OnApplicationPause(bool pause)
    {
        if (null != ColaHelper.OnApplicationPause)
        {
            ColaHelper.OnApplicationPause(pause);
        }
        gameManager.OnApplicationPause(pause);
    }

    private void OnApplicationFocus(bool focus)
    {
        gameManager.OnApplicationFocus(focus);
    }

    public void ApplicationQuit(string exitCode = "0")
    {
        gameManager.ApplicationQuit();
    }

    IEnumerator InitGameCore()
    {
        yield return null;

#if UNITY_ANDROID && (!UNITY_EDITOR)
        //从APK拷贝资源到本地
        //CopyAssetDirectory();
#else
        gameManager.InitGameCore(gameObject);
#endif
    }

#if UNITY_ANDROID && (!UNITY_EDITOR)
    ///// <summary>
    ///// 复制StreamingAsset资源
    ///// </summary>
    //private void CopyAssetDirectory()
    //{
    //    if (GloablDefine.resbasePathList.Count > 0 && resbaseIndex < GloablDefine.resbasePathList.Count)
    //    {          
    //        var resbasePath = GloablDefine.resbasePathList[resbaseIndex];
    //        var fullresbasePath = Path.Combine(StreamingAssetHelper.AssetPathDir, resbasePath);
    //        DirectoryInfo directoryInfo = new DirectoryInfo(fullresbasePath);
    //        resbaseIndex++;
    //        //Debug.LogWarning("------------------------>resbasePath" + fullresbasePath);
    //        //Debug.LogWarning("------------------------>resbasePath Is Exist" + directoryInfo.Exists);
    //        if (!directoryInfo.Exists)
    //        {
    //            UICopyingAssetHelper.Instance().UpdateUI(resbaseIndex,GloablDefine.resbasePathList.Count,"首次运行游戏正在解压资源...");
    //            StreamingAssetHelper.CopyAssetDirectoryInThread(resbasePath, resbasePath, OnCopyAssetDirectoryNext);
    //        }
    //        else
    //        {
    //            OnCopyAssetDirectoryNext(true);
    //        }
    //    }
    //    else
    //    {
    //        OnCopyAssetDirectoryFinished();
    //    }
    //}

    ///// <summary>
    ///// 复制资源的回调
    ///// </summary>
    ///// <param name="isSuccess"></param>
    //private void OnCopyAssetDirectoryNext(bool isSuccess)
    //{
    //    //Debug.LogWarning("初始化拷贝资源结果" + isSuccess);
    //    if (isSuccess)
    //    {
    //        //如果成功则继续拷贝剩余资源
    //        CopyAssetDirectory();
    //    }
    //    else
    //    {
    //        Debug.LogError("初始化拷贝资源错误，请检查手机内存空间是否充足！");
    //    }
    //}

    ///// <summary>
    ///// 所有的资源拷贝完成之后的回调
    ///// </summary>
    //private void OnCopyAssetDirectoryFinished()
    //{
    //    UICopyingAssetHelper.Instance().Close();
    //    gameManager.InitGameCore(gameObject);
    //}
#endif


    public void DelayInvokeNextFrame(System.Action action)
    {
        StartCoroutine(InvokeNextFrame(action));
    }

    private IEnumerator InvokeNextFrame(System.Action action)
    {
        yield return 1;
        if (null != action)
        {
            action();
        }
    }

    public void DelayInvokeSeconds(float seconds, System.Action action)
    {
        StartCoroutine(InvokeSeconds(seconds, action));
    }

    private IEnumerator InvokeSeconds(float seconds, System.Action action)
    {
        yield return new WaitForSeconds(seconds);
        if (null != action)
        {
            action();
        }
    }
}


