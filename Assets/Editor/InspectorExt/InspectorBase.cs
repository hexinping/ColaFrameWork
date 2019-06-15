using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ColaFrame
{
    /// <summary>
    /// ColaFramework框架的 Inspector 拓展编辑器的基类
    /// </summary>
    public class InspectorBase : Editor
    {
        protected bool ShowCustomProperties = false;

        /// <summary>
        /// 子类可以重写子函数，用于初始化
        /// </summary>
        protected virtual void OnDisable()
        {
        }

        /// <summary>
        /// 子类可以重写此函数，用于清理工作
        /// </summary>
        protected virtual void OnEnable()
        {
        }

        /// <summary>
        /// 子类不要重写此函数
        /// </summary>
        public override void OnInspectorGUI()
        {
            // Update the serializedProperty -always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();
            EditorGUI.BeginDisabledGroup(!ShowCustomProperties);
            DrawCustomGUI();
            EditorGUI.EndDisabledGroup();
            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            //需要在OnInspectorGUI之前修改属性，否则无法修改值
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }

        /// <summary>
        /// 重写此函数以便绘制你自己的GUI
        /// </summary>
        protected virtual void DrawCustomGUI()
        {

        }

    }
}
