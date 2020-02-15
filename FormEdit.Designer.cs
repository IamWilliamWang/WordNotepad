namespace 日志书写器
{
    partial class FormEdit
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEdit));
            this.textBoxMain = new System.Windows.Forms.TextBox();
            this.contextMenuStripMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.中文空格ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.查找内容ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.插入链接ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.新建文档ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.剪切ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.复制ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.粘贴ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.窗口置顶ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.精简模式ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.暗黑主题ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.自动聚焦ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.自动保存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.自动备份ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labelPath = new System.Windows.Forms.Label();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.button保存 = new System.Windows.Forms.Button();
            this.contextMenuStripSave = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.另存为ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.强制保存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存并置为终稿ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBoxSetting = new System.Windows.Forms.GroupBox();
            this.comboBoxFont = new System.Windows.Forms.ComboBox();
            this.button高级设置 = new System.Windows.Forms.Button();
            this.checkBoxMailbox = new System.Windows.Forms.CheckBox();
            this.comboBoxFontSize = new System.Windows.Forms.ComboBox();
            this.labelFontSize = new System.Windows.Forms.Label();
            this.labelFont = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelRow = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelColumn = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelEmpty = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelLockScrollBar = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelLockFullScreen = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelTextLength = new System.Windows.Forms.ToolStripStatusLabel();
            this.替换文本ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripMain.SuspendLayout();
            this.contextMenuStripSave.SuspendLayout();
            this.groupBoxSetting.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxMain
            // 
            this.textBoxMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMain.ContextMenuStrip = this.contextMenuStripMain;
            this.textBoxMain.Font = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxMain.Location = new System.Drawing.Point(12, 59);
            this.textBoxMain.Multiline = true;
            this.textBoxMain.Name = "textBoxMain";
            this.textBoxMain.Size = new System.Drawing.Size(833, 561);
            this.textBoxMain.TabIndex = 0;
            this.textBoxMain.Click += new System.EventHandler(this.textBoxMain_Click);
            this.textBoxMain.TextChanged += new System.EventHandler(this.textBoxMain_TextChanged);
            this.textBoxMain.DoubleClick += new System.EventHandler(this.FormEdit_DoubleClick);
            this.textBoxMain.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxMain_KeyDown);
            this.textBoxMain.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxMain_KeyUp);
            // 
            // contextMenuStripMain
            // 
            this.contextMenuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.中文空格ToolStripMenuItem,
            this.查找内容ToolStripMenuItem,
            this.插入链接ToolStripMenuItem,
            this.新建文档ToolStripMenuItem,
            this.替换文本ToolStripMenuItem,
            this.剪切ToolStripMenuItem,
            this.复制ToolStripMenuItem,
            this.粘贴ToolStripMenuItem,
            this.删除ToolStripMenuItem,
            this.窗口置顶ToolStripMenuItem,
            this.精简模式ToolStripMenuItem,
            this.暗黑主题ToolStripMenuItem,
            this.自动聚焦ToolStripMenuItem,
            this.自动保存ToolStripMenuItem,
            this.自动备份ToolStripMenuItem});
            this.contextMenuStripMain.Name = "contextMenuStripMain";
            this.contextMenuStripMain.Size = new System.Drawing.Size(181, 356);
            // 
            // 中文空格ToolStripMenuItem
            // 
            this.中文空格ToolStripMenuItem.Name = "中文空格ToolStripMenuItem";
            this.中文空格ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.中文空格ToolStripMenuItem.Text = "中文空格";
            this.中文空格ToolStripMenuItem.Click += new System.EventHandler(this.插入中文空格ToolStripMenuItem_Click);
            // 
            // 查找内容ToolStripMenuItem
            // 
            this.查找内容ToolStripMenuItem.Name = "查找内容ToolStripMenuItem";
            this.查找内容ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.查找内容ToolStripMenuItem.Text = "查找内容";
            this.查找内容ToolStripMenuItem.Click += new System.EventHandler(this.查找内容ToolStripMenuItem_Click);
            // 
            // 插入链接ToolStripMenuItem
            // 
            this.插入链接ToolStripMenuItem.Name = "插入链接ToolStripMenuItem";
            this.插入链接ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.插入链接ToolStripMenuItem.Text = "插入链接";
            this.插入链接ToolStripMenuItem.Click += new System.EventHandler(this.插入链接ToolStripMenuItem_Click);
            // 
            // 新建文档ToolStripMenuItem
            // 
            this.新建文档ToolStripMenuItem.Name = "新建文档ToolStripMenuItem";
            this.新建文档ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.新建文档ToolStripMenuItem.Text = "新建文档";
            this.新建文档ToolStripMenuItem.Click += new System.EventHandler(this.新建文档ToolStripMenuItem_Click);
            // 
            // 剪切ToolStripMenuItem
            // 
            this.剪切ToolStripMenuItem.Name = "剪切ToolStripMenuItem";
            this.剪切ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.剪切ToolStripMenuItem.Text = "剪切";
            this.剪切ToolStripMenuItem.Click += new System.EventHandler(this.剪切ToolStripMenuItem_Click);
            // 
            // 复制ToolStripMenuItem
            // 
            this.复制ToolStripMenuItem.Name = "复制ToolStripMenuItem";
            this.复制ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.复制ToolStripMenuItem.Text = "复制";
            this.复制ToolStripMenuItem.Click += new System.EventHandler(this.复制ToolStripMenuItem_Click);
            // 
            // 粘贴ToolStripMenuItem
            // 
            this.粘贴ToolStripMenuItem.Name = "粘贴ToolStripMenuItem";
            this.粘贴ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.粘贴ToolStripMenuItem.Text = "粘贴";
            this.粘贴ToolStripMenuItem.Click += new System.EventHandler(this.粘贴ToolStripMenuItem_Click);
            // 
            // 删除ToolStripMenuItem
            // 
            this.删除ToolStripMenuItem.Name = "删除ToolStripMenuItem";
            this.删除ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.删除ToolStripMenuItem.Text = "删除";
            this.删除ToolStripMenuItem.Click += new System.EventHandler(this.删除ToolStripMenuItem_Click);
            // 
            // 窗口置顶ToolStripMenuItem
            // 
            this.窗口置顶ToolStripMenuItem.Name = "窗口置顶ToolStripMenuItem";
            this.窗口置顶ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.窗口置顶ToolStripMenuItem.Text = "窗口置顶";
            this.窗口置顶ToolStripMenuItem.Click += new System.EventHandler(this.窗口置顶ToolStripMenuItem_Click);
            // 
            // 精简模式ToolStripMenuItem
            // 
            this.精简模式ToolStripMenuItem.Name = "精简模式ToolStripMenuItem";
            this.精简模式ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.精简模式ToolStripMenuItem.Text = "精简模式";
            this.精简模式ToolStripMenuItem.Click += new System.EventHandler(this.精简模式ToolStripMenuItem_Click);
            // 
            // 暗黑主题ToolStripMenuItem
            // 
            this.暗黑主题ToolStripMenuItem.Name = "暗黑主题ToolStripMenuItem";
            this.暗黑主题ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.暗黑主题ToolStripMenuItem.Text = "暗黑主题";
            this.暗黑主题ToolStripMenuItem.Click += new System.EventHandler(this.暗黑主题ToolStripMenuItem_Click);
            // 
            // 自动聚焦ToolStripMenuItem
            // 
            this.自动聚焦ToolStripMenuItem.Name = "自动聚焦ToolStripMenuItem";
            this.自动聚焦ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.自动聚焦ToolStripMenuItem.Text = "自动聚焦";
            this.自动聚焦ToolStripMenuItem.Click += new System.EventHandler(this.自动聚焦ToolStripMenuItem_Click);
            // 
            // 自动保存ToolStripMenuItem
            // 
            this.自动保存ToolStripMenuItem.Name = "自动保存ToolStripMenuItem";
            this.自动保存ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.自动保存ToolStripMenuItem.Text = "自动保存";
            this.自动保存ToolStripMenuItem.Click += new System.EventHandler(this.自动保存ToolStripMenuItem_Click);
            // 
            // 自动备份ToolStripMenuItem
            // 
            this.自动备份ToolStripMenuItem.Name = "自动备份ToolStripMenuItem";
            this.自动备份ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.自动备份ToolStripMenuItem.Text = "停用备份";
            this.自动备份ToolStripMenuItem.Click += new System.EventHandler(this.自动备份ToolStripMenuItem_Click);
            // 
            // labelPath
            // 
            this.labelPath.AutoSize = true;
            this.labelPath.Location = new System.Drawing.Point(202, 19);
            this.labelPath.Name = "labelPath";
            this.labelPath.Size = new System.Drawing.Size(53, 12);
            this.labelPath.TabIndex = 7;
            this.labelPath.Text = "文件路径";
            // 
            // textBoxPath
            // 
            this.textBoxPath.AllowDrop = true;
            this.textBoxPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPath.Location = new System.Drawing.Point(261, 15);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(332, 21);
            this.textBoxPath.TabIndex = 3;
            this.textBoxPath.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxPath_DragDrop);
            this.textBoxPath.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxPath_DragEnter);
            this.textBoxPath.DoubleClick += new System.EventHandler(this.textBoxPath_DoubleClick);
            this.textBoxPath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxPath_KeyDown);
            // 
            // button保存
            // 
            this.button保存.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button保存.ContextMenuStrip = this.contextMenuStripSave;
            this.button保存.Location = new System.Drawing.Point(755, 14);
            this.button保存.Name = "button保存";
            this.button保存.Size = new System.Drawing.Size(72, 23);
            this.button保存.TabIndex = 4;
            this.button保存.Text = "保存文档";
            this.button保存.UseVisualStyleBackColor = true;
            this.button保存.Click += new System.EventHandler(this.button保存_Click);
            // 
            // contextMenuStripSave
            // 
            this.contextMenuStripSave.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.另存为ToolStripMenuItem,
            this.强制保存ToolStripMenuItem,
            this.保存并置为终稿ToolStripMenuItem});
            this.contextMenuStripSave.Name = "contextMenuStripSave";
            this.contextMenuStripSave.Size = new System.Drawing.Size(137, 70);
            // 
            // 另存为ToolStripMenuItem
            // 
            this.另存为ToolStripMenuItem.Name = "另存为ToolStripMenuItem";
            this.另存为ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.另存为ToolStripMenuItem.Text = "另存为";
            this.另存为ToolStripMenuItem.Click += new System.EventHandler(this.另存为ToolStripMenuItem_Click);
            // 
            // 强制保存ToolStripMenuItem
            // 
            this.强制保存ToolStripMenuItem.Name = "强制保存ToolStripMenuItem";
            this.强制保存ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.强制保存ToolStripMenuItem.Text = "强制保存";
            this.强制保存ToolStripMenuItem.Click += new System.EventHandler(this.强制保存ToolStripMenuItem_Click);
            // 
            // 保存并置为终稿ToolStripMenuItem
            // 
            this.保存并置为终稿ToolStripMenuItem.Name = "保存并置为终稿ToolStripMenuItem";
            this.保存并置为终稿ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.保存并置为终稿ToolStripMenuItem.Text = "保存为终稿";
            this.保存并置为终稿ToolStripMenuItem.Click += new System.EventHandler(this.保存并置为终稿ToolStripMenuItem_Click);
            // 
            // groupBoxSetting
            // 
            this.groupBoxSetting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSetting.Controls.Add(this.comboBoxFont);
            this.groupBoxSetting.Controls.Add(this.button高级设置);
            this.groupBoxSetting.Controls.Add(this.checkBoxMailbox);
            this.groupBoxSetting.Controls.Add(this.comboBoxFontSize);
            this.groupBoxSetting.Controls.Add(this.labelFontSize);
            this.groupBoxSetting.Controls.Add(this.labelFont);
            this.groupBoxSetting.Controls.Add(this.labelPath);
            this.groupBoxSetting.Controls.Add(this.button保存);
            this.groupBoxSetting.Controls.Add(this.textBoxPath);
            this.groupBoxSetting.Location = new System.Drawing.Point(13, 7);
            this.groupBoxSetting.Name = "groupBoxSetting";
            this.groupBoxSetting.Size = new System.Drawing.Size(833, 46);
            this.groupBoxSetting.TabIndex = 8;
            this.groupBoxSetting.TabStop = false;
            // 
            // comboBoxFont
            // 
            this.comboBoxFont.Font = new System.Drawing.Font("黑体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBoxFont.FormattingEnabled = true;
            this.comboBoxFont.Items.AddRange(new object[] {
            "黑体",
            "微软雅黑",
            "等线",
            "宋体"});
            this.comboBoxFont.Location = new System.Drawing.Point(41, 15);
            this.comboBoxFont.Name = "comboBoxFont";
            this.comboBoxFont.Size = new System.Drawing.Size(59, 21);
            this.comboBoxFont.TabIndex = 1;
            this.comboBoxFont.Text = "黑体";
            this.comboBoxFont.SelectedIndexChanged += new System.EventHandler(this.comboBoxFont_SelectedIndexChanged);
            this.comboBoxFont.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBoxFont_KeyDown);
            // 
            // button高级设置
            // 
            this.button高级设置.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button高级设置.Location = new System.Drawing.Point(677, 14);
            this.button高级设置.Name = "button高级设置";
            this.button高级设置.Size = new System.Drawing.Size(72, 23);
            this.button高级设置.TabIndex = 10;
            this.button高级设置.Text = "高级设置";
            this.button高级设置.UseVisualStyleBackColor = true;
            this.button高级设置.Click += new System.EventHandler(this.button高级设置_Click);
            // 
            // checkBoxMailbox
            // 
            this.checkBoxMailbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxMailbox.AutoSize = true;
            this.checkBoxMailbox.Location = new System.Drawing.Point(599, 11);
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
            this.comboBoxFontSize.Location = new System.Drawing.Point(141, 15);
            this.comboBoxFontSize.Name = "comboBoxFontSize";
            this.comboBoxFontSize.Size = new System.Drawing.Size(55, 21);
            this.comboBoxFontSize.TabIndex = 2;
            this.comboBoxFontSize.SelectedIndexChanged += new System.EventHandler(this.comboBoxFontSize_SelectedIndexChanged);
            // 
            // labelFontSize
            // 
            this.labelFontSize.AutoSize = true;
            this.labelFontSize.Location = new System.Drawing.Point(106, 19);
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
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelRow,
            this.toolStripStatusLabelColumn,
            this.toolStripStatusLabelEmpty,
            this.toolStripStatusLabelLockScrollBar,
            this.toolStripStatusLabelLockFullScreen,
            this.toolStripStatusLabelTextLength});
            this.statusStrip.Location = new System.Drawing.Point(0, 623);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.ShowItemToolTips = true;
            this.statusStrip.Size = new System.Drawing.Size(858, 22);
            this.statusStrip.TabIndex = 9;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabelRow
            // 
            this.toolStripStatusLabelRow.Margin = new System.Windows.Forms.Padding(50, 3, 0, 2);
            this.toolStripStatusLabelRow.Name = "toolStripStatusLabelRow";
            this.toolStripStatusLabelRow.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabelRow.Text = "第0行";
            // 
            // toolStripStatusLabelColumn
            // 
            this.toolStripStatusLabelColumn.Margin = new System.Windows.Forms.Padding(25, 3, 0, 2);
            this.toolStripStatusLabelColumn.Name = "toolStripStatusLabelColumn";
            this.toolStripStatusLabelColumn.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabelColumn.Text = "第0列";
            // 
            // toolStripStatusLabelEmpty
            // 
            this.toolStripStatusLabelEmpty.Name = "toolStripStatusLabelEmpty";
            this.toolStripStatusLabelEmpty.Size = new System.Drawing.Size(509, 17);
            this.toolStripStatusLabelEmpty.Spring = true;
            // 
            // toolStripStatusLabelLockScrollBar
            // 
            this.toolStripStatusLabelLockScrollBar.AutoToolTip = true;
            this.toolStripStatusLabelLockScrollBar.BackgroundImage = global::日志书写器.Properties.Resources.lock_and_unlock_icon;
            this.toolStripStatusLabelLockScrollBar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.toolStripStatusLabelLockScrollBar.Margin = new System.Windows.Forms.Padding(0, 3, 29, 2);
            this.toolStripStatusLabelLockScrollBar.Name = "toolStripStatusLabelLockScrollBar";
            this.toolStripStatusLabelLockScrollBar.Size = new System.Drawing.Size(28, 17);
            this.toolStripStatusLabelLockScrollBar.Text = "     ";
            this.toolStripStatusLabelLockScrollBar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStripStatusLabelLockScrollBar.ToolTipText = "锁定垂直滚动条";
            this.toolStripStatusLabelLockScrollBar.Click += new System.EventHandler(this.toolStripStatusLockScrollBar_Click);
            // 
            // toolStripStatusLabelLockFullScreen
            // 
            this.toolStripStatusLabelLockFullScreen.AutoToolTip = true;
            this.toolStripStatusLabelLockFullScreen.BackgroundImage = global::日志书写器.Properties.Resources.lock_and_unlock_icon;
            this.toolStripStatusLabelLockFullScreen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.toolStripStatusLabelLockFullScreen.Margin = new System.Windows.Forms.Padding(0, 3, 29, 2);
            this.toolStripStatusLabelLockFullScreen.Name = "toolStripStatusLabelLockFullScreen";
            this.toolStripStatusLabelLockFullScreen.Size = new System.Drawing.Size(28, 17);
            this.toolStripStatusLabelLockFullScreen.Text = "     ";
            this.toolStripStatusLabelLockFullScreen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStripStatusLabelLockFullScreen.ToolTipText = "锁定全屏模式";
            this.toolStripStatusLabelLockFullScreen.Click += new System.EventHandler(this.toolStripStatusLockFullScreen_Click);
            // 
            // toolStripStatusLabelTextLength
            // 
            this.toolStripStatusLabelTextLength.Margin = new System.Windows.Forms.Padding(0, 3, 40, 2);
            this.toolStripStatusLabelTextLength.Name = "toolStripStatusLabelTextLength";
            this.toolStripStatusLabelTextLength.Size = new System.Drawing.Size(27, 17);
            this.toolStripStatusLabelTextLength.Text = "0字";
            this.toolStripStatusLabelTextLength.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // 替换文本ToolStripMenuItem
            // 
            this.替换文本ToolStripMenuItem.Name = "替换文本ToolStripMenuItem";
            this.替换文本ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.替换文本ToolStripMenuItem.Text = "替换文本";
            this.替换文本ToolStripMenuItem.Click += new System.EventHandler(this.替换文本ToolStripMenuItem_Click);
            // 
            // FormEdit
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(858, 645);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.groupBoxSetting);
            this.Controls.Add(this.textBoxMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormEdit";
            this.Text = "WordNotepad";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormEdit_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormEdit_FormClosed);
            this.Load += new System.EventHandler(this.FormEdit_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FormEdit_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FormEdit_DragEnter);
            this.DoubleClick += new System.EventHandler(this.FormEdit_DoubleClick);
            this.Resize += new System.EventHandler(this.FormEdit_Resize);
            this.contextMenuStripMain.ResumeLayout(false);
            this.contextMenuStripSave.ResumeLayout(false);
            this.groupBoxSetting.ResumeLayout(false);
            this.groupBoxSetting.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxMain;
        private System.Windows.Forms.Label labelPath;
        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.Button button保存;
        private System.Windows.Forms.GroupBox groupBoxSetting;
        private System.Windows.Forms.Label labelFontSize;
        private System.Windows.Forms.Label labelFont;
        private System.Windows.Forms.ComboBox comboBoxFontSize;
        private System.Windows.Forms.CheckBox checkBoxMailbox;
        private System.Windows.Forms.Button button高级设置;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripMain;
        private System.Windows.Forms.ToolStripMenuItem 剪切ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 复制ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 粘贴ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 中文空格ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 精简模式ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 暗黑主题ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 查找内容ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 自动聚焦ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 自动备份ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripSave;
        private System.Windows.Forms.ToolStripMenuItem 保存并置为终稿ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 自动保存ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 窗口置顶ToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelRow;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelColumn;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelTextLength;
        private System.Windows.Forms.ToolStripMenuItem 插入链接ToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelLockFullScreen;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelEmpty;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelLockScrollBar;
        private System.Windows.Forms.ComboBox comboBoxFont;
        private System.Windows.Forms.ToolStripMenuItem 强制保存ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 另存为ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 新建文档ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 替换文本ToolStripMenuItem;
    }
}

