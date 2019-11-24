---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                 Modules的定义
---

local Modules = {}
local Util = {}
local Ctrl = {}
local Mod = {}

-- 注册全局变量
_G.Modules = Modules

Modules.moduleId = require("Game.Main.ModuleId")
Modules.notifyId = require("Game.Main.NotifyId")

-- 把要注册的工具都放在此列表里面
local UtilList ={
    "LuaCommon",
    "UI",
    "Table",
}

-- 优先启动的Module列表
local PriorityBootList ={

}

-- 正常启动的Module列表
local NornamlBootList ={

}


-- 注册工具类
local function RegisterUtility(name)
    local result,utl = pcall(require,string.format("Utilitys.%s_Utils",name))
    if result and utl then
        Util[name] = utl
        --执行Utility的initialize初始化方法
        if utl.initialize and "function" == type(utl.initialize) then
            utl.initialize()
        end
    end
end

-- 注册Module\注册Controller
local function InitModule(name)
    local result,controller = pcall(require,string.format("Modules.%s.%s_Controller",name,name))
    if result and controller then
        Ctrl[name] = controller
    end
    local result,module = pcall(require,string.format("Modules.%s.%s_Module",name,name))
    if result and module then
        Modules[name] = module
    end
end

local function RegisterGlobalVar()
    define("Util",Util)
    define("Ctrl",Ctrl)
    define("Mod",Mod)
end

function Modules.PriorityBoot()
    RegisterGlobalVar()

    for _,v in UtilList do
        RegisterUtility(v)
    end

    for _,v in PriorityBootList do
        InitModule(v)
    end

    for _,v in PriorityBootList do
        --执行Module的OnInit方法
        local controller = Ctrl[v]
        if controller and controller.OnInit and "function" == type(controller.OnInit) then
            controller.OnInit()
        end
        --执行Module的OnInit方法
        local module = Mod[v]
        if module and module.OnInit and "function" == type(module.OnInit) then
            module.OnInit()
        end
    end
end

function Modules.Boot()
    for _,v in NornamlBootList do
        InitModule(v)
    end

    for _,v in NornamlBootList do
        --执行Module的OnInit方法
        local controller = Ctrl[v]
        if controller and controller.OnInit and "function" == type(controller.OnInit) then
            controller.OnInit()
        end
        --执行Module的OnInit方法
        local module = Mod[v]
        if module and module.OnInit and "function" == type(module.OnInit) then
            module.OnInit()
        end
    end
end

function Modules.ShutDown()

end

return Modules