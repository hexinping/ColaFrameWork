//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.IO;
using System;
using UnityEngine;

namespace ColaFramework.NetWork
{

    //常量数据
    public class Constants
    {
        //消息：数据总长度(4byte) + 协议类型(4byte) + 数据(N byte)
        public static int HEAD_DATA_LEN = 4;
        public static int HEAD_TYPE_LEN = 4;
        public static int HEAD_LEN //8byte
        {
            get { return HEAD_DATA_LEN + HEAD_TYPE_LEN; }
        }

        public static readonly int PING_PROTO_CODE = 1; //ping protocol的id
    }

    public enum NetErrorEnum
    {
        BeginConnectError = 1,
        ConnnectedError = 2,
        SendMsgError = 3,
        ReceiveError = 4,
    }

    /// <summary>
    /// 网络数据结构
    /// </summary>
    [System.Serializable]
    public struct sSocketData
    {
        public byte[] data; //数据(N byte)
        public int protocal; //协议号
        public int buffLength; //数据包总长度：数据总长度(4byte) + 协议类型(4byte) + 数据(N byte)
        public int dataLength; //数据(N byte)长度
    }

    /// <summary>
    /// 网络数据缓存器，
    /// </summary>
    [System.Serializable]
    public class DataBuffer
    {//自动大小数据缓存器
        private int minBuffLen;
        private byte[] buff;
        private int curBuffPosition;
        private int buffLength = 0;
        private int dataLength;
        private int protocal;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="minBuffLen">最小缓冲区大小</param>
        public DataBuffer(int minBuffLen = 1024)
        {
            if (minBuffLen <= 0)
            {
                this.minBuffLen = 1024;
            }
            else
            {
                this.minBuffLen = minBuffLen;
            }
            buff = new byte[this.minBuffLen];
        }

        /// <summary>
        /// 添加缓存数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataLen"></param>
        public void AddBuffer(byte[] data, int dataLen)
        {
            if (dataLen > buff.Length - curBuffPosition)//超过当前缓存
            {
                byte[] tmpBuff = new byte[curBuffPosition + dataLen];
                Array.Copy(buff, 0, tmpBuff, 0, curBuffPosition);
                Array.Copy(data, 0, tmpBuff, curBuffPosition, dataLen);
                buff = tmpBuff;
                tmpBuff = null;
            }
            else
            {
                Array.Copy(data, 0, buff, curBuffPosition, dataLen);
            }
            curBuffPosition += dataLen;//修改当前数据标记
        }

        /// <summary>
        /// 更新数据长度
        /// </summary>
        public void UpdateDataLength()
        {
            //接收超过8个字节
            if (dataLength == 0 && curBuffPosition >= Constants.HEAD_LEN)
            {
                //获取数据总长度
                byte[] tmpDataLen = new byte[Constants.HEAD_DATA_LEN];
                Array.Copy(buff, 0, tmpDataLen, 0, Constants.HEAD_DATA_LEN);
                buffLength = BitConverter.ToInt32(tmpDataLen, 0); //把字节数组转成int型
                
                //获取协议号
                byte[] tmpProtocalType = new byte[Constants.HEAD_TYPE_LEN];
                Array.Copy(buff, Constants.HEAD_DATA_LEN, tmpProtocalType, 0, Constants.HEAD_TYPE_LEN);
                protocal = BitConverter.ToUInt16(tmpProtocalType, 0);
                
                //数据(N byte) 长度
                dataLength = buffLength - Constants.HEAD_LEN;
            }
        }

        /// <summary>
        /// 获取一条可用数据，返回值标记是否有数据
        /// </summary>
        /// <param name="tmpSocketData"></param>
        /// <returns></returns>
        public bool GetData(out sSocketData tmpSocketData)
        {
            tmpSocketData = new sSocketData();

            if (buffLength <= 0)
            {
                UpdateDataLength();
            }
            //curBuffPosition 会随着每次接收数据长度更新位置
            //curBuffPosition >= buffLength 意味着至少接收完一条完整数据包
            if (buffLength > 0 && curBuffPosition >= buffLength)
            {
                tmpSocketData.buffLength = buffLength; //数据包总长度：数据总长度(4byte) + 协议类型(4byte) + 数据(N byte)
                tmpSocketData.dataLength = dataLength; //数据(N byte)长度
                tmpSocketData.protocal = protocal;
                tmpSocketData.data = new byte[dataLength];
                //把数据字节拷贝到tmpSocketData.data
                Array.Copy(buff, Constants.HEAD_LEN, tmpSocketData.data, 0, dataLength);
                curBuffPosition -= buffLength; //移动游标
                
                //重新构建一个字节数组
                byte[] tmpBuff = new byte[curBuffPosition < minBuffLen ? minBuffLen : curBuffPosition];
                Array.Copy(buff, buffLength, tmpBuff, 0, curBuffPosition);
                buff = tmpBuff;

                //重置下状态
                buffLength = 0;
                dataLength = 0;
                protocal = 0;
                return true;
            }
            return false;
        }

    }
}