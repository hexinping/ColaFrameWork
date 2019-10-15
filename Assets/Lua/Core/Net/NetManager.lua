---
--- ColaFramwork的Lua端Network管理器
---

local sproto = require "3rd/sproto/sproto"
local core = require "sproto.core"
local print_r = require "3rd/sproto/print_r"

local NetManager = {}

--- NetManager的初始化
function NetManager.Initialize()
    --测试sproto功能
    require("3rd.sproto.test")
end

--- NetManager尝试连接服务器
function NetManager.Connect(ip,port,callback)

end

--- 监听网络协议
function NetManager.Register(protoID,callback)

end

--- 取消监听网络协议
function NetManager.UnRegister(protoID)

end

--- 取消监听所有的网络协议
function NetManager.UnRegisterAll()

end

--- 关闭网络连接
function NetManager.Close(callback)

end

return NetManager