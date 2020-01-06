//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

//#define SIMULATE_MODE
using System;
using System.Collections;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace ColaFramework
{
    /// <summary>
    /// 资源加载的对外接口，封装平台和细节，可对Lua导出
    /// </summary>
    public static class AssetLoader
    {
        /// <summary>
        /// 根据类型和路径返回相应的资源(同步方法)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T Load<T>(string path) where T : Object
        {
            return Load(path, typeof(T)) as T;
        }

        /// <summary>
        /// 根据类型和路径返回相应的资源(同步方法)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Object Load(string path, Type type)
        {
#if UNITY_EDITOR && !SIMULATE_MODE
            path = GloablDefine.GameAssetBasePath + path;
            if (Path.HasExtension(path))
            {
                return AssetDatabase.LoadAssetAtPath(path, type);
            }
            else
            {
                Debug.LogWarning("资源加载的路径不合法!");
                return null;
            }
#else
        return ResourcesMgr.GetInstance().Load(path, type);
#endif
        }

        /// <summary>
        /// 根据类型和路径返回相应的资源(异步方法)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        public static void LoadAsync<T>(string path, Action<Object, string> callback) where T : Object
        {
            LoadAsync(path, typeof(T), callback);
        }

        /// <summary>
        /// 根据类型和路径返回相应的资源(异步方法)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="t"></param>
        public static void LoadAsync(string path, Type type, Action<Object, string> callback)
        {
#if UNITY_EDITOR && !SIMULATE_MODE
            //模拟异步
            var asset = Load(path, type);
            callback(asset, path);
#else
        ResourcesMgr.GetInstance().LoadAsync(path, type, callback);
#endif
        }

        /// <summary>
        /// 以字符串的形式加载文本
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string LoadTextWithString(string path)
        {
            TextAsset textAsset = Load<TextAsset>(path);
            if (null != textAsset)
            {
                return textAsset.text;
            }
            return null;
        }

        public static byte[] LoadTextWithBytes(string path)
        {
            TextAsset textAsset = Load<TextAsset>(path);
            if (null != textAsset)
            {
                return textAsset.bytes;
            }
            return null;
        }

#if UNITY_EDITOR
        [LuaInterface.NoToLua]
        public static void LoadAllAssetsAtPath(string path, out Object[] objects)
        {
            objects = AssetDatabase.LoadAllAssetsAtPath(path);
        }
#endif
    }
}