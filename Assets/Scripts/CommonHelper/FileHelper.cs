//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace ColaFramework
{
    /// <summary>
    /// 文件\目录操作助手类
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// UTF8编码格式
        /// </summary>
        private static readonly UTF8Encoding UTF8EnCode = new UTF8Encoding(false);

        /// <summary>
        /// 创建一个目录
        /// </summary>
        /// <param name="path"></param> 路径
        /// <param name="isContainFileName"></param> 路径中是否包含文件名
        public static void MkDir(string path, bool isContainFileName = true)
        {
            if (isContainFileName)
            {
                path = Path.GetDirectoryName(path);
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="orginPath"></param>
        /// <param name="destPath"></param>
        /// <param name="isOverwrite"></param>
        public static void CopyFile(string orginPath, string destPath, bool isOverwrite)
        {
            MkDir(destPath);
            File.Copy(orginPath, destPath, isOverwrite);
        }

        /// <summary>
        /// 以二进制的形式读取对应路径的文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static byte[] ReadBytes(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
            return File.ReadAllBytes(filePath);
        }

        /// <summary>
        /// 以文本的形式读取对应路径的文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ReadString(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
            return File.ReadAllText(filePath);
        }

        /// <summary>
        /// 以二进制的形式写入文件到对应路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        public static void WriteBytes(string filePath, byte[] content)
        {
            MkDir(filePath);
            File.WriteAllBytes(filePath, content);
        }

        /// <summary>
        /// 以文本的形式写入文件到对应路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        public static void WriteString(string filePath, string content)
        {
            MkDir(filePath);
            File.WriteAllText(filePath, content.Replace(Environment.NewLine, "\n"), UTF8EnCode);
        }

        /// <summary>
        /// 对指定路径的文件进行生成MD5码
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetMD5Hash(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            return GetMD5Hash(File.ReadAllBytes(filePath));
        }

        /// <summary>
        /// 二进制数据进行生成MD5码
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string GetMD5Hash(byte[] buffer)
        {
            if (buffer == null)
                return null;

            MD5 md5 = new MD5CryptoServiceProvider();
            return BitConverter.ToString(md5.ComputeHash(buffer)).Replace("-", "").ToLower();
        }
    }
}
