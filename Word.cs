using NPOI.HPSF;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日志书写器
{
    class Word
    {
        private string filename;
        private XWPFDocument doc = new XWPFDocument();
        public string Font { get; set; } = "黑体";
        public int FontSize { get; set; } = 13;
        public Word(string docxFileName="document.docx")
        {
            doc.GetProperties().CoreProperties.Creator = "王劲翔";
            this.filename = docxFileName;
        }

        public void WriteDocx(string content)
        {
            XWPFParagraph paragraph = doc.CreateParagraph();
            paragraph.Alignment = ParagraphAlignment.BOTH;

            XWPFRun r1 = paragraph.CreateRun();
            r1.SetText(content);
            r1.FontFamily = Font;
            r1.FontSize = FontSize;
            
            FileStream out1 = new FileStream(filename, FileMode.Create);
            doc.Write(out1);
            out1.Close();
        }

        //private void CreateWord_COM(string docFileName, string strContent, bool visible = false)
        //{
        //    if (!docFileName.Contains(":")) //没有使用完整路径
        //        docFileName = Directory.GetCurrentDirectory() + "\\" + docFileName;
        //    object docFileNameObj = (object)docFileName;

        //    Microsoft.Office.Interop.Word.Application wordApp;//Word应用程序变量
        //    Microsoft.Office.Interop.Word.Document wordDoc;//Word文档变量
        //    wordApp = new Microsoft.Office.Interop.Word.Application();//初始化
        //    wordApp.Visible = visible;
        //    if (File.Exists(docFileName))
        //        File.Delete(docFileName);
        //    //由于使用的是COM 库，因此有许多变量需要用Missing.Value 代替
        //    Object Nothing = Missing.Value;
        //    //新建一个word对象
        //    wordDoc = wordApp.Documents.Add(ref Nothing, ref Nothing, ref Nothing, ref Nothing);
        //    //wdFormatDocument为Word2003文档的保存格式(文档后缀.doc) / wdFormatDocumentDefault为Word2007的保存格式(文档后缀.docx)
        //    object format;
        //    if (docFileName.EndsWith("docx"))
        //        format = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocumentDefault;
        //    else
        //        format = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocument;
        //    //开始写字
        //    wordDoc.Paragraphs.Last.Range.Font.Size = documentFontSize;
        //    wordDoc.Paragraphs.Last.Range.Font.Name = documentFont;
        //    wordDoc.Paragraphs.Last.Range.Text = strContent;
        //    //将wordDoc 文档对象的内容保存为DOC 文档,并保存到path指定的路径
        //    wordDoc.SaveAs(ref docFileNameObj, ref format, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing);
        //    //关闭wordDoc文档
        //    wordDoc.Close(ref Nothing, ref Nothing, ref Nothing);
        //    //关闭wordApp组件对象
        //    wordApp.Quit(ref Nothing, ref Nothing, ref Nothing);
        //}
    }
}
