using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace 日志书写器
{
    public partial class FormEdit : Form
    {
        #region 储存变量
        // 作者姓名
        public static String AuthorName { get; } = "王劲翔";
        // 文档字号（数字）
        private float DocumentFontSize { get { return this.GetFontSizeFromText(DocumentFontSizeZh); } }
        // 文档字号（中文）
        private string DocumentFontSizeZh { get; set; } = "小四";
        // 文档显示字体
        private String DocumentFont { get { return "黑体"; } }
        // 上次保存的字符串长度（不包含换行）
        private int SavedCharLength { get; set; } = 0;
        // 各动态链接库的名称
        private readonly String[] dllNames = new String[] { "ICSharpCode.SharpZipLib.dll", "NPOI.dll", "NPOI.OOXML.dll", "NPOI.OpenXml4Net.dll", "NPOI.OpenXmlFormats.dll" };
        // 多久秒自动保存一次
        private int AutoSavePerSecond { get; set; } = 30;
        // 全屏模式
        private bool FullScreen { get{ return !this.groupBoxSetting.Visible; } set{ if (value) FullScreenModeOn(); else FullScreenModeOff(); } }
        // 暗黑模式
        private bool DarkMode { get{ return this.暗黑模式ToolStripMenuItem.Text != "暗黑模式"; } set{ if (value) DarkModeOn(); else DarkModeOff(); } }
        // 自动保存Timer
        private BackupCreater backup { get; set; }
        // 保存最后一次成功搜索的内容
        private string LastSearch { get; set; } = "";
        private KeyEventArgs LastKeyDown { get; set; }
        #endregion

        #region 启动与关闭
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
        private void CreateBackupCreater()
        {
            backup = new BackupCreater(GetDefaultDocumentFileName(), (writeFilename) => this.SaveBackup(writeFilename));
            backup.Interval = AutoSavePerSecond * 1000;
            backup.Backup后缀名 = ".autosave";
        }

        /// <summary>
        /// 恢复自动备份文件
        /// </summary>
        private void RestoreAutosave()
        {
            try
            {
                Word wordRead = new Word(backup.Backup文件名);
                this.textBoxMain.Lines = wordRead.ReadWordLines();
                if (File.Exists(backup.Original文件名))
                    this.SavedCharLength = new Word(backup.Original文件名).Length;
            }
            catch (IOException)
            {
                MessageBox.Show("读取失败，日志文件被占用，请关闭Microsoft Word软件后再打开本软件！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.checkBoxMailbox.Checked = false;
                Application.Exit();
            }
        }

        private void FormEdit_Load(object sender, EventArgs e)
        {
            // 检测后台是否运行同一程序
            if (Process.GetProcessesByName("TextWriter").Length > 1)
                if (DialogResult.No == MessageBox.Show("检测到后台已经启动本程序，强烈建议只开启一个本程序，否则可能会导致意外后果。\n请问是否继续启动？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    Environment.Exit(0);
            // 初始化自动备份
            CreateBackupCreater();
            // 加版本号
            this.Text += " v" + Program.Version(1);
            // 将实际字体替代在设计器中显示的字体
            this.textBoxMain.Font = new System.Drawing.Font(DocumentFont, DocumentFontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 选定默认字体
            for (int i = 0; i < this.comboBoxFontSize.Items.Count; i++)
            {
                string sizeItem = this.comboBoxFontSize.Items[i].ToString();
                if (sizeItem == this.DocumentFontSizeZh)
                {
                    this.comboBoxFontSize.SelectedIndex = i;
                    break;
                }
            }
            // 有自动保存文件则恢复内容
            if (File.Exists(backup.Backup文件名))
            {
                if (DialogResult.Yes == MessageBox.Show("检测到上次程序运行发生崩溃，是否还原自动保存的内容？", "还原请求", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                    backup.RestoreFile(RestoreAutosave, true);
            }
            // 有保存的文件则直接加载内容
            else if (File.Exists(backup.Original文件名))
            {
                LoadDocx(backup.Original文件名);
            }
            // 如果没有dll文件就解压文件
            if (!File.Exists(dllNames[1]))
                for (int i = 0; i < dllNames.Length; i++)
                    WriteDllFile(i).Attributes = FileAttributes.Hidden;
            // 光标点在文本最后
            this.textBoxMain.Select(this.textBoxMain.Text.Length, 0);
            // 晚上时间开启暗黑模式
            if (DateTime.Now.Hour >= 21 || DateTime.Now.Hour <= 9)
                DarkMode = true;
            // 加右键菜单项
            this.contextMenuStripMain.Items.Clear();
            this.contextMenuStripMain.Items.Add(插入tToolStripMenuItem);
            this.contextMenuStripMain.Items.Add(查找ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add("-");
            this.contextMenuStripMain.Items.Add(剪切ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add(复制ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add(粘贴ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add(删除ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add("-");
            this.contextMenuStripMain.Items.Add(全屏模式ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add(暗黑模式ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add(自动聚焦ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add(停用备份ToolStripMenuItem);
            // 启动自动保存计时器
            backup.Start();
        }

        /// <summary>
        /// 加载指定的docx文档到文本框
        /// </summary>
        /// <param name="docxFileName"></param>
        private void LoadDocx(string docxFileName)
        {
            try
            {
                Word wordRead = new Word(docxFileName);
                this.textBoxMain.Lines = wordRead.ReadWordLines();
                string readFontText = this.GetTextFromFontSize(wordRead.FontSize);
                int selectedIndex = new List<String>(this.fontTexts).IndexOf(readFontText);
                if (selectedIndex != -1)
                    this.comboBoxFontSize.SelectedIndex = selectedIndex;
                this.SavedCharLength = wordRead.Length;
            }
            catch (IOException)
            {
                MessageBox.Show("读取失败，日志文件被占用，请关闭Microsoft Word软件后再打开本软件！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.checkBoxMailbox.Checked = false;
                Application.Exit();
            }
        }
        
        /// <summary>
        /// 检查是否需要进行保存操作
        /// </summary>
        /// <returns></returns>
        private bool NeedSave() => this.textBoxMain.Text.Replace("\r", "").Replace("\n", "").Length != this.SavedCharLength;

        /// <summary>
        /// 关闭的时候检查保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (NeedSave())
            {
                var dialogResult = MessageBox.Show("有内容未被保存。是否保存后关闭程序？", "保存内容", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (dialogResult == DialogResult.Yes)
                    this.SaveDocument();
                else if (dialogResult == DialogResult.Cancel)
                    e.Cancel = true;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            backup.DeleteBackup();
            if (this.checkBoxMailbox.Checked)
                System.Diagnostics.Process.Start("https://mail.qq.com/");
        }
        #endregion

        #region 按钮点击
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

        private readonly String[] fontTexts = new String[] { "六号", "小五", "五号", "小四", "四号", "小三", "三号", "小二", "二号", "小一", "一号" };
        private readonly float[] fontSizes = new float[] { 7.5f, 9f, 10.5f, 12f, 14f, 15f, 16f, 18f, 22f, 24f, 26f };
        /// <summary>
        /// 转换字号string为实际大小
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private float GetFontSizeFromText(string text)
        {
            int myIndex = new List<String>(fontTexts).IndexOf(text);
            if (myIndex == -1)
                return 10.5f;
            else
                return fontSizes[myIndex];
        }

        private String GetTextFromFontSize(float fontSize)
        {
            int myIndex = new List<float>(this.fontSizes).IndexOf(fontSize);
            if (myIndex == -1)
                return "五号";
            else
                return fontTexts[myIndex];
        }

        /// <summary>
        /// 获得默认的文件名
        /// </summary>
        /// <returns></returns>
        private string GetDefaultDocumentFileName()
        {
            string filename = "";
            //if (this.textBoxPath.Text != "" && this.textBoxPath.Text[textBoxPath.Text.Length - 1] != '\\')
            //    filename += this.textBoxPath.Text + "\\";
            filename += String.Format("{0:0000}", DateTime.Now.Year);
            filename += String.Format("{0:00}", DateTime.Now.Month);
            filename += String.Format("{0:00}", DateTime.Now.Day);
            filename += AuthorName + ".docx";
            return filename;
        }

        /// <summary>
        /// 保存docx文档
        /// </summary>
        private void SaveDocument()
        {
            SaveDocx(backup.Original文件名);
        }

        /// <summary>
        /// 保存autosave文档
        /// </summary>
        /// <param name="backupFileName"></param>
        private void SaveBackup(string backupFileName)
        {
            SaveDocx(backupFileName, false);
        }

        /// <summary>
        /// 保存文档的具体实现
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

        /// <summary>
        /// 保存文档
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button保存_Click(object sender, EventArgs e)
        {
            this.SaveDocument();
            backup.DeleteBackup();
            MessageBox.Show("保存Word文档成功！", "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion
        
        #region groupbox内其他操作
        private void textBoxPath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Move;
        }

        /// <summary>
        /// 根据文件名判断是文件还是文件夹，返回"File"或"Directory"
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
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
                backup.WorkingDirectory = this.textBoxPath.Text;
            }
        }

        private void comboBoxFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.textBoxMain.Font = new System.Drawing.Font(this.textBoxFont.Text, this.GetFontSizeFromText(this.comboBoxFontSize.Text));
        }
        #endregion

        #region 双击操作
        private void Form_DoubleClick(object sender, EventArgs e)
        {
            FullScreenModeSwitch();
        }
        #endregion

        #region 键盘快捷键
        private void textBoxMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Control)
            {
                this.SaveDocument();
                backup.DeleteBackup();
            }
            if (e.KeyCode == Keys.A && e.Control)
                this.textBoxMain.SelectAll();
            if (e.KeyCode == Keys.O && e.Control)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "文档|*.docx";
                openFileDialog.Multiselect = false;
                openFileDialog.Title = "打开docx文档";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    this.LoadSpecificDocument(openFileDialog.FileName);
            }
            this.LastKeyDown = e;
        }
        #endregion

        #region 全屏模式、暗黑模式、自动聚焦
        /// <summary>
        /// 打开暗黑模式，推荐使用DarkMode = true代替
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
            this.暗黑模式ToolStripMenuItem.Text = "取消暗黑";
        }

        /// <summary>
        /// 关闭暗黑模式，推荐使用DarkMode = false代替
        /// </summary>
        private void DarkModeOff()
        {
            this.textBoxMain.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxMain.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxPath.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxPath.ForeColor = System.Drawing.SystemColors.WindowText;
            this.button保存.BackColor = System.Drawing.SystemColors.Control;
            this.button保存.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button保存.UseVisualStyleBackColor = true;
            this.buttonTopMost.BackColor = System.Drawing.SystemColors.Control;
            this.buttonTopMost.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonTopMost.UseVisualStyleBackColor = true;
            this.comboBoxFontSize.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxFontSize.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxFont.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxFont.ForeColor = System.Drawing.SystemColors.WindowText;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.暗黑模式ToolStripMenuItem.Text = "暗黑模式";
        }

        /// <summary>
        /// 切换暗黑模式状态
        /// </summary>
        private void DarkModeSwitch()
        {
            DarkMode = DarkMode ? false : true;
        }

        /// <summary>
        /// 打开全屏模式，推荐使用FullScreen = true代替
        /// </summary>
        private void FullScreenModeOn()
        {
            int height = this.textBoxMain.Size.Height;
            int width = textBoxMain.Size.Width;
            if (height == 0 || width == 0)
                return;
            this.groupBoxSetting.Visible = false;
            this.textBoxMain.Location = new System.Drawing.Point(13, 7);
            this.textBoxMain.Size = new System.Drawing.Size(width, height + 50);
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.全屏模式ToolStripMenuItem.Text = "取消全屏";
        }

        /// <summary>
        /// 关闭全屏模式，推荐使用FullScreen = false代替
        /// </summary>
        private void FullScreenModeOff()
        {
            int height = this.textBoxMain.Size.Height;
            int width = textBoxMain.Size.Width;
            if (height == 0 || width == 0)
                return;
            this.groupBoxSetting.Visible = true;
            this.textBoxMain.Location = new System.Drawing.Point(12, 59);
            this.textBoxMain.Size = new System.Drawing.Size(width, height - 50);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.全屏模式ToolStripMenuItem.Text = "全屏模式";
        }

        /// <summary>
        /// 切换全屏模式状态
        /// </summary>
        private void FullScreenModeSwitch()
        {
            FullScreen = FullScreen ? false : true;
        }

        private void textBoxMain_MouseHover(object sender, EventArgs e)
        {
            int selectStart = this.textBoxMain.SelectionStart; // 保存位置
            MouseSimulator.DoMouseClick(); // 鼠标点击
            this.textBoxMain.Select(selectStart, 0); // 还原位置
        }
        #endregion

        #region 自动ScrollBars、绑定全屏模式、绑定自动聚焦
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

        private void InsertKey(object insertContent)
        {
            SendKeys.SendWait(insertContent.ToString());
            SendKeys.SendWait("{LEFT}");
            this.textBoxMain.Select(textBoxMain.SelectionStart, 0);
        }

        private bool FastInsert(char keydown)
        {
            //string lefts = "(（—[【{<《\"“";
            //string rights = ")）—]】}>》\"”";
            string lefts = "(（[【{<《“";
            string rights = ")）]】}>》”";
            int index = lefts.IndexOf(keydown);
            if (index == -1)
                return false;
            //if ("—\"".IndexOf(keydown) != -1) //防止无限循环
            //    ClearEvent(this.textBoxMain, "TextChanged");
            InsertKey("{"+rights[index]+"}");
            //if ("—\"".IndexOf(keydown) != -1)
            //    this.textBoxMain.TextChanged += textBoxMain_TextChanged;
            return true;
        }

        private void textBoxMain_TextChanged(object sender, EventArgs e)
        {
            AutoScrollBar();
            if (this.textBoxMain.SelectionStart == 0) 
                return;
            var key = LastKeyDown.KeyCode;
            if (key == Keys.Back || key == Keys.Delete || key == Keys.Enter || key == Keys.Left || key == Keys.Right || key == Keys.Up || key == Keys.Down)
                return;
            char recentCh = textBoxMain.Text[this.textBoxMain.SelectionStart - 1];
            FastInsert(recentCh);
        }

        private void AutoScrollBar()
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
                FullScreen = true;
        }

        /// <summary>
        /// 删除指定控件的指定事件
        /// </summary>
        /// <param name="control"></param>
        /// <param name="eventname"></param>
        private void ClearEvent(System.Windows.Forms.Control control, string eventname)
        {
            if (control == null) return;
            if (string.IsNullOrEmpty(eventname)) return;

            BindingFlags mPropertyFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;
            BindingFlags mFieldFlags = BindingFlags.Static | BindingFlags.NonPublic;
            Type controlType = typeof(System.Windows.Forms.Control);
            PropertyInfo propertyInfo = controlType.GetProperty("Events", mPropertyFlags);
            System.ComponentModel.EventHandlerList eventHandlerList = (System.ComponentModel.EventHandlerList)propertyInfo.GetValue(control, null);
            FieldInfo fieldInfo = (typeof(System.Windows.Forms.Control)).GetField("Event" + eventname, mFieldFlags);
            Delegate d = eventHandlerList[fieldInfo.GetValue(control)];

            if (d == null) return;
            EventInfo eventInfo = controlType.GetEvent(eventname);

            foreach (Delegate dx in d.GetInvocationList())
                eventInfo.RemoveEventHandler(control, dx);

        }

        private void 自动聚焦ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.自动聚焦ToolStripMenuItem.Text == "自动聚焦")
            {
                this.textBoxMain.MouseHover += textBoxMain_MouseHover;
                this.自动聚焦ToolStripMenuItem.Text = "取消聚焦";
            }
            else
            {
                ClearEvent(this.textBoxMain, "MouseHover");
                this.自动聚焦ToolStripMenuItem.Text = "自动聚焦";
            }
        }
        #endregion

        #region 右键菜单
        private void 插入tToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBoxMain.Text = textBoxMain.Text.Insert(textBoxMain.SelectionStart, "　　");
        }

        private void 查找ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string search = Interaction.InputBox("请输入从光标处要查找的内容", defaultText: LastSearch);
            if (search == "")
                return;
            int index = this.textBoxMain.Text.IndexOf(search, this.textBoxMain.SelectionStart);
            if (index == -1)
            {
                if (DialogResult.No == MessageBox.Show("未找到。是否从头开始查找？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                    return;

                index = textBoxMain.Text.IndexOf(search, 0);
                if (index == -1)
                {
                    MessageBox.Show("仍然未找到");
                    return;
                }
            }
            LastSearch = search;
            this.textBoxMain.Select(index, search.Length);
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

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{DEL}");
        }

        private void 全屏模式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FullScreenModeSwitch();
        }

        private void 暗黑模式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DarkModeSwitch();
        }

        private void 停用备份ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Text == "停用备份")
            {
                this.backup.Stop();
                ((ToolStripMenuItem)sender).Text = "启用备份";
            }
            else if (((ToolStripMenuItem)sender).Text == "启用备份")
            {
                this.backup.Start();
                ((ToolStripMenuItem)sender).Text = "停用备份";
            }
        }
        #endregion

        /// <summary>
        /// 模拟鼠标点击事件
        /// </summary>
        class MouseSimulator
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

            private const int MOUSEEVENTF_LEFTDOWN = 0x02;
            private const int MOUSEEVENTF_LEFTUP = 0x04;
            private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
            private const int MOUSEEVENTF_RIGHTUP = 0x10;

            public static void DoMouseClick()
            {
                //Call the imported function with the cursor's current position
                uint X = (uint)Cursor.Position.X;
                uint Y = (uint)Cursor.Position.Y;
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
            }
        }

        private void FormEdit_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void LoadSpecificDocument(string docFileName)
        {
            this.LoadDocx(docFileName);
            if (this.Text.Contains("("))
                this.Text = this.Text.Substring(0, this.Text.IndexOf('(')) + " (" + docFileName + ")";
            else
                this.Text += " (" + docFileName + ")";
            backup.Stop();
            backup.WorkingDirectory = this.textBoxPath.Text;
            backup.Original文件名 = docFileName;
            backup.Start();
        }

        private void FormEdit_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string file = ((String[])e.Data.GetData(DataFormats.FileDrop))[0];
                string type = FileOrDirectory(file);
                if (type == "File")
                {
                    if (file.EndsWith(".docx"))
                    {
                        // 同步路径
                        textBoxPath_DragDrop(sender, e);
                        this.LoadSpecificDocument(file);
                    }
                    else
                        MessageBox.Show("只允许加载docx文件！", "加载失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void 应用修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.backup.WorkingDirectory = this.textBoxPath.Text;
            MessageBox.Show("修改成功！");
        }
    }
}
