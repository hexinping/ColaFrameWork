---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                   登录界面
---

local UIBase = require("Core.ui.UIBase")
local UILogin = Class("UILogin",UIBase)

local _instance = nil

function UILogin.Instance()
    if nil == _instance then
        _instance = UILogin:new()
    end
    return _instance
end

-- virtual 子类可以初始化一些变量,ResId要在这里赋值
function UILogin:InitParam()
    self.ResId = 100
    self.uiDepthLayer = ECEnumType.UIDepth.NORMAL
    self:ShowUIMask(true)
end

-- override UI面板创建结束后调用，可以在这里获取gameObject和component等操作
function UILogin:OnCreate()

    -- ObjectPool Test
    --local LOjectPool = require("Common.Collections.LObjectPool")
    --
    --local index = 0
    --local createAction = function()
    --    local gameobj = UnityEngine.GameObject.New()
    --    gameobj.name = "CacheObj" .. index
    --    index = index + 1
    --    return gameobj
    --end
    --
    --local releaseAction = function(obj)
    --    print("------------>ObjType:",type(obj))
    --    print("---->释放Obj:",obj.name)
    --end
    --
    --local mObjectPool = LOjectPool:New(createAction,releaseAction)
    --
    --local obj_1 = mObjectPool:get()
    --print("-------->第一个物体的名字",obj_1.name)
    --
    --local obj_2 = mObjectPool:get()
    --print("-------->第二个物体的名字",obj_2.name)
    --
    --mObjectPool:release(obj_1)
    --
    --local obj_3 = mObjectPool:get()
    --print("-------------->type",type(obj_3))
    --print("-------->第三个物体的名字",obj_3.name)
end

-- 界面可见性变化的时候触发
function UILogin:OnShow(isShow)
end

function UILogin:onClick(name)
    if name == "cancelBtn" then
        self:DestroySelf()
    elseif name == "okBtn" then
        self:DestroySelf()
    end
end

-- 界面销毁的过程中触发
function UILogin:OnDestroy()
    UIBase.OnDestroy(self)
    UIManager.Instance():Open(ECEnumType.UIEnum.WorldDialog)
end

return UILogin