namespace 日志书写器
{
    partial class Form1
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxMain = new System.Windows.Forms.TextBox();
            this.labelPath = new System.Windows.Forms.Label();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.button保存 = new System.Windows.Forms.Button();
            this.groupBoxSetting = new System.Windows.Forms.GroupBox();
            this.checkBoxMailbox = new System.Windows.Forms.CheckBox();
            this.comboBoxFontSize = new System.Windows.Forms.ComboBox();
            this.textBoxFont = new System.Windows.Forms.TextBox();
            this.labelFontSize = new System.Windows.Forms.Label();
            this.labelFont = new System.Windows.Forms.Label();
            this.buttonTopMost = new System.Windows.Forms.Button();
            this.groupBoxSetting.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxMain
            // 
            this.textBoxMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMain.Font = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxMain.Location = new System.Drawing.Point(12, 59);
            this.textBoxMain.Multiline = true;
            this.textBoxMain.Name = "textBoxMain";
            this.textBoxMain.Size = new System.Drawing.Size(821, 574);
            this.textBoxMain.TabIndex = 0;
            // 
            // labelPath
            // 
            this.labelPath.AutoSize = true;
            this.labelPath.Location = new System.Drawing.Point(186, 19);
            this.labelPath.Name = "labelPath";
            this.labelPath.Size = new System.Drawing.Size(53, 12);
            this.labelPath.TabIndex = 7;
            this.labelPath.Text = "保存路径";
            // 
            // textBoxPath
            // 
            this.textBoxPath.AllowDrop = true;
            this.textBoxPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPath.Location = new System.Drawing.Point(245, 14);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(336, 21);
            this.textBoxPath.TabIndex = 3;
            this.textBoxPath.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxPath_DragDrop);
            this.textBoxPath.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxPath_DragEnter);
            // 
            // button保存
            // 
            this.button保存.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button保存.Location = new System.Drawing.Point(743, 14);
            this.button保存.Name = "button保存";
            this.button保存.Size = new System.Drawing.Size(72, 23);
            this.button保存.TabIndex = 4;
            this.button保存.Text = "保存文档";
            this.button保存.UseVisualStyleBackColor = true;
            this.button保存.Click += new System.EventHandler(this.button保存_Click);
            // 
            // groupBoxSetting
            // 
            this.groupBoxSetting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSetting.Controls.Add(this.buttonTopMost);
            this.groupBoxSetting.Controls.Add(this.checkBoxMailbox);
            this.groupBoxSetting.Controls.Add(this.comboBoxFontSize);
            this.groupBoxSetting.Controls.Add(this.textBoxFont);
            this.groupBoxSetting.Controls.Add(this.labelFontSize);
            this.groupBoxSetting.Controls.Add(this.labelFont);
            this.groupBoxSetting.Controls.Add(this.labelPath);
            this.groupBoxSetting.Controls.Add(this.button保存);
            this.groupBoxSetting.Controls.Add(this.textBoxPath);
            this.groupBoxSetting.Location = new System.Drawing.Point(13, 7);
            this.groupBoxSetting.Name = "groupBoxSetting";
            this.groupBoxSetting.Size = new System.Drawing.Size(821, 46);
            this.groupBoxSetting.TabIndex = 8;
            this.groupBoxSetting.TabStop = false;
            // 
            // checkBoxMailbox
            // 
            this.checkBoxMailbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxMailbox.AutoSize = true;
            this.checkBoxMailbox.Checked = true;
            this.checkBoxMailbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMailbox.Location = new System.Drawing.Point(587, 11);
            this.checkBoxMailbox.Name = "checkBoxMailbox";
            this.checkBoxMailbox.Size = new System.Drawing.Size(72, 28);
            this.checkBoxMailbox.TabIndex = 9;
            this.checkBoxMailbox.Text = " 退出后\n打开邮箱";
            this.checkBoxMailbox.UseVisualStyleBackColor = true;
            // 
            // comboBoxFontSize
            // 
            this.comboBoxFontSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFontSize.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBoxFontSize.Font = new System.Drawing.Font("宋体", 10F);
            this.comboBoxFontSize.FormattingEnabled = true;
            this.comboBoxFontSize.Items.AddRange(new object[] {
            "六号",
            "小五",
            "五号",
            "小四",
            "四号",
            "小三",
            "三号",
            "小二",
            "二号",
            "小一",
            "一号"});
            this.comboBoxFontSize.Location = new System.Drawing.Point(125, 15);
            this.comboBoxFontSize.Name = "comboBoxFontSize";
            this.comboBoxFontSize.Size = new System.Drawing.Size(55, 21);
            this.comboBoxFontSize.TabIndex = 2;
            this.comboBoxFontSize.SelectedIndexChanged += new System.EventHandler(this.comboBoxFontSize_SelectedIndexChanged);
            // 
            // textBoxFont
            // 
            this.textBoxFont.Font = new System.Drawing.Font("黑体", 10F);
            this.textBoxFont.Location = new System.Drawing.Point(41, 14);
            this.textBoxFont.Name = "textBoxFont";
            this.textBoxFont.Size = new System.Drawing.Size(43, 23);
            this.textBoxFont.TabIndex = 1;
            this.textBoxFont.Text = "黑体";
            this.textBoxFont.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelFontSize
            // 
            this.labelFontSize.AutoSize = true;
            this.labelFontSize.Location = new System.Drawing.Point(90, 19);
            this.labelFontSize.Name = "labelFontSize";
            this.labelFontSize.Size = new System.Drawing.Size(29, 12);
            this.labelFontSize.TabIndex = 6;
            this.labelFontSize.Text = "字号";
            // 
            // labelFont
            // 
            this.labelFont.AutoSize = true;
            this.labelFont.Location = new System.Drawing.Point(6, 19);
            this.labelFont.Name = "labelFont";
            this.labelFont.Size = new System.Drawing.Size(29, 12);
            this.labelFont.TabIndex = 5;
            this.labelFont.Text = "字体";
            // 
            // buttonTopMost
            // 
            this.buttonTopMost.Location = new System.Drawing.Point(665, 14);
            this.buttonTopMost.Name = "buttonTopMost";
            this.buttonTopMost.Size = new System.Drawing.Size(72, 23);
            this.buttonTopMost.TabIndex = 10;
            this.buttonTopMost.Text = "窗口置顶";
            this.buttonTopMost.UseVisualStyleBackColor = true;
            this.buttonTopMost.Click += new System.EventHandler(this.buttonTopMost_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(846, 645);
            this.Controls.Add(this.groupBoxSetting);
            this.Controls.Add(this.textBoxMain);
            this.Name = "Form1";
            this.Text = "日志书写器";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBoxSetting.ResumeLayout(false);
            this.groupBoxSetting.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxMain;
        private System.Windows.Forms.Label labelPath;
        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.Button button保存;
        private System.Windows.Forms.GroupBox groupBoxSetting;
        private System.Windows.Forms.TextBox textBoxFont;
        private System.Windows.Forms.Label labelFontSize;
        private System.Windows.Forms.Label labelFont;
        private System.Windows.Forms.ComboBox comboBoxFontSize;
        private System.Windows.Forms.CheckBox checkBoxMailbox;
        private System.Windows.Forms.Button buttonTopMost;
    }
}

