#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ColaFramework框架中编辑器模式下的UI创建、获取UIRoot等
/// </summary>
public class ColaGUIEditor
{
    private static bool editorgetGameViewSizeError = false;
    public static bool editorgameViewReflectionError = false;

    public static Camera UICamera
    {
        get { return GetOrCreateUICamera(); }
    }


    /// <summary>
    /// 快速创建UI模版
    /// </summary>
    [MenuItem("GameObject/UI/ColaUI/UIView", false, 1)]
    public static void CreateColaUIView()
    {
        GameObject uguiRoot = GetOrCreateUGUIRoot();

        //创建新的UI Prefab
        GameObject view = new GameObject("NewUIView", typeof(RectTransform));
        view.tag = GloablDefine.UIViewTag;
        view.layer = LayerMask.NameToLayer("UI");
        string uniqueName = GameObjectUtility.GetUniqueNameForSibling(uguiRoot.transform, view.name);
        view.name = uniqueName;
        Undo.RegisterCreatedObjectUndo(view, "Create" + view.name);
        Undo.SetTransformParent(view.transform, uguiRoot.transform, "Parent" + view.name);
        GameObjectUtility.SetParentAndAlign(view, uguiRoot);

        //设置RectTransform属性
        RectTransform rect = view.GetComponent<RectTransform>();
        rect.offsetMax = rect.offsetMin = rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);

        //设置新建的UIView被选中
        Selection.activeGameObject = view;
    }


    /// <summary>
    /// 获取或者创建UGUIRoot（编辑器状态下）
    /// </summary>
    /// <returns></returns>
    public static GameObject GetOrCreateUGUIRoot()
    {
        GameObject uguiRootObj = GameObject.FindWithTag("UIRoot");
        if (uguiRootObj != null)
        {
            Canvas canvas = uguiRootObj.GetComponentInChildren<Canvas>();
            if (null != canvas && canvas.gameObject.activeInHierarchy)
            {
                return canvas.gameObject;
            }
        }

        //如果以上步骤都没有找到，那就从Resource里面加载并实例化一个
        var uguiRootPrefab = AssetLoader.Load<GameObject>("Arts/UI/Prefabs/UGUIRoot.prefab");
        GameObject uguiRoot = CommonHelper.InstantiateGoByPrefab(uguiRootPrefab, null);
        GameObject canvasRoot = uguiRoot.GetComponentInChildren<Canvas>().gameObject;
        return canvasRoot;
    }

    private static Camera GetOrCreateUICamera()
    {
        var uiRoot = GetOrCreateUGUIRoot();
        return uiRoot.GetComponentByPath<Camera>("UICamera");
    }

    // 获得分辨率，当选择 Free Aspect 直接返回相机的像素宽和高
    public static Vector2 GetScreenPixelDimensions()
    {
        Vector2 dimensions = new Vector2(UICamera.pixelWidth, UICamera.pixelHeight);

#if UNITY_EDITOR
        // 获取编辑器 GameView 的分辨率
        float gameViewPixelWidth = 0, gameViewPixelHeight = 0;
        float gameViewAspect = 0;

        if (EditorGetGameViewSize(out gameViewPixelWidth, out gameViewPixelHeight, out gameViewAspect))
        {
            if (gameViewPixelWidth != 0 && gameViewPixelHeight != 0)
            {
                dimensions.x = gameViewPixelWidth;
                dimensions.y = gameViewPixelHeight;
            }
        }
#endif

        return dimensions;
    }

    // 尝试获取 GameView 的分辨率
    // 当正确获取到 GameView 的分辨率时，返回 true
    public static bool EditorGetGameViewSize(out float width, out float height, out float aspect)
    {
        try
        {
            editorgameViewReflectionError = false;

            System.Type gameViewType = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetMainGameView = gameViewType.GetMethod("GetMainGameView", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            object mainGameViewInst = GetMainGameView.Invoke(null, null);
            if (mainGameViewInst == null)
            {
                width = height = aspect = 0;
                return false;
            }
            System.Reflection.FieldInfo s_viewModeResolutions = gameViewType.GetField("s_viewModeResolutions", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            if (s_viewModeResolutions == null)
            {
                System.Reflection.PropertyInfo currentGameViewSize = gameViewType.GetProperty("currentGameViewSize", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                object gameViewSize = currentGameViewSize.GetValue(mainGameViewInst, null);
                System.Type gameViewSizeType = gameViewSize.GetType();
                int gvWidth = (int)gameViewSizeType.GetProperty("width").GetValue(gameViewSize, null);
                int gvHeight = (int)gameViewSizeType.GetProperty("height").GetValue(gameViewSize, null);
                int gvSizeType = (int)gameViewSizeType.GetProperty("sizeType").GetValue(gameViewSize, null);
                if (gvWidth == 0 || gvHeight == 0)
                {
                    width = height = aspect = 0;
                    return false;
                }
                else if (gvSizeType == 0)
                {
                    width = height = 0;
                    aspect = (float)gvWidth / (float)gvHeight;
                    return true;
                }
                else
                {
                    width = gvWidth; height = gvHeight;
                    aspect = (float)gvWidth / (float)gvHeight;
                    return true;
                }
            }
            else
            {
                Vector2[] viewModeResolutions = (Vector2[])s_viewModeResolutions.GetValue(null);
                float[] viewModeAspects = (float[])gameViewType.GetField("s_viewModeAspects", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).GetValue(null);
                string[] viewModeStrings = (string[])gameViewType.GetField("s_viewModeAspectStrings", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).GetValue(null);
                if (mainGameViewInst != null
                    && viewModeStrings != null
                    && viewModeResolutions != null && viewModeAspects != null)
                {
                    int aspectRatio = (int)gameViewType.GetField("m_AspectRatio", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).GetValue(mainGameViewInst);
                    string thisViewModeString = viewModeStrings[aspectRatio];
                    if (thisViewModeString.Contains("Standalone"))
                    {
                        width = UnityEditor.PlayerSettings.defaultScreenWidth; height = UnityEditor.PlayerSettings.defaultScreenHeight;
                        aspect = width / height;
                    }
                    else if (thisViewModeString.Contains("Web"))
                    {
                        width = UnityEditor.PlayerSettings.defaultWebScreenWidth; height = UnityEditor.PlayerSettings.defaultWebScreenHeight;
                        aspect = width / height;
                    }
                    else
                    {
                        width = viewModeResolutions[aspectRatio].x; height = viewModeResolutions[aspectRatio].y;
                        aspect = viewModeAspects[aspectRatio];
                        // this is an error state
                        if (width == 0 && height == 0 && aspect == 0)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
        }
        catch (System.Exception e)
        {
            if (editorgetGameViewSizeError == false)
            {
                Debug.LogError("GameCamera.GetGameViewSize - has a Unity update broken this?\nThis is not a fatal error !\n" + e.ToString());
                editorgetGameViewSizeError = true;
            }
            editorgameViewReflectionError = true;
        }
        width = height = aspect = 0;
        return false;
    }
}
#endif
