using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 日志书写器
{
    class BackupHelper
    {
        public string 备份文件名 { get; set; }
        public int Interval { get; set; }
        public string 备份后缀名 { get; set; }
        public 加密算法 算法 { get; set; }
        public enum 加密算法 { 无 };
        public event BackupFunction RunBackupEvent;
        public delegate void BackupFunction();
        private Timer timer;
        public BackupHelper(string 备份文件名, int interval, BackupFunction backupFunction) : this(备份文件名, interval, backupFunction, ".backup", 加密算法.无)
        {
        }

        public BackupHelper(string 备份文件名, int interval, BackupFunction backupFunction, string 备份后缀名) : this(备份文件名, interval, backupFunction, 备份后缀名, 加密算法.无)
        {
        }
        public BackupHelper(string 备份文件名, int interval, BackupFunction backupFunction, 加密算法 算法) : this(备份文件名, interval, backupFunction, ".backup", 算法)
        {
        }
        public BackupHelper(string 备份文件名, int interval, BackupFunction backupFunction, string 备份后缀名, 加密算法 算法)
        {
            this.备份文件名 = 备份文件名;
            this.Interval = interval;
            this.RunBackupEvent += backupFunction;
            this.备份后缀名 = 备份后缀名;
            this.算法 = 算法;
        }

        public void StartBackupTimer()
        {

        }

        public void StopBackupTimer()
        {

        }

        public void DeleteBackupFiles()
        {

        }

        public void Restore(string restoredFileName)
        {

        }
    }
}
