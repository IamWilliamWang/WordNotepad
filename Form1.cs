using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace 日志书写器
{
    public partial class Form1 : Form
    {
        private float documentFontSize { get { return 12F; } }
        private String documentFont { get { return "黑体"; } }
        private bool hasSaved { get; set; }

        public Form1()
        {
            InitializeComponent();
            // 将实际字体替代在设计器中显示的字体
            this.textBoxMain.Font = new System.Drawing.Font(documentFont, documentFontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        }

        public void CreateWord(string docFileName, string strContent, bool visible = false)
        {
            if (!docFileName.Contains(":")) //没有使用完整路径
                docFileName = Directory.GetCurrentDirectory() + "\\" + docFileName;
            object docFileNameObj = (object)docFileName;

            Microsoft.Office.Interop.Word.Application wordApp;//Word应用程序变量
            Microsoft.Office.Interop.Word.Document wordDoc;//Word文档变量
            wordApp = new Microsoft.Office.Interop.Word.Application();//初始化
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
                format = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocumentDefault;
            else
                format = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocument;
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

        private bool need2Save() => this.textBoxMain.Text != "" && !this.hasSaved;

        private void button保存_Click(object sender, EventArgs e)
        {
            string filename = "";
            if (this.textBoxPath.Text != "")
                filename += this.textBoxPath.Text + "\\";
            filename += String.Format("{0:0000}", DateTime.Now.Year);
            filename += String.Format("{0:00}", DateTime.Now.Month);
            filename += String.Format("{0:00}", DateTime.Now.Day);
            filename += "王劲翔.docx";
            this.CreateWord(filename, this.textBoxMain.Text, true);
            this.hasSaved = true;
        }

        // 关闭的时候检查保存
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(need2Save())
                this.button保存_Click(null, null);
        }

        private void textBoxPath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void textBoxPath_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var file = ((String[])e.Data.GetData(DataFormats.FileDrop))[0];
                this.textBoxPath.Text = file.Substring(0, file.LastIndexOf("\\"));
            }
        }


    }
}
