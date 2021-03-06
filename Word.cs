﻿using NPOI.HPSF;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word记事本
{
    class Word
    {
        public static readonly String APPLICATION_NAME = "Word 记事本";
        public static readonly String COMPANY_NAME = "©2016-2021 William. All Rights Reserved.";
        public static readonly String VERSION = Program.Version();

        private string filename;
        private XWPFDocument docWrite = new XWPFDocument();
        public string Font { get; set; } = "黑体";
        public int FontSize { get; set; } = 13;
        public int Length { get { return this.ReadWord().Replace("\r", "").Replace("\n", "").Length; } }
        public Word(string docxFileName = "document.docx", string authorName = "")
        {
            String AUTHOR_NAME = authorName;
            String LAST_MODIFIER = authorName;
            // Core Properties
            if (authorName != "")
                docWrite.GetProperties().CoreProperties.GetUnderlyingProperties().SetCreatorProperty(AUTHOR_NAME); // 代替掉NPOI的CREATOR_AUTHOR
            docWrite.GetProperties().CoreProperties.GetUnderlyingProperties().SetLastModifiedByProperty(LAST_MODIFIER);
            docWrite.GetProperties().CoreProperties.GetUnderlyingProperties().SetVersionProperty(VERSION);
            // Extended properties
            docWrite.GetProperties().ExtendedProperties.GetUnderlyingProperties().Company = COMPANY_NAME;
            docWrite.GetProperties().ExtendedProperties.GetUnderlyingProperties().Application = APPLICATION_NAME;
            docWrite.GetProperties().ExtendedProperties.GetUnderlyingProperties().AppVersion = VERSION;
            // 类内变量
            this.filename = docxFileName;
        }

        private void EditDocxProperties(string[] contentLines)
        {
            // Core properties
            if (contentLines.Length > 0) 
                docWrite.GetProperties().CoreProperties.GetUnderlyingProperties().SetTitleProperty(contentLines[0].Substring(0, Math.Min(contentLines[0].Length, 20)));
            // Extended properties
            //docWrite.GetProperties().ExtendedProperties.GetUnderlyingProperties().CharactersWithSpaces = String.Join("", contentLines).Length;
            //docWrite.GetProperties().ExtendedProperties.GetUnderlyingProperties().Characters = String.Join("", contentLines).Replace(" ", "").Length;
        }

        public void WriteDocx(string content)
        {
            string[] contentLines = content.Split(FormEdit.ConstVariables.CRLF.ToCharArray(), StringSplitOptions.RemoveEmptyEntries); //提取每一行的内容
            WriteDocx(contentLines);
        }

        public void WriteDocx(string[] contentLines)
        {
            EditDocxProperties(contentLines);

            foreach (string line in contentLines)
            {
                XWPFParagraph paragraph = docWrite.CreateParagraph(); //每一行对应word中的一段
                paragraph.Alignment = ParagraphAlignment.BOTH;
                XWPFRun run = paragraph.CreateRun(); //给这一段插入一行文字内容
                run.SetText(line);
                run.FontFamily = Font;
                run.FontSize = FontSize;
            }
            try
            {
                using (FileStream outStream = new FileStream(filename, FileMode.Create))
                    docWrite.Write(outStream);
            }
            catch (DirectoryNotFoundException)
            {
                throw;
            }
        }

        public String ReadWord()
        {
            StringBuilder textReaded = new StringBuilder();
            String[] contentLines = ReadWordLines();
            foreach (String line in contentLines)
                textReaded.AppendLine(line);
            return textReaded.ToString();
        }
        
        public String[] ReadWordLines()
        {
            List<String> lines = new List<string>();
            try
            {
                using (FileStream docxStream = File.OpenRead(this.filename))
                {
                    XWPFDocument docRead = new XWPFDocument(docxStream);
                    if (docRead.Paragraphs.Count == 0)
                        return lines.ToArray();
                    foreach (XWPFParagraph paragraph in docRead.Paragraphs)
                    {
                        string paragraphText = paragraph.ParagraphText; //获得该段的文本，因为不需要管文字格式所以不用获取XWPFRun
                        lines.Add(paragraphText);
                    }
                    this.FontSize = docRead.Paragraphs[0].Runs[0].FontSize;
                    this.Font = docRead.Paragraphs[0].Runs[0].FontFamily;
                }
                return lines.ToArray();
            }
            catch(ICSharpCode.SharpZipLib.Zip.ZipException)
            {
                return new String[] { };
            }
        }

        public static String[] ReadDocxLines(string docxFileName = "document.docx")
        {
            return new Word(docxFileName).ReadWordLines();
        }

        public static String[] ReadDocLines(string docFileName)
        {
            throw new NotImplementedException();
        }
    }
}
