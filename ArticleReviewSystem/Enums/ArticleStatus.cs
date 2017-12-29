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
        [Description("Need newly assigned reviewer review")]
        NewReviewerReview,
        [Description("Positevley Reviewed")]
        PositivelyReviewed,
        [Description("Minor changes need")]
        MinorChangesWithoutNewReview,
        [Description("Need to reupload changed article for new review")]
        ChangesWithNewReview,
        [Description("New reviewes needed")]
        ReReviewsNeeded,
        [Description("Negatively reviewed")]
        NegativelyReviewed,
    }
}