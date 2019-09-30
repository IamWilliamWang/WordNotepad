using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 日志书写器
{
    public partial class FormSettings : Form
    {
        private FormEdit Main { get { return FormEdit.Instance; } }

        public FormSettings()
        {
            InitializeComponent();
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            if (Main.IsReadOnly())
            {
                this.checkBox自动保存.Enabled = false;
                this.checkBox自动备份.Enabled = false;
                this.textBox计时器时长.Enabled = false;
                this.button计时器时长变更.Enabled = false;
                this.button清除备份文件.Enabled = false;
                return;
            }

            if (Main.AutoSaverTimerBusy)
                this.checkBox自动保存.Checked = true;
            if (Main.BackupTimerBusy)
                this.checkBox自动备份.Checked = true;
        }
        
        private void checkBox自动保存_CheckedChanged(object sender, EventArgs e)
        {
            Main.AutoSaverTimerBusy = this.checkBox自动保存.Checked;
        }

        private void checkBox自动备份_CheckedChanged(object sender, EventArgs e)
        {
            Main.BackupTimerBusy = this.checkBox自动备份.Checked;
        }

        private void button计时器时长变更_Click(object sender, EventArgs e)
        {
            Main.ChangeTimerPerSecond(int.Parse(this.textBox计时器时长.Text));
            MessageBox.Show("变更成功！");
        }

        private void button清除备份文件_Click(object sender, EventArgs e)
        {
            Main.DeleteBackup();
        }

        private void buttonAlwaysOnScrollBar_Click(object sender, EventArgs e)
        {
            Main.ScrollBarAlwaysOn = true;
            MessageBox.Show("已设置为强制打开");
        }
    }
}
