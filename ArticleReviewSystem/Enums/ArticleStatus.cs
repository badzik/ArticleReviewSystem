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
        [Description("Positevley Reviewed")]
        PositivelyReviewed,
        [Description("Negatively reviewed")]
        NegativelyReviewed,
    }
}