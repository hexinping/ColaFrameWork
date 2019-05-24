---
--- Module类的管理器
---

local ModuleManager = Class("ModuleManager")

ModuleManager._instance = nil

function ModuleManager:Instance()
    if nil == ModuleManager._instance then
        ModuleManager._instance = ModuleManager:new()
    end
    return ModuleManager
end

-- override 初始化各种数据
function ModuleManager:initialize()
end

return ModuleManager