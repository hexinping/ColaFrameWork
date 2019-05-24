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
public class ResourceMgr
{
    private static ResourceMgr instance;
    private ResourceLoader resourceLoader;

    /// <summary>
    /// 资源ID与实际资源的映射表
    /// </summary>
    private Dictionary<int, ResourceInfo> id2ResourceDic;

    /// <summary>
    /// 需要清理的资源列表
    /// </summary>
    private List<int> resClearList;

    /// <summary>
    /// 用于计时的变量
    /// </summary>
    private float timeCount = 0f;

    /// <summary>
    /// 资源路径信息
    /// </summary>
    private ResPathDataMap resPathDataMap;

    public static ResourceMgr GetInstance()
    {
        if (null == instance)
        {
            instance = new ResourceMgr();
        }
        return instance;
    }

    private ResourceMgr()
    {
        GameObject resourceLoaderObj = new GameObject("ResourceLoaderObj");
        GameObject.DontDestroyOnLoad(resourceLoaderObj);

        timeCount = 0f;
        resClearList = new List<int>();
        resourceLoader = resourceLoaderObj.AddComponent<ResourceLoader>();
        id2ResourceDic = new Dictionary<int, ResourceInfo>();
    }

    public void Init()
    {
        resPathDataMap = LocalDataMgr.GetLocalDataMap<ResPathDataMap>();
    }

    /// <summary>
    /// 加载文本(同步)
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <param name="callback"></param>
    public void LoadText(string path, string fileName, Action<string, string> callback)
    {
        TextAsset textAsset = resourceLoader.Load<TextAsset>(path);
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
        resourceLoader.LoadAsync<TextAsset>(path, (obj, name) =>
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
        TextAsset textAsset = resourceLoader.Load<TextAsset>(path);
        if (null != textAsset)
        {
            return textAsset.text;
        }
        Debug.LogWarning(string.Format("加载路径为{0}的文件失败！",path));
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
                using (var enumator = id2ResourceDic.GetEnumerator())
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
    /// 清理列表中对应ID的资源
    /// </summary>
    /// <param name="removeList"></param>需要清理的资源对应的ID列表
    private void RemoveResList(List<int> removeList)
    {
        if (null == removeList || 0 == removeList.Count) return;
        for (int i = 0; i < removeList.Count; i++)
        {
            id2ResourceDic.Remove(removeList[i]);
            Debug.LogWarning(string.Format("清理ID为{0}的资源",removeList[i]));
        }
    }

    /// <summary>
    /// 强制清除全部资源
    /// </summary>
    public void ClearAllResourcesForce()
    {
        id2ResourceDic.Clear();
        Resources.UnloadUnusedAssets();
        //清理bundle
    }

    /// <summary>
    /// 向资源缓存池中添加资源
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="resId"></param>
    /// <param name="type"></param>
    private void AddResource(Object obj, int resId, Type type)
    {
        if (null != obj)
        {
            ResourceInfo resourceInfo = new ResourceInfo();
            resourceInfo.Res = obj;
            resourceInfo.RemainSec = -1;
            resourceInfo.ResourceType = type;
            if (null != resPathDataMap)
            {
                resourceInfo.RemainSec = resPathDataMap.GetDataById(resId).resWaitSec;
            }
            else
            {
                Debug.LogWarning("resPathDataMap初始化错误！");
            }
            id2ResourceDic[resId] = resourceInfo;
        }
        else
        {
            Debug.LogWarning(string.Format("添加ID为{0}的资源到缓存池失败!",resId));
        }
    }

    /// <summary>
    /// 向资源缓存池中添加资源(泛型接口)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="resId"></param>
    private void AddResource<T>(Object obj, int resId) where T : Object
    {
        AddResource(obj, resId, typeof(T));
    }

    /// <summary>
    /// 获取资源缓存池中的资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="resID"></param>
    /// <returns></returns>
    public T GetCacheResById<T>(int resID) where T : Object
    {
        T resObj = null;
        if (null != id2ResourceDic && id2ResourceDic.ContainsKey(resID))
        {
            resObj = id2ResourceDic[resID].Res as T;
        }
        return resObj;
    }

    /// <summary>
    /// 根据资源ID获取对应资源的名字(不包含路径和拓展名的纯名字)
    /// </summary>
    /// <returns></returns>
    public string GetResNameById(int resID)
    {
        string fullName = GetResPathById(resID);
        if (!string.IsNullOrEmpty(fullName))
        {
            return Path.GetFileNameWithoutExtension(fullName);
        }
        Debug.LogWarning(string.Format("获取资源ID为：{0}的资源名称出错！", resID));
        return string.Empty;
    }

    /// <summary>
    /// 根据资源ID获取对应资源的完整路径(包含拓展名)
    /// </summary>
    /// <param name="resID"></param>
    /// <returns></returns>
    public string GetResPathById(int resID)
    {
        if (null != resPathDataMap)
        {
            ResPathData data = resPathDataMap.GetDataById(resID);
            if (null != data)
            {
                return data.resPath;
            }
            Debug.LogWarning(string.Format("resPathDataMap 键{0}对应的值为空！", resID));
            return string.Empty;
        }
        Debug.LogWarning("resPathDataMap初始化错误！");
        return string.Empty;
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
    /// 根据资源ID获取对应资源配置信息
    /// </summary>
    /// <param name="resID"></param>
    /// <returns></returns>
    public ResPathData GetResPathDataById(int resID)
    {
        if (null != resPathDataMap)
        {
            var data = resPathDataMap.GetDataById(resID);
            if (null != data)
            {
                return data;
            }
            Debug.LogWarning(string.Format("resPathDataMap 键{0}对应的值为空！", resID));
        }
        Debug.LogWarning("resPathDataMap初始化错误！");
        return null;
    }

    /// <summary>
    /// 根据资源ID获取对应资源，缓存池中没有则懒加载(同步方法)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="resID"></param>
    /// <returns></returns>
    public T GetResourceById<T>(int resID) where T : Object
    {
        T resObj = null;
        //先去缓存池中找，如果没有再懒加载
        resObj = GetCacheResById<T>(resID);

        if (null == resObj)
        {
            ResPathData data = GetResPathDataById(resID);
            if (null != data)
            {
                string resPath = GetResPathWithExtension(data.resPath);
                if (string.IsNullOrEmpty(resPath))
                {
                    Debug.LogWarning(string.Format("加载资源ID为：{0}的资源出错！资源路径不存在！", resID));
                }
                else
                {
                    resObj = GetResourceByPath<T>(resPath, data.resLoadMode);
                    AddResource(resObj,resID,typeof(T));
                }
            }
        }
        return resObj;
    }

    /// <summary>
    /// 根据资源路径获取对应资源，不存在则懒加载(同步方法)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="resPath"></param>
    /// <param name="resLoadType"></param>
    /// <returns></returns>
    private T GetResourceByPath<T>(string resPath, int resLoadMode) where T : Object
    {
        T resObj = null;
        if (string.IsNullOrEmpty(resPath))
        {
            Debug.LogWarning(string.Format("加载资源路径为：{0}的资源出错！资源路径不存在！", resPath));
        }
        else
        {
            if (0 == resLoadMode)
            {
                resObj = resourceLoader.Load<T>(resPath);
            }
            else if (1 == resLoadMode)
            {
                //todo:从bundle加载资源
            }
        }
        if (null == resObj)
        {
            Debug.LogWarning(string.Format("加载资源失败！路径:{0}", resPath));
        }
        return resObj;
    }
}
