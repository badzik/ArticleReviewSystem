using ArticleReviewSystem.Enums;
using ArticleReviewSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.ViewModels
{
    public class ReviewersAssignViewModel
    {
        public Article Article { get; set; }
        public string SearchPhrase { get; set; }
        public UserSortBy SortBy { get; set; }
        public int CurrentPage { get; set; }
        public int NumberOfPages { get; set; }
        public int ResultsForPage { get; set; }
        public List<SimpleUser> AssignedReviewers { get; set; }
        public List<ApplicationUser> AvailableReviewers { get; set; }
    }
}