using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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

        public int MaxCoAuthors { get; set; }
        public int CoAuthorsCounter { get; set; }


        [Display(Name = "Co-authors")]
        public List<CoAuthorViewModel> CoAuthors;

        public Boolean onlyReupload { get; set; }
    }
}