using ArticleReviewSystem.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
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
            var html = GetHtml();
            Byte[] bytes = new SimplePechkin(new GlobalConfig()).Convert(html);
            return new FileContentResult(bytes, "application/pdf");
        }
        private string GetHtml()
        {
            var html = File.ReadAllText(_file);
            return Razor.Parse(html, _review);
        }
    }
}