//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 资源信息
/// </summary>
public class ResourceInfo
{
    /// <summary>
    /// 实际的资源
    /// </summary>
    public Object Res;

    /// <summary>
    /// 资源的生存时间 -2 永久存在 -1 当前系统 大于0则按时间删除
    /// </summary>
    public int RemainSec;

    /// <summary>
    /// 资源的类型
    /// </summary>
    public Type ResourceType;
}


/// <summary>
/// 资源管理器
/// </summary>
public class ResourcesMgr
{
    private static ResourcesMgr instance;

    /// <summary>
    /// 资源路径与实际资源的映射表
    /// </summary>
    private Dictionary<string, ResourceInfo> path2ResourceDic;

    /// <summary>
    /// 需要清理的资源列表
    /// </summary>
    private List<string> resClearList;

    /// <summary>
    /// 用于计时的变量
    /// </summary>
    private float timeCount = 0f;

    public static ResourcesMgr GetInstance()
    {
        if (null == instance)
        {
            instance = new ResourcesMgr();
        }
        return instance;
    }

    private ResourcesMgr()
    {
        GameObject resourceLoaderObj = new GameObject("ResourceLoaderObj");
        GameObject.DontDestroyOnLoad(resourceLoaderObj);

        timeCount = 0f;
        resClearList = new List<string>();
        path2ResourceDic = new Dictionary<string, ResourceInfo>();
    }

    /// <summary>
    /// 加载文本(同步)
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <param name="callback"></param>
    public void LoadText(string path, string fileName, Action<string, string> callback)
    {
        TextAsset textAsset = Load(path, typeof(TextAsset)) as TextAsset;
        if (null != callback)
            callback(fileName, textAsset.text);
    }


    /// <summary>
    /// 支持从Resources以外目录读取
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <param name="callback"></param>
    public void LoadText(string path, string fileName, Action<string, byte[]> callback)
    {

#if UNITY_ANDROID && !UNITY_EDITOR
        WWW www = new WWW(path);
        while (!www.isDone)
        {
        }
        if (null != callback)
        {
            callback(fileName,www.bytes);
        }
#else
        //支持从Resources以外目录读取
        var bytes = File.ReadAllBytes(path);
        if (null != callback)
            callback(fileName, bytes);
#endif
    }

    /// <summary>
    /// 加载文本(异步)
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <param name="callback"></param>
    public void LoadTextAsync(string path, string fileName, Action<string, string> callback)
    {
        LoadAsync(path, typeof(TextAsset), (obj, name) =>
         {
             TextAsset textAsset = obj as TextAsset;
             if (null != callback)
                 callback(fileName, textAsset.text);
         });
    }

    /// <summary>
    /// 简单读取一个文本，同步无回调方式
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public string ReadTextFile(string path)
    {
        TextAsset textAsset = Load(path, typeof(TextAsset)) as TextAsset;
        if (null != textAsset)
        {
            return textAsset.text;
        }
        Debug.LogWarning(string.Format("加载路径为{0}的文件失败！", path));
        return string.Empty;
    }


    /// <summary>
    /// 模拟 Update
    /// </summary>
    /// <param name="deltaTime"></param>
    public void Update(float deltaTime)
    {
        if (null != instance)
        {
            timeCount += deltaTime;
            if (timeCount >= 1f)
            {
                timeCount = 0;
                using (var enumator = path2ResourceDic.GetEnumerator())
                {
                    while (enumator.MoveNext())
                    {
                        if (enumator.Current.Value.RemainSec > 0)
                        {
                            enumator.Current.Value.RemainSec--;
                        }
                        if (enumator.Current.Value.RemainSec == 0)
                        {
                            resClearList.Add(enumator.Current.Key);
                        }
                    }
                }
                RemoveResList(resClearList);
                resClearList.Clear();
            }
        }
    }

    /// <summary>
    /// 清理列表中对应路径的资源
    /// </summary>
    /// <param name="removeList"></param>需要清理的资源对应的ID列表
    private void RemoveResList(List<string> removeList)
    {
        if (null == removeList || 0 == removeList.Count) return;
        for (int i = 0; i < removeList.Count; i++)
        {
            path2ResourceDic.Remove(removeList[i]);
            Debug.LogWarning(string.Format("清理路径为{0}的资源", removeList[i]));
        }
    }

    /// <summary>
    /// 强制清除全部资源
    /// </summary>
    public void ClearAllResourcesForce()
    {
        path2ResourceDic.Clear();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        //清理bundle
    }

    /// <summary>
    /// 向资源缓存池中添加资源
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="resId"></param>
    /// <param name="type"></param>
    private void AddResource(Object obj, string resPath, Type type)
    {
        if (null != obj)
        {
            ResourceInfo resourceInfo = new ResourceInfo();
            resourceInfo.Res = obj;
            resourceInfo.ResourceType = type;

            resourceInfo.RemainSec = 180; //暂时设定180s

            path2ResourceDic[resPath] = resourceInfo;
        }
        else
        {
            Debug.LogWarning(string.Format("添加路径为{0}的资源到缓存池失败!", resPath));
        }
    }

    /// <summary>
    /// 获取资源缓存池中的资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="resID"></param>
    /// <returns></returns>
    private Object GetCacheResByPath(string resPath, Type type)
    {
        Object resObj = null;
        if (null != path2ResourceDic && path2ResourceDic.ContainsKey(resPath))
        {
            resObj = path2ResourceDic[resPath].Res;
        }
        return resObj;
    }

    /// <summary>
    /// 返回不包含拓展名的资源路径(用于资源加载)
    /// </summary>
    /// <param name="path"></param>完整的资源路径
    /// <returns></returns>
    public string GetResPathWithExtension(string path)
    {
        if (null != path)
        {
            string[] ss = path.Split('.');
            if (null != ss[0])
            {
                return ss[0];
            }
        }
        return string.Empty;
    }

    /// <summary>
    /// 根据资源路径获取对应资源，不存在则懒加载(同步方法)
    /// </summary>
    /// <param name="resPath"></param>
    /// <param name="type"></param>
    /// <param name="resLoadMode"></param>
    /// <returns></returns>
    public Object GetResourceByPath(string resPath, Type type, int resLoadMode)
    {
        Object resObj = null;
        //先去缓存池中找，如果没有再懒加载
        resObj = GetCacheResByPath(resPath, type);

        if (null == resObj)
        {
            string relativePath = GetResPathWithExtension(resPath);
            if (string.IsNullOrEmpty(resPath))
            {
                Debug.LogWarning(string.Format("加载资源路径为：{0}的资源出错！资源路径不存在！", resPath));
            }
            else
            {
                if (0 == resLoadMode)
                {
                    resObj = Load(relativePath, type);
                }
                else if (1 == resLoadMode)
                {
                    //todo:从bundle加载资源
                }
                AddResource(resObj, resPath, type);
            }
        }

        if (null == resObj)
        {
            Debug.LogWarning(string.Format("加载资源失败！路径:{0}", resPath));
        }
        return resObj;
    }

    /// <summary>
    /// 根据资源路径获取对应资源，不存在则懒加载(异步方法)
    /// </summary>
    /// <param name="resPath"></param>
    /// <param name="type"></param>
    /// <param name="resLoadMode"></param>
    /// <returns></returns>
    public void GetResourceByPathAsync(string resPath, Type type, int resLoadMode, Action<Object> callback)
    {
        Object resObj = null;
        //先去缓存池中找，如果没有再懒加载
        resObj = GetCacheResByPath(resPath, type);

        if (null == resObj)
        {
            string relativePath = GetResPathWithExtension(resPath);
            if (string.IsNullOrEmpty(resPath))
            {
                Debug.LogWarning(string.Format("加载资源路径为：{0}的资源出错！资源路径不存在！", resPath));
            }
            else
            {
                if (0 == resLoadMode)
                {
                    LoadAsync(resPath, type, (obj, path) =>
                    {
                        if (null != callback)
                        {
                            callback(obj);
                        }
                    });
                }
                else if (1 == resLoadMode)
                {
                    //todo:从bundle加载资源
                }
                AddResource(resObj, resPath, type);
            }
        }
        else
        {
            if (null != callback)
            {
                callback(resObj);
            }
        }

        if (null == resObj)
        {
            Debug.LogWarning(string.Format("加载资源失败！路径:{0}", resPath));
        }
    }

    /// <summary>
    /// 根据类型和路径返回相应的资源(同步方法)
    /// </summary>
    /// <param name="path"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public Object Load(string path, Type type)
    {
        return Resources.Load(path, type);
    }

    /// <summary>
    /// 延迟一帧以后加载资源
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public void LoadWaitOneFrame(string path, Action<Object, string> callback)
    {
        CommonHelper.StartCoroutine(WaitOneFrameCall(path, callback));
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
    /// <param name="path"></param>
    /// <param name="t"></param>
    public void LoadAsync(string path, Type type, Action<Object, string> callback)
    {
        CommonHelper.StartCoroutine(LoadAsyncCall(path, type, callback));
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
}
