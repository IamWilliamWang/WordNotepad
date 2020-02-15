using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 日志书写器
{
    static class Program
    {
        #region 版本获取
        public static string Version()
        {
            string version = Version(2);
            if (version[version.Length - 1] == '0')
                return Version(1);
            return version;
        }
        public static string Version(int 版本号保留几个点)
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString(版本号保留几个点 + 1);
        }
        #endregion

        public static bool LogWriter { get; set; } = false; // true:日志书写器 or false:Word文本编辑器

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormEdit(args));
        }
    }
}
