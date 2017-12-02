using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.Enums.ReviewEnums
{
    public enum FinalRecommendation
    {
        [Display(Name = "Accept with no changes")]
        ReviewedPositively,
        [Display(Name = "Accept after minor revisions (witchout second review)")]
        ArticleNeedMinorChanges,
        [Display(Name = "Revise and resubmit")]
        ArticleNeedImprovement,
        [Display(Name = "Reject")]
        ReviewedNegatively
    }
}