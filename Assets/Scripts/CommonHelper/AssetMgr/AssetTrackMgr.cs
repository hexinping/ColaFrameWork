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
        void SetCapcitySize(string group, int capcity);

        void SetDisposeInterval(string group, int disposeTimeInterval);

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
        #region CONST
        public const int DISPOSE_TIME_VALUE = 15;
        public const int DISPOSE_CHECK_INTERVAL = 5;
        public const int CAPCITY_SIZE = 15;

        private const int ILLEGAL_VALUE = -1;
        #endregion

        #region Params
        public Transform rootTF { get; private set; }
        public InstantiateAction instantiateAction { get; private set; }

        private Dictionary<string, AssetContainer> assetContainerMap = new Dictionary<string, AssetContainer>();
        private Dictionary<string, GameObjectContainer> gameObjectContainerMap = new Dictionary<string, GameObjectContainer>();

        private Dictionary<string, int> capcityValueMap = new Dictionary<string, int>();
        private Dictionary<string, int> disposeTimeMap = new Dictionary<string, int>();

        private int G_Capcity = ILLEGAL_VALUE;
        private int G_DisposeTime = ILLEGAL_VALUE;
        #endregion

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
            foreach (var container in assetContainerMap.Values)
            {
                container.Release();
            }
            foreach (var container in gameObjectContainerMap.Values)
            {
                container.Release();
            }
        }

        public void ReleaseGameObject(string path, GameObject gameObject)
        {
        }

        public void ReleaseAsset(string path, Object obj)
        {
        }

        public void SetCapcitySize(string group, int capcity)
        {
            if (string.IsNullOrEmpty(group))
            {
                G_Capcity = capcity > 0 ? capcity : ILLEGAL_VALUE;
            }
            else
            {
                if (capcity > 0)
                {
                    if (capcityValueMap.ContainsKey(group))
                    {
                        capcityValueMap[group] = capcity;
                    }
                    else
                    {
                        capcityValueMap.Add(group, capcity);
                    }
                }
                else
                {
                    capcityValueMap.Remove(group);
                }
            }

            foreach (var container in gameObjectContainerMap)
            {
                container.Value.Capcaity = CalcCapcitySize(container.Key);
            }
        }

        public void SetDisposeInterval(string group, int disposeTimeInterval)
        {
            if (string.IsNullOrEmpty(group))
            {
                G_DisposeTime = disposeTimeInterval > 0 ? disposeTimeInterval : ILLEGAL_VALUE;
            }
            else
            {
                if (disposeTimeInterval > 0)
                {
                    if (disposeTimeMap.ContainsKey(group))
                    {
                        disposeTimeMap[group] = disposeTimeInterval;
                    }
                    else
                    {
                        disposeTimeMap.Add(group, disposeTimeInterval);
                    }
                }
                else
                {
                    disposeTimeMap.Remove(group);
                }
            }

            foreach (var item in assetContainerMap)
            {
                item.Value.DisposeTime = CalcDisposeTime(item.Key);
            }
            foreach (var item in gameObjectContainerMap)
            {
                item.Value.DisposeTime = CalcDisposeTime(item.Key);
            }
        }

        #region private
        private int CalcCapcitySize(string assetPath)
        {
            if (!string.IsNullOrEmpty(assetPath))
            {
                foreach (var item in capcityValueMap)
                {
                    if (assetPath.StartsWith(item.Key))
                    {
                        return item.Value;
                    }
                }
            }
            if (G_Capcity > ILLEGAL_VALUE)
            {
                return G_Capcity;
            }
            return CAPCITY_SIZE;
        }

        private int CalcDisposeTime(string assetPath)
        {
            if (!string.IsNullOrEmpty(assetPath))
            {
                foreach (var item in disposeTimeMap)
                {
                    if (assetPath.StartsWith(item.Key))
                    {
                        return item.Value;
                    }
                }
            }
            if (G_DisposeTime > ILLEGAL_VALUE)
            {
                return G_DisposeTime;
            }
            return DISPOSE_TIME_VALUE;
        }
        #endregion
    }
}


