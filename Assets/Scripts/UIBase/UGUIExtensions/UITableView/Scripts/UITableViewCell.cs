//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI.Extensions
{
    public class UITableViewCell : MonoBehaviour
    {

        [HideInInspector]
        public int index;
        [HideInInspector]
        public RectTransform cacheTransform;
        [HideInInspector]
        public GameObject cacheGameObject;

        internal UITableView tableView;

        void Awake()
        {
            cacheTransform = transform as RectTransform;
            cacheGameObject = this.gameObject;
            Debug.Assert(cacheTransform != null, "transform should be RectTransform");
        }

        void OnDestroy()
        {
            index = -1;
            cacheTransform = null;
            cacheGameObject = null;
        }
    }
}