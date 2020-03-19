//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "AppVersion", menuName = "ColaFramework/AppVersion", order = 2)]
public class AppVersion : SerializedScriptableObject
{
    [LabelText("主版本号")]
    [OnValueChanged("OnValueChanged")]
    public int MainVersion;

    [LabelText("商店版本号")]
    [OnValueChanged("OnValueChanged")]
    public int StoreVersion;


    [LabelText("热更版本号")]
    [OnValueChanged("OnValueChanged")]
    public int HotUpdateVersion;

    [LabelText("编译版本号")]
    [OnValueChanged("OnValueChanged")]
    public int BuildVersion;

    [ReadOnly]
    public string Version;

    [LabelText("强更版本号")]
    public string MinVersion;

    [LabelText("推荐版本号")]
    public string RecommandVersion;

    public void OnValueChanged()
    {
        Version = string.Format("{0}.{1}.{2}.{3}", MainVersion, StoreVersion, HotUpdateVersion, BuildVersion);
    }
}
