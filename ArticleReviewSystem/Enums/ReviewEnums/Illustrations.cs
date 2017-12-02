using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.Enums.ReviewEnums
{
    public enum Illustrations
    {
        [Display(Name = "Adequate")]
        Adequate,
        [Display(Name = "Inadequate technical quality")]
        InadequteTechicalQuality,
        [Display(Name = "Non very informative")]
        NonVeryInformative,
        [Display(Name = "Some may be ommited (Write in a detailed comment)")]
        SomeMayBeOmmitted,
        [Display(Name = "More should be added (Write in a detailed comment)")]
        MoreShouldBeAdded
    }
}