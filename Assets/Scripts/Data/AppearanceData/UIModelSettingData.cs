using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New UIModelSetting", menuName = "ColaFrame/UIModelSettingData", order = 1)]
public class UIModelSettingData : ScriptableObject
{
    [Tooltip("配置文件对应的key")]
    public string key;
    [Tooltip("模型文件资源的相对路径")]
    public string modelResPath;

    public float cameraPitch;
    public float cameraYaw;
    public float cameraDistance;
    public float cameraHeightOffset;
    public float modelCameraDepth;
    public float positionX;
    public float positionZ;
}
