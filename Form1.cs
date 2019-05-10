using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MSWord = Microsoft.Office.Interop.Word;

namespace 日志书写器
{
    public partial class Form1 : Form
    {
        private readonly float documentFontSize = 12F;
        private readonly String documentFont = "黑体";
        public Form1()
        {
            InitializeComponent();
        }
        public void CreateWord(string docFileName, string strContent, bool visible=false)
        {
            if (!docFileName.Contains(":")) //没有使用完整路径
                docFileName = Directory.GetCurrentDirectory() + "\\" + docFileName;
            object docFileNameObj = (object)docFileName;

            MSWord.Application wordApp;//Word应用程序变量
            MSWord.Document wordDoc;//Word文档变量
            wordApp = new MSWord.ApplicationClass();//初始化
            wordApp.Visible = visible;
            if (File.Exists(docFileName))
                File.Delete(docFileName);
            //由于使用的是COM 库，因此有许多变量需要用Missing.Value 代替
            Object Nothing = Missing.Value;
            //新建一个word对象
            wordDoc = wordApp.Documents.Add(ref Nothing, ref Nothing, ref Nothing, ref Nothing);
            //wdFormatDocument为Word2003文档的保存格式(文档后缀.doc) / wdFormatDocumentDefault为Word2007的保存格式(文档后缀.docx)
            object format;
            if (docFileName.EndsWith("docx"))
                format = MSWord.WdSaveFormat.wdFormatDocumentDefault;
            else
                format = MSWord.WdSaveFormat.wdFormatDocument;
            //开始写字
            wordDoc.Paragraphs.Last.Range.Font.Size = documentFontSize;
            wordDoc.Paragraphs.Last.Range.Font.Name = documentFont;
            wordDoc.Paragraphs.Last.Range.Text = strContent;
            //将wordDoc 文档对象的内容保存为DOC 文档,并保存到path指定的路径
            wordDoc.SaveAs(ref docFileNameObj, ref format, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing);
            //关闭wordDoc文档
            wordDoc.Close(ref Nothing, ref Nothing, ref Nothing);
            //关闭wordApp组件对象
            wordApp.Quit(ref Nothing, ref Nothing, ref Nothing);
        }

        // 关闭的时候执行保存操作
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.textBoxMain.Text == "")
                return;

            string filename = "";
            filename += String.Format("{0:0000}", DateTime.Now.Year);
            filename += String.Format("{0:00}", DateTime.Now.Month);
            filename += String.Format("{0:00}", DateTime.Now.Day);
            filename += "王劲翔.docx";
            this.CreateWord(filename, this.textBoxMain.Text);
        }
    }
}
