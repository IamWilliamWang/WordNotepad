namespace 日志书写器
{
    partial class FormSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox计时器时长 = new System.Windows.Forms.TextBox();
            this.label计时器时长 = new System.Windows.Forms.Label();
            this.button计时器时长变更 = new System.Windows.Forms.Button();
            this.checkBox自动备份 = new System.Windows.Forms.CheckBox();
            this.checkBox自动保存 = new System.Windows.Forms.CheckBox();
            this.groupBox状态控制 = new System.Windows.Forms.GroupBox();
            this.button清除备份文件 = new System.Windows.Forms.Button();
            this.groupBox状态控制.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox计时器时长
            // 
            this.textBox计时器时长.Location = new System.Drawing.Point(247, 21);
            this.textBox计时器时长.Name = "textBox计时器时长";
            this.textBox计时器时长.Size = new System.Drawing.Size(100, 21);
            this.textBox计时器时长.TabIndex = 0;
            this.textBox计时器时长.Text = "30";
            // 
            // label计时器时长
            // 
            this.label计时器时长.AutoSize = true;
            this.label计时器时长.Location = new System.Drawing.Point(140, 24);
            this.label计时器时长.Name = "label计时器时长";
            this.label计时器时长.Size = new System.Drawing.Size(101, 12);
            this.label计时器时长.TabIndex = 1;
            this.label计时器时长.Text = "计时器时长(秒)：";
            // 
            // button计时器时长变更
            // 
            this.button计时器时长变更.Location = new System.Drawing.Point(367, 21);
            this.button计时器时长变更.Name = "button计时器时长变更";
            this.button计时器时长变更.Size = new System.Drawing.Size(75, 23);
            this.button计时器时长变更.TabIndex = 2;
            this.button计时器时长变更.Text = "变更";
            this.button计时器时长变更.UseVisualStyleBackColor = true;
            this.button计时器时长变更.Click += new System.EventHandler(this.button计时器时长变更_Click);
            // 
            // checkBox自动备份
            // 
            this.checkBox自动备份.AutoSize = true;
            this.checkBox自动备份.Location = new System.Drawing.Point(19, 55);
            this.checkBox自动备份.Name = "checkBox自动备份";
            this.checkBox自动备份.Size = new System.Drawing.Size(72, 16);
            this.checkBox自动备份.TabIndex = 3;
            this.checkBox自动备份.Text = "自动备份";
            this.checkBox自动备份.UseVisualStyleBackColor = true;
            this.checkBox自动备份.CheckedChanged += new System.EventHandler(this.checkBox自动备份_CheckedChanged);
            // 
            // checkBox自动保存
            // 
            this.checkBox自动保存.AutoSize = true;
            this.checkBox自动保存.Location = new System.Drawing.Point(19, 20);
            this.checkBox自动保存.Name = "checkBox自动保存";
            this.checkBox自动保存.Size = new System.Drawing.Size(72, 16);
            this.checkBox自动保存.TabIndex = 4;
            this.checkBox自动保存.Text = "自动保存";
            this.checkBox自动保存.UseVisualStyleBackColor = true;
            this.checkBox自动保存.CheckedChanged += new System.EventHandler(this.checkBox自动保存_CheckedChanged);
            // 
            // groupBox状态控制
            // 
            this.groupBox状态控制.Controls.Add(this.checkBox自动备份);
            this.groupBox状态控制.Controls.Add(this.checkBox自动保存);
            this.groupBox状态控制.Location = new System.Drawing.Point(12, 12);
            this.groupBox状态控制.Name = "groupBox状态控制";
            this.groupBox状态控制.Size = new System.Drawing.Size(122, 87);
            this.groupBox状态控制.TabIndex = 5;
            this.groupBox状态控制.TabStop = false;
            this.groupBox状态控制.Text = "状态控制";
            // 
            // button清除备份文件
            // 
            this.button清除备份文件.Location = new System.Drawing.Point(247, 63);
            this.button清除备份文件.Name = "button清除备份文件";
            this.button清除备份文件.Size = new System.Drawing.Size(88, 23);
            this.button清除备份文件.TabIndex = 6;
            this.button清除备份文件.Text = "清除备份文件";
            this.button清除备份文件.UseVisualStyleBackColor = true;
            this.button清除备份文件.Click += new System.EventHandler(this.button清除备份文件_Click);
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 117);
            this.Controls.Add(this.button清除备份文件);
            this.Controls.Add(this.groupBox状态控制);
            this.Controls.Add(this.button计时器时长变更);
            this.Controls.Add(this.label计时器时长);
            this.Controls.Add(this.textBox计时器时长);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormSettings";
            this.Text = "设置窗格";
            this.Load += new System.EventHandler(this.FormSettings_Load);
            this.groupBox状态控制.ResumeLayout(false);
            this.groupBox状态控制.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox计时器时长;
        private System.Windows.Forms.Label label计时器时长;
        private System.Windows.Forms.Button button计时器时长变更;
        private System.Windows.Forms.CheckBox checkBox自动备份;
        private System.Windows.Forms.CheckBox checkBox自动保存;
        private System.Windows.Forms.GroupBox groupBox状态控制;
        private System.Windows.Forms.Button button清除备份文件;
    }
}