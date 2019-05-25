using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/// <summary>
/// UGUIMODEL组件，用来展示3D人物形象
/// </summary>
[RequireComponent(typeof(RectTransform),typeof(EmptyRaycast))]
public class UGUIModel : UIBehaviour, IDragHandler
{
    /// <summary>
    /// 模型的LayerName
    /// </summary>
    private const string UIModelLayerName = "UI_Model";

    [SerializeField]
    private Vector3 scale = Vector3.one;
    [SerializeField]
    private float positionX = 0.0f;
    [SerializeField]
    private float positionY = 0.0f;
    [SerializeField]
    private float modelOffsetX = 0.0f;
    [SerializeField]
    private float modelOffsetY = 0.0f;

    [SerializeField]
    private float cameraDistance = 3.0f;
    [SerializeField]
    private float cameraHeightOffset = 0.0f;
    [SerializeField]
    private bool enableRotate = true;

    public void OnDrag(PointerEventData eventData)
    {

    }
}
