---
--- UIModelPanel UI类
---

local UIBase = require("Core.ui.UIBase")
local UIModelPanel = Class("UIModelPanel", UIBase)

local _instance = nil

local SettingNames = {
    "Merchant_female",
    "Boy",
    "Girl",
}

local ResPath = {
    "Arts/Avatar/Merchant_female.prefab",
    "Arts/Avatar/Boy.prefab",
    "Arts/Avatar/Girl.prefab",
}

-- 获取UI实例的接口
function UIModelPanel.Instance()
    if nil == _instance then
        _instance = UIModelPanel:new()
    end
    return _instance
end

-- virtual 子类可以初始化一些变量,ResId要在这里赋值
function UIModelPanel:InitParam()
    self.ResId = 104
end

-- override UI面板创建结束后调用，可以在这里获取gameObject和component等操作
function UIModelPanel:OnCreate()
    self.uiModel = self.Panel:GetComponentByPath("UIModel", "UGUIModel")
    self.uiModel.onModelClick = function(name)
        self:OnModelClick(name)
    end
end

-- 界面可见性变化的时候触发
function UIModelPanel:OnShow(isShow)

end

-- 界面销毁的过程中触发
function UIModelPanel:OnDestroy()
    UIBase.OnDestroy(self)
end

-- 注册UI事件监听
function UIModelPanel:RegisterEvent()

end

-- 取消注册UI事件监听
function UIModelPanel:UnRegisterEvent()

end

------------------- UI事件回调 --------------------------
function UIModelPanel:onClick(name)
    if name == "Btn_One" then
        self:UpdateModel(1)
    elseif name == "Btn_Two" then
        self:UpdateModel(2)
    elseif name == "Btn_Three" then
        self:UpdateModel(3)
    elseif name == "Btn_Close" then
        self:DestroySelf()
    elseif name == "Btn_Switch" then
        Common_Utils.GetSceneMgr():UnLoadLevelAsync("xinshoucun",nil)
    end
end

function UIModelPanel:onBoolValueChange(name, isSelect)

end

---------------------- UI事件回调 --------------------------

function UIModelPanel:OnModelClick(name)
    print("----------->点击了", name)
end

function UIModelPanel:UpdateModel(index)
    local isModelExist = self.uiModel:IsModelExist(index)
    if not isModelExist then
        local character = SceneCharacter.CreateSceneCharacterInf(ResPath[index] or "")
        self.uiModel:SetModelAt(index,character)
    end
    self.uiModel:UpdateModelShownIndex(index)
    self.uiModel:ImportSetting(SettingNames[index] or "")
end

return UIModelPanel