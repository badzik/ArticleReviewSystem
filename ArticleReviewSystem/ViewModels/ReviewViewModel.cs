using ArticleReviewSystem.Enums.ReviewEnums;
using ArticleReviewSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.ViewModels
{
    public class ReviewViewModel
    {
        public string ArticleTitle { get; set; }
        public int ArticleID { get; set; }
        [Required(ErrorMessage ="This field is required")]
        [EnumDataType(typeof(Scope))]
        public Scope? Scope { get; set; } = null;
        [Required(ErrorMessage = "This field is required")]
        [EnumDataType(typeof(Illustrations))]
        public Illustrations? Illustrations { get; set; } = null;
        [Required(ErrorMessage = "This field is required")]
        [EnumDataType(typeof(Content))]
        public Content? Content { get; set; } = null;
        [Required(ErrorMessage = "This field is required")]
        [EnumDataType(typeof(Tables))]
        public Tables? Tables { get; set; } = null;
        [Required(ErrorMessage = "This field is required")]
        [EnumDataType(typeof(ConclusionDrawn))]
        public ConclusionDrawn? ConclusionDrawn { get; set; } = null;
        [Required(ErrorMessage = "This field is required")]
        [EnumDataType(typeof(AbbereviationsFormulaeUnits))]
        public AbbereviationsFormulaeUnits? AbbereviationsFormulaeUnits { get; set; } = null;
        [Required(ErrorMessage = "This field is required")]
        [EnumDataType(typeof(LiteratureReferences))]
        public LiteratureReferences? LiteratureReferences { get; set; } = null;
        [Required(ErrorMessage = "This field is required")]
        [EnumDataType(typeof(Presentation))]
        public Presentation? Presentation { get; set; } = null;
        [Required(ErrorMessage = "This field is required")]
        [EnumDataType(typeof(OverallEvaluation))]
        public OverallEvaluation? OverallEvaluation { get; set; } = null;
        [Required(ErrorMessage = "This field is required")]
        [EnumDataType(typeof(Language))]
        public Language? Language { get; set; } = null;
        [Required(ErrorMessage = "This field is required")]
        [EnumDataType(typeof(Abstract))]
        public Abstract? Abstract{ get; set; } = null;
        [Required(ErrorMessage = "This field is required")]
        [EnumDataType(typeof(FinalRecommendation))]
        public FinalRecommendation? FinalRecommendation { get; set; } = null;
        [Required(ErrorMessage = "This field is required")]
        public string DetailComments { get; set; }
    }
}