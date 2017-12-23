using ArticleReviewSystem.Enums;
using ArticleReviewSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.ViewModels
{
    public class DisplayArticleViewModel
    {
        public byte[] Document { get; set; }
        [Display(Name = "Article name")]
        public string ArticleName { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public ArticleStatus Status { get; set; }
        public ICollection<CoAuthor> CoAuthors { get; set; }
    }
}