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
using Sirenix.OdinInspector;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// UIModel组件，用来展示3D人物形象
    /// 不同于UGUIModel组件，该组件使用RenderTexture原理实现
    /// </summary>
    [RequireComponent(typeof(RawImage)), DisallowMultipleComponent]
    public class UIModel : IControl, IDragHandler
    {
        [LabelText("是否支持拖拽旋转")]
        public bool isRotate;

        [LabelText("自动旋转速度")]
        public int autoRotateSpeed = 0;
        
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

    /// <summary>
    ///  模型数据
    /// </summary>
    [System.Serializable]
    public class ModelData
    {
        public string name;
        public RectTransform transform;

        private ISceneCharacter _character;

        public ISceneCharacter Character
        {
            get { return _character; }
            set
            {
                if (value != _character)
                {
                    if (_character.isNotNull())
                    {
                        _character.Release();
                    }

                    _character = value;
                }
            }
        }
    }
}