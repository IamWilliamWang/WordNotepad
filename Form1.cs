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
            this.comboBoxFontSize.SelectedIndex = 3;
        }

        private bool need2Save() => this.textBoxMain.Text != "" && !this.hasSaved;

        private float GetFontSizeFromText(string text)
        {
            if (text == "六号")
                return 7.5f;
            else if (text == "小五")
                return 9f;
            else if (text == "五号")
                return 10.5f;
            else if (text == "小四")
                return 12f;
            else if (text == "四号")
                return 14f;
            else if (text == "小三")
                return 15f;
            else if (text == "三号")
                return 16f;
            else if (text == "小二")
                return 18f;
            else if (text == "二号")
                return 22f;
            else if (text == "小一")
                return 24f;
            else if (text == "一号")
                return 26f;
            return 10.5f;
        }

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
            var nowFontSize = this.GetFontSizeFromText(this.comboBoxFontSize.Text);
            if (nowFontSize != this.DocumentFontSize)
                word.FontSize = (int)nowFontSize;
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

        private void comboBoxFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.textBoxMain.Font = new System.Drawing.Font(this.textBoxFont.Text, this.GetFontSizeFromText(this.comboBoxFontSize.Text));
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.checkBoxMailbox.Checked)
                System.Diagnostics.Process.Start("https://mail.qq.com/");
        }

        private FileInfo WriteDll(int dllIndex, string[] dllNameList)
        {
            switch (dllIndex)
            {
                case 0:
                    return BinaryFileWriter.WriteFileToDisk(Properties.Resources.ICSharpCode_SharpZipLib, dllNameList[dllIndex]);
                case 1:
                    return BinaryFileWriter.WriteFileToDisk(Properties.Resources.NPOI, dllNameList[dllIndex]);
                case 2:
                    return BinaryFileWriter.WriteFileToDisk(Properties.Resources.NPOI_OOXML, dllNameList[dllIndex]);
                case 3:
                    return BinaryFileWriter.WriteFileToDisk(Properties.Resources.NPOI_OpenXml4Net, dllNameList[dllIndex]);
                case 4:
                    return BinaryFileWriter.WriteFileToDisk(Properties.Resources.NPOI_OpenXmlFormats, dllNameList[dllIndex]);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            String[] dllNames = new String[] { "ICSharpCode.SharpZipLib.dll", "NPOI.dll", "NPOI.OOXML.dll", "NPOI.OpenXml4Net.dll", "NPOI.OpenXmlFormats.dll" };
            for(int i = 0; i < dllNames.Length; i++)
            {
                if(!File.Exists(dllNames[i]))
                {
                    WriteDll(i, dllNames).Attributes = FileAttributes.Hidden;
                }
            }
        }
    }
}
