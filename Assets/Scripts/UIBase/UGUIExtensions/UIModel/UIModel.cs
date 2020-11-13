//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using ColaFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// UIModel组件，用来展示3D人物形象
    /// 不同于UGUIModel组件，该组件使用RenderTexture原理实现
    /// </summary>
    [RequireComponent(typeof(RawImage)), DisallowMultipleComponent]
    public class UIModel : IControl,IDragHandler
    {
        void Awake()
        {

        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnDrag(PointerEventData eventData)
        {

        }
    }
}