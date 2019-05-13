using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace 日志书写器
{
    public partial class Form1 : Form
    {
        private float DocumentFontSize { get { return 12F; } }
        private String DocumentFont { get { return "黑体"; } }
        private bool hasSaved { get; set; }

        public Form1()
        {
            InitializeComponent();
            // 将实际字体替代在设计器中显示的字体
            this.textBoxMain.Font = new System.Drawing.Font(DocumentFont, DocumentFontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        }

        private bool need2Save() => this.textBoxMain.Text != "" && !this.hasSaved;

        private void saveDocx()
        {
            string filename = "";
            if (this.textBoxPath.Text != "")
                filename += this.textBoxPath.Text + "\\";
            filename += String.Format("{0:0000}", DateTime.Now.Year);
            filename += String.Format("{0:00}", DateTime.Now.Month);
            filename += String.Format("{0:00}", DateTime.Now.Day);
            filename += "王劲翔.docx";
            Word word = new Word(filename);
            if (this.textBoxFont.Text != this.DocumentFont)
                word.Font = this.textBoxFont.Text;
            else
                word.Font = this.DocumentFont;
            if (this.textBoxFontSize.Text != this.DocumentFontSize.ToString())
                word.FontSize = (int)float.Parse(this.textBoxFontSize.Text);
            else
                word.FontSize = (int)this.DocumentFontSize;
            word.WriteDocx(this.textBoxMain.Text);
            this.hasSaved = true;
        }

        private void button保存_Click(object sender, EventArgs e)
        {
            saveDocx();
            MessageBox.Show("保存Word文档成功！");
        }

        // 关闭的时候检查保存
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (need2Save())
            {
                if (MessageBox.Show("有内容未被保存。是否保存后关闭程序？", "保存内容", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    this.saveDocx();
            }
        }

        private void textBoxPath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void textBoxPath_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var file = ((String[])e.Data.GetData(DataFormats.FileDrop))[0];
                this.textBoxPath.Text = file.Substring(0, file.LastIndexOf("\\"));
            }
        }
        
    }
}
