using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.Enums.ReviewEnums
{
    public enum AbbereviationsFormulaeUnits
    {
        [Display(Name = "Confirm to accepted norm")]
        ConformToAcceptedNorm,
        [Display(Name = "Should be changed")]
        ShouldBeChanged,
        [Display(Name = "Should bee explained")]
        ShouldBeExplained
    }
}