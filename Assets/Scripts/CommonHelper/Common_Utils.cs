using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 通用工具类，为导出lua接口调用准备
/// </summary>
public static class Common_Utils
{
    /// <summary>
    /// 设置共用的临时变量，避免频繁创建
    /// </summary>
    private static Vector3 vec3Tmp = Vector3.zero;
    private static Vector2 vec2Tmp = Vector2.zero;

    /// <summary>
    /// 给按钮添加点击事件(以后可以往这里添加点击声音)
    /// </summary>
    /// <param name="go"></param>
    /// <param name="callback"></param>
    public static void AddBtnMsg(GameObject go, Action<GameObject> callback)
    {
        CommonHelper.AddBtnMsg(go, callback);
    }

    /// <summary>
    /// 删除一个按钮的点击事件
    /// </summary>
    /// <param name="go"></param>
    /// <param name="callback"></param>
    public static void RemoveBtnMsg(GameObject go, Action<GameObject> callback)
    {
        CommonHelper.RemoveBtnMsg(go, callback);
    }

    /// <summary>
    /// 根据路径实例化一个Prefab(同步方法)
    /// </summary>
    /// <param name="path"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static GameObject InstantiateGoByPath(string path, GameObject parent)
    {
        GameObject prefab = AssetLoader.Load<GameObject>(path);
        return CommonHelper.InstantiateGoByPrefab(prefab, parent);
    }

    /// <summary>
    /// 根据路径实例化一个Prefab(异步方法)
    /// </summary>
    /// <param name="path"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static void InstantiateGoByPathAsync(string path, GameObject parent, Action<GameObject> callback)
    {
        AssetLoader.LoadAsync<GameObject>(path, (obj, name) =>
        {
            var go = InstantiateGoByPrefab(obj as GameObject, parent);
            if (null != callback)
            {
                callback(go);
            }
        });
    }

    /// <summary>
    /// 根据一个预制实例化一个Prefab
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static GameObject InstantiateGoByPrefab(GameObject prefab, GameObject parent)
    {
        return CommonHelper.InstantiateGoByPrefab(prefab, parent);
    }

    /// <summary>
    /// 给物体添加一个单一组件
    /// </summary>
    /// <param name="go"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Component AddSingleComponent(this GameObject go, Type type)
    {
        if (null != go)
        {
            Component component = go.GetComponent(type);
            if (null == component)
            {
                component = go.AddComponent(type);
            }
            return component;
        }
        Debug.LogWarning("要添加组件的物体为空！");
        return null;
    }

    /// <summary>
    /// 给物体添加一个单一组件
    /// </summary>
    /// <param name="go"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Component AddSingleComponent(this GameObject go, string type)
    {
        if (null != go)
        {
            Component component = go.GetComponent(type);
            if (null == component)
            {
                component = go.AddCustomComponent(type);
            }
            return component;
        }
        Debug.LogWarning("要添加组件的物体为空！");
        return null;
    }

    /// <summary>
    /// 给物体添加组件，注意：未导出的类型可能会产生警告，脚本类型会被强制转换为基类
    /// </summary>
    /// <param name="go"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Component AddCustomComponent(this GameObject go, string type)
    {
        if (null != go)
        {
            switch (type)
            {
                case "Image":
                    return go.AddComponent<Image>();
                case "RawImage":
                    return go.AddComponent<RawImage>();
                case "Canvas":
                    return go.AddComponent<Canvas>();
                case "Button":
                    return go.AddComponent<Button>();
                case "RectTransform":
                    return go.AddComponent<RectTransform>();
                case "UGUIMsgHandler":
                    return go.AddComponent<UGUIMsgHandler>();
                case "SorterTag":
                    return go.AddComponent<SorterTag>();
                case "GraphicRaycaster":
                    return go.AddComponent<GraphicRaycaster>();
                case "ParticleOrderAutoSorter":
                    return go.AddComponent<ParticleOrderAutoSorter>();
                default:
                    Debug.LogWarning(string.Format("期望添加的类型{0}未做处理!", type));
                    return null;
            }
        }
        Debug.LogWarning("要添加组件的物体为空！");
        return null;
    }

    /// <summary>
    /// 获取某个物体下对应名字的子物体(如果有重名的，就返回第一个符合的)
    /// </summary>
    /// <param name="go"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
    public static GameObject GetGameObjectByName(this GameObject go, string childName)
    {
        GameObject ret = null;
        if (go != null)
        {
            Transform[] childrenObj = go.GetComponentsInChildren<Transform>(true);
            if (childrenObj != null)
            {
                for (int i = 0; i < childrenObj.Length; ++i)
                {
                    if ((childrenObj[i].name == childName))
                    {
                        ret = childrenObj[i].gameObject;
                        break;
                    }
                }
            }
        }
        return ret;
    }

    /// <summary>
    /// 根据路径查找物体(可以是自己本身/子物体/父节点)
    /// </summary>
    /// <param name="obj"></param>父物体节点
    /// <param name="childPath"></param>子物体路径+子物体名称
    /// <returns></returns>
    public static GameObject FindChildByPath(this GameObject obj, string childPath)
    {
        if (null == obj)
        {
            Debug.LogWarning("FindChildByPath方法传入的根节点为空！");
            return null;
        }
        if ("." == childPath) return obj;
        if (".." == childPath)
        {
            Transform parentTransform = obj.transform.parent;
            return parentTransform == null ? null : parentTransform.gameObject;
        }
        Transform transform = obj.transform.Find(childPath);
        return null != transform ? transform.gameObject : null;
    }


    /// <summary>
    /// 根据路径查找物体上的某个类型的组件(可以是自己本身/子物体/父节点)
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="childPath"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Component GetComponentByPath(this GameObject obj, string childPath, string type)
    {
        GameObject childObj = FindChildByPath(obj, childPath);
        if (null == childObj)
        {
            return null;
        }
        Component component = childObj.GetComponent(type);
        if (null == component)
        {
            Debug.LogWarning(String.Format("没有在路径:{0}上找到组件:{1}!", childPath, type));
            return null;
        }
        return component;
    }

    /// <summary>
    /// 获取当前运行的设备平台信息
    /// </summary>
    /// <returns></returns>
    public static string GetDeviceInfo()
    {
        return CommonHelper.GetDeviceInfo();
    }

    /// <summary>
    /// 返回UI画布的根节点
    /// </summary>
    /// <returns></returns>
    public static GameObject GetUIRootObj()
    {
        return GUIHelper.GetUIRootObj();
    }

    /// <summary>
    /// 返回UI相机节点
    /// </summary>
    /// <returns></returns>
    public static GameObject GetUICameraObj()
    {
        return GUIHelper.GetUICameraObj();
    }

    /// <summary>
    /// 返回UI画布
    /// </summary>
    /// <returns></returns>
    public static Canvas GetUIRoot()
    {
        return GUIHelper.GetUIRoot();
    }

    /// <summary>
    /// 返回UI相机
    /// </summary>
    /// <returns></returns>
    public static Camera GetUICamera()
    {
        return GUIHelper.GetUICamera();
    }

    /// <summary>
    /// 获取主相机
    /// </summary>
    /// <returns></returns>
    public static Camera GetMainCamera()
    {
        return GUIHelper.GetMainCamera();
    }

    /// <summary>
    /// 获取主相机节点
    /// </summary>
    /// <returns></returns>
    public static GameObject GetMainGameObj()
    {
        return GUIHelper.GetMainGameObj();
    }

    /// <summary>
    /// 获取Effect相机节点
    /// </summary>
    /// <returns></returns>
    public static GameObject GetEffectCameraObj()
    {
        return GUIHelper.GetEffectCameraObj();
    }

    public static Camera GetEffectCamera()
    {
        return GUIHelper.GetEffectCamera();
    }

    /// <summary>
    /// 获取设备的电量
    /// </summary>
    /// <returns></returns>
    public static float GetBatteryLevel()
    {
        return CommonHelper.GetBatteryLevel();
    }

    /// <summary>
    /// 获取设备的电池状态
    /// </summary>
    /// <returns></returns>
    public static int GetBatteryStatus()
    {
        return (int)CommonHelper.GetBatteryStatus();
    }

    /// <summary>
    /// 获取设备网络的状况
    /// </summary>
    /// <returns></returns>
    public static int GetNetworkStatus()
    {
        return (int)CommonHelper.GetNetworkStatus();
    }

    /// <summary>
    /// 将世界坐标转化UGUI坐标
    /// </summary>
    /// <param name="gameCamera"></param>
    /// <param name="canvas"></param>
    /// <param name="worldPos"></param>
    public static Vector2 WorldToUIPoint(Camera gameCamera, Canvas canvas, Vector3 worldPos)
    {
        return CommonHelper.WorldToUIPoint(gameCamera, canvas, worldPos);
    }

    /// <summary>
    /// 获取一个Transform组件下所有处于Active状态的子物体的数量
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static int ActivedChildCount(this Transform transform)
    {
        int childCount = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
                childCount++;
        }
        return childCount;
    }

    /// <summary>
    /// 获取资源路径(可读写)
    /// </summary>
    /// <returns></returns>
    public static string GetAssetPath()
    {
        return CommonHelper.GetAssetPath();
    }

    /// <summary>
    /// 获取主机IP地址
    /// </summary>
    /// <returns></returns>
    public static string GetHostIp()
    {
        return CommonHelper.GetHostIp();
    }

    /// <summary>
    /// 获取一个GameObject下所有子物体的数量
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static int ChildCount(this GameObject obj)
    {
        if (null != obj)
        {
            return obj.transform.childCount;
        }

        return 0;
    }

    /// <summary>
    /// 根据索引获取一个GameOject的子物体
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static GameObject GetChild(this GameObject obj, int index)
    {
        if (null != obj)
        {
            return obj.transform.GetChild(index).gameObject;
        }

        return null;
    }

    /// <summary>
    /// 检查本地文件是否存在,如果目录不存在则创建目录
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static bool CheckLocalFileExist(string filePath)
    {
        return CommonHelper.CheckLocalFileExist(filePath);
    }

    public static Component[] GetComponentsInChildren(this GameObject obj, string type, bool includeInactive = false)
    {
        List<Component> components = new List<Component>();
        Component component = null;
        Transform[] objChildren = obj.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < objChildren.Length; ++i)
        {
            component = objChildren[i].GetComponent(type);
            components.Add(component);
        }
        return components.ToArray();
    }

    public static void AttachScreenText(Text text)
    {
        GameLauncher.Instance.LogHelper.AttachScreenText(text);
    }

    public static void UnAttachScreenText()
    {
        GameLauncher.Instance.LogHelper.UnAttachScreenText();
    }

    public static void ClearSreenLog()
    {
        GameLauncher.Instance.LogHelper.ClearSreenLog();
    }

    /// <summary>
    /// 获取当前位置距离地面的高度
    /// </summary>
    /// <param name="vPos"></param>
    /// <param name="fRadius"></param>
    /// <returns></returns>
    public static float GetTerrainHeight(float x, float y, float z, float fRadius = 0)
    {
        vec3Tmp.Set(x, y, z);
        return CommonHelper.GetTerrainHeight(vec3Tmp, fRadius);
    }

    /// <summary>
    /// 显示UI背景模糊
    /// </summary>
    /// <param name="ui"></param>
    public static void ShowUIBlur(GameObject uiPanel, string panelName)
    {
        string uiBlurName = string.Format("blur_{0}", panelName);
        GameObject uiBlurObj = uiPanel.FindChildByPath(uiBlurName);
        if (null != uiBlurObj)
        {
            RawImage rawImage = uiBlurObj.GetComponent<RawImage>();
            SetBlurRawImage(rawImage);
        }
        else
        {
            CreateUIBlur(uiPanel, uiBlurName);
        }
    }

    /// <summary>
    /// 创建UI背景模糊
    /// </summary>
    /// <param name="ui"></param>
    /// <param name="blurName"></param>
    private static void CreateUIBlur(GameObject uiPanel, string blurName)
    {
        GameObject uiBlurObj = new GameObject(blurName);
        uiBlurObj.transform.SetParent(uiPanel.transform, false);
        uiBlurObj.layer = uiPanel.layer;
        RawImage rawImage = uiBlurObj.AddComponent<RawImage>();
        Button button = uiBlurObj.AddComponent<Button>();
        button.transition = Selectable.Transition.None;
        RectTransform rectTransform = uiBlurObj.GetComponent<RectTransform>();
        if (null == rectTransform)
        {
            rectTransform = uiBlurObj.AddComponent<RectTransform>();
        }
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.SetAsFirstSibling();
        SetBlurRawImage(rawImage);
    }

    /// <summary>
    /// 设置背景模糊RawImage
    /// </summary>
    /// <param name="rawImage"></param>
    /// <param name="blurName"></param>
    /// <returns></returns>
    private static void SetBlurRawImage(RawImage rawImage)
    {
        if (null != rawImage)
        {
            rawImage.gameObject.SetActive(false);
            RenderTexture texture = GUIHelper.GetEffectCameraObj().GetComponent<ImageEffectUIBlur>().FinalTexture;
            if (texture)
            {
                rawImage.texture = texture;
            }
            rawImage.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 销毁渲染出来的临时UIBlur RenderTexture
    /// </summary>
    public static void DestroyUIBlur()
    {
        GUIHelper.GetEffectCameraObj().GetComponent<ImageEffectUIBlur>().FinalTexture = null;
    }


    public static SceneMgr GetSceneMgr()
    {
        return GameManager.GetInstance().GetSceneMgr();
    }

    public static void DelayInvokeNextFrame(Action action)
    {
        GameLauncher.Instance.DelayInvokeNextFrame(action);
    }

    public static void ResetMainCameraPostion()
    {

        //测试函数，用来验证动态模糊背景图的生成是否正常
        var Transform = GUIHelper.GetMainGameObj().transform;
        //测试函数
        Transform.position = new Vector3(84, 17, 20);
        Transform.localRotation = Quaternion.Euler(44.16f, -152, 4.6f);

        var effectCamera = GUIHelper.GetEffectCameraObj().transform;
        effectCamera.position = Transform.position;
        effectCamera.localRotation = Transform.localRotation;
    }
}
