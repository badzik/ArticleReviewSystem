using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ArticleReviewSystem.Models
{
    public class AddArticleViewModel
    {
        [Required]
        [Display(Name = "Tytuł")]
        public string Title { get; set; }

        [Display(Name = "Imię")]
        public string Name { get; set; }

        [Display(Name = "Nazwisko")]
        public string Surname { get; set; }

        [Required]
        [DataType(DataType.Upload)]  
        [Display(Name = "Plik")]
        public HttpPostedFileBase files { get; set; }
    }
}