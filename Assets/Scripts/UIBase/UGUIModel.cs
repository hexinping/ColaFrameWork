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

    #region 属性字段
    /// <summary>
    /// 模型的LayerName
    /// </summary>
    private const string UIModelLayerName = "UI_Model";

    [SerializeField]
    [Tooltip("模型的缩放")]
    private Vector3 scale = Vector3.one;
    [SerializeField]
    [Tooltip("模型的X坐标")]
    private float positionX = 0.0f;
    [SerializeField]
    [Tooltip("模型的Y坐标")]
    private float positionY = 0.0f;
    [SerializeField]
    [Tooltip("模型的X轴偏移量")]
    private float modelOffsetX = 0.0f;
    [SerializeField]
    [Tooltip("模型的Y轴偏移量")]
    private float modelOffsetY = 0.0f;

    [SerializeField]
    [Tooltip("相机距离模型的距离")]
    private float cameraDistance = 3.0f;

    [SerializeField]
    [Tooltip("相机相对模型高度")]
    public float cameraHeightOffset = 0.0f;

    [SerializeField]
    [Tooltip("相机视野范围")]
    private int fieldOfView = 60;

    [SerializeField]
    [Tooltip("相机裁剪距离")]
    private int farClipPlane = 20;

    [SerializeField]
    [Tooltip("相机深度")]
    private float modelCameraDepth = 1;

    [SerializeField]
    [Tooltip("模型是否可以旋转")]
    private bool enableRotate = true;

    [SerializeField]
    private float camYaw = 90;

    private GameObject root;
    private Camera uiCamera;
    private Camera modelCamera;
    private RectTransform rectTransform;
    private Transform camModelRoot;
    private static Vector3 curPos = Vector3.zero;
    private Transform model;
    private int frameCount = 1;
    private bool isInEditor = false;

    public Transform Model
    {
        get { return model; }
        set
        {
            model = value;
            model.SetParent(camModelRoot);
            frameCount = 1;
        }
    }

    public float ModelCameraDepth
    {
        get { return ModelCameraDepth; }
        set
        {
            modelCameraDepth = value;
            modelCamera.depth = modelCameraDepth;
        }
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();
        uiCamera = GUIHelper.GetUICamera();
        rectTransform = transform as RectTransform;
        root = new GameObject("uguiModel");
        root.transform.position = curPos;
        curPos += new Vector3(200, 0, 0);

        modelCamera = new GameObject("modelCamera", typeof(Camera)).GetComponent<Camera>();
        modelCameraDepth = modelCamera.depth + 1.0f;
    }

    protected override void OnEnable()
    {
       
    }



    public void OnDrag(PointerEventData eventData)
    {
        if (enableRotate)
        {
            camYaw -= eventData.delta.x;
        }
    }

    public void OnClickModel()
    {

    }

    /// <summary>
    /// 设置模型的整体的Layer(包括子节点)
    /// </summary>
    /// <param name="modelTrans"></param>
    private void SetModelLayer(Transform modelTrans)
    {
        foreach (var trans in modelTrans.GetComponentsInChildren<Transform>())
        {
            trans.gameObject.layer = LayerMask.NameToLayer(UIModelLayerName);
        }
    }

    private void Update()
    {

    }

    protected override void OnDisable()
    {

    }


    protected override void OnDestroy()
    {

    }


}
