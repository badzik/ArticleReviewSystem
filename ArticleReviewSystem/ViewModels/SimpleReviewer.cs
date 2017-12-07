using ArticleReviewSystem.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.ViewModels
{
    public class SimpleReviewer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Affiliation { get; set; }
        public ReviewStatus ReviewStatus { get; set; }
    }
}