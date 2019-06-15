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
            model.InitInEditor(ColaGUIEditor.GetOrCreateUICamera());
        }
    }

    protected override void DrawCustomGUI()
    {
        if (GUILayout.Button("导入设置"))
        {
            this.OnEnable();
            Debug.Log("----->导入设置");
        }

        if (GUILayout.Button("导出设置"))
        {
            Debug.Log("----->导出设置");
        }

        if (GUILayout.Button("恢复默认设置"))
        {
            Debug.Log("----->恢复默认设置");
        }

        UpdateModel();
    }

    /// <summary>
    /// 更新编辑器中的模型信息
    /// </summary>
    private void UpdateModel()
    {
        if(model && model.gameObject && model.gameObject.activeSelf)
        {
            model.UpdateInEditor();
        }
    }
}
