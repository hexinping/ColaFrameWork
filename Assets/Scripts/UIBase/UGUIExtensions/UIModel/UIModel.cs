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
    public class UIModel : MonoBehaviour, IControl, IDragHandler
    {
        [LabelText("是否支持拖拽旋转")] public bool isRotate;

        [LabelText("旋转速度")] public float rotateSpeed = 2f;

        [LabelText("自动旋转速度")] public int autoRotateSpeed = 0;

        [SerializeField] private List<ModelData> _modelDatas = new List<ModelData>();

        private RectTransform _rectTransform;
        private RenderTexture _renderTexture;
        private RawImage _rawImage;
        private Camera _modelCamera;
        private int _modelIndex = 1;
        private static List<UIModel> _modelList = new List<UIModel>();

        void Awake()
        {
            _rectTransform = transform as RectTransform;
            if (null == _rawImage)
            {
                _rawImage = this.GetComponent<RawImage>();
            }
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
            if (isRotate && this._modelDatas.Count > 0)
            {
                _modelDatas[0].Character.transform.Rotate(0f, -(eventData.delta.x * rotateSpeed), 0f);
            }
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