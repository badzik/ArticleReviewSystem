using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.Enums
{
    public enum ReviewStatus
    {
        NotReviewedYet,
        ReviewedPositively,
        ArticleNeedMinorChanges,
        ArticleNeedImprovement,
        ReviewedNegatively
    }
}