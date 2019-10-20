//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ColaFramework 编辑器助手类
/// </summary>
public class ColaEditHelper
{

    /// <summary>
    /// 打开指定文件夹(编辑器模式下)
    /// </summary>
    /// <param name="path"></param>
    public static void OpenDirectory(string path)
    {
        if (string.IsNullOrEmpty(path)) return;

        path = path.Replace("/", "\\");
        if (!Directory.Exists(path))
        {
            Debug.LogError("No Directory: " + path);
            return;
        }

        System.Diagnostics.Process.Start("explorer.exe", path);
    }

    /// <summary>
    /// 合并Lua代码，并复制到StreamingAsset目录中准备打包
    /// </summary>
    public static void BuildLuaToStreamingAsset()
    {
        RmDir(LuaConst.streamingAssetLua);
        CopyDir(LuaConst.toluaDir, LuaConst.streamingAssetLua);
        CopyDir(LuaConst.luaDir, LuaConst.streamingAssetLua);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 创建目录
    /// </summary>
    /// <param name="path"></param> 路径
    /// <param name="isOverride"></param> 是否覆盖原有同名目录
    public static void Mkdir(string path, bool isOverride = false)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        else if(Directory.Exists(path) && isOverride)
        {
            Directory.Delete(path, true);
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// 删除目录
    /// </summary>
    /// <param name="path"></param>
    public static void RmDir(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    public static void CopyDir(string srcPath, string destPath)
    {
        if(string.IsNullOrEmpty(srcPath) || string.IsNullOrEmpty(destPath))
        {
            return;
        }
        Mkdir(destPath);
        DirectoryInfo sDir = new DirectoryInfo(srcPath);
        FileInfo[] fileArray = sDir.GetFiles();
        foreach (FileInfo file in fileArray)
        {
            if (file.Extension != ".meta")
                file.CopyTo(destPath + "/" + file.Name, true);
        }
        //递归复制子文件夹
        DirectoryInfo[] subDirArray = sDir.GetDirectories();
        foreach (DirectoryInfo subDir in subDirArray)
        {
            if(subDir.Name != ".idea")
            {
                CopyDir(subDir.FullName, destPath + "/" + subDir.Name);
            }
        }
    }

    /// <summary>
    /// 清空目录下内容
    /// </summary>
    /// <param name="path"></param>
    public static void ClearDir(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        DirectoryInfo sDir = new DirectoryInfo(path);
        FileInfo[] fileArray = sDir.GetFiles();
        foreach (FileInfo file in fileArray)
        {
            file.Delete();
        }
        DirectoryInfo[] subDirArray = sDir.GetDirectories();
        foreach (DirectoryInfo subDir in subDirArray)
        {
            subDir.Delete(true);
        }
    }
}

#endif
