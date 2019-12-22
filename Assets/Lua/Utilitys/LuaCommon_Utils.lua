---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                 通用接口工具类
---

local LuaCommon_Utils = Class("LuaCommon_Utils")

LuaCommon_Utils._instance = nil

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

function LuaCommon_Utils.GetResourceById(id, type)
    local resConfig = ConfigMgr.Instance():GetItem("ResPathConfig", id)
    if resConfig and resConfig.path then
        return Common_Utils.Load(resConfig.path, type)
    else
        error("ResPathConfig表中未配置" .. id)
        return nil
    end
end

function LuaCommon_Utils.GetResourceByIdAsync(id, type, resLoadMode, callback)
    local resConfig = ConfigMgr.Instance():GetItem("ResPathConfig", id)
    if resConfig and resConfig.path then
        Common_Utils.LoadAsync(resConfig.path, type, callback)
    else
        error("ResPathConfig表中未配置" .. id)
    end
end

return LuaCommon_Utils