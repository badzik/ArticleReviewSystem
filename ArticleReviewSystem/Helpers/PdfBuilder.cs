using ArticleReviewSystem.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Pechkin;
using RazorEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ArticleReviewSystem.Helpers
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
            //var html = GetHtml();
            //Byte[] bytes = new SimplePechkin(new GlobalConfig()).Convert(html);
            //return new FileContentResult(bytes, "application/pdf");
            byte[] pdf; // result will be here

            using (var memoryStream = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 50, 50, 60, 60);
                var writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                using (var cssMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(_css)))
                {
                    using (var htmlMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(GetHtml())))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, htmlMemoryStream, cssMemoryStream);
                    }
                }

                document.Close();

                pdf = memoryStream.ToArray();
            }
            return new FileContentResult(pdf, "application/pdf");
        }
        private string GetHtml()
        {
            var html = File.ReadAllText(_file);
            return Razor.Parse(html, _review);
        }
    }
}