using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ArticleReviewSystem.ViewModels;

namespace ArticleReviewSystem.ViewModels
{
    public class AddArticleViewModel
    {

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Co-authors")]
        public IEnumerable<CoAuthorViewModel> CoAuthors;

        public int MaxCoAuthors { get; set; }
        public int CoAuthorsCounter { get; set; }

        public string ArticleName { get; set; }

        [Required]
        [DataType(DataType.Upload)]  
        [Display(Name = "File")]
        public HttpPostedFileBase File { get; set; }
    }
}