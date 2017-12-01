using ArticleReviewSystem.Enums.ReviewEnums;
using ArticleReviewSystem.Models;
using ArticleReviewSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArticleReviewSystem.Controllers
{
    public class ReviewController : Controller
    {
        // GET: Review
        [Authorize]
        
        public ActionResult ArticlesForReview()
        {
            ArticlesReviewersViewModel articlesReviewersViewModel = new ArticlesReviewersViewModel();
            ApplicationDbContext dbContext = new ApplicationDbContext();
            ////TODO: check working this element. Should show article only for this user. Now i want see every article 
            ////articlesReviewersViewModel.Articles = dbContext.Articles.Where(x => x.Reviews.Any(y => y.Reviewer.UserName == User.Identity.Name)).OrderBy(a => a.Reviews.Count).Take(10).ToList();
            articlesReviewersViewModel.Articles = dbContext.Articles.OrderBy(a => a.Reviews.Count).Take(10).ToList();
            articlesReviewersViewModel.CurrentPage = 1;
            articlesReviewersViewModel.ResultsForPage = 10;
            articlesReviewersViewModel.SortBy = Enums.ArticleSortBy.NumberOfAssignedReviewersAsc;
            articlesReviewersViewModel.NumberOfPages = (int)Math.Ceiling((double)dbContext.Articles.Count() / articlesReviewersViewModel.ResultsForPage);
            return View(articlesReviewersViewModel);
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddReview()
        {
            ReviewViewModel model = new ReviewViewModel();
            Scope scope = Scope.
            return View(model);
        }
        [Authorize]
        [HttpPost]
        public ActionResult AddReview(ReviewViewModel model)
        {
            var x = model.Scope.ToString();
            return View(model);
        }

    }
}