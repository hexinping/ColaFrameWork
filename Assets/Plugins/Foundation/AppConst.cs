//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 定义一些框架内的设置
/// </summary>
public class AppConst
{
    public static bool SimulateMode = false;                       //调试/模式模式-用于在编辑器上模拟手机
    public static bool isLocalResServer = true;

    public const bool LuaByteMode = false;                       //Lua字节码模式-默认关闭 
    public static bool LuaBundleMode = true;                    //Lua代码AssetBundle模式

    public const string LuaBaseBundle = "lua/lua_base";         //包内的lua AssetBundle
    public const string LuaUpdateBundle = "lua/lua_update";     //热更下载的lua AseetBundle
    public const string LuaBundlePrefix = "lua/";               //lua AssetBundle的前缀
    public static List<string> LuaBundles = new List<string>() { LuaUpdateBundle,LuaBaseBundle };

    public static int GameFrameRate = 30;                        //游戏帧频

    public const string AppName = "ColaFramework";               //应用程序名称
    public const string ExtName = ".cab";                   //AssetBundle的扩展名
    public const string WebUrl = "http://localhost:6688/";      //测试更新地址
}

