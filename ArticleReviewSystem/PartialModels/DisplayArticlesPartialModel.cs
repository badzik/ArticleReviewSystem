using ArticleReviewSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.PartialModels
{
    public class DisplayArticlesPartialModel
    {
        public IEnumerable<Article> Articles { get; set; }
        public int MaxPages { get; set; }
    }
}