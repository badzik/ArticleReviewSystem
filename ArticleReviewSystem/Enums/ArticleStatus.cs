using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.Enums
{
    public enum ArticleStatus
    {
        [Description("Waiting for assignation")]
        WaitingToAssignReviewers,
        [Description("Reviewers assigned")]
        ReviewersAssigned,
        [Description("Need new reviewer")]
        NewReviewerNeeded,
        [Description("Positevley Reviewed")]
        PositivelyReviewed,
        [Description("Minor changes need")]
        MinorChangesWithoutNewReview,
        [Description("New reviewer needed")]
        ChangesWithNewReview,
        [Description("Negatively reviewed")]
        NegativelyReviewed,
    }
}