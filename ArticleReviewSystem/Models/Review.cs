using ArticleReviewSystem.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.Models
{
    public class Review
    {
        [Key]
        public int ReviewId{ get; set; }
        public ReviewStatus Status { get; set; }
        public string Scope { get; set; }
        public string Illustrations { get; set; }
        public string Content { get; set; }
        public string Tables { get; set; }
        public string ConclusionDraw { get; set; }
        public string AbbreviationsFormulaeUnits { get; set; }
        public string LiteratureReferences { get; set; }
        public string Presentation { get; set; }
        public string Language { get; set; }
        public string Abstract { get; set; }
        public string OverallEvaluation { get; set; }
        public string FinalRecommendation { get; set; }
        public string DetailComments { get; set; }

        public virtual ApplicationUser Reviewer { get; set; }
        public virtual Article RelatedArticle { get; set; }
    }
}