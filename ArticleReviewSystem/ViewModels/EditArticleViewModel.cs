using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ArticleReviewSystem.ViewModels;

namespace ArticleReviewSystem.ViewModels
{
    public class EditArticleViewModel
    {
        public int ArticleId { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        public string ArticleName { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "File")]
        public HttpPostedFileBase File { get; set; }
    }
}