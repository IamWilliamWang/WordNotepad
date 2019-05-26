using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace 日志书写器
{
    public partial class FormEdit : Form
    {
        public static String AuthorName { get; } = "王劲翔";
        private float DocumentFontSize { get { return 12F; } } //文档字号
        private String DocumentFont { get { return "黑体"; } } //文档字体
        private int SavedCharLength { get; set; } = 0; //上次保存的字符串长度
        private readonly String[] dllNames = new String[] { "ICSharpCode.SharpZipLib.dll", "NPOI.dll", "NPOI.OOXML.dll", "NPOI.OpenXml4Net.dll", "NPOI.OpenXmlFormats.dll" };
        private bool FullScreen { get; set; } = false;
        private Timer autoSaveTimer;
        private string lastSearch = "";

        #region 启动与关闭操作
        public FormEdit()
        {
            InitializeComponent();
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

        /// <summary>
        /// 创建并开启自动保存计时器
        /// </summary>
        private void CreateAutoSaveTimer()
        {
            autoSaveTimer = new Timer();
            autoSaveTimer.Interval = 30000;
            autoSaveTimer.Tick += (sender, e) =>
            {
                this.SaveDocx(GetDefaultDocumentFileName().Replace(".docx", ".autosave"), false);
                //if (File.Exists(GetDefaultDocumentFileName()))
                //    this.SavedCharLength = new Word(GetDefaultDocumentFileName()).Length;
            };
        }

        /// <summary>
        /// 打开暗黑模式
        /// </summary>
        private void DarkModeOn()
        {
            this.textBoxMain.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBoxMain.ForeColor = System.Drawing.SystemColors.Window;
            this.textBoxPath.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBoxPath.ForeColor = System.Drawing.SystemColors.Window;
            this.button保存.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.button保存.ForeColor = System.Drawing.SystemColors.Window;
            this.button保存.UseVisualStyleBackColor = false;
            this.buttonTopMost.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.buttonTopMost.ForeColor = System.Drawing.SystemColors.Window;
            this.buttonTopMost.UseVisualStyleBackColor = false;
            this.comboBoxFontSize.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBoxFontSize.ForeColor = System.Drawing.SystemColors.Window;
            this.textBoxFont.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBoxFont.ForeColor = System.Drawing.SystemColors.Window;
            this.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.ForeColor = System.Drawing.SystemColors.Window;
        }

        private void FormEdit_Load(object sender, EventArgs e)
        {
            // 加版本号
            this.Text += " v" + Program.Version(1);
            // 将实际字体替代在设计器中显示的字体
            this.textBoxMain.Font = new System.Drawing.Font(DocumentFont, DocumentFontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 选定默认字体
            this.comboBoxFontSize.SelectedIndex = 3;
            // 有自动保存文件则恢复内容
            if (File.Exists(GetDefaultDocumentFileName().Replace(".docx", ".autosave")))
            {
                if (DialogResult.Yes == MessageBox.Show("检测到上次程序运行发生崩溃，是否还原自动保存的内容？", "还原请求", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    try
                    {
                        Word wordRead = new Word(GetDefaultDocumentFileName().Replace(".docx", ".autosave"));
                        this.textBoxMain.Lines = wordRead.ReadWordLines();
                        if(File.Exists(GetDefaultDocumentFileName()))
                            this.SavedCharLength = new Word(GetDefaultDocumentFileName()).Length;
                        File.Delete(GetDefaultDocumentFileName().Replace(".docx", ".autosave"));
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("读取失败，日志文件被占用，请关闭Microsoft Word软件后再打开本软件！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.checkBoxMailbox.Checked = false;
                        Application.Exit();
                    }
                    
                }
            }
            // 有保存的文件则直接加载内容
            else if (File.Exists(GetDefaultDocumentFileName()))
            {
                try
                {
                    Word wordRead = new Word(GetDefaultDocumentFileName());
                    this.textBoxMain.Lines = wordRead.ReadWordLines();
                    this.SavedCharLength = wordRead.Length;
                }
                catch (IOException)
                {
                    MessageBox.Show("读取失败，日志文件被占用，请关闭Microsoft Word软件后再打开本软件！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.checkBoxMailbox.Checked = false;
                    Application.Exit();
                }
            }
            // 如果没有dll文件就解压文件
            if (!File.Exists(dllNames[1]))
                for (int i = 0; i < dllNames.Length; i++)
                    WriteDllFile(i).Attributes = FileAttributes.Hidden;
            // 光标点在文本最后
            this.textBoxMain.Select(this.textBoxMain.Text.Length, 0);
            // 启动自动保存计时器
            CreateAutoSaveTimer();
            autoSaveTimer.Start();
            // 晚上时间开启暗黑模式
            if (DateTime.Now.Hour >= 21 || DateTime.Now.Hour <= 9)
                DarkModeOn();
        }

        /// <summary>
        /// 检查是否需要进行保存操作
        /// </summary>
        /// <returns></returns>
        private bool NeedSave() => this.textBoxMain.Text.Replace("\r","").Replace("\n","").Length != this.SavedCharLength;

        /// <summary>
        /// 关闭的时候检查保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (NeedSave())
                if (MessageBox.Show("有内容未被保存。是否保存后关闭程序？", "保存内容", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    this.SaveDocx();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            File.Delete(GetDefaultDocumentFileName().Replace(".docx", ".autosave"));
            if (this.checkBoxMailbox.Checked)
                System.Diagnostics.Process.Start("https://mail.qq.com/");
        }
        #endregion

        #region 按钮点击操作
        private void buttonTopMost_Click(object sender, EventArgs e)
        {
            Button topMostButton = (Button)sender;
            if (topMostButton.Text == "窗口置顶")
            {
                this.TopMost = true;
                topMostButton.Text = "取消置顶";
            }
            else
            {
                this.TopMost = false;
                topMostButton.Text = "窗口置顶";
            }
        }

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

        /// <summary>
        /// 获得默认的文件名
        /// </summary>
        /// <returns></returns>
        private string GetDefaultDocumentFileName()
        {
            string filename = "";
            if (this.textBoxPath.Text != "" && this.textBoxPath.Text[textBoxPath.Text.Length - 1] != '\\')
                filename += this.textBoxPath.Text + "\\";
            filename += String.Format("{0:0000}", DateTime.Now.Year);
            filename += String.Format("{0:00}", DateTime.Now.Month);
            filename += String.Format("{0:00}", DateTime.Now.Day);
            filename += AuthorName + ".docx";
            return filename;
        }

        /// <summary>
        /// 保存docx文档
        /// </summary>
        private void SaveDocx()
        {
            SaveDocx(GetDefaultDocumentFileName());
        }

        /// <summary>
        /// 保存docx文档
        /// </summary>
        private void SaveDocx(string docxName, bool changeSavedCharLength = true)
        {
            Word word = new Word(docxName);
            if (this.textBoxFont.Text != this.DocumentFont)
                word.Font = this.textBoxFont.Text;
            else
                word.Font = this.DocumentFont;
            var nowFontSize = this.GetFontSizeFromText(this.comboBoxFontSize.Text);
            if (nowFontSize != this.DocumentFontSize)
                word.FontSize = (int)nowFontSize;
            else
                word.FontSize = (int)this.DocumentFontSize;
            try
            {
                word.WriteDocx(this.textBoxMain.Lines);
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("保存目录不存在，请重新填写正确的保存目录！", "保存警告", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.textBoxPath.Text = "";
            }
            if(changeSavedCharLength)
                this.SavedCharLength = this.textBoxMain.Text.Replace("\r","").Replace("\n","").Length;
        }

        private void button保存_Click(object sender, EventArgs e)
        {
            this.SaveDocx();
            File.Delete(GetDefaultDocumentFileName().Replace(".docx", ".autosave"));
            MessageBox.Show("保存Word文档成功！", "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion
        
        #region groupbox内其他操作
        private void textBoxPath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Move;
        }

        private string FileOrDirectory(string filename)
        {
            int lastRightSlashIndex = filename.LastIndexOf('\\');
            string shortFilename;
            if (lastRightSlashIndex == -1)
                shortFilename = filename;
            else
                shortFilename = filename.Substring(lastRightSlashIndex + 1);
            if (shortFilename.Contains("."))
                return "File";
            else
                return "Directory";
        }

        private void textBoxPath_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string file = ((String[])e.Data.GetData(DataFormats.FileDrop))[0];
                string type = FileOrDirectory(file);
                if (type == "File")
                    this.textBoxPath.Text = file.Substring(0, file.LastIndexOf("\\"));
                else
                    this.textBoxPath.Text = file;
            }
        }

        private void comboBoxFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.textBoxMain.Font = new System.Drawing.Font(this.textBoxFont.Text, this.GetFontSizeFromText(this.comboBoxFontSize.Text));
        }

        #endregion

        #region 双击操作
        private void FullScreenModeOn(object sender)
        {
            if (!FullScreen)
            {
                int height = this.textBoxMain.Size.Height;
                int width = textBoxMain.Size.Width;
                if (height == 0 || width == 0)
                    return;
                this.groupBoxSetting.Visible = false;
                this.textBoxMain.Location = new System.Drawing.Point(13, 7);
                this.textBoxMain.Size = new System.Drawing.Size(width, height + 50);
                this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                FullScreen = true;
            }
            else
            {
                int height = this.textBoxMain.Size.Height;
                int width = textBoxMain.Size.Width;
                if (height == 0 || width == 0)
                    return;
                this.groupBoxSetting.Visible = true;
                this.textBoxMain.Location = new System.Drawing.Point(12, 59);
                this.textBoxMain.Size = new System.Drawing.Size(width, height - 50);
                this.FormBorderStyle = FormBorderStyle.Sizable;
                FullScreen = false;
            }
        }

        private void Form_DoubleClick(object sender, EventArgs e)
        {
            FullScreenModeOn(sender);
        }
        #endregion

        private void textBoxMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Control)
            {
                this.SaveDocx();
                File.Delete(GetDefaultDocumentFileName().Replace(".docx", ".autosave"));
            }
            if (e.KeyCode == Keys.A && e.Control)
                this.textBoxMain.SelectAll();
        }

        /// <summary>
        /// 显示在屏幕上有多少行字
        /// </summary>
        private int ShowedTextLines
        {
            get
            {
                double 每行文字实际高度 = 16.65 / GetFontSizeFromText("小四") * GetFontSizeFromText(this.comboBoxFontSize.Text);
                return (int)(this.textBoxMain.ClientSize.Height / 每行文字实际高度);
            }
        }

        private void textBoxMain_TextChanged(object sender, EventArgs e)
        {
            int lineCount = this.textBoxMain.GetLineFromCharIndex(this.textBoxMain.Text.Length) + 1;
            // 当总行数大于显示，显示ScrollBar
            if (this.textBoxMain.ScrollBars == ScrollBars.None && lineCount > ShowedTextLines) // 提高执行效率
                this.textBoxMain.ScrollBars = ScrollBars.Vertical;
            // 当总行数小于显示，隐藏ScrollBar
            if (this.textBoxMain.ScrollBars == ScrollBars.Vertical && lineCount < ShowedTextLines) // 提高执行效率
                this.textBoxMain.ScrollBars = ScrollBars.None;
        }

        private void FormEdit_Resize(object sender, EventArgs e)
        {
            // 如果上方空间太挤，自动打开全屏模式
            if (this.textBoxPath.ClientSize.Width == 0 && !FullScreen)
                this.FullScreenModeOn(sender);
        }

        private void 插入tToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBoxMain.Text = textBoxMain.Text.Insert(textBoxMain.SelectionStart, "　　");
        }

        private void 剪切ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^{x}");
        }

        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^{c}");
        }

        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^{v}");
        }

        private void 全屏模式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FullScreenModeOn(sender);
        }

        private void 暗黑模式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DarkModeOn();
        }

        private void 查找ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string search = Interaction.InputBox("请输入从光标处要查找的内容", defaultText: lastSearch);
            if (search == "")
                return;
            int index = this.textBoxMain.Text.IndexOf(search, this.textBoxMain.SelectionStart);
            if (index == -1)
            {
                if (DialogResult.Yes == MessageBox.Show("未找到。是否从头开始查找？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    index = textBoxMain.Text.IndexOf(search, 0);
                    if (index == -1)
                    {
                        MessageBox.Show("仍然未找到");
                        return;
                    }
                }
            }
            lastSearch = search;
            this.textBoxMain.Select(index, search.Length);
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{DEL}");
        }
    }
}
