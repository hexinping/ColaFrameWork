---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                Lua端Network管理器
---

local sproto = require "3rd/sproto/sproto"
local core = require "sproto.core"

local NetManager = {}

local ByteBuffer = ByteBuffer
local luabuffer = ByteBuffer.New()

--- NetManager的初始化
function NetManager.Initialize()
    luabuffer:WriteInt(1)
    luabuffer:WriteString("abc")
end

--- NetManager尝试连接服务器
function NetManager.Connect(ip,port,callback)

end

--- 监听网络协议
function NetManager.Register(code,callback)

end

--- 取消监听网络协议
function NetManager.UnRegister(code)

end

--- 取消监听所有的网络协议
function NetManager.UnRegisterAll()

end

--- 关闭网络连接
function NetManager.Close(callback)

end

return NetManager