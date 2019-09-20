using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System;
using System.Text;
using System.IO;
using ProtoBuf;

/// <summary>
/// Socket网络套接字管理器
/// </summary>
public class SocketManager : IDisposable
{
    private static SocketManager _instance;
    public static SocketManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SocketManager();
            }
            return _instance;
        }
    }
    private string _currIP;
    private int _currPort;
    private int _timeOutMilliSec = 5000;

    private bool _isConnected = false;
    public bool IsConnceted { get { return _isConnected; } }
    private Socket clientSocket = null;
    private Thread receiveThread = null;

    /// <summary>
    /// 网络数据缓存器
    /// </summary>
    private DataBuffer _databuffer = new DataBuffer();

    /// <summary>
    /// 数据接收缓冲区
    /// </summary>
    byte[] _tmpReceiveBuff = new byte[4096];

    /// <summary>
    /// 网络数据结构
    /// </summary>
    private sSocketData _socketData = new sSocketData();

    #region 对外回调
    public Action OnTimeOut;
    public Action OnFailed;
    public Action OnConnected;
    public Action OnClose;
    #endregion

    #region 对外基本方法
    /// <summary>
    /// 以二进制方式发送
    /// </summary>
    /// <param name="_protocalType"></param>
    /// <param name="_byteStreamBuff"></param>
    public void SendMsg(eProtocalCommand _protocalType, ByteStreamBuff _byteStreamBuff)
    {
        SendMsgBase(_protocalType, _byteStreamBuff.ToArray());
    }

    /// <summary>
    /// 以ProtoBuf方式发送
    /// </summary>
    /// <param name="_protocalType"></param>
    /// <param name="data"></param>
    public void SendMsg(eProtocalCommand _protocalType, ProtoBuf.IExtensible data)
    {
        SendMsgBase(_protocalType, ProtoBuf_Serializer(data));
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    /// <param name="_currIP"></param>
    /// <param name="_currPort"></param>
    public void Connect(string _currIP, int _currPort)
    {
        if (!IsConnceted)
        {
            this._currIP = _currIP;
            this._currPort = _currPort;
            _onConnet();
        }
    }

    public void Close()
    {
        _close();
    }

    /// <summary>
    /// 设置超时的阈值
    /// </summary>
    /// <param name="milliSec"></param>
    public void SetTimeOut(int milliSec)
    {
        _timeOutMilliSec = milliSec;
    }
    #endregion

    /// <summary>
    /// 断开
    /// </summary>
    private void _close()
    {
        if (!_isConnected)
            return;

        _isConnected = false;

        if (receiveThread != null)
        {
            receiveThread.Abort();
            receiveThread = null;
        }

        if (clientSocket != null && clientSocket.Connected)
        {
            clientSocket.Close();
            clientSocket = null;
        }
        if(null != OnClose)
        {
            OnClose();
        }
    }

    /// <summary>
    /// 重连机制
    /// </summary>
    private void _ReConnect()
    {
    }

    /// <summary>
    /// 连接
    /// </summary>
    private void _onConnet()
    {
        try
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建套接字
            IPAddress ipAddress = IPAddress.Parse(_currIP);//解析IP地址
            IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, _currPort);
            IAsyncResult result = clientSocket.BeginConnect(ipEndpoint, new AsyncCallback(_onConnect_Sucess), clientSocket);//异步连接
            bool success = result.AsyncWaitHandle.WaitOne(_timeOutMilliSec, true);
            if (!success) //超时
            {
                _onConnect_Outtime();
            }
        }
        catch (System.Exception _e)
        {
            _onConnect_Fail();
        }
    }

    private void _onConnect_Sucess(IAsyncResult iar)
    {
        try
        {
            Socket client = (Socket)iar.AsyncState;
            client.EndConnect(iar);

            receiveThread = new Thread(new ThreadStart(_onReceiveSocket));
            receiveThread.IsBackground = true;
            receiveThread.Start();
            _isConnected = true;
            Debug.Log("连接成功");
            if(null != OnConnected)
            {
                OnConnected();
            }
        }
        catch (Exception _e)
        {
            Close();
        }
    }

    /// <summary>
    /// 连接服务器超时
    /// </summary>
    private void _onConnect_Outtime()
    {
        if(null != OnTimeOut)
        {
            OnTimeOut();
        }
        _close();
    }

    /// <summary>
    /// 连接服务器失败
    /// </summary>
    private void _onConnect_Fail()
    {
        if(null != OnFailed)
        {
            OnFailed();
        }
        _close();
    }

    /// <summary>
    /// 发送消息结果回调，可判断当前网络状态
    /// </summary>
    /// <param name="asyncSend"></param>
    private void _onSendMsg(IAsyncResult asyncSend)
    {
        try
        {
            Socket client = (Socket)asyncSend.AsyncState;
            client.EndSend(asyncSend);
        }
        catch (Exception e)
        {
            Debug.Log("send msg exception:" + e.StackTrace);
        }
    }

    /// <summary>
    /// 接收网络数据
    /// </summary>
    private void _onReceiveSocket()
    {
        while (true)
        {
            if (!clientSocket.Connected)
            {
                _isConnected = false;
                _ReConnect();
                break;
            }
            try
            {
                int receiveLength = clientSocket.Receive(_tmpReceiveBuff);
                if (receiveLength > 0)
                {
                    _databuffer.AddBuffer(_tmpReceiveBuff, receiveLength);//将收到的数据添加到缓存器中
                    while (_databuffer.GetData(out _socketData))//取出一条完整数据
                    {
                        sEvent_NetMessageData tmpNetMessageData = new sEvent_NetMessageData();
                        tmpNetMessageData._eventType = _socketData._protocallType;
                        tmpNetMessageData._eventData = _socketData._data;

                        //锁死消息中心消息队列，并添加数据
                        lock (MessageCenter.Instance._netMessageDataQueue)
                        {
                            Debug.Log(tmpNetMessageData._eventType);
                            MessageCenter.Instance._netMessageDataQueue.Enqueue(tmpNetMessageData);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                clientSocket.Disconnect(true);
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                break;
            }
        }
    }





    /// <summary>
    /// 数据转网络结构
    /// </summary>
    /// <param name="_protocalType"></param>
    /// <param name="_data"></param>
    /// <returns></returns>
    private sSocketData BytesToSocketData(eProtocalCommand _protocalType, byte[] _data)
    {
        sSocketData tmpSocketData = new sSocketData();
        tmpSocketData._buffLength = Constants.HEAD_LEN + _data.Length;
        tmpSocketData._dataLength = _data.Length;
        tmpSocketData._protocallType = _protocalType;
        tmpSocketData._data = _data;
        return tmpSocketData;
    }

    /// <summary>
    /// 网络结构转数据
    /// </summary>
    /// <param name="tmpSocketData"></param>
    /// <returns></returns>
    private byte[] SocketDataToBytes(sSocketData tmpSocketData)
    {
        byte[] _tmpBuff = new byte[tmpSocketData._buffLength];
        byte[] _tmpBuffLength = BitConverter.GetBytes(tmpSocketData._buffLength);
        byte[] _tmpDataLenght = BitConverter.GetBytes((UInt16)tmpSocketData._protocallType);

        Array.Copy(_tmpBuffLength, 0, _tmpBuff, 0, Constants.HEAD_DATA_LEN);//缓存总长度
        Array.Copy(_tmpDataLenght, 0, _tmpBuff, Constants.HEAD_DATA_LEN, Constants.HEAD_TYPE_LEN);//协议类型
        Array.Copy(tmpSocketData._data, 0, _tmpBuff, Constants.HEAD_LEN, tmpSocketData._dataLength);//协议数据

        return _tmpBuff;
    }

    /// <summary>
    /// 合并协议，数据
    /// </summary>
    /// <param name="_protocalType"></param>
    /// <param name="_data"></param>
    /// <returns></returns>
    private byte[] DataToBytes(eProtocalCommand _protocalType, byte[] _data)
    {
        return SocketDataToBytes(BytesToSocketData(_protocalType, _data));
    }


    /// <summary>
    /// ProtoBuf序列化
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] ProtoBuf_Serializer(ProtoBuf.IExtensible data)
    {
        using (MemoryStream m = new MemoryStream())
        {
            byte[] buffer = null;
            Serializer.Serialize(m, data);
            m.Position = 0;
            int length = (int)m.Length;
            buffer = new byte[length];
            m.Read(buffer, 0, length);
            return buffer;
        }
    }

    /// <summary>
    /// ProtoBuf反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_data"></param>
    /// <returns></returns>
    public static T ProtoBuf_Deserialize<T>(byte[] _data)
    {
        using (MemoryStream m = new MemoryStream(_data))
        {
            return Serializer.Deserialize<T>(m);
        }
    }

    /// <summary>
    /// 发送消息基本方法
    /// </summary>
    /// <param name="_protocalType"></param>
    /// <param name="_data"></param>
    private void SendMsgBase(eProtocalCommand _protocalType, byte[] _data)
    {
        if (clientSocket == null || !clientSocket.Connected)
        {
            _ReConnect();
            return;
        }

        byte[] _msgdata = DataToBytes(_protocalType, _data);
        clientSocket.BeginSend(_msgdata, 0, _msgdata.Length, SocketFlags.None, new AsyncCallback(_onSendMsg), clientSocket);
    }

    public void Dispose()
    {

    }
}