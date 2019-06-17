#define SIMULATE_MODE
using System;
using System.Collections;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 资源加载助手类
/// </summary>
public class AssetLoader
{
    /// <summary>
    /// 根据类型和路径返回相应的资源(同步方法)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T Load<T>(string path) where T : Object
    {
        return Load(path, typeof(T)) as T;
    }

    /// <summary>
    /// 根据类型和路径返回相应的资源(同步方法)
    /// </summary>
    /// <param name="path"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Object Load(string path, Type type)
    {
#if UNITY_EDITOR && ! SIMULATE_MODE
        if (path.StartsWith("Assets/") && Path.HasExtension(path))
        {
            return AssetDatabase.LoadAssetAtPath(path, type);
        }
        else
        {
            Debug.LogWarning("资源加载的路径不合法!");
            return null;
        }
#else
        //TODO:非编辑器下加载
        return null;
#endif
    }

    /// <summary>
    /// 延迟一帧以后加载资源
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public static void LoadWaitOneFrame(string path, Action<Object, string> callback)
    {
#if UNITY_EDITOR && ! SIMULATE_MODE
        Debug.LogError("This Function is not implement in UnityEditor!");
#else
        CommonHelper.StartCoroutine(WaitOneFrameCall(path, callback));
#endif
    }

    private static IEnumerator WaitOneFrameCall(string path, Action<Object, string> callback)
    {
        yield return 1;
        Object result = Resources.Load<Object>(path);
        if (null != callback) callback(result, path);
    }

    /// <summary>
    /// 根据类型和路径返回相应的资源(异步方法)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public static void LoadAsync<T>(string path, Action<Object, string> callback) where T : Object
    {
#if UNITY_EDITOR && ! SIMULATE_MODE
        Debug.LogError("This Function is not implement in UnityEditor!");
#else
        CommonHelper.StartCoroutine(LoadAsyncCall<T>(path, callback));
#endif

    }

    private IEnumerator LoadAsyncCall<T>(string path, Action<Object, string> callback) where T : Object
    {
        ResourceRequest request = Resources.LoadAsync<T>(path);
        yield return request;
        if (null != callback)
        {
            callback(request.asset, path);
        }
    }

    /// <summary>
    /// 根据类型和路径返回相应的资源(异步方法)
    /// </summary>
    /// <param name="path"></param>
    /// <param name="t"></param>
    public void LoadAsync(string path, Type type, Action<Object, string> callback)
    {
#if UNITY_EDITOR && ! SIMULATE_MODE
        Debug.LogError("This Function is not implement in UnityEditor!");
#else
        CommonHelper.StartCoroutine(LoadAsyncCall(path, type, callback));
#endif
    }

    private IEnumerator LoadAsyncCall(string path, Type type, Action<Object, string> callback)
    {
        ResourceRequest request = Resources.LoadAsync(path, type);
        yield return request;
        if (null != callback)
        {
            callback(request.asset, path);
        }
    }

#if UNITY_EDITOR
    public static void LoadAllAssetsAtPath(string path, out Object[] objects)
    {
        objects = AssetDatabase.LoadAllAssetsAtPath(path);
    }
#endif
}
