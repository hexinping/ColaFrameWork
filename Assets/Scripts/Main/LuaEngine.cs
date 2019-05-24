using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using UnityEngine;

/// <summary>
/// 继承自LuaClient 相当于Lua管理器
/// </summary>
public class LuaEngine : LuaClient {

    protected override LuaFileUtils InitLoader()
    {
        return base.InitLoader();
    }

    protected override void LoadLuaFiles()
    {
#if UNITY_EDITOR
        luaState.AddSearchPath(Application.dataPath + "/lua");
#endif
        base.LoadLuaFiles();
    }
}
