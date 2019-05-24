---
--- UIBase基类
---

local UIBase = Class("UIBase")

-- override 初始化各种数据
function UIBase:initialize()
    self.Panel = nil
    self.ResId = 0
    self.Layer = 0
    self.UILevel = 0
    self.subUIList = {}
    self.uiDepthLayer = 0
    self.uiCanvas = nil
end

return UIBase