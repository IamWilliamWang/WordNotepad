using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 日志书写器
{
    public partial class FormReplacer : Form
    {
        private static FormReplacer instance;
        private TextBox mainTextBox;
        FormEdit.FormerSaver<String> former;
        
        private FormReplacer(TextBox mainTextBox, FormEdit.FormerSaver<String> former)
        {
            InitializeComponent();
            this.mainTextBox = mainTextBox;
            this.former = former;
        }

        /// <summary>
        /// 显示替换文本窗体
        /// </summary>
        /// <param name="mainTextBox"></param>
        /// <param name="former"></param>
        public static void ShowReplacer(TextBox mainTextBox, FormEdit.FormerSaver<String> former, bool topMost = false)
        {
            if (instance == null)
            {
                instance = new FormReplacer(mainTextBox, former);
                instance.textBox替换内容.Text = Clipboard.GetText();
                instance.TopMost = topMost;
                instance.Show();
                instance.textBox替换为.Focus();
            }
            else
            {
                instance.textBox替换内容.Text = Clipboard.GetText();
                instance.TopMost = topMost;
                instance.Focus();
                instance.textBox替换为.Focus();
            }
        }

        private void 插入第一行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBox替换内容.Text += '\t';
        }

        private void 插入第二行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBox替换为.Text += '\t';
        }

        private void 撤销替换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (this.executedReplacement == 0) 
            //    return;
            this.mainTextBox.Text = this.former.Undo();
            //this.executedReplacement--;
        }

        private void 执行替换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string original = this.mainTextBox.Text;
            this.former.SaveText(this.mainTextBox.Text);
            mainTextBox.Text = mainTextBox.Text.Replace(this.textBox替换内容.Text, this.textBox替换为.Text);
            if (mainTextBox.Text == original) 
            {
                MessageBox.Show("替换完毕，但未找到可替换内容", "成功！");
                return;
            }
            //if (executedReplacement == 0)
            //    MessageBox.Show("替换完毕！", "成功");
            //executedReplacement++;
        }

        private void Replacer_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.former.SaveText(this.mainTextBox.Text);
            instance = null;
        }

        private void 重做替换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mainTextBox.Text = this.former.Redo();
        }
    }
}
