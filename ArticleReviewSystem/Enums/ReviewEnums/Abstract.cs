using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.Enums.ReviewEnums
{
    public enum Abstract
    {
        [Display(Name = "Clear and adequate")]
        ClearAndAdequate,
        [Display(Name = "Should be rewritten (or missing)")]
        ShouldBeRewritten
    }
}