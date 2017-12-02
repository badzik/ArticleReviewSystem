using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.Enums.ReviewEnums
{
    public enum Presentation
    {
        Adequate,
        [Display(Name = "Too brief for clarity")]
        TooBriefForClarity,
        [Display(Name = "Too comprehensive")]
        TooComprehensive,
        [Display(Name = "General organisation unsuitable")]
        GeneralOrganisationUnsuitale,
        [Display(Name = "Badly written, hardly readable")]
        BadlyWritten,
        [Display(Name = "Contains irrevelant material")]
        ContainsIrrelevantMaterial
    }
}