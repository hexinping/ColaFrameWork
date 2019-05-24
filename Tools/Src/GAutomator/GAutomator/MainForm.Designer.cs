namespace GAutomator
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.record = new System.Windows.Forms.Button();
            this.start_game = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.loginName = new System.Windows.Forms.ComboBox();
            this.random = new System.Windows.Forms.Button();
            this.play_back = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.serverNameBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // record
            // 
            this.record.Location = new System.Drawing.Point(240, 60);
            this.record.Name = "record";
            this.record.Size = new System.Drawing.Size(79, 23);
            this.record.TabIndex = 10;
            this.record.Text = "开始录制";
            this.record.UseVisualStyleBackColor = true;
            this.record.Click += new System.EventHandler(this.button_record_Click);
            // 
            // start_game
            // 
            this.start_game.Location = new System.Drawing.Point(14, 60);
            this.start_game.Name = "start_game";
            this.start_game.Size = new System.Drawing.Size(86, 23);
            this.start_game.TabIndex = 8;
            this.start_game.Text = "开始游戏";
            this.start_game.UseVisualStyleBackColor = true;
            this.start_game.Click += new System.EventHandler(this.start_game_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Gray;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "用户名";
            // 
            // loginName
            // 
            this.loginName.FormattingEnabled = true;
            this.loginName.Location = new System.Drawing.Point(59, 9);
            this.loginName.Name = "loginName";
            this.loginName.Size = new System.Drawing.Size(115, 20);
            this.loginName.TabIndex = 6;
            // 
            // random
            // 
            this.random.Location = new System.Drawing.Point(106, 60);
            this.random.Name = "random";
            this.random.Size = new System.Drawing.Size(79, 23);
            this.random.TabIndex = 9;
            this.random.Text = "随机";
            this.random.UseVisualStyleBackColor = true;
            this.random.Click += new System.EventHandler(this.button_random_Click);
            // 
            // play_back
            // 
            this.play_back.Location = new System.Drawing.Point(325, 60);
            this.play_back.Name = "play_back";
            this.play_back.Size = new System.Drawing.Size(79, 23);
            this.play_back.TabIndex = 11;
            this.play_back.Text = "回放录制";
            this.play_back.UseVisualStyleBackColor = true;
            this.play_back.Click += new System.EventHandler(this.play_back_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Gray;
            this.label2.Location = new System.Drawing.Point(193, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 17;
            this.label2.Text = "服务器";
            // 
            // serverNameBox
            // 
            this.serverNameBox.FormattingEnabled = true;
            this.serverNameBox.Location = new System.Drawing.Point(240, 9);
            this.serverNameBox.Name = "serverNameBox";
            this.serverNameBox.Size = new System.Drawing.Size(162, 20);
            this.serverNameBox.TabIndex = 7;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(414, 105);
            this.Controls.Add(this.serverNameBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.play_back);
            this.Controls.Add(this.random);
            this.Controls.Add(this.loginName);
            this.Controls.Add(this.start_game);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.record);
            this.ForeColor = System.Drawing.SystemColors.Desktop;
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "自动化测试";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button record;
        private System.Windows.Forms.Button start_game;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox loginName;
        private System.Windows.Forms.Button random;
        private System.Windows.Forms.Button play_back;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox serverNameBox;
    }
}

