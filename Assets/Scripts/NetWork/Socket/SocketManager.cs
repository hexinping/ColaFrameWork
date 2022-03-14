//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System;
using ColaFramework.Foundation;


namespace ColaFramework.NetWork
{
    /// <summary>
    /// Socket网络套接字管理器
    /// </summary>
    public class SocketManager : IDisposable
    {
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
        public bool IsConnceted { get { return isConnected; } }

        private static SocketManager _instance;
        private string currIP;
        private int currPort;
        private int timeOutMilliSec = 10;
        private float pingloopSec = 5.0f; //每5秒ping一次
        private long pingTimerId = -1;
        private byte[] pingBytes = System.Text.Encoding.UTF8.GetBytes(AppConst.AppName);

        private bool isConnected = false;

        private Socket clientSocket = null;
        private Thread receiveThread = null; //网络消息接收线程

        /// <summary>
        /// 网络数据缓存器, 内部也就是一个字节数组
        /// </summary>
        private DataBuffer databuffer = new DataBuffer();

        /// <summary>
        /// 数据接收缓冲区
        /// </summary>
        byte[] tmpReceiveBuff = new byte[4096];

        /// <summary>
        /// 网络数据结构
        /// </summary>
        private sSocketData socketData = new sSocketData();

        #region 对外回调
        public Action OnTimeOut; //连接服务器超时回调
        public Action OnFailed;  //连接服务器失败回调
        public Action OnConnected;//socket连接成功回调
        public Action OnReConnected; //重连回调
        public Action OnClose; //关闭socket回调
        public Action<int> OnErrorCode; //出错回调
        #endregion

        private SocketManager()
        {
        }

        #region 对外基本方法

        /// <summary>
        /// 向服务器发送消息
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="byteMsg">已经序列化过的字节数组</param>
        [LuaInterface.LuaByteBuffer]
        public void SendMsg(int protocol, byte[] byteMsg)
        {
            SendMsgBase(protocol, byteMsg);
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="currIP"></param>
        /// <param name="currPort"></param>
        public void Connect(string currIP, int currPort)
        {
            if (!IsConnceted)
            {
                this.currIP = currIP;
                this.currPort = currPort;
                //使用多线程
                ColaLoom.RunAsync(_onConnet);
            }
        }

        public void Close()
        {
            _close();
            OnClose?.Invoke();
        }

        /// <summary>
        /// 设置超时的阈值
        /// </summary>
        /// <param name="milliSec"></param>
        public void SetTimeOut(int milliSec)
        {
            timeOutMilliSec = milliSec;
        }
        #endregion

        /// <summary>
        /// 断开
        /// </summary>
        private void _close()
        {
            if (!isConnected)
                return;

            isConnected = false;
            //停止pingServer
            Timer.Cancel(pingTimerId);

            if (receiveThread != null)
            {
                //停止接收线程，
                receiveThread.Abort();
                receiveThread = null;
            }

            if (clientSocket != null && clientSocket.Connected)
            {
                //socket close
                clientSocket.Close();
                clientSocket = null;
            }
        }

        /// <summary>
        /// 重连机制，直接把socket关了
        /// </summary>
        private void _ReConnect()
        {
            _close();
            OnReConnected?.Invoke();
        }

        private void StartPingServer()
        {
            //启动pingServer
            pingTimerId = Timer.RunBySeconds(pingloopSec, PingServer, null);
        }

        /// <summary>
        /// 连接
        /// </summary>
        private void _onConnet()
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建套接字
                IPAddress ipAddress = IPAddress.Parse(currIP);//解析IP地址
                IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, currPort);
                IAsyncResult result = clientSocket.BeginConnect(ipEndpoint, new AsyncCallback(_onConnect_Sucess), clientSocket);//异步连接
                bool success = result.AsyncWaitHandle.WaitOne(timeOutMilliSec, true);
                if (!success) //超时
                {
                    _onConnect_Outtime();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
                OnErrorCode?.Invoke((int)NetErrorEnum.BeginConnectError);
                _onConnect_Fail();
            }
        }

        private void _onConnect_Sucess(IAsyncResult iar)
        {
            try
            {
                Socket client = (Socket)iar.AsyncState; //客户端socket实例
                client.EndConnect(iar); //结束连接

                receiveThread = new Thread(new ThreadStart(_onReceiveSocket)); //接收消息线程
                receiveThread.IsBackground = true; //后台线程
                receiveThread.Start();
                isConnected = true;
                Debug.Log("连接成功");
                //开启ping操作
                ColaLoom.QueueOnMainThread(StartPingServer);
                OnConnected?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                OnErrorCode?.Invoke((int)NetErrorEnum.ConnnectedError);
                _close();
            }
        }

        /// <summary>
        /// 连接服务器超时
        /// </summary>
        private void _onConnect_Outtime()
        {
            _close();
            OnTimeOut?.Invoke();
        }

        /// <summary>
        /// 连接服务器失败
        /// </summary>
        private void _onConnect_Fail()
        {
            _close();
            OnFailed?.Invoke();
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
                client.EndSend(asyncSend); //结束本次发送
            }
            catch (Exception e)
            {
                OnErrorCode?.Invoke((int)NetErrorEnum.SendMsgError);
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
                    isConnected = false;
                    _ReConnect();
                    break;
                }
                try
                {
                    int receiveLength = clientSocket.Receive(tmpReceiveBuff);
                    if (receiveLength > 0)
                    {
                        //receiveLength 接收数据长度（字节数总数）
                        databuffer.AddBuffer(tmpReceiveBuff, receiveLength);//将收到的数据添加到缓存器中，仅仅更新了游标位置
                        while (databuffer.GetData(out socketData))//取出一条完整数据
                        {
                            //只有消息协议才进入队列
                            if (Constants.PING_PROTO_CODE != socketData.protocal)
                            {
                                NetMessageData netMsgData = new NetMessageData();
                                netMsgData.protocol = socketData.protocal;
                                netMsgData.eventData = socketData.data;

                                //锁死消息中心消息队列，并添加数据
                                lock (NetMessageCenter.Instance.NetMessageQueue)
                                {
                                    //GameManage的update方法会遍历队列处理
                                    NetMessageCenter.Instance.NetMessageQueue.Enqueue(netMsgData);
                                }
                            }
                            else
                            {
                                //TODO:处理ping协议
                            }

                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.ToString());
                    OnErrorCode?.Invoke((int)NetErrorEnum.ReceiveError);
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
        /// <param name="data"></param>
        /// <returns></returns>
        private sSocketData BytesToSocketData(int protocol, byte[] data)
        {
            //自定义数据包结构，//消息：数据总长度(4byte) + 协议类型(4byte) + 数据(N byte)
            sSocketData tmpSocketData = new sSocketData();
            tmpSocketData.buffLength = Constants.HEAD_LEN + data.Length;
            tmpSocketData.protocal = protocol; //协议id
            tmpSocketData.dataLength = data.Length; //发送数据长度
            tmpSocketData.data = data; //发送数据
            return tmpSocketData;
        }

        /// <summary>
        /// 网络结构转数据
        /// </summary>
        /// <param name="tmpSocketData"></param>
        /// <returns></returns>
        private byte[] SocketDataToBytes(sSocketData tmpSocketData)
        {
            //把所有的数据都拷贝到一个字符数组里，这里其实可以用对象池优化下
            byte[] tmpBuff = new byte[tmpSocketData.buffLength]; //数据总长度（数据长度+协议id+字节数据）
            byte[] tmpBuffLength = BitConverter.GetBytes(tmpSocketData.buffLength); //把整形转成bytes
            byte[] tmpDataLenght = BitConverter.GetBytes(tmpSocketData.protocal); //把整形转成bytes

            Array.Copy(tmpBuffLength, 0, tmpBuff, 0, Constants.HEAD_DATA_LEN);//缓存总长度
            Array.Copy(tmpDataLenght, 0, tmpBuff, Constants.HEAD_DATA_LEN, Constants.HEAD_TYPE_LEN);//协议类型
            Array.Copy(tmpSocketData.data, 0, tmpBuff, Constants.HEAD_LEN, tmpSocketData.dataLength);//协议数据

            return tmpBuff;
        }

        /// <summary>
        /// 合并协议，数据
        /// </summary>
        /// <param name="_protocalType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte[] DataToBytes(int protocol, byte[] data)
        {
            return SocketDataToBytes(BytesToSocketData(protocol, data));
        }

        /// <summary>
        /// 发送消息基本方法
        /// </summary>
        /// <param name="_protocalType"></param>
        /// <param name="data"></param>
        private void SendMsgBase(int protocol, byte[] data)
        {
            if (clientSocket == null || !clientSocket.Connected)
            {
                _ReConnect();
                return;
            }

            byte[] msgdata = DataToBytes(protocol, data);
            //异步发送消息，发送成功的回调_onSendMsg
            clientSocket.BeginSend(msgdata, 0, msgdata.Length, SocketFlags.None, new AsyncCallback(_onSendMsg), clientSocket);
        }

        /// <summary>
        /// PingServer
        /// </summary>
        private void PingServer()
        {
            SendMsgBase(Constants.PING_PROTO_CODE, pingBytes);
            Debug.Log("PingServer");
        }

        public void Dispose()
        {
            _close();
            OnClose = null;
            OnConnected = null;
            OnFailed = null;
            OnTimeOut = null;
        }
    }
}
