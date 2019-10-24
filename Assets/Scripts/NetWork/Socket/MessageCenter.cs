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

public class MessageCenter : SingletonMonoBehaviour<MessageCenter>
{
    public Queue<sEvent_NetMessageData> _netMessageDataQueue = new Queue<sEvent_NetMessageData>();


    void Update()
    {
        while (_netMessageDataQueue.Count > 0)
        {
            lock (_netMessageDataQueue)
            {
                sEvent_NetMessageData tmpNetMessageData = _netMessageDataQueue.Dequeue();               
            }
        }
    }
}