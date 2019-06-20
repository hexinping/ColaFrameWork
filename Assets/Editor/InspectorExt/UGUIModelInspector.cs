using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ColaFrame;


/// <summary>
/// UGUIModel组件的Inspector编辑器拓展
/// </summary>
[CustomEditor(typeof(UGUIModel), true)]
public class UGUIModelInspector : InspectorBase
{
    private UGUIModel model;

    protected override void OnEnable()
    {
        ShowCustomProperties = true;
        model = target as UGUIModel;
        if (model.gameObject.activeSelf)
        {
            model.InitInEditor(ColaGUIEditor.UICamera);
        }
    }

    protected override void DrawCustomGUI()
    {
        if (model)
        {
            var cameraYaw = serializedObject.FindProperty("cameraYaw");
            cameraYaw.floatValue = EditorGUILayout.Slider("相机Y轴旋转参数:", cameraYaw.floatValue, 0, 180);
            EditorGUILayout.Space();

            SerializedProperty modelOffsetX = serializedObject.FindProperty("modelOffsetX");
            modelOffsetX.floatValue = EditorGUILayout.Slider("偏移量X:", modelOffsetX.floatValue, -0.5f, 0.5f);
            DrawProgressBar("偏移量X", modelOffsetX.floatValue + 0.5f);

            SerializedProperty modelOffsetZ = serializedObject.FindProperty("modelOffsetZ");
            modelOffsetZ.floatValue = EditorGUILayout.Slider("偏移量Z:", modelOffsetZ.floatValue, -0.5f, 0.5f);
            DrawProgressBar("偏移量Z", modelOffsetZ.floatValue + 0.5f);
        }

        if (GUILayout.Button("导入设置"))
        {
            this.OnEnable();
            if (model)
            {
                model.ImportModelInEditor();
            }
        }

        if (GUILayout.Button("导出设置"))
        {
            if (model)
            {
                model.SaveSetting();
            }
        }

        if (GUILayout.Button("恢复默认设置"))
        {
            if (model)
            {
                model.DefaultSetting();
            }
        }

        UpdateModel();
    }

    /// <summary>
    /// 更新编辑器中的模型信息
    /// </summary>
    private void UpdateModel()
    {
        if (model && model.gameObject && model.gameObject.activeSelf)
        {
            model.UpdateInEditor();
        }
    }

    private void DrawProgressBar(string label, float value)
    {
        Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
        EditorGUI.ProgressBar(rect, value, label);
        EditorGUILayout.Space();
    }
}
