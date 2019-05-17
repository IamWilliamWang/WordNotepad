using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace 日志书写器
{
    public partial class Form1 : Form
    {
        private float DocumentFontSize { get { return 12F; } } //文档字号
        private String DocumentFont { get { return "黑体"; } } //文档字体
        private int SavedCharLength { get; set; } = 0; //上次保存的字符串长度
        private readonly String[] dllNames = new String[] { "ICSharpCode.SharpZipLib.dll", "NPOI.dll", "NPOI.OOXML.dll", "NPOI.OpenXml4Net.dll", "NPOI.OpenXmlFormats.dll" };

        #region 启动与关闭操作
        public Form1()
        {
            InitializeComponent();
            // 将实际字体替代在设计器中显示的字体
            this.textBoxMain.Font = new System.Drawing.Font(DocumentFont, DocumentFontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 选定默认字体
            this.comboBoxFontSize.SelectedIndex = 3;
            if (File.Exists(GetDefaultDocumentFileName()))
                try
                {
                    Word wordRead = new Word(GetDefaultDocumentFileName());
                    this.textBoxMain.Lines = wordRead.ReadWordLines();
                    this.SavedCharLength = wordRead.ReadWord().Length;
                }
                catch(IOException)
                {
                    MessageBox.Show("读取失败，日志文件被占用，请在保存前关闭Microsoft Word软件！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }

        /// <summary>
        /// 所写的dll文件的索引
        /// </summary>
        /// <param name="dllIndex"></param>
        /// <returns></returns>
        private FileInfo WriteDllFile(int dllIndex)
        {
            switch (dllIndex)
            {
                case 0:
                    return BinaryFileWriter.WriteFileToDisk(Properties.Resources.ICSharpCode_SharpZipLib, dllNames[dllIndex]);
                case 1:
                    return BinaryFileWriter.WriteFileToDisk(Properties.Resources.NPOI, dllNames[dllIndex]);
                case 2:
                    return BinaryFileWriter.WriteFileToDisk(Properties.Resources.NPOI_OOXML, dllNames[dllIndex]);
                case 3:
                    return BinaryFileWriter.WriteFileToDisk(Properties.Resources.NPOI_OpenXml4Net, dllNames[dllIndex]);
                case 4:
                    return BinaryFileWriter.WriteFileToDisk(Properties.Resources.NPOI_OpenXmlFormats, dllNames[dllIndex]);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text += " v" + Program.Version(1);
            if (!File.Exists(dllNames[1]))
                for (int i = 0; i < dllNames.Length; i++)
                    WriteDllFile(i).Attributes = FileAttributes.Hidden;
        }
        /// <summary>
        /// 检查是否需要进行保存操作
        /// </summary>
        /// <returns></returns>
        private bool needSave() => this.textBoxMain.Text.Length != this.SavedCharLength;

        /// <summary>
        /// 关闭的时候检查保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (needSave())
                if (MessageBox.Show("有内容未被保存。是否保存后关闭程序？", "保存内容", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    this.SaveDocx();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.checkBoxMailbox.Checked)
                System.Diagnostics.Process.Start("https://mail.qq.com/");
        }
        #endregion

        #region 按钮点击操作
        /// <summary>
        /// 转换字号string为实际大小
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
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
        private string GetDefaultDocumentFileName()
        {
            string filename = "";
            if (this.textBoxPath.Text != "")
                filename += this.textBoxPath.Text + "\\";
            filename += String.Format("{0:0000}", DateTime.Now.Year);
            filename += String.Format("{0:00}", DateTime.Now.Month);
            filename += String.Format("{0:00}", DateTime.Now.Day);
            filename += "王劲翔.docx";
            return filename;
        }

        /// <summary>
        /// 保存docx文档
        /// </summary>
        private void SaveDocx()
        {
            Word word = new Word(GetDefaultDocumentFileName());
            if (this.textBoxFont.Text != this.DocumentFont)
                word.Font = this.textBoxFont.Text;
            else
                word.Font = this.DocumentFont;
            var nowFontSize = this.GetFontSizeFromText(this.comboBoxFontSize.Text);
            if (nowFontSize != this.DocumentFontSize)
                word.FontSize = (int)nowFontSize;
            else
                word.FontSize = (int)this.DocumentFontSize;
            word.WriteDocx(this.textBoxMain.Lines);
            this.SavedCharLength = this.textBoxMain.Text.Length;
        }

        private void button保存_Click(object sender, EventArgs e)
        {
            SaveDocx();
            MessageBox.Show("保存Word文档成功！");
        }
        #endregion
        
        #region 拖拽与选择选项操作
        private void textBoxPath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Move;
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
        #endregion
    }
}
