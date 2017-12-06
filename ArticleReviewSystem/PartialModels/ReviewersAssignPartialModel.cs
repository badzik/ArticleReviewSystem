using ArticleReviewSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.PartialModels
{
    public class ReviewersAssignPartialModel
    {
        public List<ApplicationUser> AvailableReviewers { get; set; }
        public int MaxPages { get; set; }
    }
}