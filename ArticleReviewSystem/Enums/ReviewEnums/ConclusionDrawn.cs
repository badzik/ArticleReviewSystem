using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.Enums.ReviewEnums
{
    public enum ConclusionDrawn
    {
        [Display(Name = "Adequate")]
        Adequate,
        [Display(Name = "Not justified")]
        NotJustified,
        [Display(Name = "Suffer from substantial omnisions")]
        SufferFromSubstantialOmmisions,
        [Display(Name = "Suffer from loose generalisations")]
        SufferFromLooseGeneralisations
    }
}