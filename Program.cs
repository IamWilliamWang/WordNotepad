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
        public static string Version(int 版本号保留几个点)
        {
            if (版本号保留几个点 < 0 || 版本号保留几个点 > 3)
                throw new ArgumentException("参数输入不正确");

            string original = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (版本号保留几个点 == 3)
                return original;
            int len = 0;
            for (int i = 0; i <= 版本号保留几个点; i++)
            {
                len = original.IndexOf('.', len + 1);
            }
            return original.Substring(0, len);
        }
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
