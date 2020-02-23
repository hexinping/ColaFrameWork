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
    public const bool SimulateMode = false;                       //调试/模式模式-用于在编辑器上模拟手机
    public const bool isLocalServer = true;

    /// <summary>
    /// 如果开启更新模式，前提必须启动框架自带服务器端。
    /// 否则就需要自己将StreamingAssets里面的所有内容
    /// 复制到自己的Webserver上面，并修改下面的WebUrl。
    /// </summary>
    public const bool UpdateMode = false;                       //更新模式-默认关闭 
    public const bool LuaByteMode = false;                       //Lua字节码模式-默认关闭 
    public const bool LuaBundleMode = false;                    //Lua代码AssetBundle模式

    public const int GameFrameRate = 30;                        //游戏帧频

    public const string AppName = "ColaFramework";               //应用程序名称
    public const string ExtName = ".cab";                   //AssetBundle的扩展名
    public const string WebUrl = "http://localhost:6688/";      //测试更新地址
}

