---
---  通用接口工具类
---

local LuaCommon_Utils = Class("LuaCommon_Utils")

LuaCommon_Utils._instance = nil
-- 可读写的路径
LuaCommon_Utils.AssetPath = ""
-- lua脚本的根目录
LuaCommon_Utils.LuaDir = UnityEngine.Application.dataPath .. "/Lua"

-- override 初始化各种数据
function LuaCommon_Utils.initialize()

end

function LuaCommon_Utils.InstantiateGoById(id, parent)
    local resConfig = ConfigMgr.Instance():GetItem("ResPathConfig", id)
    if resConfig and resConfig.path then
        return Common_Utils.InstantiateGoByPath(resConfig.path, parent)
    else
        error("ResPathConfig表中未配置" .. id)
        return nil
    end
end

function LuaCommon_Utils.InstantiateGoByPathAsync(id, parent, callback)
    local resConfig = ConfigMgr.Instance():GetItem("ResPathConfig", id)
    if resConfig and resConfig.path then
        Common_Utils.InstantiateGoByPathAsync(resConfig.path, parent, callback)
    else
        error("ResPathConfig表中未配置" .. id)
        return
    end
end

function LuaCommon_Utils.GetResourceByPath(path, type)
    return AssetLoader.Load(path, type)
end

function LuaCommon_Utils.GetResourceByPathAsync(path, type, callback)
    return AssetLoader.LoadAsync(path, type, callback)
end

function LuaCommon_Utils.GetResourceById(id, type)
    local resConfig = ConfigMgr.Instance():GetItem("ResPathConfig", id)
    if resConfig and resConfig.path then
        return AssetLoader.Load(resConfig.path, type)
    else
        error("ResPathConfig表中未配置" .. id)
        return nil
    end
end

function LuaCommon_Utils.GetResourceByIdAsync(id, type, resLoadMode, callback)
    local resConfig = ConfigMgr.Instance():GetItem("ResPathConfig", id)
    if resConfig and resConfig.path then
        AssetLoader.LoadAsync(resConfig.path, type, callback)
    else
        error("ResPathConfig表中未配置" .. id)
    end
end

return LuaCommon_Utils