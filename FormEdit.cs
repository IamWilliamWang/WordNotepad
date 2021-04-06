using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace 日志书写器
{
    public partial class FormEdit : Form
    {
        #region 控制器
        /// <summary>
        /// 控制文档字号（数字）
        /// </summary>
        private float DocumentFontSize { get { return this.GetFontSizeFromText(DocumentFontSizeZh); } }

        /// <summary>
        /// 控制文档字号（中文）
        /// </summary>
        private string DocumentFontSizeZh { get { return fontSizeZh; } set { fontSizeZh = value; this.textBoxMain.Font = new System.Drawing.Font(DocumentFont, DocumentFontSize); } } //先更新值然后用新值更新textBox

        /// <summary>
        /// 控制文档显示字体
        /// </summary>
        private String DocumentFont
        {
            get { return font; }
            set
            {
                font = value;
                this.textBoxMain.Font = new System.Drawing.Font(DocumentFont, DocumentFontSize);
                if (this.comboBoxFont.Text != font) //始终保持Font与textBoxFont同步
                    this.comboBoxFont.Text = font;
            }
        }

        /// <summary>
        /// 控制全屏模式的开关
        /// </summary>
        private bool FullScreen { get { return !this.groupBoxSetting.Visible; } set { if (value) FullScreenModeOn(); else FullScreenModeOff(); } }

        /// <summary>
        /// 控制暗黑模式的开关
        /// </summary>
        private bool DarkMode { get { return this.暗黑主题ToolStripMenuItem.Text != "暗黑主题"; } set { if (value) DarkModeOn(); else DarkModeOff(); } }

        /// <summary>
        /// 自动保存Timer的开关（会同步右键菜单的文字）
        /// </summary>
        public bool AutoSaverRunning
        {
            get { return this.AutoSaver.IsBusy; }
            set
            {
                if (value != this.AutoSaver.IsBusy)
                    this.自动保存ToolStripMenuItem_Click(null, null);
            }
        }

        /// <summary>
        /// 自动备份Timer的开关（会同步右键菜单的文字）
        /// </summary>
        public bool AutoBackupRunning
        {
            get { return this.AutoBackup.IsBusy; }
            set
            {
                if (value != this.AutoBackup.IsBusy)
                    this.自动备份ToolStripMenuItem_Click(null, null);
            }
        }
        /* 以下基本变量只能在本region内使用！！ */
        private string fontSizeZh = "小四";
        private string font = "黑体";
        #endregion

        #region 一般变量
        /// <summary>
        /// 作者姓名
        /// </summary>
        public static String AuthorName { get; } = Hex2ChiEngString("&#xe7;&#x8e;&#x8b;&#xe5;&#x8a;&#xb2;&#xe7;&#xbf;&#x94;");

        /// <summary>
        /// 上次保存的文档字符串长度（不包含换行）
        /// </summary>
        private int savedCharLength = 0;

        /// <summary>
        /// 上次保存的备份字符串长度（不包含换行）
        /// </summary>
        private int savedBackupCharLength = 0;

        /// <summary>
        /// 多久秒自动保存、备份一次
        /// </summary>
        private int TimerIntervalSecond { get; set; } = 30;

        /// <summary>
        /// 保存最后一次成功搜索的内容
        /// </summary>
        private string LastSearch { get; set; } = "";

        /// <summary>
        /// 保存最后一次按下的Key
        /// </summary>
        private KeyEventArgs LastKeyDown { get; set; }

        /// <summary>
        /// 是否要一直锁定垂直滚动条的状态
        /// </summary>
        private bool LockScrollBarStatus { get; set; } = true;

        /// <summary>
        /// 是否要一直锁定全屏模式的状态
        /// </summary>
        private bool LockFullScreenMode { get; set; } = true;

        /// <summary>
        /// 储存拖入的文件是否为只读文件。key=文件名，value=是否只读
        /// </summary>
        private Dictionary<string, bool> readOnly = new Dictionary<string, bool>();

        /// <summary>
        /// 自动备份Timer（储存了所有的文件信息）
        /// </summary>
        private BackupCreater AutoBackup { get; set; }

        /// <summary>
        /// 自动保存Timer（不存储任何的文件信息，想获取需要访问AutoBackup）
        /// </summary>
        private BackupCreater AutoSaver { get; set; }

        /// <summary>
        /// 保存Undo、Redo功能要显示的内容
        /// </summary>
        private FormerSaver<String> former = new FormerSaver<String>();

        /// <summary>
        /// 获得FormEdit单例
        /// </summary>
        public static FormEdit Instance { get { return instance; } }

        /// <summary>
        /// 已保存的FormEdit实例
        /// </summary>
        private static FormEdit instance;

        /// <summary>
        /// 准确表达执行结果的枚举
        /// </summary>
        public enum Result { Done, Failed, Skipped, Canceled };

        /// <summary>
        /// 程序运行前拖入的文件名
        /// </summary>
        private String 传入的文件名;

        /// <summary>
        /// 上一次加载的txt文件名
        /// </summary>
        private String lastLoadedTxtFile;

        private Encoding currentTxtEncoding = Encoding.UTF8;
        #endregion

        #region 启动与关闭
        /// <summary>
        /// 构造函数
        /// </summary>
        public FormEdit(String[] args = null)
        {
            InitializeComponent();
            instance = this; // 保存实例
            if (args != null && args.Length == 1)
                传入的文件名 = args[0].Replace("\"", "");
        }

        /// <summary>
        /// 解压检测不到的dll文件
        /// </summary>
        /// <param name="dllIndex">所写的dll文件的索引</param>
        /// <returns></returns>
        private void WriteDllFiles()
        {
            String[] dllNames = new String[] { "ICSharpCode.SharpZipLib.dll", "NPOI.dll", "NPOI.OOXML.dll", "NPOI.OpenXml4Net.dll", "NPOI.OpenXmlFormats.dll" };
            for (int dllIndex = 0; dllIndex < dllNames.Length; dllIndex++)
            {
                var dllName = dllNames[dllIndex];
                // 如果没有dll文件就解压文件
                if (File.Exists(dllName) == false)
                {
                    switch (dllIndex)
                    {
                        case 0:
                            BinaryFileWriter.WriteFileToDisk(DecompressBytes(Properties.Resources.ICSharpCode_SharpZipLib_dll), dllName)
                                .Attributes = FileAttributes.Hidden;
                            break;
                        case 1:
                            BinaryFileWriter.WriteFileToDisk(DecompressBytes(Properties.Resources.NPOI_dll), dllName)
                                .Attributes = FileAttributes.Hidden;
                            break;
                        case 2:
                            BinaryFileWriter.WriteFileToDisk(DecompressBytes(Properties.Resources.NPOI_OOXML_dll), dllName)
                                .Attributes = FileAttributes.Hidden;
                            break;
                        case 3:
                            BinaryFileWriter.WriteFileToDisk(DecompressBytes(Properties.Resources.NPOI_OpenXml4Net_dll), dllName)
                                .Attributes = FileAttributes.Hidden;
                            break;
                        case 4:
                            BinaryFileWriter.WriteFileToDisk(DecompressBytes(Properties.Resources.NPOI_OpenXmlFormats_dll), dllName)
                                .Attributes = FileAttributes.Hidden;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        /// <summary>
        /// 获得默认的文档文件名
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
        /// 检查是否需要进行保存操作
        /// </summary>
        /// <returns></returns>
        private bool NeedSave() => this.textBoxMain.Text.Replace("\r", "").Replace("\n", "").Length != this.savedCharLength;

        /// <summary>
        /// 程序被意外终止后恢复文件
        /// </summary>
        private void RestoreAutosave()
        {
            try
            {
                Word wordRead = new Word(AutoBackup.Backup文件名, AuthorName);
                this.textBoxMain.Lines = wordRead.ReadWordLines();
                if (File.Exists(AutoBackup.Original文件名))
                    this.savedCharLength = new Word(AutoBackup.Original文件名, AuthorName).Length;
            }
            catch (IOException)
            {
                MessageBox.Show("读取失败，日志文件被占用，请关闭Microsoft Word软件后再打开本软件！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.checkBoxMailbox.Checked = false;
                Application.Exit();
            }
        }

        /// <summary>
        /// 是否存在进程名为processName的进程
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        private bool ExistProcess(string processName)
        {
            return Process.GetProcessesByName(processName).Length > 1;
        }

        /// <summary>
        /// 创建自动保存计时器（使用默认的参数或传入的参数）
        /// </summary>
        /// <param name="data"></param>
        private void CreateAutoSaver(BackupCreaterFactory.Data data = null)
        {
            /* 没指定就提取创建记录，有指定就使用data进行创建 */
            if (data == null && BackupCreaterFactory.GetDataById(BackupCreaterFactory.ID_SAVE) == null)
                AutoSaver = BackupCreaterFactory.CreateAutoSaver(GetDefaultDocumentFileName(), (docxFile) => SaveDocument(), TimerIntervalSecond * 1000); // 直接传入SaveDocument，架空内部命名逻辑，只使用其中的计时器，避免意外。
            else
                AutoSaver = BackupCreaterFactory.CreateAutoSaver(data);
        }

        /// <summary>
        /// 创建自动备份计时器
        /// </summary>
        private void CreateAutoBackup(BackupCreaterFactory.Data data = null)
        {
            /* 没指定就提取创建记录，有指定就使用data进行创建 */
            if (data == null && BackupCreaterFactory.GetDataById(BackupCreaterFactory.ID_BACKUP) == null)
                AutoBackup = BackupCreaterFactory.CreateAutoBackup(GetDefaultDocumentFileName(), (backupFileName) => { SaveBackup(backupFileName); former.SaveText(textBoxMain.Text); }, TimerIntervalSecond * 1000, ".autosave");
            else
                AutoBackup = BackupCreaterFactory.CreateAutoBackup(data);
        }

        /// <summary>
        /// 加载窗体准备的工作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormEdit_Load(object sender, EventArgs e)
        {
            // 只有日志书写器要检测后台是否运行同一程序
            if (Program.LogWriter)
                if (ExistProcess(Assembly.GetExecutingAssembly().GetName().Name) || ExistProcess("TextWriter") || ExistProcess("日志书写器"))
                    if (DialogResult.No == MessageBox.Show("检测到后台已经启动本程序，强烈建议只开启一个本程序，否则可能会导致意外后果。\n请问是否继续启动？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                        Environment.Exit(0);
            // 设置ControlStyle为双缓冲，可以避免界面频繁闪烁。Set the value of the double-buffering style bits to true. 
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
            // 初始化自动备份
            CreateAutoBackup();
            // 初始化自动保存
            CreateAutoSaver();
            // 加版本号
            Title.Version = Program.Version();
            // 状态栏内的锁初始化
            this.SwitchScrollBarLockStatus();
            this.SwitchFullScreenLockStatus();
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
            if (Program.LogWriter) // 只有日志书写器要执行的预处理模块
            {
                // 有autosave文件则恢复内容
                if (File.Exists(AutoBackup.Backup文件名))
                {
                    if (DialogResult.Yes == MessageBox.Show("检测到上次程序运行发生崩溃，是否还原自动保存的内容？", "还原请求", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                        AutoBackup.RestoreFile(RestoreAutosave, true);
                }
                // 有保存的文档则直接加载内容
                else if (File.Exists(AutoBackup.Original文件名))
                {
                    LoadDocx(AutoBackup.Original文件名);
                }
            }
            // 解压dll文件
            WriteDllFiles();
            // 光标点在文本最后
            this.textBoxMain.Select(this.textBoxMain.Text.Length, 0);
            // 晚上、早上开启暗黑模式
            if (DateTime.Now.Hour >= 23 || DateTime.Now.Hour <= 8)
                DarkMode = true;
            // 加右键菜单项
            this.contextMenuStripMain.Items.Clear();
            this.contextMenuStripMain.Items.Add(新建文档ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add(打开文档ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add(中文空格ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add(查找内容ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add(插入链接ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add(替换文本ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add("-");
            this.contextMenuStripMain.Items.Add(剪切ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add(复制ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add(粘贴ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add(删除ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add("-");
            this.contextMenuStripMain.Items.Add(窗口置顶ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add(精简模式ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add(暗黑主题ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add(自动聚焦ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add(自动保存ToolStripMenuItem);
            this.contextMenuStripMain.Items.Add(自动备份ToolStripMenuItem);
            // 显示工作路径
            this.textBoxPath.Text = AutoBackup.WorkingDirectory;
            // 不同模式的初始化
            if (Program.LogWriter)
            {
                // 启动自动保存计时器
                if (!IsReadOnly())
                    this.AutoBackupRunning = true;
                Title.TitleName = "日志书写器";
            }
            else
            {
                Title.TitleName = "Word 记事本";
                Title.Untitled = true;
                // 停用并禁止掉所有的自动保存和自动备份（在SaveTo和LoadSpecificDocument函数中解封）
                this.AutoBackupRunning = false;
                this.AutoSaverRunning = false;
                this.自动备份ToolStripMenuItem.Text = "启用备份";
                this.自动备份ToolStripMenuItem.Enabled = false;
                this.自动保存ToolStripMenuItem.Text = "自动保存";
                this.自动保存ToolStripMenuItem.Enabled = false;
                // 不是日志管理器就隐藏打开邮箱按钮和高级选项
                this.checkBoxMailbox.Checked = false;
                this.checkBoxMailbox.Visible = false;
                this.button高级设置.Visible = false;
                this.textBoxPath.Size = new Size(488, textBoxPath.Size.Height);
            }
            if (this.传入的文件名 != null)
                this.LoadNewDocx(this.传入的文件名);
        }

        /// <summary>
        /// 加载指定的docx文档到文本框，并修改字号、字体、字数，更新存储字数。（不操作任何Timer）
        /// </summary>
        /// <param name="docxFileName"></param>
        private Result LoadDocx(string docxFileName)
        {
            try
            {
                Word wordRead = new Word(docxFileName, AuthorName);
                this.textBoxMain.Lines = wordRead.ReadWordLines();
                // 读取字号
                string readFontText = this.GetTextFromFontSize(wordRead.FontSize);
                int selectedIndex = new List<String>(ConstVariables.FONT_TEXTS).IndexOf(readFontText);
                if (selectedIndex != -1)
                    this.comboBoxFontSize.SelectedIndex = selectedIndex;
                // 读取字体
                this.DocumentFont = wordRead.Font;
                // 储存字数
                this.savedCharLength = wordRead.Length;
                return Result.Done;
            }
            catch (IOException)
            {
                MessageBox.Show("读取失败，日志文件被占用，请关闭Microsoft Word软件后再打开本软件！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.checkBoxMailbox.Checked = false;
                Application.Exit();
                return Result.Failed;
            }
        }

        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncodingUsingBOM(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16 LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16 BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.Default;
        }

        /// <summary>
        /// 将字符编码显示在状态栏中
        /// </summary>
        /// <param name="encoding">字符编码</param>
        /// <param name="withBOM">如果是UTF8，是否包含BOM</param>
        private static void ShowEncoding(Encoding encoding, bool withBOM = false)
        {
            FormEdit.Instance.currentTxtEncoding = encoding; // 记录当前encoding

            var label = FormEdit.Instance.toolStripStatusLabelEncoding;
            if (encoding == null)
                label.Text = "";
            if (encoding == Encoding.ASCII)
                label.Text = "ASCII";
            else if (encoding == Encoding.UTF8)
                label.Text = (withBOM ? "带有 BOM 的 " : "") + "UTF-8";
            else if (encoding == Encoding.Default)
                label.Text = "GBK";
            else if (encoding == Encoding.UTF7)
                label.Text = "UTF-7";
            else if (encoding == Encoding.Unicode)
                label.Text = "UTF-16 LE";
            else if (encoding == Encoding.BigEndianUnicode)
                label.Text = "UTF-16 BE";
            else if (encoding == Encoding.UTF32)
                label.Text = "UTF-32";
        }

        /// <summary>
        /// 检测filename文本的字符编码。支持UTF-8、UTF-8 with BOM、GBK、UTF-16 LE、UTF-16 BE、UTF-7、UTF-32
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Encoding GetEncoding(string filename)
        {
            var nonDefaultEncoding = GetEncodingUsingBOM(filename);
            if (nonDefaultEncoding != Encoding.Default) // 由于上方法会将无BOM的UTF-8当作GBK，所以要另外处理
            {
                ShowEncoding(nonDefaultEncoding, true);
                return nonDefaultEncoding;
            }
            // 以下所有代码是判断的default(GBK)还是UTF8 without BOM
            //var dialogText = new StringBuilder("请输入以下文字中正常的一行前面对应的数字。\r\n");
            //本函数注释的遗留代码是弹窗选择编码。抛弃的原因是GBK和UTF8可以自动比较，即使编码错误也可以手动选择编码重新加载txt
            var unicodeRegex = new Regex(@"[\u4E00-\u9FFF，。？！；：‘’“”【】（）—·]");
            using (var binaryReader = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read)))
            {
                var bytes = binaryReader.ReadBytes(10000); // 读取所有内容，防止最后一个字符是unicode
                var utf8String = Encoding.UTF8.GetString(bytes); // 解析成UTF8
                var gbkString = Encoding.Default.GetString(bytes); // 解析成GBK
                //dialogText.Append("1：");
                var countUnicodeInUTF8 = 0;
                var countUnicodeInGBK = 0;
                for (int i = 0; i < utf8String.Length; i++)
                {
                    if (countUnicodeInUTF8 > 20)
                        break;
                    if (!unicodeRegex.IsMatch(utf8String[i] + "")) // 跳过所有非Unicode字符
                        continue;
                    //dialogText.Append(utf8String[i]);
                    countUnicodeInUTF8++;
                }
                //dialogText.AppendLine();
                //dialogText.Append("2：");
                for (int i = 0; i < gbkString.Length; i++)
                {
                    if (countUnicodeInGBK > 20)
                        break;
                    if (!unicodeRegex.IsMatch(gbkString[i] + "")) // 跳过所有非Unicode字符
                        continue;
                    //dialogText.Append(gbkString[i]);
                    countUnicodeInGBK++;
                }
                // 排除法能确定的话直接就确定了
                if (countUnicodeInUTF8 == 0 || countUnicodeInGBK == 0)
                {
                    // 如果发现两者都没找到unicode字符，则返回ASCII
                    if (countUnicodeInUTF8 == 0 && countUnicodeInGBK == 0)
                    {
                        ShowEncoding(Encoding.ASCII);
                        return Encoding.ASCII;
                    }
                    ShowEncoding(countUnicodeInUTF8 == 0 ? Encoding.Default : Encoding.UTF8);
                    return countUnicodeInUTF8 == 0 ? Encoding.Default : Encoding.UTF8;
                }
                //dialogText.AppendLine();
            }
            //if ("2" == Interaction.InputBox(title: "无法检测文本编码", content: dialogText.ToString(), charCountPerline: 10000).Trim()) // charCountPerline较大等同于禁用自动换行
            //{
            //    ShowEncoding(Encoding.Default);
            //    return Encoding.Default;
            //}
            ShowEncoding(Encoding.UTF8);
            return Encoding.UTF8;
        }

        /// <summary>
        /// 加载txt文件，改变存储字数（不修改任何Timer）
        /// </summary>
        /// <param name="txtFileName"></param>
        private Result LoadTxt(string txtFileName, Encoding encoding = null)
        {
            // 当重命名状态下更改字符会发生这种情况
            if (txtFileName == null)
                return Result.Skipped;

            try
            {
                if (encoding == null)
                    encoding = GetEncoding(txtFileName);
                this.textBoxMain.Lines = File.ReadAllLines(txtFileName, encoding);
                this.savedCharLength = 0;
                foreach (var str in this.textBoxMain.Lines)
                    this.savedCharLength += str.Length;
                return Result.Done;
            }
            catch (IOException)
            {
                throw;
            }
        }

        /// <summary>
        /// 关闭时做的操作，保存未保存的文档
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 如果要保存，询问是否保存
            if (NeedSave())
            {
                var dialogResult = MessageBox.Show("有内容未被保存。是否保存后关闭程序？", "保存内容", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (dialogResult == DialogResult.Yes)
                {
                    if (Title.Untitled)
                        this.button保存_Click(sender, e);
                    else
                        this.SaveDocument();
                }
                else if (dialogResult == DialogResult.Cancel)
                    e.Cancel = true;
            }
        }

        /// <summary>
        /// 关闭后的操作，用于打开邮箱
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormEdit_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 删除缓存，打开邮箱
            DeleteBackup();
            if (this.checkBoxMailbox.Checked)
                System.Diagnostics.Process.Start("https://mail.qq.com/");
        }
        #endregion

        #region 按钮点击
        /// <summary>
        /// 高级设置按钮按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button高级设置_Click(object sender, EventArgs e)
        {
            new FormSettings().ShowDialog();
        }

        /// <summary>
        /// 检测WorkingDirectory是否有未提交的修改，并询问是否提交修改
        /// </summary>
        private void WorkingDirectorySynchronize()
        {
            if (this.textBoxPath.Text.Replace("\\", "") != AutoBackup.WorkingDirectory.Replace("\\", ""))
            {
                // 暂停计时器等待用户的选择
                using (PauseAutoBackup)
                using (PauseAutoSaver)
                {
                    // 弹出选择
                    if (DialogResult.Yes == MessageBox.Show("检测到文件路径未被更新。如果想改变文件路径为 " + this.textBoxPath.Text + " 请点“是”，如果想保持文件路径为 " + AutoBackup.WorkingDirectory + " 请点“否”", "文件路径不一致检测", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                        this.textBoxPath_KeyDown(null, new KeyEventArgs(Keys.Enter));
                    else
                        this.textBoxPath.Text = AutoBackup.WorkingDirectory;
                    // 恢复计时器
                }
            }
        }

        /// <summary>
        /// 转换字号string为实际大小（仅限黑体）。与GetTextFromFontSize互逆
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private float GetFontSizeFromText(string text)
        {
            int myIndex = new List<String>(ConstVariables.FONT_TEXTS).IndexOf(text);
            if (myIndex == -1)
                return 10.5f;
            else
                return ConstVariables.FONT_SIZES[myIndex];
        }

        /// <summary>
        /// 转换实际大小到字号string（仅限黑体）。与GetFontSizeFromText互逆
        /// </summary>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        private String GetTextFromFontSize(float fontSize)
        {
            int myIndex = new List<float>(ConstVariables.FONT_SIZES).IndexOf(fontSize);
            if (myIndex == -1)
                return "五号";
            else
                return ConstVariables.FONT_TEXTS[myIndex];
        }

        /// <summary>
        /// 执行保存docx文档（委托调用）
        /// </summary>
        private Result SaveDocument()
        {
            return SaveDocx(AutoBackup.Original文件名);
        }

        /// <summary>
        /// 执行保存备份（委托调用）
        /// </summary>
        /// <param name="backupFileName"></param>
        private Result SaveBackup(string backupFileName)
        {
            return SaveDocx(backupFileName, false);
        }

        /// <summary>
        /// 执行保存文档的兼容接口
        /// </summary>
        /// <param name="docxName">要保存的文件名</param>
        /// <param name="changeSavedCharLength">是否修改SavedCharLength，取决于调用时是对docx操作还是对autosave操作</param>
        /// <returns></returns>
        private Result SaveDocx(string docxName, bool changeSavedCharLength = true)
        {
            /* 原先此函数未考虑备份保存时判断跳过，所以没有使用SavedBackupCharLength。此接口为兼容接口，继续使用changeSavedCharLength来判断保存的是文档还是备份 */
            if (changeSavedCharLength) // 保存的是文档
                return SaveDocx(docxName, ref this.savedCharLength);
            else // 保存的是备份
                return SaveDocx(docxName, ref this.savedBackupCharLength);
        }

        /// <summary>
        /// 执行保存文档的内部实现。执行写入docx文件并根据需要更新存储字数
        /// </summary>
        /// <param name="docxName">要保存的文件名</param>
        /// <param name="changedCharLength">要被修改的charLength</param>
        /// <param name="changeCharLength">是否需要修改changedCharLength</param>
        /// <returns></returns>
        private Result SaveDocx(string docxName, ref int changedCharLength, bool changeCharLength = true)
        {
            // 检查文件路径是否一致
            WorkingDirectorySynchronize();
            // 如果不需要保存就返回Skipped
            if (this.textBoxMain.Text != "" && changedCharLength == this.textBoxMain.Text.Replace("\r", "").Replace("\n", "").Length)
                return Result.Skipped;
            // 不要写入只读文件
            if (new FileInfo(docxName).Attributes == FileAttributes.ReadOnly)
            {
                MessageBox.Show("请勿写入只读文件！", "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return Result.Failed;
            }
            // 开始调用Word类写入
            Word word = new Word(docxName, AuthorName);
            // 设置字体和字号
            if (this.comboBoxFont.Text != this.DocumentFont)
                word.Font = this.comboBoxFont.Text;
            else
                word.Font = this.DocumentFont;
            var nowFontSize = this.GetFontSizeFromText(this.comboBoxFontSize.Text);
            if (nowFontSize != this.DocumentFontSize)
                word.FontSize = (int)nowFontSize;
            else
                word.FontSize = (int)this.DocumentFontSize;
            // 写入文件
            try
            {
                word.WriteDocx(this.textBoxMain.Lines);
            }
            catch (DirectoryNotFoundException) // 目录不存在
            {
                MessageBox.Show("保存目录不存在，请重新填写正确的保存目录！", "保存警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.textBoxPath.Text = "";
                return Result.Failed;
            }
            catch (UnauthorizedAccessException) // 权限不够
            {
                if (DialogResult.OK == MessageBox.Show("保存失败！因为软件没有足够的权限，可以使用管理员身份重启本程序。是否同意如此操作？", "保存警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Error))
                {
                    this.checkBoxMailbox.Checked = false;
                    RestartWithAdminRight(true);
                }
                else
                {
                    this.textBoxPath.Text = Directory.GetCurrentDirectory();
                    this.AutoBackup.WorkingDirectory = Directory.GetCurrentDirectory();
                }
                return Result.Failed;
            }
            // 如果需要修改存储长度就修改它
            if (changeCharLength)
                changedCharLength = this.textBoxMain.Text.Replace("\r", "").Replace("\n", "").Length;
            return Result.Done;
        }

        /// <summary>
        /// 在Word记事本的未命名状态下按下保存事件
        /// </summary>
        /// <returns></returns>
        private Result SaveTo()
        {
            // 保存
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Word文档|*.docx|文本文档|*.txt";
            saveFileDialog.Title = "保存到";
            saveFileDialog.FileName = "新建 Microsoft Word 文档";
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return Result.Canceled;
            // 保存txt文件时只需要写入文件
            if (saveFileDialog.FileName.EndsWith(".txt"))
            {
                MessageBox.Show("已将文档保存到 " + saveFileDialog.FileName + " ，字符编码为" + WriteTxtFile(saveFileDialog.FileName, this.textBoxMain.Text) + "。", "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return Result.Done;
            }

            Result saveResult = SaveDocx(saveFileDialog.FileName);
            if (saveResult == Result.Done)
                MessageBox.Show("已将文档保存到 " + saveFileDialog.FileName + " ！", "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                MessageBox.Show("发生了未知错误，保存失败", "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return Result.Failed;
            }
            // 重复拖入文档执行的操作，但是简化了判断只读和清除记录
            Title.SpecifiedDocumentFullName = saveFileDialog.FileName;
            // 生成新的计时器
            AutoBackup = BackupCreaterFactory.CreateAutoBackup(saveFileDialog.FileName, (docxFile) => SaveBackup(docxFile), TimerIntervalSecond * 1000, ".autosave");
            // 更新文件路径框
            UpdatePath(AutoBackup.WorkingDirectory);
            // 重新打开计时器
            AutoBackupRunning = true;
            // 取消标题提示
            Title.Untitled = false;
            // 保存完后启用备份按钮
            this.自动备份ToolStripMenuItem.Enabled = true;
            this.自动保存ToolStripMenuItem.Enabled = true;
            // 恢复高级设置按钮并重置文件路径栏的宽度
            this.button高级设置.Visible = true;
            this.textBoxPath.Size = new Size(410, textBoxPath.Size.Height);
            return Result.Done;
        }

        /// <summary>
        /// 使用encoding编码写入txt文本文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private string WriteTxtFile(string filename, string content, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = currentTxtEncoding;
            bool withBOM = this.toolStripStatusLabelEncoding.Text.Contains("BOM");
            if (currentTxtEncoding == Encoding.UTF8 && !withBOM) 
                File.WriteAllText(filename, content);
            else
                File.WriteAllText(filename, content, encoding);
            if (encoding == Encoding.UTF8)
                return (withBOM ? "带有 BOM 的 " : "") + "UTF-8";
            else if (encoding == Encoding.Default)
                return "GBK";
            else if (encoding == Encoding.ASCII)
                return "ASCII";
            else if (encoding == Encoding.UTF7)
                return "UTF-7";
            else if (encoding == Encoding.Unicode)
                return "UTF-16 LE";
            else if (encoding == Encoding.BigEndianUnicode)
                return "UTF-16 BE";
            else if (encoding == Encoding.UTF32)
                return "UTF-32";
            throw new ArgumentException("Encoding illegal.");
        }

        /// <summary>
        /// 保存按钮按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button保存_Click(object sender, EventArgs e)
        {
            // Word记事本的未命名状态
            if (Title.Untitled)
            {
                this.SaveTo();
            }
            // Word记事本的已命名状态或者日志管理器状态。此时AutoSaver和AutoBackup一切正常
            else
            {
                Result saveResult = this.SaveDocument(); // 调用保存文档函数
                if (saveResult == Result.Done)
                {
                    DeleteBackup();
                    MessageBox.Show("保存Word文档成功！", "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (saveResult == Result.Skipped)
                    MessageBox.Show("无需保存，已跳过保存文档。如果仍然想保存，请点击强制保存", "已跳过", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 另存为按钮被按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Title.Untitled)
            {
                this.SaveTo();
                return;
            }
            // 另存为
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Word文档|*.docx|文本文档|*.txt";
            saveFileDialog.Title = "另存为文档";
            saveFileDialog.InitialDirectory = AutoBackup.WorkingDirectory;
            saveFileDialog.FileName = AutoBackup.Original文件名.Substring(AutoBackup.Original文件名.LastIndexOf("\\") + 1);
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog.FileName.EndsWith(".txt"))
                {
                    MessageBox.Show("已将文档另存为 " + saveFileDialog.FileName + " ，字符编码为" + WriteTxtFile(saveFileDialog.FileName, this.textBoxMain.Text) + "。", "另存为成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    savedCharLength = -1;
                    Result saveResult = SaveDocx(saveFileDialog.FileName);
                    if (saveResult == Result.Done)
                    {
                        DeleteBackup();
                        MessageBox.Show("已将文档另存为 " + saveFileDialog.FileName + " ！", "另存为成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("发生了未知错误，另存为失败", "另存为失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    // 以下的操作是拖入文档执行的操作，只是简化了判断只读和清除记录
                    Title.SpecifiedDocumentFullName = saveFileDialog.FileName;
                    // 停止计时器，修改源文件，开启计时器
                    using (PauseAutoBackup)
                    {
                        AutoBackup.Original文件名 = saveFileDialog.FileName;
                        // 更新文件路径框
                        this.textBoxPath.Text = AutoBackup.WorkingDirectory;
                        // 重新打开计时器
                    }
                }
            }
        }

        /// <summary>
        /// 强制保存被按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 强制保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            savedCharLength = -1;
            button保存_Click(sender, e);
        }

        /// <summary>
        /// 保存为终稿按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 保存并置为终稿ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.button保存_Click(sender, e);
            this.textBoxMain.Enabled = false;
        }
        #endregion

        #region 所有拖拽操作
        /// <summary>
        /// 拖拽进入文件路径框事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxPath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Move;
        }

        /// <summary>
        /// 根据文件名判断是文件还是文件夹，返回"File"或"Directory"。发生错误返回null
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string FileOrDirectory(string filename)
        {
            if (filename == "")
                return null;
            if (filename[filename.Length - 1] == '\\')
                return "Directory";
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

        /// <summary>
        /// 更新路径
        /// </summary>
        /// <param name="fileName"></param>
        private void UpdatePath(string fileName)
        {
            string type = FileOrDirectory(fileName);
            if (type == "File")
                this.textBoxPath.Text = fileName.Substring(0, fileName.LastIndexOf("\\"));
            else
                this.textBoxPath.Text = fileName;
            AutoBackup.WorkingDirectory = this.textBoxPath.Text;
            //BackupCreaterFactory.UpdateData(0);
            AutoSaver.WorkingDirectory = this.textBoxPath.Text;
            //BackupCreaterFactory.UpdateData(1);
        }

        /// <summary>
        /// 拖拽放置到文件路径框事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxPath_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                UpdatePath(((String[])e.Data.GetData(DataFormats.FileDrop))[0]);
            }
        }

        /// <summary>
        /// 拖拽入窗体任意（未注册事件的）区域事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormEdit_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        /// <summary>
        /// 判断是否为只读文件的快捷调用
        /// </summary>
        /// <returns></returns>
        public bool IsReadOnly()
        {
            return IsReadOnly(this.AutoBackup.Original文件名);
        }

        /// <summary>
        /// 是否为只读文件
        /// </summary>
        public bool IsReadOnly(String docxName)
        {
            if (docxName == "") // 用于AutoSaver是Empty时
                return false;
            // 如果没保存过就获取并保存
            if (!readOnly.ContainsKey(docxName))
                this.readOnly[docxName] = new FileInfo(docxName).Attributes.HasFlag(FileAttributes.ReadOnly);
            return this.readOnly[docxName];
        }

        /// <summary>
        /// 执行加载特定的docx文件到文本框，并修改操作目标文件
        /// </summary>
        /// <param name="docxFileName"></param>
        private Result LoadSpecificDocument(string docxFileName)
        {
            // 检查切换文件时是否需要先行保存
            if (NeedSave())
            {
                var selection = MessageBox.Show("当前文档检测到已被修改，是否保存后再打开新文档？", "修改提醒", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (selection == DialogResult.Yes)
                {
                    if (SaveDocx(this.AutoBackup.Original文件名) != Result.Done)
                    {
                        MessageBox.Show("保存失败，已经取消加载文档。");
                        return Result.Failed;
                    }
                }
                else if (selection == DialogResult.Cancel)
                    return Result.Canceled;
            }
            /* 开始执行加载过程 */
            // 更新文件路径框
            UpdatePath(docxFileName);
            // 加载前先清空保存的记录
            this.former.Clear();
            // 加载文档内容
            this.LoadDocx(docxFileName);
            // 加载完保存一下内容
            this.former.SaveText(this.textBoxMain.Text);
            // 修改标题为新的docx文件名
            Title.SpecifiedDocumentFullName = docxFileName;
            // 判断是否为只读文件
            if (IsReadOnly(docxFileName))
                Title.ReadOnly = true;
            else
                Title.ReadOnly = false;
            // 停止计时器，修改源文件，开启计时器
            AutoBackupRunning = false;
            AutoBackup.Original文件名 = docxFileName;
            //BackupCreaterFactory.UpdateData(0);
            using (PauseAutoSaver)
            {
                AutoSaver.Original文件名 = docxFileName;
                //BackupCreaterFactory.UpdateData(1);
            }
            // 启动AutoBackup并恢复禁用的按钮
            if (!IsReadOnly(docxFileName))
            {
                AutoBackupRunning = true;
                this.自动保存ToolStripMenuItem.Enabled = true;
                this.自动备份ToolStripMenuItem.Enabled = true;
            }
            // 取消标题未命名状态
            Title.Untitled = false;
            // 恢复高级设置按钮并重置文件路径栏的宽度
            this.button高级设置.Visible = true;
            this.textBoxPath.Size = new Size(410, textBoxPath.Size.Height);
            // 状态栏的字符编码被禁用
            ShowEncoding(null);
            return Result.Done;
        }

        /// <summary>
        /// 从txt或其他格式加载Text（委托调用）
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private Result LoadTxtFileWithCheck(string file)
        {
            try
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.Length > 20 * 1024 * 1024)
                {
                    MessageBox.Show("文件过大，请不要加载大于20M的文件！", "加载失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return Result.Failed;
                }
                LoadTxt(file); //加载txt内容但不改变操作目标文件（即不支持txt保存）
                this.lastLoadedTxtFile = file;
                if (fileInfo.Length > this.textBoxMain.Text.Length * 4 + 2) // 最长是UTF16的情况
                {
                    MessageBox.Show("只允许加载docx或文本文件！", "加载失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.textBoxMain.Text = "";
                    return Result.Failed;
                }
                return Result.Done;
            }
            catch (IOException)
            {
                throw;
            }
        }

        private Result LoadNewDocx(string file)
        {
            string type = FileOrDirectory(file);
            if (type == "File")
            {
                if (file.EndsWith(".docx"))
                    return this.LoadSpecificDocument(file);
                else
                    return this.LoadTxtFileWithCheck(file);
            }
            return Result.Failed;
        }

        /// <summary>
        /// 拖拽放置到窗体任意（未注册事件的）区域事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormEdit_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string file = ((String[])e.Data.GetData(DataFormats.FileDrop))[0];
                LoadNewDocx(file);
                if (File.Exists(AutoBackup.Backup文件名))
                {
                    using (PauseAutoBackup)
                        if (DialogResult.Yes == MessageBox.Show("检测到上次程序运行发生崩溃，是否还原自动保存的内容？", "还原请求", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                            AutoBackup.RestoreFile(RestoreAutosave, true);
                }
            }
        }
        #endregion

        #region groupbox内其他操作
        /// <summary>
        /// 字体框输入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxFont_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.comboBoxFont.Text = this.comboBoxFont.Text.Trim();
                this.DocumentFont = this.comboBoxFont.Text;
            }
        }

        /// <summary>
        /// 字体选择改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.DocumentFont = this.comboBoxFont.Text;
        }

        /// <summary>
        /// 字号框选项被改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.DocumentFontSizeZh = this.comboBoxFontSize.Text;
            AutoScrollBar();
        }

        /// <summary>
        /// 工作路径输入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (FileOrDirectory(this.textBoxPath.Text) == "File")
                {
                    MessageBox.Show("路径名有误！");
                    this.textBoxPath.Text = "";
                    return;
                }
                this.AutoBackup.WorkingDirectory = this.textBoxPath.Text;
                //BackupCreaterFactory.UpdateData(0);
                MessageBox.Show("文件路径已被成功修改为 " + this.AutoBackup.WorkingDirectory);
            }
        }
        #endregion

        #region 单双击操作
        /// <summary>
        /// 窗体双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormEdit_DoubleClick(object sender, EventArgs e)
        {
            FullScreenModeSwitch();
        }

        /// <summary>
        /// 工作路径双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxPath_DoubleClick(object sender, EventArgs e)
        {
            Process.Start("Explorer.exe", this.textBoxPath.Text);
        }

        /// <summary>
        /// 主界面单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxMain_Click(object sender, EventArgs e)
        {
            UpdateStatusStripInfo();
        }
        #endregion

        #region 键盘事件
        /// <summary>
        /// 转换文本框中的所有LF为CRLF
        /// </summary>
        /// <returns></returns>
        private Result ConvertLF2CRLFInTextBox()
        {
            var strBuilder = new StringBuilder(this.textBoxMain.Text);
            strBuilder.Replace(ConstVariables.CRLF, ConstVariables.LF); // 先把CRLF统一成LF
            strBuilder.Replace(ConstVariables.LF, ConstVariables.CRLF); // 再把LF变成CRLF达到转换的目的
            if (this.textBoxMain.Text.Length != strBuilder.Length)
            {
                if (MessageBox.Show("检测到存在LF，已将其转换为CRLF！如果想撤回转换，请点击取消，但是会导致文本布局混乱。", "换行符不统一", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                    return Result.Canceled;
                this.textBoxMain.Text = strBuilder.ToString();
                return Result.Done;
            }
            return Result.Skipped;
        }

        /// <summary>
        /// 执行撤销操作
        /// </summary>
        private void UndoText()
        {
            try
            {
                var peek = this.former.GetPeek();
                var oldContentText = this.former.Undo();
                if (peek == oldContentText && peek != this.textBoxMain.Text) // 如果按下Undo会永久丢失当前状态
                {
                    // 先重做
                    this.former.Redo();
                    // 保存进度后Undo，假装什么都没发生
                    this.former.SaveText(this.textBoxMain.Text);
                    this.former.Undo();
                }
                if (oldContentText != null)
                    this.textBoxMain.Text = oldContentText;
            }
            catch (ArgumentOutOfRangeException) { }
        }

        /// <summary>
        /// 执行重做操作
        /// </summary>
        private void RedoText()
        {
            var newContentText = this.former.Redo();
            if (newContentText != null)
                this.textBoxMain.Text = newContentText;
        }
        /// <summary>
        /// 删除屏幕中当前行内容
        /// </summary>
        /// <returns>被删除的内容</returns>
        private string RemoveCurrentRow()
        {
            StringBuilder stringBuilder = new StringBuilder(this.textBoxMain.Text);
            int firstCharIndex = 0;
            string removedText = "";
            using (SafeEdit)
            {
                firstCharIndex = TextBoxUtil.GetFirstCharIndexOfCurrentLine();
                try
                {
                    stringBuilder.Remove(firstCharIndex, TextBoxUtil.GetNowLineLength() + ConstVariables.CRLF.Length);
                    removedText = this.textBoxMain.Text.Substring(firstCharIndex, TextBoxUtil.GetNowLineLength() + ConstVariables.CRLF.Length);
                }
                catch (ArgumentOutOfRangeException)
                {
                    stringBuilder.Remove(firstCharIndex, TextBoxUtil.GetNowLineLength());
                    removedText = this.textBoxMain.Text.Substring(firstCharIndex, TextBoxUtil.GetNowLineLength()) + ConstVariables.CRLF;
                }
                finally
                {
                    this.textBoxMain.Text = stringBuilder.ToString();
                    this.former.SaveText(this.textBoxMain.Text);
                }
            }
            this.textBoxMain.Select(firstCharIndex, 0);
            return removedText;
        }

        /// <summary>
        /// 主编辑框输入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxMain_KeyDown(object sender, KeyEventArgs e)
        {
            // 按下回车时保存内容
            if (e.KeyCode == Keys.Enter)
                this.former.SaveText(this.textBoxMain.Text);
            // 只有全屏模式才执行的快捷键
            if (this.FullScreen)
            {
                // Ctrl + Alt + L 切换全屏锁状态
                if (e.Control && e.Alt && e.KeyCode == Keys.L)
                {
                    this.SwitchFullScreenLockStatus();
                    return;
                }
            }
            // Ctrl + Alt + H 隐匿窗口
            if (e.Control && e.Alt && e.KeyCode == Keys.H)
            {
                this.ShowInTaskbar = !this.ShowInTaskbar;
                return;
            }
            // Shift + Delete 删除本行并保存到剪切板
            if (e.Shift && e.KeyCode == Keys.Delete)
            {
                var removedStringWithCRLF = this.RemoveCurrentRow();
                Clipboard.SetText(removedStringWithCRLF);
                return;
            }
            // Ctrl + Alt + F 继续搜索
            if (e.Control && e.Alt && e.KeyCode == Keys.F)
            {
                if (LastSearch != "")
                {
                    this.textBoxMain.SelectionStart += 1;
                    SearchString(LastSearch);
                }
            }
            // Ctrl + Shift + Alt + D 进入调试模式
            if (e.Control && e.Shift && e.Alt && e.KeyCode == Keys.D)
            {
                Title.DebugInfo = "已进入调试模式";
                return;
            }
            // Ctrl + 某按键（一个按键）
            if (e.Control)
            {
                // Ctrl + S 保存文档
                if (e.KeyCode == Keys.S)
                {
                    if (Title.Untitled)
                    {
                        this.button保存_Click(sender, e);
                    }
                    else
                    {
                        this.SaveDocument();
                        DeleteBackup();
                    }
                }
                // Ctrl + A 全选文档
                else if (e.KeyCode == Keys.A)
                    this.textBoxMain.SelectAll();
                // Ctrl + O 打开文档
                else if (e.KeyCode == Keys.O)
                    this.打开文档ToolStripMenuItem_Click(sender, e);
                // Ctrl + F 查找内容
                else if (e.KeyCode == Keys.F)
                    this.SearchString();
                // Ctrl + Z 撤回
                else if (e.KeyCode == Keys.Z)
                    this.UndoText();
                // Ctrl + Y 重做
                else if (e.KeyCode == Keys.Y)
                    this.RedoText();
                // Ctrl + T 插入两个中文空格
                else if (e.KeyCode == Keys.T)
                    this.插入中文空格ToolStripMenuItem_Click(sender, e);
                // Ctrl + I 插入超链接
                else if (e.KeyCode == Keys.I)
                    this.插入链接ToolStripMenuItem_Click(sender, e);
                // Ctrl + D 删除本行
                else if (e.KeyCode == Keys.D)
                    this.RemoveCurrentRow();
                // Ctrl + V 粘贴文本
                else if (e.KeyCode == Keys.V)
                    this.former.SaveText(this.textBoxMain.Text);
            }
            // Alt + 某按键（一个按键）
            else if (e.Alt)
            {
                // Alt + ↑ 代码上移
                if (e.KeyCode == Keys.Up)
                {
                    if (this.ConvertLF2CRLFInTextBox() != Result.Done)
                    {
                        // 为了解决多行出现严重错误的问题，变换前先把字号改成及其小后保证每行为一段
                        using (SafeEdit)
                        {
                            try
                            {
                                // 储存本行和上面一行开始位置和内容
                                int nowLineStartAt = TextBoxUtil.GetFirstCharIndexOfCurrentLine();
                                int upperLineStartAt = TextBoxUtil.GetFirstCharIndexOfLine(TextBoxUtil.GetNowLineIndex() - 1);
                                String nowLine = TextBoxUtil.GetNowLine();
                                String upperLine = TextBoxUtil.GetLine(TextBoxUtil.GetNowLineIndex() - 1);
                                StringBuilder content = new StringBuilder(this.textBoxMain.Text);
                                // 交换两行内容
                                content.Remove(nowLineStartAt, nowLine.Length);
                                content.Insert(nowLineStartAt, upperLine);
                                content.Remove(upperLineStartAt, upperLine.Length);
                                content.Insert(upperLineStartAt, nowLine);
                                // 保存列号和选择长度
                                int savedColumnIndex = TextBoxUtil.GetColumnIndex();
                                int savedSelectionLength = this.textBoxMain.SelectionLength;
                                // 屏幕指针指向新位置
                                this.textBoxMain.Text = content.ToString();
                                this.textBoxMain.SelectionStart = upperLineStartAt + savedColumnIndex;
                                this.textBoxMain.SelectionLength = savedSelectionLength;
                            }
                            catch (System.Exception)
                            { }
                        }// 恢复字号
                    }
                }
                // Alt + ↓ 代码上移
                else if (e.KeyCode == Keys.Down)
                {
                    if (this.ConvertLF2CRLFInTextBox() != Result.Done)
                    {
                        // 为了解决多行出现严重错误的问题，变换前先把字号改成及其小后保证每行为一段
                        using (SafeEdit)
                        {
                            try
                            {
                                // 储存本行和下面一行开始位置和内容
                                int nowLineStartAt = TextBoxUtil.GetFirstCharIndexOfCurrentLine();
                                int lowerLineStartAt = TextBoxUtil.GetFirstCharIndexOfLine(TextBoxUtil.GetNowLineIndex() + 1);
                                String nowLine = TextBoxUtil.GetNowLine();
                                String lowerLine = TextBoxUtil.GetLine(TextBoxUtil.GetNowLineIndex() + 1);
                                StringBuilder content = new StringBuilder(this.textBoxMain.Text);
                                // 交换两行内容
                                content.Remove(lowerLineStartAt, lowerLine.Length);
                                content.Insert(lowerLineStartAt, nowLine);
                                content.Remove(nowLineStartAt, nowLine.Length);
                                content.Insert(nowLineStartAt, lowerLine);
                                // 保存列号和选择长度与新的行号
                                int savedSelectionLength = this.textBoxMain.SelectionLength;
                                int savedColumnIndex = TextBoxUtil.GetColumnIndex();
                                int newLineIndex = TextBoxUtil.GetNowLineIndex() + 1;
                                // 屏幕指针指向新位置
                                this.textBoxMain.Text = content.ToString();
                                this.textBoxMain.SelectionStart = TextBoxUtil.GetFirstCharIndexOfLine(newLineIndex) + savedColumnIndex;
                                this.textBoxMain.SelectionLength = savedSelectionLength;
                            }
                            catch (System.Exception)
                            { }
                        }// 恢复字号
                    }
                }
            }
            this.LastKeyDown = e; // 保存按下按键的内容
        }

        /// <summary>
        /// 主编辑框按钮按完操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxMain_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateStatusStripInfo();
            // Shift + Delete 需要还原被系统delete删掉的那个字符
            if (e.Shift && e.KeyCode == Keys.Delete)
                this.textBoxMain.Text = this.former.GetPeek();
        }
        #endregion

        #region 全屏模式、暗黑模式、自动聚焦
        /// <summary>
        /// （弃用）打开暗黑模式。请使用DarkMode = true代替本函数
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
            this.button高级设置.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.button高级设置.ForeColor = System.Drawing.SystemColors.Window;
            this.button高级设置.UseVisualStyleBackColor = false;
            this.comboBoxFontSize.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBoxFontSize.ForeColor = System.Drawing.SystemColors.Window;
            this.comboBoxFont.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBoxFont.ForeColor = System.Drawing.SystemColors.Window;
            this.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.ForeColor = System.Drawing.SystemColors.Window;
            this.statusStrip.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.statusStrip.ForeColor = System.Drawing.SystemColors.Window;
            this.暗黑主题ToolStripMenuItem.Text = "取消暗黑";
        }

        /// <summary>
        /// （弃用）关闭暗黑模式。请使用DarkMode = false代替本函数
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
            this.button高级设置.BackColor = System.Drawing.SystemColors.Control;
            this.button高级设置.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button高级设置.UseVisualStyleBackColor = true;
            this.comboBoxFontSize.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxFontSize.ForeColor = System.Drawing.SystemColors.WindowText;
            this.comboBoxFont.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxFont.ForeColor = System.Drawing.SystemColors.WindowText;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statusStrip.BackColor = System.Drawing.SystemColors.Control;
            this.statusStrip.ForeColor = System.Drawing.SystemColors.ControlText;
            this.暗黑主题ToolStripMenuItem.Text = "暗黑主题";
        }

        /// <summary>
        /// 切换暗黑模式状态
        /// </summary>
        private void DarkModeSwitch()
        {
            DarkMode = DarkMode ? false : true;
        }

        /// <summary>
        /// （弃用）打开全屏模式。请使用FullScreen = true代替本函数
        /// </summary>
        private void FullScreenModeOn()
        {
            if (LockFullScreenMode)
                return;
            int height = this.textBoxMain.Size.Height;
            int width = textBoxMain.Size.Width;
            if (height == 0 || width == 0)
                return;
            this.groupBoxSetting.Visible = false;
            this.textBoxMain.Location = new System.Drawing.Point(13, 7);
            this.textBoxMain.Size = new System.Drawing.Size(width, height + 65);
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.精简模式ToolStripMenuItem.Text = "普通模式";

            this.statusStrip.Visible = false;
        }

        /// <summary>
        /// （弃用）关闭全屏模式。请使用FullScreen = false代替本函数
        /// </summary>
        private void FullScreenModeOff()
        {
            if (LockFullScreenMode)
                return;
            int height = this.textBoxMain.Size.Height;
            int width = textBoxMain.Size.Width;
            if (height == 0 || width == 0)
                return;
            this.groupBoxSetting.Visible = true;
            this.textBoxMain.Location = new System.Drawing.Point(12, 59);
            this.textBoxMain.Size = new System.Drawing.Size(width, height - 65);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.精简模式ToolStripMenuItem.Text = "精简模式";

            this.statusStrip.Visible = true;
        }

        /// <summary>
        /// 切换全屏模式状态
        /// </summary>
        private void FullScreenModeSwitch()
        {
            FullScreen = FullScreen ? false : true;
        }

        /// <summary>
        /// 鼠标盘旋事件（用于自动聚焦功能）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxMain_MouseHover(object sender, EventArgs e)
        {
            int selectStart = this.textBoxMain.SelectionStart; // 保存位置
            MouseSimulator.DoMouseClick(); // 鼠标点击
            this.textBoxMain.Select(selectStart, 0); // 还原位置
        }
        #endregion

        #region 自动ScrollBars、绑定全屏模式、绑定自动聚焦
        /// <summary>
        /// 显示在屏幕上有多少行字（仅适用于黑体）
        /// </summary>
        private int ShowedTextLines
        {
            get
            {
                double 每行文字实际高度 = 16.65 / GetFontSizeFromText("小四") * GetFontSizeFromText(this.comboBoxFontSize.Text);
                return (int)(this.textBoxMain.ClientSize.Height / 每行文字实际高度);
            }
        }

        /// <summary>
        /// 向光标位置按规则插入字符串或按键
        /// </summary>
        /// <param name="insertContent">有规则字符串</param>
        private void InsertKey(object insertContentObj)
        {
            string insertContent = insertContentObj.ToString();
            try
            {
                SendKeys.SendWait(insertContent);
                SendKeys.SendWait("{LEFT}");
                this.textBoxMain.Select(textBoxMain.SelectionStart, 0);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                insertContent = insertContent.Substring(1, insertContent.Length - 2); //去掉{}
                int nowStart = textBoxMain.SelectionStart;
                this.textBoxMain.Text = this.textBoxMain.Text.Insert(textBoxMain.SelectionStart, insertContent);
                this.textBoxMain.Select(nowStart, 0);
            }
        }

        /// <summary>
        /// 快捷插入功能失效一次（用于阻止无限循环插入）
        /// </summary>
        private bool fastInsertDisabledOnce = false;

        /// <summary>
        /// 快捷插入的内部实现，负责查找需要查找的右半符号并执行插入
        /// </summary>
        /// <param name="keydown"></param>
        /// <returns></returns>
        private bool FastInsert(char keydown)
        {
            string lefts = ConstVariables.FAST_LEFTS;
            string rights = ConstVariables.FAST_RIGHTS;
            int index = lefts.IndexOf(keydown);
            if (index == -1)
                return false;
            // 由于InsertKey是递归调用，防止无限循环
            if (keydown == '\"')
                fastInsertDisabledOnce = true; // 禁止下一次快捷插入
            InsertKey("{" + rights[index] + "}");
            return true;
        }

        /// <summary>
        /// 计算original字符串中出现过几次value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private int ContainsCount<T>(String original, T value)
        {
            int count = 0;
            int startIndex = 0;
            while ((startIndex = original.IndexOf(value.ToString(), startIndex)) != -1)
            {
                count++;
                startIndex += value.ToString().Length;
                if (startIndex >= original.Length)
                    break;
            }
            return count;
        }

        /// <summary>
        /// 快捷删除的内部实现，自动判断是否需要删除多余的符号并执行
        /// </summary>
        private void FastDelete()
        {
            char leftCh;
            // 检测光标左边的char
            if (this.textBoxMain.SelectionStart > 0)
                leftCh = textBoxMain.Text[this.textBoxMain.SelectionStart - 1];
            else
                leftCh = '\0'; // 越界情况
            // 检测光标右边的char
            char rightCh;
            if (this.textBoxMain.SelectionStart < this.textBoxMain.TextLength)
                rightCh = textBoxMain.Text[this.textBoxMain.SelectionStart];
            else
                rightCh = '\0'; // 越界情况
            // 都越界则返回
            if (leftCh == '\0' && rightCh == '\0')
                return;
            // 右边有要删的东西，判断应不应该删
            if (ConstVariables.FAST_RIGHTS.IndexOf(rightCh) != -1)
            {
                bool delete = false;
                if (ConstVariables.FAST_LEFTS.IndexOf(leftCh) == -1 && ConstVariables.FAST_RIGHTS.IndexOf(rightCh) == -1)  // 左边是正常内容，就删
                    delete = true;
                else // 左边和右边都是成对符号，需要判断是否数量一样
                {
                    String nowLine = TextBoxUtil.GetNowLine(this.textBoxMain);
                    if (ContainsCount(nowLine, leftCh) != ContainsCount(nowLine, rightCh))
                        delete = true;
                }
                if (delete)
                {
                    InsertKey("{DELETE}");
                    this.textBoxMain.SelectionStart++;
                }
            }
            return;
        }

        /// <summary>
        /// 主编辑框字数改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxMain_TextChanged(object sender, EventArgs e)
        {
            // 如果被禁用一次说明现在不是用户输入，而是快捷插入在输入
            if (fastInsertDisabledOnce)
            {
                fastInsertDisabledOnce = false;
                return;
            }
            // 自动滚动条
            AutoScrollBar();
            // 按下这些键不要进行快速补全
            var key = LastKeyDown == null ? Keys.None : LastKeyDown.KeyCode;
            if (key == Keys.Delete || key == Keys.Enter || key == Keys.Left || key == Keys.Right || key == Keys.Up || key == Keys.Down || key == Keys.None)
                return;
            // 如果指针在首部（退格键除外）
            if (this.textBoxMain.SelectionStart == 0 && key != Keys.Back)
                return;
            // 删除右括号等
            if (key == Keys.Back)
            {
                try
                {
                    FastDelete();
                }
                catch // 失败就算了，不要报错
                { }
                return;
            }
            // 执行插入操作
            FastInsert(textBoxMain.Text[this.textBoxMain.SelectionStart - 1]);
        }

        /// <summary>
        /// 自动开启或关闭垂直滚动条
        /// </summary>
        private void AutoScrollBar()
        {
            if (this.LockScrollBarStatus)
                return;

            int lineCount = this.textBoxMain.GetLineFromCharIndex(this.textBoxMain.Text.Length) + 1;
            // 当总行数大于显示，显示ScrollBar
            if (this.textBoxMain.ScrollBars == ScrollBars.None && lineCount > ShowedTextLines) // 提高执行效率
                this.textBoxMain.ScrollBars = ScrollBars.Vertical;
            // 当总行数小于显示，隐藏ScrollBar
            if (this.textBoxMain.ScrollBars == ScrollBars.Vertical && lineCount < ShowedTextLines) // 提高执行效率
                this.textBoxMain.ScrollBars = ScrollBars.None;
        }

        /// <summary>
        /// 主窗体调整大小事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormEdit_Resize(object sender, EventArgs e)
        {
            // 如果上方空间太挤，自动打开全屏模式
            if (this.textBoxPath.ClientSize.Width == 0 && !FullScreen)
                FullScreen = true;
            // 自动滚动条
            AutoScrollBar();
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

        /// <summary>
        /// 自动聚焦按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// 新建文档按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 新建文档ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.checkBoxMailbox.Checked = false;
            Application.Restart();
        }

        /// <summary>
        /// 打开文档按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 打开文档ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "docx文档|*.docx|文本文档|*.txt|所有文件|*.*";
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "打开Docx文档";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.FileName.Contains(".docx"))
                    this.LoadSpecificDocument(openFileDialog.FileName);
                else
                    this.LoadTxtFileWithCheck(openFileDialog.FileName);
            }
        }

        /// <summary>
        /// 插入中文空格按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 插入中文空格ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int nowStart = textBoxMain.SelectionStart;
            this.textBoxMain.Text = textBoxMain.Text.Insert(textBoxMain.SelectionStart, "　　");
            this.textBoxMain.Select(nowStart + 2, 0);
            this.ConvertLF2CRLFInTextBox();
            this.former.SaveText(this.textBoxMain.Text);
        }

        /// <summary>
        /// 查找文本的内部实现
        /// </summary>
        /// <param name="search"></param>
        private void SearchString(string search = null)
        {
            if (search == null)
                search = Interaction.InputBox("请输入从光标处要查找的内容", defaultText: LastSearch);
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
                    MessageBox.Show("仍然未找到", "搜索结果", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            LastSearch = search;
            this.textBoxMain.Select(index, search.Length);
        }

        /// <summary>
        /// 查找内容按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 查找内容ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool savedSetting = this.TopMost;
            this.TopMost = false;
            SearchString();
            this.TopMost = savedSetting;
        }

        /// <summary>
        /// 插入链接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 插入链接ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.former.SaveText(this.textBoxMain.Text);
            if (ConvertLF2CRLFInTextBox() == Result.Done)
                return;

            bool savedSetting = this.TopMost;
            this.TopMost = false;
            try
            {
                int startAt = this.textBoxMain.SelectionStart;
                int length = this.textBoxMain.SelectionLength;
                string removedText = this.textBoxMain.Text.Substring(startAt, length);
                string inputString = Interaction.InputBox("输入链接网址Url：", hint: "提示：想取消插入请点击取消");
                if (inputString == "")
                {
                    return;
                }

                String newText = this.former.GetNewest().Insert(startAt, "[");
                newText = newText.Insert(startAt + length + 1, "](" + inputString + ")");
                this.textBoxMain.Text = newText;
            }
            catch (ArgumentOutOfRangeException) { }
            finally
            {
                this.TopMost = savedSetting;
            }
        }

        /// <summary>
        /// 替换文本按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 替换文本ToolStripMenuItem_Click(object sender, EventArgs e) =>
            FormReplacer.ShowReplacer(this.textBoxMain, this.former, this.TopMost);

        /// <summary>
        /// 剪切按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 剪切ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^{x}");
            this.former.SaveText(this.textBoxMain.Text);
        }

        /// <summary>
        /// 复制按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^{c}");
        }

        /// <summary>
        /// 粘贴按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^{v}");
            former.SaveText(this.textBoxMain.Text);
            this.ConvertLF2CRLFInTextBox();
        }

        /// <summary>
        /// 删除按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{DEL}");
        }

        /// <summary>
        /// 窗口置顶/取消置顶按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 窗口置顶ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.窗口置顶ToolStripMenuItem.Text == "窗口置顶")
            {
                this.TopMost = true;
                窗口置顶ToolStripMenuItem.Text = "取消置顶";
            }
            else
            {
                this.TopMost = false;
                窗口置顶ToolStripMenuItem.Text = "窗口置顶";
            }
        }

        /// <summary>
        /// 精简模式/普通模式按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 精简模式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FullScreenModeSwitch();
        }

        /// <summary>
        /// 暗黑主题/取消暗黑按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 暗黑主题ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DarkModeSwitch();
        }

        /// <summary>
        /// 停用备份/启用备份按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 自动备份ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.AutoBackupRunning)
            {
                this.AutoBackup.Stop();
                this.自动备份ToolStripMenuItem.Text = "启用备份";
            }
            else
            {
                if (IsReadOnly())
                {
                    MessageBox.Show("只读文件无需启用备份。");
                    return;
                }
                this.AutoBackup.Start();
                this.自动备份ToolStripMenuItem.Text = "停用备份";
            }
        }

        /// <summary>
        /// 开启/关闭自动保存按钮按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 自动保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.AutoSaverRunning)
            {
                AutoSaver.Stop();
                this.自动保存ToolStripMenuItem.Text = "自动保存";
            }
            else
            {
                if (IsReadOnly())
                {
                    MessageBox.Show("只读文件无需启用自动保存。");
                    return;
                }
                AutoSaver.Start();
                this.自动保存ToolStripMenuItem.Text = "不自动保存";
            }
        }
        #endregion

        #region 状态栏
        /// <summary>
        /// 更新状态栏内的行、列和字数
        /// </summary>
        private void UpdateStatusStripInfo()
        {
            // 更新状态栏里的行、列、字数信息
            this.toolStripStatusLabelRow.Text = "第" + (TextBoxUtil.GetNowLineIndex() + 1) + "行";
            this.toolStripStatusLabelColumn.Text = "第" + (TextBoxUtil.GetColumnIndex() + 1) + "列";
            if (this.textBoxMain.SelectionLength == 0)
                this.toolStripStatusLabelTextLength.Text = this.textBoxMain.Text.Replace("\r", "").Replace("\n", "").Length + "字";
            else
                this.toolStripStatusLabelTextLength.Text = this.textBoxMain.SelectedText.Length + "字";
        }

        /// <summary>
        /// 切换全屏锁
        /// </summary>
        private void SwitchFullScreenLockStatus()
        {
            var icon = Properties.Resources.lock_and_unlock_icon;
            Rectangle cropRect;
            if (LockFullScreenMode) // 锁定变不锁定
            {
                cropRect = new Rectangle(0, 0, 200, 200);
                LockFullScreenMode = false;
                this.toolStripStatusLabelLockFullScreen.ToolTipText = "普通/精简模式未被锁定";
            }
            else // 不锁定变锁定
            {
                cropRect = new Rectangle(200, 0, 200, 200);
                LockFullScreenMode = true;
                if (this.FullScreen)
                    this.toolStripStatusLabelLockFullScreen.ToolTipText = "已锁定为精简模式";
                else
                    this.toolStripStatusLabelLockFullScreen.ToolTipText = "已锁定为普通模式";
            }
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(icon, new Rectangle(0, 0, target.Width, target.Height),
                      cropRect,
                      GraphicsUnit.Pixel);
            }
            this.toolStripStatusLabelLockFullScreen.BackgroundImage = target;
        }

        private void toolStripStatusLockFullScreen_Click(object sender, EventArgs e)
        {
            SwitchFullScreenLockStatus();
        }

        /// <summary>
        /// 切换滚动条锁
        /// </summary>
        private void SwitchScrollBarLockStatus()
        {
            var icon = Properties.Resources.lock_and_unlock_icon;
            Rectangle cropRect;
            if (LockScrollBarStatus) // 锁定变不锁定
            {
                cropRect = new Rectangle(0, 0, 200, 200);
                LockScrollBarStatus = false;
                this.toolStripStatusLabelLockScrollBar.ToolTipText = "滚动条状态未被锁定";
            }
            else // 不锁定边锁定
            {
                cropRect = new Rectangle(200, 0, 200, 200);
                LockScrollBarStatus = true;
                if (this.textBoxMain.ScrollBars == ScrollBars.Vertical)
                    this.toolStripStatusLabelLockScrollBar.ToolTipText = "滚动条已被锁定为开启状态";
                else
                    this.toolStripStatusLabelLockScrollBar.ToolTipText = "滚动条已被锁定为关闭状态";
            }
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(icon, new Rectangle(0, 0, target.Width, target.Height),
                      cropRect,
                      GraphicsUnit.Pixel);
            }
            this.toolStripStatusLabelLockScrollBar.BackgroundImage = target;
        }

        private void toolStripStatusLockScrollBar_Click(object sender, EventArgs e)
        {
            SwitchScrollBarLockStatus();
        }

        private Encoding GetEncodingFromString(string text)
        {
            text = text.Replace("-", "").Replace(" ", "").ToUpper();
            if(text.Contains("GBK")|| text.Contains("GB2312"))
                return Encoding.Default;
            else if (text.Contains("UTF8"))
                return Encoding.UTF8;
            else if (text.Contains("ASCII"))
                return Encoding.ASCII;
            else if (text.Contains("UTF7"))
                return Encoding.UTF7;
            else if (text.Contains("UTF16LE"))
                return Encoding.Unicode;
            else if (text.Contains("UTF16BE"))
                return Encoding.BigEndianUnicode;
            else if (text.Contains("UTF32"))
                return Encoding.UTF32;
            return Encoding.UTF8;
        }

        private void toolStripStatusLabelEncoding_Click(object sender, EventArgs e)
        {
            var input = Interaction.InputBox("可供选择的字符编码：\nASCII、GBK(GB2312)、UTF-8、带有 BOM 的 UTF-8、UTF-7、UTF-16 LE、UTF-16 BE、UTF-32", "请选择要切换的字符编码", defaultText: "UTF-8", charCountPerline: 56);
            var inputEncoding = GetEncodingFromString(input);
            if (inputEncoding == null)
                return;

            LoadTxt(this.lastLoadedTxtFile, inputEncoding);
            ShowEncoding(inputEncoding, input.ToUpper().Contains("BOM"));
        }
        #endregion

        #region 帮助类封装
        /// <summary>
        /// 保存所有静态常量
        /// </summary>
        class ConstVariables
        {
            /// <summary>
            /// （只读）快捷插入功能支持的符号（左）
            /// </summary>
            public const string FAST_LEFTS = "(（[【{<《“‘\"";

            /// <summary>
            /// （只读）快捷插入功能支持的符号（右）
            /// </summary>
            public const string FAST_RIGHTS = ")）]】}>》”’\"";
            /// <summary>
            /// （只读）字号中文列表
            /// </summary>
            public static readonly String[] FONT_TEXTS = new String[] { "六号", "小五", "五号", "小四", "四号", "小三", "三号", "小二", "二号", "小一", "一号" };

            /// <summary>
            /// （只读）字号浮点数列表
            /// </summary>
            public static readonly float[] FONT_SIZES = new float[] { 7.5f, 9f, 10.5f, 12f, 14f, 15f, 16f, 18f, 22f, 24f, 26f };

            // LF、CRLF
            public const string LF = "\n";
            public const string CRLF = "\r\n";
        }
        /// <summary>
        /// 文本定位帮助类
        /// </summary>
        private class TextBoxUtil
        {
            /// <summary>
            /// 获得当前的行号（从0开始）
            /// </summary>
            /// <param name="textBox"></param>
            /// <returns></returns>
            public static int GetNowLineIndex(TextBox textBox = null)
            {
                if (textBox == null)
                    textBox = FormEdit.Instance.textBoxMain;
                return textBox.GetLineFromCharIndex(textBox.SelectionStart);
            }

            /// <summary>
            /// 获得当前的列号（从0开始）
            /// </summary>
            /// <param name="textBox"></param>
            /// <returns></returns>
            public static int GetColumnIndex(TextBox textBox = null)
            {
                if (textBox == null)
                    textBox = FormEdit.Instance.textBoxMain;
                return textBox.SelectionStart - GetFirstCharIndexOfCurrentLine(textBox);
            }

            /// <summary>
            /// 获得当前行的内容
            /// </summary>
            /// <param name="textBox"></param>
            /// <returns></returns>
            public static String GetNowLine(TextBox textBox = null)
            {
                if (textBox == null)
                    textBox = FormEdit.Instance.textBoxMain;
                return GetLine(GetNowLineIndex(textBox), textBox);
            }

            /// <summary>
            /// 获得指定行的内容
            /// </summary>
            /// <param name="lineNumber"></param>
            /// <param name="textBox"></param>
            /// <returns></returns>
            public static String GetLine(int lineNumber, TextBox textBox = null)
            {
                if (textBox == null)
                    textBox = FormEdit.Instance.textBoxMain;
                if (lineNumber < 0 || lineNumber >= textBox.Lines.Length)
                    return "";
                return textBox.Lines[lineNumber];
            }

            /// <summary>
            /// 获得当前行的长度
            /// </summary>
            /// <param name="textBox"></param>
            /// <returns></returns>
            public static int GetNowLineLength(TextBox textBox = null)
            {
                if (textBox == null)
                    textBox = FormEdit.Instance.textBoxMain;
                return GetLineLength(GetNowLineIndex(textBox), textBox);
            }

            /// <summary>
            /// 获得指定行的长度
            /// </summary>
            /// <param name="lineNumber"></param>
            /// <param name="textBox"></param>
            /// <returns></returns>
            public static int GetLineLength(int lineNumber, TextBox textBox = null)
            {
                if (textBox == null)
                    textBox = FormEdit.Instance.textBoxMain;
                return GetLine(lineNumber, textBox).Length;
            }

            /// <summary>
            /// 获得当前行的第一个字符在TextBox中的索引
            /// </summary>
            /// <param name="textBox"></param>
            /// <returns></returns>
            public static int GetFirstCharIndexOfCurrentLine(TextBox textBox = null)
            {
                if (textBox == null)
                    textBox = FormEdit.Instance.textBoxMain;
                return GetFirstCharIndexOfLine(GetNowLineIndex(textBox), textBox);
            }

            /// <summary>
            /// 获得指定行的第一个字符在TextBox中的索引
            /// </summary>
            /// <param name="lineNumber"></param>
            /// <param name="textBox"></param>
            /// <returns></returns>
            public static int GetFirstCharIndexOfLine(int lineNumber, TextBox textBox = null)
            {
                if (textBox == null)
                    textBox = FormEdit.Instance.textBoxMain;
                return textBox.GetFirstCharIndexFromLine(lineNumber);
            }
        }

        /// <summary>
        /// Undo和Redo的实现类
        /// </summary>
        public class FormerSaver<T>
        {
            /* 由于要使用Redo，就不能使用Stack类进行操作 */
            private List<T> formerText = new List<T>(20); // 储存每个状态下的Text的记录顺序表
            private int formerText_NowPlace = -1; // 最新元素的储存位置

            /// <summary>
            /// 撤销，如果越界会返回null
            /// </summary>
            /// <returns></returns>
            public T Undo()
            {
                if (this.formerText_NowPlace < 0)
                    return default(T);
                return formerText[formerText_NowPlace--];
            }

            /// <summary>
            /// 重做，如果越界会返回null
            /// </summary>
            /// <returns></returns>
            public T Redo()
            {
                if (this.formerText_NowPlace > this.formerText.Count - 2)
                    return default(T);
                return formerText[++formerText_NowPlace];
            }

            /// <summary>
            /// 保存Text到FormerSaver类中
            /// </summary>
            /// <param name="text"></param>
            public void SaveText(T text)
            {
                if (this.formerText.Count - 1 > this.formerText_NowPlace)
                    this.formerText.RemoveRange(this.formerText_NowPlace + 1, this.formerText.Count - 1 - this.formerText_NowPlace);
                if (formerText.Count > 0 && formerText[formerText.Count - 1].Equals(text))
                    return;

                formerText.Add(text);
                this.formerText_NowPlace++;
                // 只为使用时debug用，不是正式功能
                if (Title.DebugInfo != null)
                    Title.DebugInfo = this.formerText_NowPlace.ToString();
            }

            /// <summary>
            /// 获得当前指针指向的Text
            /// </summary>
            /// <returns></returns>
            public T GetNewest()
            {
                return formerText[formerText_NowPlace];
            }

            /// <summary>
            /// 获得FormerSaver类中保存的最后一个字符串
            /// </summary>
            /// <returns></returns>
            public T GetPeek()
            {
                if (formerText.Count != 0)
                    return formerText[formerText.Count - 1];
                return default(T);
            }

            /// <summary>
            /// 重置改类所有变量
            /// </summary>
            public void Clear()
            {
                this.formerText.Clear();
                this.formerText_NowPlace = -1;
            }

            /// <summary>
            /// 输出类的内容
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                String result = "[";
                for (int i = 0; i < this.formerText.Count - 1; i++)
                {
                    result += this.formerText[i] + ",";
                }
                result += this.formerText[this.formerText.Count - 1] + "], nowPlace=" + this.formerText_NowPlace;
                return result;
            }
        }

        /// <summary>
        /// 主程序的标题管理类
        /// </summary>
        public static class Title
        {
            private static string titleName = "日志书写器";
            private static string version = null;
            private static string specifiedDocumentFullName = null;
            private static bool isReadOnly = false;
            private static bool isUntitled = false;
            private static string debugInfo = null;

            public static string TitleName
            {
                set
                {
                    titleName = value;
                    UpdateTitle();
                }
            }

            public static string Version
            {
                set
                {
                    version = value;
                    UpdateTitle();
                }
            }

            public static string SpecifiedDocumentFullName
            {
                set
                {
                    specifiedDocumentFullName = value;
                    UpdateTitle();
                }
            }

            public static bool ReadOnly
            {
                set
                {
                    isReadOnly = value;
                    UpdateTitle();
                }
            }

            public static bool Untitled
            {
                set
                {
                    isUntitled = value;
                    UpdateTitle();
                }
                get
                {
                    return Title.isUntitled;
                }
            }

            public static string DebugInfo
            {
                set
                {
                    debugInfo = value;
                    UpdateTitle();
                }
                get
                {
                    return Title.debugInfo;
                }
            }

            private static void UpdateTitle()
            {
                StringBuilder finalTitle = new StringBuilder();
                finalTitle.Append(titleName);
                if (version != null)
                    finalTitle.Append(" v" + version);
                if (specifiedDocumentFullName != null)
                    finalTitle.Append(" (" + specifiedDocumentFullName + ")");
                if (isReadOnly)
                    finalTitle.Append(" [只读]");
                if (isUntitled)
                    finalTitle.Append(" (未命名)");
                if (debugInfo != null)
                    finalTitle.Append(" {" + debugInfo + "}");
                FormEdit.Instance.Text = finalTitle.ToString();
            }
        }

        /// <summary>
        /// 管理BackupCreater的创建
        /// </summary>
        private static class BackupCreaterFactory
        {
            // 一共两种ID
            public const int ID_SAVE = 1;
            public const int ID_BACKUP = 0;

            /// <summary>
            /// BackupCreater中必要的数据
            /// </summary>
            public class Data
            {
                public string 源文件名 { get; set; }
                public BackupCreater.WriteProcedure writeFileProcedure { get; set; }
                public int interval { get; set; }
                public string 后缀名 { get; set; }
                public bool hideBackup { get; set; }
            }

            private static Dictionary<int, Data> savedData = new Dictionary<int, Data>(); // 保存{0:自动备份数据, 1:自动保存数据}

            /// <summary>
            /// 创建BackupCreater对象
            /// </summary>
            /// <param name="源文件名"></param>
            /// <param name="writeFileProcedure"></param>
            /// <param name="interval"></param>
            /// <param name="后缀名"></param>
            /// <param name="hideBackup"></param>
            /// <returns></returns>
            private static BackupCreater Create(string 源文件名, BackupCreater.WriteProcedure writeFileProcedure, int interval, string 后缀名, bool hideBackup)
            {
                return new BackupCreater(源文件名, writeFileProcedure, interval, 后缀名, hideBackup);
            }

            /// <summary>
            /// 根据data创建AutoBackup
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            public static BackupCreater CreateAutoBackup(Data data)
            {
                savedData[0] = data;
                return Create(data.源文件名, data.writeFileProcedure, data.interval, data.后缀名, data.hideBackup);
            }

            /// <summary>
            /// 根据传入的参数创建AutoBackup
            /// </summary>
            /// <param name="备份源文件名"></param>
            /// <param name="writeFileProcedure"></param>
            /// <param name="interval"></param>
            /// <param name="备份后缀名"></param>
            /// <param name="hideBackup"></param>
            /// <returns></returns>
            public static BackupCreater CreateAutoBackup(string 备份源文件名, BackupCreater.WriteProcedure writeFileProcedure = null, int interval = 1000, string 备份后缀名 = ".backup", bool hideBackup = false)
            {
                Data data = new Data();
                data.源文件名 = 备份源文件名;
                data.writeFileProcedure = writeFileProcedure;
                data.interval = interval;
                data.后缀名 = 备份后缀名;
                data.hideBackup = hideBackup;
                return CreateAutoBackup(data);
            }

            /// <summary>
            /// 根据data创建AutoSaver
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            public static BackupCreater CreateAutoSaver(Data data)
            {
                savedData[1] = data;
                return Create(data.源文件名, data.writeFileProcedure, data.interval, data.后缀名, data.hideBackup);
            }

            /// <summary>
            /// 根据传入的参数创建AutoSaver
            /// </summary>
            /// <param name="源文件名"></param>
            /// <param name="writeFileProcedure"></param>
            /// <param name="interval"></param>
            /// <param name="后缀名"></param>
            /// <param name="hideBackup"></param>
            /// <returns></returns>
            public static BackupCreater CreateAutoSaver(string 源文件名, BackupCreater.WriteProcedure writeFileProcedure = null, int interval = 1000, string 后缀名 = ".backup", bool hideBackup = false)
            {
                Data data = new Data();
                data.源文件名 = 源文件名;
                data.writeFileProcedure = writeFileProcedure;
                data.interval = interval;
                data.后缀名 = 后缀名;
                data.hideBackup = hideBackup;
                return CreateAutoSaver(data);
            }

            /// <summary>
            /// 储存的数据存在id为id的项
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static bool ContainsKey(int id)
            {
                return BackupCreaterFactory.savedData.ContainsKey(id);
            }

            /// <summary>
            /// 获得指定id的数据
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static Data GetDataById(int id)
            {
                if (savedData.ContainsKey(id))
                    return savedData[id];
                return null;
            }

            /// <summary>
            /// 储存修改data
            /// </summary>
            /// <param name="id"></param>
            /// <param name="original文件名"></param>
            /// <param name="interval"></param>
            /// <param name="backup后缀名"></param>
            /// <param name="hiddenBackupFile"></param>
            /// <returns></returns>
            private static bool SetData(int id, string original文件名, int interval, string backup后缀名, bool hiddenBackupFile)
            {
                if (!ContainsKey(id))
                    return false;
                savedData[id].源文件名 = original文件名;
                savedData[id].interval = interval;
                savedData[id].后缀名 = backup后缀名;
                savedData[id].hideBackup = hiddenBackupFile;
                return true;
            }

            /// <summary>
            /// 获取当前最新数据并更新id为id的项
            /// </summary>
            /// <param name="id"></param>
            public static void UpdateData(int id)
            {
                if (id == ID_SAVE)
                    BackupCreaterFactory.SetData(ID_SAVE, Instance.AutoSaver.Original文件名, Instance.AutoSaver.Interval, Instance.AutoSaver.Backup后缀名, Instance.AutoSaver.HiddenBackupFile);
                else if (id == ID_BACKUP)
                    BackupCreaterFactory.SetData(ID_BACKUP, Instance.AutoBackup.Original文件名, Instance.AutoBackup.Interval, Instance.AutoBackup.Backup后缀名, Instance.AutoBackup.HiddenBackupFile);
            }
        }

        /// <summary>
        /// 可写在using语句块中的classes
        /// </summary>
        private class Disposable
        {
            /// <summary>
            /// 暂停AutoSaver直到using语句块结束
            /// </summary>
            public class CloseAndOpenAutoSaver : IDisposable
            {
                private bool running;
                public CloseAndOpenAutoSaver()
                {
                    running = FormEdit.Instance.AutoSaverRunning;
                    FormEdit.Instance.AutoSaverRunning = false;
                }
                public void Dispose()
                {
                    if (running)
                        FormEdit.Instance.AutoSaverRunning = true;
                }
            }
            /// <summary>
            /// 暂停AutoBackup直到using语句块结束
            /// </summary>
            public class CloseAndOpenAutoBackup : IDisposable
            {
                private bool running;
                public CloseAndOpenAutoBackup()
                {
                    running = FormEdit.Instance.AutoBackupRunning;
                    FormEdit.Instance.AutoBackupRunning = false;
                }
                public void Dispose()
                {
                    if (running)
                        FormEdit.Instance.AutoBackupRunning = true;
                }
            }
            /// <summary>
            /// 先缩小文字保证每行都在单独一行中，using语句块结束后还原文字大小
            /// </summary>
            public class MakeTextFontSmallAndThenRestore : IDisposable
            {
                private string savedDocumentFontSizeZh;
                public MakeTextFontSmallAndThenRestore()
                {
                    savedDocumentFontSizeZh = FormEdit.Instance.DocumentFontSizeZh;
                    FormEdit.Instance.textBoxMain.Font = new Font(FormEdit.Instance.DocumentFont, 1);
                }

                public void Dispose()
                {
                    FormEdit.Instance.DocumentFontSizeZh = savedDocumentFontSizeZh;
                }
            }
        }

        /// <summary>
        /// 暂停AutoSaver直到using语句块结束
        /// </summary>
        private IDisposable PauseAutoSaver { get { return new Disposable.CloseAndOpenAutoSaver(); } }
        /// <summary>
        /// 暂停AutoBackup直到using语句块结束
        /// </summary>
        private IDisposable PauseAutoBackup { get { return new Disposable.CloseAndOpenAutoBackup(); } }
        /// <summary>
        /// 安全编辑，用于需要使用行号进行内容修改的操作。保存当前Text并避免修改时行号异常的问题
        /// </summary>
        private IDisposable SafeEdit { get { former.SaveText(this.textBoxMain.Text); return new Disposable.MakeTextFontSmallAndThenRestore(); } }

        /// <summary>
        /// 将普通英文与UTF8混合字串转换成正常字串
        /// </summary>
        /// <param name="混合字符串">英文与UTF8混合字串</param>
        /// <returns></returns>
        private static string Hex2ChiEngString(string 混合字符串)
        {
            int 处理结束所在Index = 0;
            int 中文字开始Index, 中文字结束Index;
            StringBuilder result = new StringBuilder(); // 真实字符串
                                                        // 每次循环优先处理英文，然后在处理中文。以此类推
            while (处理结束所在Index < 混合字符串.Length)
            {
                中文字开始Index = 混合字符串.IndexOf("&#x", 处理结束所在Index);
                if (中文字开始Index == -1) // 如果接下来没有UTF8字符了
                    中文字开始Index = 混合字符串.Length; // 使开始Index出界
                if (中文字开始Index != 处理结束所在Index) // 有不用处理的英文字母直接放入原文
                {
                    result.Append(混合字符串.Substring(处理结束所在Index, 中文字开始Index - 处理结束所在Index));
                    处理结束所在Index = 中文字开始Index;
                }
                if (处理结束所在Index < 混合字符串.Length) // 还没处理完所有的字符串
                {
                    中文字结束Index = 混合字符串.IndexOf(";", 中文字开始Index) + 1; // 字符格式是&#x…;
                                                                     // 循环查找连续的中文字符
                    while (true)
                    {
                        // 如果不再是中文字符，就跳出循环
                        if (混合字符串.IndexOf("&#x", 中文字结束Index) != 中文字结束Index)
                            break;
                        // 下个字还是中文就接着查找
                        中文字结束Index = 混合字符串.IndexOf(";", 中文字结束Index) + 1;
                    }
                    if (中文字结束Index == 0)
                        throw new Exception("中文Hex格式错误");
                    // 得到所有中文字符的utf8编码
                    string chinCharacterUtf8编码 = 混合字符串.Substring(中文字开始Index, 中文字结束Index - 中文字开始Index).Replace("&#x", "").Replace(";", "");
                    // 转化为汉字添加进去
                    result.Append(UnicodeSaverUtil.GetChsFromHex(chinCharacterUtf8编码));
                    处理结束所在Index = 中文字结束Index;
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// 汉字与固定Unicode格式之间互换的帮助类
        /// </summary>
        /// 这个类诞生的原因：由于不能解决将排序规则由默认的SQL_Latin1_General_CP1_CI_AS
        /// 修改为Chinese_PRC_CI_AS的问题，所以数据库文件只存ASCII文本，所以需要一个类将
        /// 字符转换成对应的ASCII字段储存进数据库
        static class UnicodeSaverUtil
        {
            private readonly static string codingType = "utf-8";
            /// <summary>
            /// 判断ch是否是中文
            /// </summary>
            /// <param name="ch"></param>
            /// <returns></returns>
            public static bool IsChineseChar(char ch)
            {
                return ch > 127;
            }

            public static bool IsChineseString(string content)
            {
                bool result = false;
                if (content.IndexOf("&#x") != -1)
                    return true;
                foreach (char ch in content.ToCharArray())
                {
                    if (IsChineseChar(ch))
                        result = true;
                }
                return result;
            }

            /// <summary>
            /// 从汉字串转换到16进制，字符类型不建议使用该方法
            /// </summary>
            /// <param name="s"></param>
            /// <returns></returns>
            public static string GetHexFromChs(string s)
            {
                if ((s.Length % 2) != 0)
                {
                    s += " ";//空格
                             //throw new ArgumentException("s is not valid chinese string!");
                }

                System.Text.Encoding chs = System.Text.Encoding.GetEncoding(codingType);

                byte[] bytes = chs.GetBytes(s);

                string str = "";

                for (int i = 0; i < bytes.Length; i++)
                {
                    str += string.Format("&#x{0:x};", bytes[i]);
                }

                return str;
            }

            /// <summary>
            /// 将汉字字符转为utf8编码
            /// </summary>
            /// <param name="ch"></param>
            /// <returns></returns>
            public static string GetHexFromChs(char ch)
            {
                string result = GetHexFromChs(ch + "");
                if (result.Substring(result.Length - 6) == "&#x20;")
                    result = result.Substring(0, result.Length - 6);
                return result;
            }

            /// <summary>
            /// 从16进制转换成汉字，请勿按字节调用
            /// </summary>
            /// <param name="hex"></param>
            /// <returns></returns>
            public static string GetChsFromHex(string hex)
            {
                if (hex == null)
                    throw new ArgumentNullException("hex not found");
                if (hex.Length <= 6)
                    throw new ArgumentException("请勿按字节调用该函数，请至少传入两个hex");
                if (hex.Length % 2 != 0)
                {
                    hex += "20";//空格
                                //throw new ArgumentException("hex is not a valid number!", "hex");
                }

                hex = hex.Replace("&#x", "").Replace(";", "");

                // 需要将 hex 转换成 byte 数组。
                byte[] bytes = new byte[hex.Length / 2];

                for (int i = 0; i < bytes.Length; i++)
                {
                    try
                    {
                        // 每两个字符是一个 byte。
                        bytes[i] = byte.Parse(hex.Substring(i * 2, 2),
                            System.Globalization.NumberStyles.HexNumber);
                    }
                    catch
                    {
                        // Rethrow an exception with custom message.
                        throw new ArgumentException("hex is not a valid hex number!", "hex");
                    }
                }

                // 获得 UTF-8，Chinese Simplified。
                System.Text.Encoding chs = System.Text.Encoding.GetEncoding(codingType);


                return chs.GetString(bytes);
            }
        }
        #endregion

        #region 外部调用接口
        /// <summary>
        /// 改变所有的Timer的间隔事件为autoSavePerSecond秒，并重启所有正在运行的Timer
        /// </summary>
        /// <param name="autoSavePerSecond"></param>
        public void ChangeTimerPerSecond(int autoSavePerSecond)
        {
            this.TimerIntervalSecond = autoSavePerSecond;
            // 保存当前AutoSaver状态
            using (PauseAutoSaver)
            {
                // 在这里保存最新的数据
                BackupCreaterFactory.UpdateData(BackupCreaterFactory.ID_SAVE);
                // 再很方便的提取出来
                var data = BackupCreaterFactory.GetDataById(BackupCreaterFactory.ID_SAVE);
                // 从未创建过AutoSaver就创建一个默认的并获取默认的data
                if (data == null)
                {
                    CreateAutoSaver();
                    data = BackupCreaterFactory.GetDataById(BackupCreaterFactory.ID_SAVE);
                }
                // 修改interval并再次创建实例
                data.interval = this.TimerIntervalSecond * 1000;
                CreateAutoSaver(data);
            }

            // 保存当前AutoBackup状态
            using (PauseAutoBackup)
            {
                // 在这里保存最新的数据
                BackupCreaterFactory.UpdateData(BackupCreaterFactory.ID_BACKUP);
                // 再很方便的提取出来
                var data = BackupCreaterFactory.GetDataById(BackupCreaterFactory.ID_BACKUP);
                // 从未创建过AutoBackup就创建一个默认的并获取默认的data
                if (data == null)
                {
                    CreateAutoBackup();
                    data = BackupCreaterFactory.GetDataById(BackupCreaterFactory.ID_BACKUP);
                }
                // 修改interval并再次创建实例
                data.interval = this.TimerIntervalSecond * 1000;
                CreateAutoBackup(data);
            }
        }

        /// <summary>
        /// 获得指定id的Timer的Interval
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetTimerInterval(int id)
        {
            try
            {
                if (id == 0)
                    return AutoBackup.Interval;
                else if (id == 1)
                    return AutoSaver.Interval;
                return -1;
            }
            catch (NullReferenceException)
            {
                return -1;
            }
        }

        /// <summary>
        /// 直接删除当前的备份文件
        /// </summary>
        public void DeleteBackup()
        {
            AutoBackup.DeleteBackup();
        }
        #endregion

        #region 外部引用模块（无错勿动）
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

        /// <summary>
        /// 解压GZip字节数组，返回原字节数组
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] DecompressBytes(byte[] bytes)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(bytes), CompressionMode.Decompress))
            {
                using (MemoryStream outputStream = new MemoryStream())
                {
                    stream.CopyTo(outputStream);
                    return outputStream.ToArray();
                }
            }
        }

        /// <summary>
        /// 以管理管身份重新启动本程序
        /// </summary>
        /// <param name="muteMessage">禁止弹出提示</param>
        public static void RestartWithAdminRight(bool muteMessage = false)
        {
            if (!muteMessage && MessageBox.Show("请使用管理员权限重启本程序再进行此操作，是否程序允许获取权限？", "操作失败", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }
            else
            {
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = Environment.CurrentDirectory;
                proc.FileName = Application.ExecutablePath;
                proc.Arguments = "MessageUnabled";
                proc.Verb = "runas";

                try
                {
                    Process.Start(proc);
                }
                catch
                {
                    // 用户点击不要授权
                    // 提示错误，什么都不要做
                    MessageBox.Show("获得权限失败", "用户未允许授权", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Application.Exit();
            }
        }
        #endregion
    }
}
