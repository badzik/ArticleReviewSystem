using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.Enums.ReviewEnums
{
    public enum LiteratureReferences
    {
        Adequate, 
        Inadequate,
        [Display(Name = "Some may be ommitted")]
        SomeMayBeOmmitted,
        [Display(Name = "More should be added (Write in a detailed comment)")]
        MoreShouldBeAdded
    }
}