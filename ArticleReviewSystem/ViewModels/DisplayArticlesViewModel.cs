﻿using ArticleReviewSystem.Enums;
using ArticleReviewSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.ViewModels
{
    public class DisplayArticlesViewModel
    {
        public IEnumerable<Article> Articles { get; set; }
        public string SearchPhrase { get; set; }
        public ArticleSortBy SortBy { get; set; }
        public int CurrentPage { get; set; }
        public int NumberOfPages { get; set; }
        public int ResultsForPage { get; set; }
    }
}