using System;
using System.IO;
using System.Windows.Forms;

namespace 日志书写器
{
    class BackupCreater
    {
        #region 保存的属性
        /// <summary>
        /// 备份的源文件名
        /// </summary>
        public string Original文件名 { get { return original文件名; } set {
                original文件名 = value;
                /* 不是初始化过程的时候，要用新的Original文件名更新Backup文件名 */
                if (Backup文件名 != null)
                {
                    this.Backup后缀名 = this.Backup后缀名; //同步信息到Backup文件名
                    int directoryEndIndex = original文件名.LastIndexOf('\\');
                    if (directoryEndIndex != -1)
                        this.workingDirectory = original文件名.Substring(0, directoryEndIndex);
                }
            }
        }
        /// <summary>
        /// 自动保存Timer.Interval（启动后不可以修改）
        /// </summary>
        public int Interval { get { return backupFileTimer.Interval; } set { Alert(); backupFileTimer.Interval = value; } }
        /// <summary>
        /// 备份文件的后缀名
        /// </summary>
        public string Backup后缀名
        {
            get
            {
                if (Backup文件名 == null)
                    throw new Exception("Backup文件名未被初始化！");
                string shortFileName = Backup文件名.Substring(Backup文件名.LastIndexOf("\\") + 1);
                int dotIndex = shortFileName.IndexOf('.'); //可以支持多个dot的备份文件名
                if (dotIndex == -1) return "";
                else
                    return shortFileName.Substring(dotIndex);
            }
            set
            {
                if (value[0] != '.')
                    value = '.' + value;
                int dotIndex = this.Original文件名.IndexOf('.');
                if (dotIndex == -1) //源文件没后缀，直接加
                    this.Backup文件名 = this.Original文件名 + value;
                else
                    this.Backup文件名 = this.Original文件名.Substring(0, dotIndex) + value;
            }
        }
        /// <summary>
        /// BackupCreater的工作路径
        /// </summary>
        public string WorkingDirectory
        {
            get
            {
                return workingDirectory;
            }
            set
            {
                workingDirectory = value;
                string shortOriginalFileName = Original文件名.Substring(Original文件名.LastIndexOf('\\') + 1);
                string shortBackupFileName = Backup文件名.Substring(Backup文件名.LastIndexOf('\\') + 1);
                if (workingDirectory.EndsWith("\\")) //统一去掉\
                    workingDirectory = workingDirectory.Substring(0, workingDirectory.Length - 1);
                Original文件名 = workingDirectory + "\\" + shortOriginalFileName;
                Backup文件名 = workingDirectory + "\\" + shortBackupFileName;
            }
        }
        /// <summary>
        /// 备份文件名
        /// </summary>
        public string Backup文件名 { get; set; }
        /// <summary>
        /// 备份加密算法
        /// </summary>
        public 加密算法 Encrypt算法 { get { return encrypt算法; } set { Alert(); encrypt算法 = value; } }
        /// <summary>
        /// 隐藏备份文件
        /// </summary>
        public bool HiddenBackupFile { get; set; }
        /* 注意：下方变量只能在本region内使用！ */
        private string original文件名;
        private string workingDirectory = Directory.GetCurrentDirectory();
        private 加密算法 encrypt算法;
        #endregion

        public enum 加密算法 { 无 };

        #region 事件注册
        public event WriteProcedure WriteBackupEvent;
        public delegate void WriteProcedure(string writeFileName);
        public delegate void RestoreProcedure();
        #endregion

        private Timer backupFileTimer { get; set; }
        private bool ParametersReadOnly { get; set; } = false;

        public BackupCreater(string 备份源文件名, WriteProcedure writeFileProcedure = null, int interval = 1000, string 备份后缀名 = ".backup", bool hideBackup = false, 加密算法 算法 = 加密算法.无)
        {
            this.Original文件名 = 备份源文件名;
            if (writeFileProcedure != null)
                this.WriteBackupEvent += writeFileProcedure;
            else
                this.WriteBackupEvent += DefaultBackupFunction;
            if (备份后缀名.StartsWith(".") == false)
                this.Backup后缀名 = "." + 备份后缀名;
            else
                this.Backup后缀名 = 备份后缀名;
            this.Encrypt算法 = 算法;
            this.HiddenBackupFile = hideBackup;

            backupFileTimer = new Timer();
            this.Interval = interval;
            backupFileTimer.Tick += WriteBackupInvoke;
        }

        private void WriteBackupInvoke(object sender, EventArgs e)
        {
            WriteBackupEvent.Invoke(this.Backup文件名);
            if (this.HiddenBackupFile)
                File.SetAttributes(this.Backup文件名, FileAttributes.Hidden);
        }

        /// <summary>
        /// 默认的备份函数。只是复制文件
        /// </summary>
        /// <param name="backupFileName"></param>
        private void DefaultBackupFunction(string backupFileName)
        {
            if (backupFileName != this.Backup文件名)
                throw new Exception("Fetal problem: 参数传递异常！");
            File.Delete(this.Backup文件名);
            File.Copy(this.Original文件名, this.Backup文件名);
        }

        /// <summary>
        /// 默认的还原函数
        /// </summary>
        private void DefaultRestoreFunction()
        {
            File.Delete(this.Original文件名);
            File.Copy(this.Backup文件名, this.Original文件名);
        }

        /// <summary>
        /// 启动自动备份Timer
        /// </summary>
        public void Start()
        {
            if (WriteBackupEvent.GetInvocationList().Length == 0)
                throw new System.Exception("BackupCreater未经初始化就强迫开始执行！");
            this.backupFileTimer.Start();
            this.ParametersReadOnly = true;
        }

        /// <summary>
        /// 停止自动备份Timer
        /// </summary>
        public void Stop()
        {
            this.backupFileTimer.Stop();
            this.ParametersReadOnly = false;
        }

        /// <summary>
        /// 不使用Timer，单次执行备份操作
        /// </summary>
        public void StartOnce()
        {
            this.WriteBackupInvoke(null, null);
        }

        /// <summary>
        /// 删除备份文件
        /// </summary>
        public void DeleteBackup()
        {
            File.Delete(this.Backup文件名);
        }

        /// <summary>
        /// 使用指定函数从备份文件恢复（不传参则调用默认恢复函数）
        /// </summary>
        /// <param name="restoreProcedure"></param>
        /// <param name="deleteBackupFile"></param>
        public void RestoreFile(RestoreProcedure restoreProcedure = null, bool deleteBackupFile = false)
        {
            if (restoreProcedure == null)
                DefaultRestoreFunction();
            else
                restoreProcedure();
            if (deleteBackupFile)
                this.DeleteBackup();
        }

        /// <summary>
        /// 如果Timer在运行时修改部分变量则会报错
        /// </summary>
        private void Alert()
        {
            if (this.ParametersReadOnly)
                throw new System.FieldAccessException("开始后不可以修改此变量！");
        }
    }
}
