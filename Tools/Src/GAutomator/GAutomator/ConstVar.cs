using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace GAutomator
{
    class ConstVar
    {
        public static string autoTestPath = System.Environment.CurrentDirectory.Remove(System.Environment.CurrentDirectory.LastIndexOf(@"\") + 1);
        public static string unityLogPath = autoTestPath + "logs\\unity_log.txt";
        public static string recordTextPath = autoTestPath + "records";
        public static string saveTextPath = autoTestPath + "bin\\player_info.txt";
        public static string startPyPath = autoTestPath + "start_game.py";
        public static string recordPyPath = autoTestPath + "record.py";
        public static string playBackPyPath = autoTestPath + "play_back.py";
        public static string randomPyPath = autoTestPath + "random.py";
        public static string screenshotPath = autoTestPath + "screenshot";

        public static long GetDirectoryLength(string dirPath)
        {
            long len = 0;
            if (Directory.Exists(dirPath))
            {
                DirectoryInfo di = new DirectoryInfo(dirPath);
                if (di.GetFiles().Length > 0)
                {
                    foreach (FileInfo fi in di.GetFiles())
                    {
                        len += fi.Length/(1024*1024);
                    }
                }
                //获取di中所有的文件夹，并存到一个新的对象数组中，以进行递归
                DirectoryInfo[] dis = di.GetDirectories();
                if (dis.Length > 0)
                {
                    for (int i = 0; i < dis.Length; i++)
                    {
                        len += GetDirectoryLength(dis[i].FullName);
                    }
                }
            }
            return len;
        }
    }

    class Time
    {
        public System.Timers.Timer timer;
        public delegate void Callback(object sender, System.Timers.ElapsedEventArgs e);
        public void InitTimer(int second,bool repeat,Callback callback)
        {
            int interval = second * 1000;
            timer = new System.Timers.Timer(interval);
            timer.AutoReset = repeat;
            timer.Enabled = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(callback);
        }
    }
}
