using ArticleReviewSystem.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using RazorEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ArticleReviewSystem
{
    public class PdfBuilder
    {
        private  Review _review;

        private readonly string _file;
        private readonly string _css;

        public PdfBuilder(Review review, string file,string css)
        {
            _review = review;
            _file = file;
            _css = css;
        }

        public FileContentResult GetPdf()
        {
            var html = GetHtml();
            var css = File.ReadAllText(_css);
            Byte[] bytes;
            using (var ms = new MemoryStream())
            {
                using (var doc = new Document())
                {
                    using (var writer = PdfWriter.GetInstance(doc, ms))
                    {
                        doc.Open();
                        try
                        {
                            using (var msHtml = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(html)))
                            {
                                using (var msCss = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(css)))
                                {
                                    iTextSharp.tool.xml.XMLWorkerHelper.GetInstance()
                                    .ParseXHtml(writer, doc, msHtml, msCss);
                                }
                            }
                        }
                        finally
                        {
                            doc.Close();
                        }
                    }
                }
                bytes = ms.ToArray();
            }
            return new FileContentResult(bytes, "application/pdf");
        }
        private string GetHtml()
        {
            var html = File.ReadAllText(_file);
            return Razor.Parse(html, _review);
        }
    }
}