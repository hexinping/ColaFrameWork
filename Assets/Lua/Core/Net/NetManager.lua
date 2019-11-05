---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                Lua端Network管理器
---

local sproto = require "3rd/sproto/sproto"
local core = require "sproto.core"
local Protocol = require("Protocols.Protocol")

local NetManager = {}
local Socket = nil
local ByteBuffer = ByteBuffer
local luabuffer = ByteBuffer.New()
local sprotoCoder = nil
local SPROTO_BYTES_PATH = "SprotoBytes/sproto.bytes"

local OnConnectedCallback = nil

--- NetManager的初始化
function NetManager.Initialize()
    Socket = SocketManager.Instance
    local sprotoBytes = Common_Utils.LoadTextWithBytes(SPROTO_BYTES_PATH)
    sprotoCoder = sproto.new(sprotoBytes)

    Socket.OnConnected = NetManager.OnConnected
    Socket.OnReConnected = NetManager.OnReConnected
    Socket.OnClose = NetManager.OnClosed
    Socket.OnFailed = NetManager.OnFailed
    Socket.OnTimeOut = NetManager.OnTimeOut
    --NetMessageCenter.OnMessage = NetManager.OnMessage
    -- TODO:配置网络加密等
end

--- NetManager尝试连接服务器
function NetManager.Connect(ip,port,callback)
    print("-------->try to connect:",ip,port)
    OnConnectedCallback = callback
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
function NetManager.OnMessage(byteMsg)

end

--- 处理Socket成功连接服务器
function NetManager.OnConnected()
    if nil ~= OnConnectedCallback then
        OnConnectedCallback()
    end
end

--- 处理网络重连
function NetManager.OnReConnected()

end

--- 处理网络关闭
function NetManager.OnClosed()

end

--- 连接服务器失败
function NetManager.OnFailed()

end

--- 连接服务器超时
function NetManager.OnTimeOut()

end

function NetManager.Tick(deltaTime)
    print("------------>Update",deltaTime)
end

return NetManager