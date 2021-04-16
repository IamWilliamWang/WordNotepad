using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Word记事本
{
    public class InputBoxFormInner : Form
    {
        #region InputBoxForm.designer.cs
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
            this.labelContent = new System.Windows.Forms.Label();
            this.textBoxInput = new System.Windows.Forms.TextBox();
            this.button确定 = new System.Windows.Forms.Button();
            this.button取消 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelContent
            // 
            this.labelContent.AutoSize = true;
            this.labelContent.Location = new System.Drawing.Point(12, 12);
            this.labelContent.Name = "labelContent";
            this.labelContent.Size = new System.Drawing.Size(35, 12);
            this.labelContent.TabIndex = 0;
            this.labelContent.Text = "label";
            // 
            // textBoxInput
            // 
            this.textBoxInput.Location = new System.Drawing.Point(11, 82);
            this.textBoxInput.Name = "textBoxInput";
            this.textBoxInput.Size = new System.Drawing.Size(449, 21);
            this.textBoxInput.TabIndex = 1;
            this.textBoxInput.Click += new System.EventHandler(this.TextBoxInput_Click);
            this.textBoxInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxInput_KeyPress);
            // 
            // button确定
            // 
            this.button确定.Location = new System.Drawing.Point(378, 12);
            this.button确定.Name = "button确定";
            this.button确定.Size = new System.Drawing.Size(83, 23);
            this.button确定.TabIndex = 2;
            this.button确定.Text = "确定";
            this.button确定.UseVisualStyleBackColor = true;
            this.button确定.Click += new System.EventHandler(this.button确定_Click);
            // 
            // button取消
            // 
            this.button取消.Location = new System.Drawing.Point(378, 41);
            this.button取消.Name = "button取消";
            this.button取消.Size = new System.Drawing.Size(83, 23);
            this.button取消.TabIndex = 3;
            this.button取消.Text = "取消";
            this.button取消.UseVisualStyleBackColor = true;
            this.button取消.Click += new System.EventHandler(this.button取消_Click);
            // 
            // InputBoxFormInner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 114);
            this.Controls.Add(this.button取消);
            this.Controls.Add(this.button确定);
            this.Controls.Add(this.textBoxInput);
            this.Controls.Add(this.labelContent);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputBoxFormInner";
            this.Text = "InputBoxForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelContent;
        private System.Windows.Forms.TextBox textBoxInput;
        private System.Windows.Forms.Button button确定;
        private System.Windows.Forms.Button button取消;

        #endregion
        
        #region InputBoxForm.cs
        public string BoxText { get { return this.textBoxInput.Text; } set { this.textBoxInput.Text = value; } }
        private int CharCountPerLine { get; set; } = 30;
        public string Title { get { return this.Text; } set { this.Text = value; } }
        public string HeaderText { get { return this.labelContent.Text; } set { this.labelContent.Text = value; } }
        private string defaultText;
        private bool? needClearDefaultText = null;
        private bool 输入成功 = false;

        public InputBoxFormInner(string title, string content, int? charCountPerLine = null)
        {
            InitializeComponent();
            this.Text = title;
            if (charCountPerLine != null)
                this.CharCountPerLine = charCountPerLine ?? 30;
            this.labelContent.Text = AddNewline(content);
        }

        private string AddNewline(string content)
        {
            StringBuilder stringBuilder = new StringBuilder(content);
            for (int i = CharCountPerLine * (int)(content.Length / CharCountPerLine); i >= 0; i -= CharCountPerLine)
                stringBuilder.Insert(i, "\r\n");
            return stringBuilder.ToString();
        }

        private void button确定_Click(object sender, EventArgs e)
        {
            this.输入成功 = true;
            this.Close();
        }

        private void button取消_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBoxInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 回车键确认
            if (e.KeyChar == '\r')
                this.button确定_Click(sender, e);
            // Esc键取消
            else if (e.KeyChar == 27)
                this.button取消_Click(sender, e);
            // 确保在hint状态下输入时删除掉提示
            if (this.needClearDefaultText == true)
            {
                this.BoxText = String.Empty;
                this.needClearDefaultText = false;
            }
        }
        
        public void SetDefaultText(string text)
        {
            BoxText = text;
            this.defaultText = text;
            this.needClearDefaultText = false;
        }

        public void SetHint(string hint)
        {
            BoxText = hint;
            this.defaultText = hint;
            this.needClearDefaultText = true;
        }

        private void TextBoxInput_Click(object sender, EventArgs e)
        {
            // 在hint状态下清空文本框
            if (this.needClearDefaultText == true) 
                BoxText = String.Empty;
            this.needClearDefaultText = false;
        }

        public bool InputSuccess()
        {
            return this.输入成功 && this.BoxText != String.Empty;
        }
        #endregion
    }
    
    class Interaction
    {
        /// <summary>
        /// 弹出一个输入框，返回所输入的内容
        /// </summary>
        /// <param name="content">主体的文字</param>
        /// <param name="title">标题</param>
        /// <param name="charCountPerline">主体文字每行有多少字符</param>
        /// <param name="defaultText">输入框的默认字符串</param>
        /// <param name="hint">输入框的提示(会覆盖defaultText属性)</param>
        /// <param name="defaultReturn">点击取消所返回的字符串</param>
        /// <returns></returns>
        public static string InputBox(string content, string title = "请输入", int? charCountPerline = null, 
                                            string defaultText = null, string hint = null, string defaultReturn = "")
        {
            InputBoxFormInner inputBox;
            if (charCountPerline != null)
                inputBox = new InputBoxFormInner(title, content, charCountPerline);
            else
                inputBox = new InputBoxFormInner(title, content);
            if (defaultText != null) 
                inputBox.SetDefaultText(defaultText);
            if (hint != null)
                inputBox.SetHint(hint);
            inputBox.ShowDialog();
            if (inputBox.InputSuccess() == false) 
                return defaultReturn;
            return inputBox.BoxText;
        }
    }
}
