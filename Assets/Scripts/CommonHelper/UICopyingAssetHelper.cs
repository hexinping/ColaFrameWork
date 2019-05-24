using UnityEngine;
using UnityEngine.UI;

public class UICopyingAssetHelper : UIBase
{
    /// <summary>
    /// 拷贝资源进度框比较特殊，路径是固定的，并且不参与uiMgr的管理，不走资源管理器
    /// </summary>
    private static string resPath = "Arts/Gui/Prefabs/uicopying";
    private static UICopyingAssetHelper instance = null;
    private static RawImage background;
    private static Text progressText;
    private bool isCreate = false;

    private UICopyingAssetHelper() : base()
    {
        GameObject prefab = Resources.Load<GameObject>(resPath);
        if (null == prefab)
        {
            InitError();
        }
        GameObject uiGameobject = GameObject.Instantiate(prefab);
        if (null == uiGameobject)
        {
            InitError();
        }

        uiGameobject.name = prefab.name;
        uiGameobject.transform.SetParent(GUIHelper.GetUIRootObj().transform, false);
        uiGameobject.transform.localScale = prefab.transform.localScale;
        uiGameobject.transform.localPosition = prefab.transform.localPosition;
        uiGameobject.transform.localRotation = prefab.transform.localRotation;
        CreateWithGO(uiGameobject, UILevel.Common);
        sortEnable = false;
    }

    private void InitError()
    {
        Debug.LogError("拷贝解压资源预制加载失败！");
        Application.Quit();
    }

    /// <summary>
    /// 本UI设计单例，便于访问操作等
    /// </summary>
    /// <returns></returns>
    public static UICopyingAssetHelper Instance()
    {
        if (null == instance)
        {
            instance = new UICopyingAssetHelper();
        }
        return instance;
    }

    public override void Open()
    {
        //如果已经创建过一次了就不再二次创建
        if (isCreate) return;
        base.Open();
        isCreate = true;
    }

    public override void OnCreate()
    {
        base.OnCreate();
        background = Panel.GetComponentByPath<RawImage>("bg");
        progressText = Panel.GetComponentByPath<Text>("progress_text");
    }

    public void UpdateUI(int progress, int totalCount, string HintText)
    {
        //首次调用刷新的时候创建面板
        if (!isCreate)
        {
            Open();
        }
        progressText.text = string.Format("{0}:({1}/{2})", HintText, progress, totalCount);
    }


    public override void Destroy()
    {
        uiCreateType = UICreateType.Res;
        base.Destroy();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        instance = null;
        background = null;
        progressText = null;
        isCreate = false;
    }
}
