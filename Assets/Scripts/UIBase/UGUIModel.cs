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
    public void OnDrag(PointerEventData eventData)
    {

    }
}
