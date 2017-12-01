using ArticleReviewSystem.Enums.ReviewEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.ViewModels
{
    public class ReviewViewModel
    {
        //Required dont work. I use nullable enum and i must check it in code (controler)
        public Scope? Scope { get; set; } = null;
    }
}