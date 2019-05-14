using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日志书写器
{
    public class BinaryFileWriter
    {
        public static FileInfo WriteFileToDisk(byte[] fileContent, string fullFilename)
        {
            try
            {
                using (FileStream fs = new FileStream(fullFilename, FileMode.Create))
                    fs.Write(fileContent, 0, fileContent.Length);
            }
            catch (System.UnauthorizedAccessException)
            {
                return null;
            }

            return new FileInfo(fullFilename);
        }

        public static void RemoveFileFromDisk(string fullFilename)
        {
            try
            {
                System.IO.File.Delete(fullFilename);
            }
            catch(UnauthorizedAccessException)
            {
                CMD("del \"" + fullFilename + "\"");
            }
        }

        private static String CMD(String command, bool adminAuthorized = false)
        {
            Process process = new Process();

            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            if (adminAuthorized == true)
                process.StartInfo.Verb = "runas";

            try
            {
                process.Start();
                process.StandardInput.WriteLine(command);
                process.StandardInput.WriteLine("exit");
            }
            catch (Exception exception)
            {
                System.Windows.Forms.MessageBox.Show(exception.ToString(), "错误信息", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }

            return process.StandardOutput.ReadToEnd();
        }
    }
}
