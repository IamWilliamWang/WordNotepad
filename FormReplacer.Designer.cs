namespace 日志书写器
{
    partial class FormReplacer
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
            this.label要替换的内容 = new System.Windows.Forms.Label();
            this.label替换为 = new System.Windows.Forms.Label();
            this.textBox替换为 = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.插入TabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.插入第一行ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.插入第二行ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.撤销替换ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.重做替换ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.执行替换ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBox替换内容 = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label要替换的内容
            // 
            this.label要替换的内容.AutoSize = true;
            this.label要替换的内容.Location = new System.Drawing.Point(12, 35);
            this.label要替换的内容.Name = "label要替换的内容";
            this.label要替换的内容.Size = new System.Drawing.Size(77, 12);
            this.label要替换的内容.TabIndex = 0;
            this.label要替换的内容.Text = "要替换的内容";
            // 
            // label替换为
            // 
            this.label替换为.AutoSize = true;
            this.label替换为.Location = new System.Drawing.Point(28, 62);
            this.label替换为.Name = "label替换为";
            this.label替换为.Size = new System.Drawing.Size(41, 12);
            this.label替换为.TabIndex = 1;
            this.label替换为.Text = "替换为";
            // 
            // textBox替换为
            // 
            this.textBox替换为.Location = new System.Drawing.Point(95, 59);
            this.textBox替换为.Name = "textBox替换为";
            this.textBox替换为.Size = new System.Drawing.Size(217, 21);
            this.textBox替换为.TabIndex = 3;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.插入TabToolStripMenuItem,
            this.撤销替换ToolStripMenuItem,
            this.重做替换ToolStripMenuItem,
            this.执行替换ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(326, 25);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 插入TabToolStripMenuItem
            // 
            this.插入TabToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.插入第一行ToolStripMenuItem,
            this.插入第二行ToolStripMenuItem});
            this.插入TabToolStripMenuItem.Name = "插入TabToolStripMenuItem";
            this.插入TabToolStripMenuItem.Size = new System.Drawing.Size(66, 21);
            this.插入TabToolStripMenuItem.Text = "插入Tab";
            // 
            // 插入第一行ToolStripMenuItem
            // 
            this.插入第一行ToolStripMenuItem.Name = "插入第一行ToolStripMenuItem";
            this.插入第一行ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.插入第一行ToolStripMenuItem.Text = "插入第一行";
            this.插入第一行ToolStripMenuItem.Click += new System.EventHandler(this.插入第一行ToolStripMenuItem_Click);
            // 
            // 插入第二行ToolStripMenuItem
            // 
            this.插入第二行ToolStripMenuItem.Name = "插入第二行ToolStripMenuItem";
            this.插入第二行ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.插入第二行ToolStripMenuItem.Text = "插入第二行";
            this.插入第二行ToolStripMenuItem.Click += new System.EventHandler(this.插入第二行ToolStripMenuItem_Click);
            // 
            // 撤销替换ToolStripMenuItem
            // 
            this.撤销替换ToolStripMenuItem.Name = "撤销替换ToolStripMenuItem";
            this.撤销替换ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.撤销替换ToolStripMenuItem.Text = "撤销";
            this.撤销替换ToolStripMenuItem.Click += new System.EventHandler(this.撤销替换ToolStripMenuItem_Click);
            // 
            // 重做替换ToolStripMenuItem
            // 
            this.重做替换ToolStripMenuItem.Name = "重做替换ToolStripMenuItem";
            this.重做替换ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.重做替换ToolStripMenuItem.Text = "重做";
            this.重做替换ToolStripMenuItem.Click += new System.EventHandler(this.重做替换ToolStripMenuItem_Click);
            // 
            // 执行替换ToolStripMenuItem
            // 
            this.执行替换ToolStripMenuItem.Name = "执行替换ToolStripMenuItem";
            this.执行替换ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.执行替换ToolStripMenuItem.Text = "执行替换";
            this.执行替换ToolStripMenuItem.Click += new System.EventHandler(this.执行替换ToolStripMenuItem_Click);
            // 
            // textBox替换内容
            // 
            this.textBox替换内容.Location = new System.Drawing.Point(95, 32);
            this.textBox替换内容.Name = "textBox替换内容";
            this.textBox替换内容.Size = new System.Drawing.Size(217, 21);
            this.textBox替换内容.TabIndex = 2;
            // 
            // Replacer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(326, 92);
            this.Controls.Add(this.textBox替换内容);
            this.Controls.Add(this.textBox替换为);
            this.Controls.Add(this.label替换为);
            this.Controls.Add(this.label要替换的内容);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Replacer";
            this.Text = "替换文本";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Replacer_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label要替换的内容;
        private System.Windows.Forms.Label label替换为;
        private System.Windows.Forms.TextBox textBox替换为;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 插入TabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 执行替换ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 插入第一行ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 插入第二行ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 撤销替换ToolStripMenuItem;
        private System.Windows.Forms.TextBox textBox替换内容;
        private System.Windows.Forms.ToolStripMenuItem 重做替换ToolStripMenuItem;
    }
}