using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace GAutomator
{
    public partial class MainForm : Form
    {
        public enum EStrType
        {
            ServerName = 1,
            LoginName  = 2,
        }

        public enum EState
        {
            None ,
            Login ,
            Record ,
            RandomRun, 
            PlayBack,
        }

        private EState state = EState.None;
        
        public MainForm()
        {
            InitializeComponent();
            InitializeForm();
            //在多线程程序中,新创建的线程不能访问UI线程创建的窗口控件
            CheckForIllegalCrossThreadCalls = false;
        }

        private void InitializeForm()
        {
            List<string> loginName = GetSelectPath(EStrType.LoginName);
            List<string> serverName = GetSelectPath(EStrType.ServerName);
            if (loginName.Count > 0)
            {
                this.loginName.Text = loginName[loginName.Count - 1];
                foreach (var item in loginName)
                {
                    this.loginName.Items.Add(item);
                }
            }
            if (serverName.Count > 0)
            {
                this.serverNameBox.Text = serverName[serverName.Count - 1];
                foreach (var item in serverName)
                {
                    this.serverNameBox.Items.Add(item);
                }
            }
            else
            {
                this.serverNameBox.Text = "192.168.1.204";
            }
            long size = ConstVar.GetDirectoryLength(ConstVar.screenshotPath);
            if (size > 20480)
            {
                DialogResult result = MessageBox.Show("录制文件截图过大，是否进行清除?", "", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    Directory.Delete(ConstVar.screenshotPath);
                }
            }
        }

        //读取上次设置的路径填充到选择路径
        private List<string> GetSelectPath(EStrType type)
        {
            List<string> strs = new List<string>();
            if (File.Exists(ConstVar.saveTextPath))
            {
                StreamReader sr = new StreamReader(ConstVar.saveTextPath, Encoding.Default);
                string line;
                string serverName;
                string loginName;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("ServerName") && type == EStrType.ServerName)
                    {
                        serverName = line.Replace("ServerName:", "");
                        if (!strs.Contains(serverName))
                        {
                            strs.Add(serverName);
                        }
                    }
                    else if (line.Contains("LoginName") && type == EStrType.LoginName)
                    {
                        loginName = line.Replace("LoginName:", "");
                        if (!strs.Contains(loginName))
                        {
                            strs.Add(loginName);
                        }
                    }
                }
                sr.Close();
            }
            return strs;
        }

        //用户设置写入文件
        private void SaveStr(string str, EStrType type)
        {
            if (File.Exists(ConstVar.saveTextPath))
            {
                string[] lines = File.ReadAllLines(ConstVar.saveTextPath);
                foreach (var line in lines)
                {
                    if (line.Contains(str)) return;
                }
            }
            using (FileStream fs = new FileStream(ConstVar.saveTextPath, FileMode.Append, FileAccess.Write))
            {
                StreamWriter writer = new StreamWriter(fs);
                if (type == EStrType.ServerName)
                {
                    str = string.Format("ServerName:{0}", str);
                }
                else if (type == EStrType.LoginName)
                {
                    str = string.Format("LoginName:{0}", str);
                }
                writer.WriteLine(str);
                writer.Flush();
                writer.Close();
                fs.Close();
            }
        }

        private void start_game_Click(object sender, EventArgs e)
        {
            if (state == EState.None)
            {
                if (!string.IsNullOrEmpty(this.loginName.Text))
                {
                    string cmd = ConstVar.autoTestPath + "bin\\start_game.bat";
                    if (string.IsNullOrEmpty(this.serverNameBox.Text))
                    {
                        MessageBox.Show("please input server ip!");
                        return;
                    }
                    ReplaceStartGameFileName(this.loginName.Text, this.serverNameBox.Text);
                    if (!File.Exists(cmd))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(string.Format("cd /d {0}\n", ConstVar.autoTestPath));
                        sb.Append("python start_game.py\n");
                        File.WriteAllText(cmd, sb.ToString());
                        sb.Clear();
                    }
                    RunCmd(cmd, ProcessWindowStyle.Normal);
                    SaveStr(this.loginName.Text, EStrType.LoginName);
                    SaveStr(this.serverNameBox.Text, EStrType.ServerName);
                    state = EState.Login;
                    this.start_game.Text = "结束游戏";
                }
                else
                {
                    MessageBox.Show("please input player name!");
                }
            }
            else
            {
                string cmd = ConstVar.autoTestPath + "bin\\endgame.bat";
                if (!File.Exists(cmd))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(@"adb shell am force-stop com.tencent.tfwl.yw");
                    File.WriteAllText(cmd, sb.ToString());
                    sb.Clear();
                }
                RunCmd(cmd);
                state = EState.None;
                this.start_game.Text = "开始游戏";
            }
        }

        private void button_record_Click(object sender, EventArgs e)
        {
            if (state == EState.Record)
            {
                this.record.Text = "开始录制";
                state = EState.Login;
            }
            else
            {
                CreateRecordForm form = new CreateRecordForm();
                form.Show();
                form.OnCreateRecordShow += callback;
            }
        }

        private void callback(string str,Action dispose)
        {
            dispose();
            //this.record.Text = "停止录制";
            state = EState.Record;
            string cmd = ConstVar.autoTestPath + "bin\\record.bat";
            //替换文件名称
            ReplaceRecordFileName(str,true);
            if (!File.Exists(cmd))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("cd /d {0}\n", ConstVar.autoTestPath));
                sb.Append("python record.py\n");
                sb.Append("pause");
                File.WriteAllText(cmd, sb.ToString());
                sb.Clear();
            }
            RunCmd(cmd);
        }

        private void button_random_Click(object sender, EventArgs e)
        {
            string cmd = ConstVar.autoTestPath + "bin\\random.bat";
            if (!File.Exists(cmd))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("cd /d {0}\n", ConstVar.autoTestPath));
                sb.Append("python random_run.py\n");
                sb.Append("pause");
                File.WriteAllText(cmd, sb.ToString());
                sb.Clear();
            }
            RunCmd(cmd);
            state = EState.RandomRun;
        }

        private void play_back_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = ConstVar.recordTextPath;
            dialog.Filter = "所有文本文件|*.txt";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string recordPath = dialog.FileName;
                string strs = File.ReadAllText(recordPath);
                if (string.IsNullOrEmpty(strs))
                {
                    MessageBox.Show("is an empty file,please select another one!");
                    File.Delete(recordPath);
                    return;
                }
                //替换play_back.py 播放文件
                ReplaceRecordFileName(Path.GetFileNameWithoutExtension(recordPath),false);
                string cmd = ConstVar.autoTestPath + "bin\\play_back.bat";
                if (!File.Exists(cmd))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(string.Format("cd /d {0}\n", ConstVar.autoTestPath));
                    sb.Append("python play_back.py\n");
                    File.WriteAllText(cmd, sb.ToString());
                    sb.Clear();
                }
                RunCmd(cmd);
                state = EState.PlayBack;
            }
        }

        private void ReplaceStartGameFileName(string loginName,string ip)
        {
            string path = ConstVar.startPyPath;
            string[] strs = File.ReadAllLines(path);
            string[] newStrs = new string[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                string line = strs[i];
                if (line.Contains(@"player_name = "))
                {
                    string fileName = Regex.Match(line, "\".*\"").Value.Replace("\"", "");
                    line = line.Replace(fileName, loginName);
                }
                else if (line.Contains(@"ip = "))
                {
                    string fileName = Regex.Match(line, "\".*\"").Value.Replace("\"", "");
                    line = line.Replace(fileName, ip);
                }
                newStrs[i] = line;
            }
            File.WriteAllLines(path, newStrs);
        }

        private void ReplaceRecordFileName(string str,bool isrecord)
        {
            string path = (isrecord) ? ConstVar.recordPyPath : ConstVar.playBackPyPath;
            string[] strs = File.ReadAllLines(path);
            string[] newStrs = new string[strs.Length];
            for(int i= 0 ;i<strs.Length;i++ )
            {
                string line = strs[i];
                if (line.Contains(@"record_file_dir = "))
                {
                    string fileName = Regex.Match(line, "\".*\\.").Value.Replace("\"", "").Replace(".","");
                    line = line.Replace(fileName, string.Format("records/{0}", str));
                }
                newStrs[i] = line;
            }
            File.WriteAllLines(path, newStrs);
        }

        /// <summary>
        /// 运行cmd
        /// </summary>
        /// <param name="cmdPath">cmd路径</param>
        /// <param name="runInBackground">是否后台运行cmd</param>
        /// <param name="logBox">后台运行时显示cmd日志的RichTextBox </param>
        public void RunCmd(string cmdPath, ProcessWindowStyle style = ProcessWindowStyle.Minimized)
        {
            try
            {
                Process proc = new Process();
                proc.StartInfo.FileName = cmdPath;
                proc.StartInfo.CreateNoWindow = false;
                proc.StartInfo.WindowStyle = style;
                proc.Start();
                SetButtonEnable(false);
                proc.WaitForExit();
                SetButtonEnable(true);
                proc.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("running cmd error！");
            }
        }

        //cmd运行时不可点击按钮
        private void SetButtonEnable(bool enable)
        {
            this.start_game.Enabled = enable;
            this.record.Enabled = enable;
            this.random.Enabled = enable;
            this.play_back.Enabled = enable;
        }
    }
}
