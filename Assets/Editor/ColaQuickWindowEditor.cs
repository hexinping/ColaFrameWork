using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

public class ColaQuickWindowEditor : EditorWindow
{
    [MenuItem("ColaFramework/Open Quick Window %Q")]
    static void Popup()
    {
        ColaQuickWindowEditor window = EditorWindow.GetWindow<ColaQuickWindowEditor>();
        window.titleContent = new GUIContent("快捷工具窗");
        window.position = new Rect(400, 100, 640, 480);
        window.Show();
    }

    public void OnGUI()
    {
        DrawColaFrameworkUI();
        GUILayout.Space(20);
        DrawAssetBundleUI();
        GUILayout.Space(20);
        DrawMiscUI();
        GUILayout.Space(20);
        DrawAssetUI();
    }


    public void DrawColaFrameworkUI()
    {
        GUILayout.BeginHorizontal("HelpBox");
        EditorGUILayout.LabelField("== UI相关辅助 ==");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("创建NewUIView", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
            ColaGUIEditor.CreateColaUIView();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("创建C#版UIView脚本", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
            CreateScriptsEditor.CreateCSharpUIView();
        }
        if (GUILayout.Button("创建C#版Module脚本", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
            CreateScriptsEditor.CreateCSharpModule();
        }
        if (GUILayout.Button("创建C#版Templates(UIView和Module)", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
            CreateScriptsEditor.CreateCSharpModule();
        }
        GUILayout.EndHorizontal();
    }

    public void DrawAssetBundleUI()
    {
        GUILayout.BeginHorizontal("HelpBox");
        EditorGUILayout.LabelField("== Assetbundle相关 ==");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("打包Assetbundle（增量）", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
        }
        if (GUILayout.Button("重新打包Assetbundle（先删除再重打）", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        if (GUILayout.Button("为所有资源设置Assetbundle name", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
        }
        if (GUILayout.Button("清除所有资源的Assetbundle name", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("AssetBundle Browser", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
            AssetBundleBrowser.AssetBundleBrowserMain.ShowWindow();
            this.Close();
        }
        GUILayout.EndHorizontal();
    }

    private void DrawMiscUI()
    {
        GUILayout.BeginHorizontal("HelpBox");
        EditorGUILayout.LabelField("== 快捷功能 ==");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("打开AssetPath目录", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
            ColaEditHelper.OpenDirectory(CommonHelper.GetAssetPath());
        }
        if (GUILayout.Button("打开GameLog文件目录", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
            ColaEditHelper.OpenDirectory(Path.Combine(CommonHelper.GetAssetPath(), "logs"));
        }
        GUILayout.EndHorizontal();
    }

    private void DrawAssetUI()
    {
        GUILayout.BeginHorizontal("HelpBox");
        EditorGUILayout.LabelField("== 快捷功能 ==");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Build Lua To StreamingAsset", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
            ColaEditHelper.BuildLuaToStreamingAsset();
        }
        GUILayout.EndHorizontal();
    }
}
