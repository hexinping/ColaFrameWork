//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

namespace ColaFramework.ToolKit
{
    /// <summary>
    /// 可视化打包窗口，方便在本地进行打包测试
    /// </summary>
    public class ColaBuildWindow : OdinEditorWindow
    {
        private static ColaBuildWindow window;

        [LabelText("是否母包")]
        [SerializeField]
        [LabelWidth(200)]
        private bool isMotherPkg;

        [LabelText("是否热更")]
        [SerializeField]
        [LabelWidth(200)]
        private bool isHotUpdate;

        [LabelText("是否是Development Debug包")]
        [LabelWidth(200)]
        [SerializeField]
        private bool isDebug = false;

        [LabelText("是Mono包还是il2cpp包")]
        [LabelWidth(200)]
        [SerializeField]
        private bool isMono = false;

        [LabelText("C#宏定义")]
        [SerializeField]
        [LabelWidth(200)]
        private string CSSymbolDefine;

        [Button("一键打包", ButtonSizes.Large, ButtonStyle.Box)]
        private void BuildPlayer()
        {
            ColaBuildTool.SetEnvironmentVariable(EnvOption.MOTHER_PKG, isMotherPkg.ToString(), false);
            ColaBuildTool.SetEnvironmentVariable(EnvOption.HOT_UPDATE_BUILD, isHotUpdate.ToString(), false);
            ColaBuildTool.BuildPlayer(BuildTarget.Android);
        }

        private void Init()
        {
            ColaBuildTool.ClearEnvironmentVariable();
        }

        [MenuItem("Build/快速打包窗口")]
        public static void PopUp()
        {
            window = GetWindow<ColaBuildWindow>("快速打包窗口");
            window.maximized = false;
            window.maxSize = window.minSize = new Vector2(500, 300);
            window.Init();
            window.Show();
        }
    }
}
