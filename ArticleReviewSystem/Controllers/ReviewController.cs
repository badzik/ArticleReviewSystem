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
            return View(new ReviewViewModel { ArticleTitle = article.Title, ArticleID = article.ArticleId});
        }
        [Authorize]
        [HttpPost]
        public ActionResult AddReview(ReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //TODO: add 'updating article status' I need waiting on Becia
            var article = dbContext.Articles.SingleOrDefault(m => m.ArticleId == model.ArticleID);
            var reviewStatus = (ReviewStatus) Enum.Parse(typeof(ReviewStatus), model.FinalRecommendation.ToString()) ;
            var emptyReview = dbContext.Reviews.SingleOrDefault(x => x.Reviewer.UserName == User.Identity.Name && x.RelatedArticle.ArticleId == article.ArticleId);
            emptyReview.AbbreviationsFormulaeUnits = model.AbbereviationsFormulaeUnits.ToString();
            emptyReview.Abstract = model.Abstract.ToString();
            emptyReview.ConclusionDraw = model.ConclusionDrawn.ToString();
            emptyReview.Content = model.Content.ToString();
            emptyReview.FinalRecommendation = model.FinalRecommendation.ToString();
            emptyReview.Illustrations = model.Illustrations.ToString();
            emptyReview.Language = model.Language.ToString();
            emptyReview.LiteratureReferences = model.LiteratureReferences.ToString();
            emptyReview.OverallEvaluation = model.OverallEvaluation.ToString();
            emptyReview.Presentation = model.Presentation.ToString();
            emptyReview.Scope = model.Scope.ToString();
            emptyReview.Tables = model.Tables.ToString();
            emptyReview.Status = reviewStatus;
            dbContext.SaveChanges();
            return RedirectToAction("ArticlesForReview", "Review");
        }

    }
}