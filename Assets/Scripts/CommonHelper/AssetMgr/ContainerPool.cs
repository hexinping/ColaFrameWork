//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColaFramework.Foundation
{
    public class ContainerPool
    {
        private ObjectPool<AssetContainer> assetContainerPool = new ObjectPool<AssetContainer>(CreateAssetContainer, null, AssetContainer.Dispose);
        private ObjectPool<GameObjectContainer> gameobjectContainerPool = new ObjectPool<GameObjectContainer>(CreateGameObjectContainer, null, GameObjectContainer.Dispose);

        #region get && release
        public AssetContainer GetAssetContainer(int disposeTime)
        {
            var container = assetContainerPool.Get();
            return AssetContainer.Process(container, disposeTime);
        }

        public void ReleaseAssetContainer(AssetContainer container)
        {
            assetContainerPool.Release(container);
        }

        public GameObjectContainer GetGameObjectContainer(AssetTrackMgr assetTrackMgr, string path, GameObject prefab, int disposeTime, int capcity)
        {
            var container = gameobjectContainerPool.Get();
            return GameObjectContainer.Process(assetTrackMgr, container, path, prefab, disposeTime, capcity);
        }

        public void ReleaseGameObjectContainer(GameObjectContainer container)
        {
            gameobjectContainerPool.Release(container);
        }
        #endregion

        #region CreateContainer
        private static AssetContainer CreateAssetContainer()
        {
            return new AssetContainer();
        }

        private static GameObjectContainer CreateGameObjectContainer()
        {
            return new GameObjectContainer();
        }
        #endregion
    }

    public class AssetContainer
    {
        private Object hardRef;
        private System.WeakReference weakRef;
        private float disposeTimeTicker;

        public int DisposeTime { get; set; }

        public static AssetContainer Process(AssetContainer container, int disposeTime)
        {
            container.DisposeTime = disposeTime;
            return container;
        }

        public bool IsAlive(float dt)
        {
            if (null != hardRef)
            {
                if (disposeTimeTicker > 0)
                {
                    disposeTimeTicker -= dt;
                }
                else
                {
                    if (null == weakRef)
                    {
                        weakRef = new System.WeakReference(hardRef);
                    }
                    else
                    {
                        weakRef.Target = hardRef;
                    }
                    hardRef = null;
                }
                return true;
            }
            else
            {
                return null != weakRef && null != weakRef.Target;
            }
        }

        public Object GetObject()
        {
            Object obj = null;
            if (null != hardRef)
            {
                obj = hardRef;
            }
            else
            {
                if (null != weakRef)
                {
                    hardRef = weakRef.Target as Object;
                    weakRef.Target = null;
                    obj = hardRef;
                }
                else
                {
                    Debug.LogWarning("the asset has been dispose but not be removed from pool");
                }
            }
            disposeTimeTicker = DisposeTime;
            return obj;
        }

        public void ReturnObject(Object target)
        {

        }

        public static void Dispose(AssetContainer container)
        {

        }
    }

    public class GameObjectContainer
    {
        public static GameObjectContainer Process(AssetTrackMgr assetTrackMgr, GameObjectContainer container, string path, GameObject prefab, int disposeTime, int capcity)
        {
            return container;
        }

        public static void Dispose(GameObjectContainer container)
        {

        }
    }
}


