using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.Enums.ReviewEnums
{
    public enum FinalRecommendation
    {
        [Description("Accept with no changes")]
        [Display(Name = "Accept with no changes")]
        ReviewedPositively,
        [Description("Accept after minor revisions (without second review)")]
        [Display(Name = "Accept after minor revisions (without second review)")]
        ArticleNeedMinorChanges,
        [Description("Revise and resubmit)")]
        [Display(Name = "Revise and resubmit")]
        ArticleNeedImprovement,
        [Description("Reject")]
        [Display(Name = "Reject")]
        ReviewedNegatively
    }
}