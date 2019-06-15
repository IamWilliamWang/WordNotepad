using System;
using System.IO;
using System.Windows.Forms;

namespace 日志书写器
{
    class BackupCreater
    {
        #region 保存的属性
        public string Original文件名 { get { return original文件名; } set { Alert(); original文件名 = value; } }
        public int Interval { get { return backupFileTimer.Interval; } set { Alert(); backupFileTimer.Interval = value; } }
        public string Backup后缀名 { get { return backup后缀名; } set { Alert(); backup后缀名 = value; } }
        public 加密算法 Encrypt算法 { get { return encrypt算法; } set { Alert(); encrypt算法 = value; } }
        public bool HiddenBackupFile { get { return hiddenBackupFile; } set { Alert(); hiddenBackupFile = value; } }
        /* 注意：下方变量只能在本region内使用！ */
        private string original文件名;
        private string backup后缀名;
        private 加密算法 encrypt算法;
        private bool hiddenBackupFile;
        #endregion
        public enum 加密算法 { 无 };
        #region 事件注册
        public event WriteProcedure WriteFileEvent;
        public delegate void WriteProcedure(string writeFileName);
        public delegate void RestoreProcedure();
        #endregion
        public string Backup文件名 { get
            {
                int lastDotIndex = this.Original文件名.LastIndexOf('.');
                if (lastDotIndex == -1)
                    return this.Original文件名 + this.Backup后缀名;
                else
                    return this.Original文件名.Substring(0, lastDotIndex) + Backup后缀名;
            }
        }
        private Timer backupFileTimer { get; set; }
        private bool ParametersReadOnly { get; set; } = false;

        public BackupCreater(string 备份源文件名, WriteProcedure writeFileProcedure = null, int interval = 1000, string 备份后缀名 = ".backup", bool hideBackup = false, 加密算法 算法 = 加密算法.无)
        {
            this.Original文件名 = 备份源文件名;
            if (writeFileProcedure != null)
                this.WriteFileEvent += writeFileProcedure;
            else
                this.WriteFileEvent += DefaultBackupFunction;
            if (备份后缀名.StartsWith(".") == false)
                this.Backup后缀名 = "." + 备份后缀名;
            else
                this.Backup后缀名 = 备份后缀名;
            this.Encrypt算法 = 算法;
            this.HiddenBackupFile = hideBackup;

            backupFileTimer = new Timer();
            this.Interval = interval;
            backupFileTimer.Tick += (sender, e) =>
            {
                WriteFileEvent.Invoke(this.Backup文件名);
                if (this.HiddenBackupFile)
                    File.SetAttributes(this.Backup文件名, FileAttributes.Hidden);
            };
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
            if (WriteFileEvent.GetInvocationList().Length == 0)
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
        public void RestoreFile(RestoreProcedure restoreProcedure=null, bool deleteBackupFile = false)
        {
            if (restoreProcedure == null)
                DefaultRestoreFunction();
            else
                restoreProcedure();
            if (deleteBackupFile)
                this.DeleteBackup();
        }

        /// <summary>
        /// 如果Timer在运行时修改变量则会报错
        /// </summary>
        private void Alert()
        {
            if (this.ParametersReadOnly)
                throw new System.FieldAccessException("开始后不可以修改任何变量！");
        }
    }
}
