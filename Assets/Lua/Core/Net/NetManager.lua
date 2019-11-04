---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                Lua端Network管理器
---

local sproto = require "3rd/sproto/sproto"
local core = require "sproto.core"
local Protocol = require("Protocols.Protocol")

local NetManager = {}

local ByteBuffer = ByteBuffer
local luabuffer = ByteBuffer.New()
local sprotoCoder = nil
local SPROTO_BYTES_PATH = "SprotoBytes/sproto.bytes"

--- NetManager的初始化
function NetManager.Initialize()
    local sprotoBytes = Common_Utils.LoadTextWithBytes(SPROTO_BYTES_PATH)
    sprotoCoder = sproto.new(sprotoBytes)

    --测试bytes
    local person = sprotoCoder:default "Person"
    print(person)
    -- TODO:配置网络加密等
end

--- NetManager尝试连接服务器
function NetManager.Connect(ip,port,callback)
    print("-------->try to connect:",ip,port)
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

--- 分帧处理网络消息
function NetManager.Update()

end

--- 处理C#端传到Lua端的消息
function NetManager.OnMessage()

end

return NetManager