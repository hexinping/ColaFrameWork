/// <summary>
/// 网络消息处理中心
/// 
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

public class NetMessageCenter : SingletonMonoBehaviour<NetMessageCenter>,IManager
{
    public Queue<sEvent_NetMessageData> _netMessageDataQueue;

    /// <summary>
    /// 每帧处理3个协议
    /// </summary>
    private int perHandleCnt = 3; 

    public float TimeSinceUpdate { get; set; }

    void IManager.Init()
    {
        _netMessageDataQueue = new Queue<sEvent_NetMessageData>();
    }

    public void Update(float deltaTime)
    {
        int handledCnt = 0;
        while (_netMessageDataQueue.Count > 0)
        {
            lock (_netMessageDataQueue)
            {
                sEvent_NetMessageData tmpNetMessageData = _netMessageDataQueue.Dequeue();
                //TODO:Handle Msg
                handledCnt++;
                if(handledCnt >=3)
                {
                    break;
                }
            }
        }
    }

    public void Dispose()
    {
        if(null != _netMessageDataQueue)
        {
            _netMessageDataQueue.Clear();
        }
        _netMessageDataQueue = null;
    }
}