/// <summary>
/// 网络消息处理中心
/// 缓存消息，然后分帧泵到lua端进行处理
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public struct sEvent_NetMessageData
{
    public eProtocalCommand _eventType;
    public byte[] _eventData;
}

public class NetMessageCenter : SingletonMonoBehaviour<NetMessageCenter>, IManager
{
    [LuaInterface.NoToLua]
    public Queue<sEvent_NetMessageData> _netMessageDataQueue;

    [LuaInterface.LuaByteBuffer]
    public Action<byte[]> OnMessage;

    /// <summary>
    /// 每帧默认处理2个协议
    /// </summary>
    private int perHandleCnt = 3;

    public float TimeSinceUpdate { get; set; }

    void IManager.Init()
    {
        _netMessageDataQueue = new Queue<sEvent_NetMessageData>();
    }

    public void SetPerFrameHandleCnt(int value)
    {
        perHandleCnt = value;
    }

    [LuaInterface.NoToLua]
    public void Update(float deltaTime)
    {
        int handledCnt = 0;
        while (_netMessageDataQueue.Count > 0)
        {
            lock (_netMessageDataQueue)
            {
                sEvent_NetMessageData tmpNetMessageData = _netMessageDataQueue.Dequeue();
                handledCnt++;
                try
                {
                    if (null != OnMessage)
                    {
                        OnMessage(tmpNetMessageData._eventData);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("try to handle message error!");
                }
                if (handledCnt >= perHandleCnt)
                {
                    break;
                }
            }
        }
    }

    public void Dispose()
    {
        if (null != _netMessageDataQueue)
        {
            _netMessageDataQueue.Clear();
        }
        _netMessageDataQueue = null;
        OnMessage = null;
    }
}