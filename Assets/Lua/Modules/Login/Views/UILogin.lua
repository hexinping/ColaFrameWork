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

    -- stack test
    local LStack = require("Common.Collections.LStack")
    local mStack = LStack:New()
    local yStack = LStack:New()
    mStack:push(1)
    mStack:push(2)
    mStack:push(3)

    yStack:push(4)
    yStack:push(5)
    yStack:push(6)

    print("------>长度：",mStack:count())
    print("------>长度：",yStack:count())

    while not mStack:isEmpty() do
        print(mStack:pop())
    end
    print("------>长度：",mStack:count())
    print("------>长度：",yStack:count())

    while not yStack:isEmpty() do
        print(yStack:pop())
    end
    print("------>长度：",mStack:count())
    print("------>长度：",yStack:count())
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