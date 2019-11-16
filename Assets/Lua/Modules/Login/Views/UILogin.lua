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

    -- 网络测试
    NetManager.Register(Protocol.C2S_Login,function (code,msg)
        self:OnNetTest(code,msg)
    end)
    NetManager.RequestSproto(Protocol.C2S_Login,{accountId = 1001,charId = 10086,userName = "Jackson"})
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

function UILogin:OnNetTest(code,msg)
    print("------------->接受到网络消息：",code,msg)
end

-- 界面销毁的过程中触发
function UILogin:OnDestroy()
    UIBase.OnDestroy(self)
    UIManager.Instance():Open(ECEnumType.UIEnum.WorldDialog)
end

return UILogin