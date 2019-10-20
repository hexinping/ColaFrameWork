//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITableViewCell : MonoBehaviour
{

    [HideInInspector]
    public int index;
    [HideInInspector]
    public RectTransform cacheTransform;
    [HideInInspector]
    public GameObject gameObject;
    [HideInInspector]
    public Dictionary<string, Component> extenParams;

    void Awake()
    {
        cacheTransform = transform as RectTransform;
        extenParams = new Dictionary<string, Component>();
        Debug.Assert(cacheTransform != null, "transform should be RectTransform");
    }

    void OnDestroy()
    {
        index = -1;
        cacheTransform = null;
        if (null != extenParams)
        {
            extenParams.Clear();
        }
        extenParams = null;
        gameObject = null;
    }


}
