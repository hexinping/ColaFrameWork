using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace ColaFramework.EditorExtension
{
    public class AppSetting : SerializedScriptableObject
    {
        [LabelText("是否开启模拟模式")]
        [OnValueChanged("OnValueChanged")]
        public bool SimulateMode;

        [LabelText("是否使用框架自带HttpServer")]
        [OnValueChanged("OnValueChanged")]
        public bool isLocalServer;

        [LabelText("游戏帧率")]
        [OnValueChanged("OnValueChanged")]
        public int GameFrameRate;


        private void OnValueChanged()
        {
            OnInitialize();
        }

        [InitializeOnLoadMethod]
        private static void OnInitialize()
        {
            var setting = GetSetting();
            AppConst.SimulateMode = setting.SimulateMode;
            AppConst.isLocalServer = setting.isLocalServer;
            AppConst.GameFrameRate = setting.GameFrameRate;
        }

        private static AppSetting GetSetting()
        {
            return ColaEditHelper.GetScriptableObjectAsset<AppSetting>("Assets/Editor/Settings/AppSetting.asset");
        }
    }
}