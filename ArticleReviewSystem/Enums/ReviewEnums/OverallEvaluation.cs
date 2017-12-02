using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.Enums.ReviewEnums
{
    public enum OverallEvaluation
    {
        Excellent,
        Good,
        Acceptable,
        Poor,
        [Display(Name = "Sound, but dull")]
        SoundButDull,
        [Display(Name = "Witchout obvious significance")]
        WitchoutObviousSignificance
    }
}