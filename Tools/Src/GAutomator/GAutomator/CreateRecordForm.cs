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

namespace GAutomator
{
    public partial class CreateRecordForm : Form
    {
        public delegate void OnShow(string fileName,Action dispose);
        public event OnShow OnCreateRecordShow;

        public CreateRecordForm()
        {
            InitializeComponent();
        }

        
        private void confirm_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.fileName.Text))
            {
                string str = string.Format("{0}\\{1}.txt",ConstVar.recordTextPath,this.fileName.Text);
                if (File.Exists(str))
                {
                    DialogResult result = MessageBox.Show("已存在对应的录制文件，是否覆盖？", "", MessageBoxButtons.OKCancel);
                    if (result == DialogResult.OK)
                    {
                        OnCreateRecordShow(this.fileName.Text, () => { this.Dispose(); });
                    }
                    else
                    {
                        this.fileName.Text = "";
                    }
                }
                else
                {
                    OnCreateRecordShow(this.fileName.Text, () => { this.Dispose(); });
                }
            }
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
