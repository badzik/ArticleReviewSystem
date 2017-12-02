using ArticleReviewSystem.Enums;
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
        ApplicationDbContext dbContext = new ApplicationDbContext();
        // GET: Review
        [Authorize]
        public ActionResult ArticlesForReview()
        {
            ArticlesReviewersViewModel articlesReviewersViewModel = new ArticlesReviewersViewModel();
            ApplicationDbContext dbContext = new ApplicationDbContext();
            ////TODO: check working this element. Should show article only for this user. Now i want see every article 
            articlesReviewersViewModel.Articles = dbContext.Articles.Where(x => x.Reviews.Any(y => y.Reviewer.UserName == User.Identity.Name)).OrderBy(a => a.Reviews.Count).Take(10).ToList();
            //articlesReviewersViewModel.Articles = dbContext.Articles.OrderBy(a => a.Reviews.Count).Take(10).ToList();
            articlesReviewersViewModel.CurrentPage = 1;
            articlesReviewersViewModel.ResultsForPage = 10;
            articlesReviewersViewModel.SortBy = Enums.ArticleSortBy.NumberOfAssignedReviewersAsc;
            articlesReviewersViewModel.NumberOfPages = (int)Math.Ceiling((double)dbContext.Articles.Count() / articlesReviewersViewModel.ResultsForPage);
            return View(articlesReviewersViewModel);
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddReview(int? articleID)
        {
            Article article;
            try
            {
                article= dbContext.Articles.SingleOrDefault(x => x.ArticleId == articleID);
                var authorization = article.Reviews.Any(y => y.Reviewer.UserName == User.Identity.Name);
                if (!authorization)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new ReviewViewModel { ArticleName = article.Title});
        }
        [Authorize]
        [HttpPost]
        public ActionResult AddReview(ReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var reviewStatus = (ReviewStatus) Enum.Parse(typeof(ReviewStatus), model.FinalRecommendation.ToString()) ;
            Review review = new Review
            {
                AbbreviationsFormulaeUnits = model.AbbereviationsFormulaeUnits.ToString(),
                Abstract = model.Abstract.ToString(),
                ConclusionDraw = model.ConclusionDrawn.ToString(),
                Content = model.Content.ToString(),
                FinalRecommendation = model.FinalRecommendation.ToString(),
                Illustrations = model.Illustrations.ToString(),
                Language = model.Language.ToString(),
                LiteratureReferences = model.LiteratureReferences.ToString(),
                OverallEvaluation = model.OverallEvaluation.ToString(),
                Presentation = model.Presentation.ToString(),
                Scope = model.Scope.ToString(),
                Tables = model.Tables.ToString(),
                Status = reviewStatus
            };
            //its update nod added new row becouse Bartek create empty review.
            //modal probably need ajax becouse i need show this only when model.isValid.
           


            return View(model);
        }

    }
}