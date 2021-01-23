//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using ColaFramework;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.Forms;
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

        private const float rate = 1.5f;

        void Awake()
        {
            _rectTransform = transform as RectTransform;
            if (null == _rawImage)
            {
                _rawImage = this.GetComponent<RawImage>();
            }
        }

        private void LateUpdate()
        {
            if (0 != autoRotateSpeed)
            {
                var y = autoRotateSpeed * Time.unscaledDeltaTime;
                RotateYAxis(y);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isRotate && this._modelDatas.Count > 0)
            {
                _modelDatas[0].Character.transform.Rotate(0f, -(eventData.delta.x * rotateSpeed), 0f);
            }
        }

        public void SetVisible(bool isVisible)
        {
            if (!_modelCamera)
            {
                gameObject.SetActive(isVisible);
                return;
            }

            if (isVisible != _modelCamera.gameObject.activeSelf || isVisible != gameObject.activeSelf)
            {
                //创建Texture和Model，激活相机
                if (isVisible)
                {
                    gameObject.SetActive(true);
                    _modelCamera.gameObject.SetActive(true);
                    InitTargetTexture();
                    _rawImage.texture = _renderTexture;
                }
                //销毁Texture和模型，关闭相机
                else
                {
                    //删除Model
                    SetCharacter((null));
                    
                    //释放RenderTexture
                    var tempRenderTexture = _renderTexture;
                    _rawImage.texture = null;
                    _renderTexture = null;
                    _modelCamera.targetTexture = null;
                    RenderTexture.ReleaseTemporary(tempRenderTexture);
                    
                    //隐藏相机和组件
                    _modelCamera.gameObject.SetActive(false);
                    gameObject.SetActive(false);
                }

            }
            
        }

        private void OnDestroy()
        {
            Release();
        }

        public void Release()
        {
            foreach (var item in _modelDatas)
            {
                if (null != item.Character)
                {
                    item.Character.Release();
                }

                item.Character = null;
            }

            if (_modelDatas.Count > _modelIndex)
            {
                if (this == _modelList[_modelIndex])
                {
                    var modelCamera = _modelList[_modelIndex]._modelCamera;
                    if (null != modelCamera)
                    {
                        modelCamera.targetTexture = null;
                        CommonUtil.ReleaseGameObject(modelCamera.name,modelCamera.gameObject);
                    }
                }
            }
        }

        private void InitTargetTexture()
        {
            var rect = _rectTransform.rect;
            var width = (int)(rect.width * rate);
            var height = (int)(rect.height * rate);
            _renderTexture = RenderTexture.GetTemporary(width, height, 16, RenderTextureFormat.ARGB32);
            _renderTexture.antiAliasing = 1;
            _modelCamera.targetTexture = _renderTexture;
        }
        
        private void RotateYAxis(float y)
        {
            var curCharacter = GetCharacter(_modelIndex);
            if (curCharacter.isNotNull())
            {
                curCharacter.transform.Rotate(0, y, 0);
            }
        }

        #region Camera Control && model Control

        private void PrepareModelCamera()
        {
        }

        private void AdjustModel(int index, ISceneCharacter character)
        {
        }

        #endregion

        #region Character API

        public ISceneCharacter GetCharacter()
        {
            return GetCharacter(0);
        }

        public ISceneCharacter GetCharacter(int index)
        {
            if (index >= 0 && index < _modelDatas.Count)
            {
                return _modelDatas[index].Character;
            }

            return null;
        }

        public void SetCharacter(ISceneCharacter character)
        {
            SetCharacter(0, character);
        }

        public void SetCharacter(int index, ISceneCharacter character)
        {
            SetCharacter(index, character, AnimCurveNames.Idle, null);
        }

        public void SetCharacter(int index, ISceneCharacter character, string curAnimName, string nextAnimName)
        {
            if (index >= 0 && index < _modelDatas.Count)
            {
                var modelData = _modelDatas[index];
                modelData.Character = null;
                if (null != character)
                {
                    PrepareModelCamera();
                    modelData.Character = character;
                    character.transform.SetParent(_modelCamera.transform, false);
                    AdjustModel(index, character);
                    if (!string.IsNullOrEmpty(nextAnimName))
                    {
                        character.PlayAnimation(curAnimName,
                            (value) => { character.PlayAnimation(nextAnimName, null); });
                    }
                    else
                    {
                        character.PlayAnimation(curAnimName);
                    }
                }
            }
        }

        #endregion
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