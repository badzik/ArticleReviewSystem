using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.Enums.ReviewEnums
{
    public enum Scope
    {
        [Display(Name = "Of General Relevance")]
        OfGeneralRelevance,
        [Display(Name = "Relevant to the field of DSP")]
        RelevantToTheFieldOfDSP,
        [Display(Name = "Very Specialisted")]
        VerySpecialisted,
        [Display(Name = "Out of scope")]
        OutOfScope
    }
}