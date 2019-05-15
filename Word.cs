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
        private readonly string authorName = "王劲翔";
        private XWPFDocument doc = new XWPFDocument();
        public string Font { get; set; } = "黑体";
        public int FontSize { get; set; } = 13;
        public Word(string docxFileName="document.docx")
        {
            doc.GetProperties().CoreProperties.Creator = authorName;
            this.filename = docxFileName;
        }

        public void WriteDocx(string content)
        {
            string[] contentLines = content.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries); //提取每一行的内容
            foreach (string line in contentLines)
            {
                XWPFParagraph paragraph = doc.CreateParagraph(); //每一行对应word中的一段
                paragraph.Alignment = ParagraphAlignment.BOTH;
                XWPFRun run = paragraph.CreateRun(); //给这一段插入一行文字内容
                run.SetText(line);
                run.FontFamily = Font;
                run.FontSize = FontSize;
            }
            using (FileStream outStream = new FileStream(filename, FileMode.Create))
                doc.Write(outStream);
        }
    }
}
