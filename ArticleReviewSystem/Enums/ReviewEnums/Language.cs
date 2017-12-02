using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.Enums.ReviewEnums
{
    public enum Language
    {

        Satisfactory,
        [Display(Name = "Should be rearranged to improve clarity")]
        NeedsCorrections,
        [Display(Name = "Should be rearranged to improve clarity")]
        NeedsSubstantialRevision
    }
}