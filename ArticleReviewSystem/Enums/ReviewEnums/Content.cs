using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.Enums.ReviewEnums
{
    public enum Content
    {
        [Display(Name = "New technique of theory")]
        NewTechniqueOrTheory,
        [Display(Name = "New application of known concepts")]
        NewApplicationOfKnownConcepts,
        [Display(Name = "Confirmationo of known techniques")]
        ConfirmationOfKnownTechniques,
        [Display(Name = "Repetition of known techniques")]
        RepetitionOfKnownTechniques,
        [Display(Name = "Too speculative or theoretical")]
        TooSpeculativeOrTheoretical,
        [Display(Name = "Too technical")]
        TooTechnical,
        [Display(Name = "Too preliminary")]
        TooPreliminary
    }
}