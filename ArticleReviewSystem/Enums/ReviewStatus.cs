using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.Enums
{
    public enum ReviewStatus
    {
        [Display(Name = "Not Reviewed Yet")]
        NotReviewedYet,
        [Display(Name = "Reviewed Positively")]
        ReviewedPositively,
        [Display(Name = "Reviewed Negatively")]
        ReviewedNegatively,
        [Display(Name = "Article Need Improvement")]
        ArticleNeedImprovement,
        [Display(Name = "Article Need Minor Changes")]
        ArticleNeedMinorChanges
    }
}