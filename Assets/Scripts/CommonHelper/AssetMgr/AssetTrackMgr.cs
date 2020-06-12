//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ColaFramework.Foundation
{
    public delegate Object InstantiateAction(Object obj);

    public interface IAssetTrackMgr
    {
        void SetCapcity(string group, int capcity);

        void SetDisposeInterval(string group, int fadeout);

        void Release();

        GameObject GetGameObject(string path, Transform parent);

        void ReleaseGameObject(string path, GameObject gameObject);

        void DiscardGameObject(string path, GameObject gameObject);

        T GetAsset<T>(string path) where T : Object;

        Object GetAsset(string path, Type type);

        void ReleaseAsset(string path, Object obj);
    }

    public class AssetTrackMgr : IAssetTrackMgr
    {
        public Transform rootTF { get; private set; }
        public InstantiateAction instantiateAction { get; private set; }

        public AssetTrackMgr(Transform rootTransform = null, InstantiateAction action = null)
        {
            rootTF = null == rootTransform ? new GameObject("AssetTrackRoot").transform : rootTransform;
            instantiateAction = null == action ? GameObject.Instantiate : action;
            GameObject.DontDestroyOnLoad(rootTF.gameObject);

        }

        public void DiscardGameObject(string path, GameObject gameObject)
        {
        }

        public T GetAsset<T>(string path) where T : Object
        {
            return null;
        }

        public Object GetAsset(string path, Type type)
        {
            return null;
        }

        public GameObject GetGameObject(string path, Transform parent)
        {
            return null;
        }

        public void Release()
        {
        }

        public void ReleaseGameObject(string path, GameObject gameObject)
        {
        }

        public void ReleaseAsset(string path, Object obj)
        {
        }

        public void SetCapcity(string group, int capcity)
        {
        }

        public void SetDisposeInterval(string group, int fadeout)
        {
        }
    }
}


