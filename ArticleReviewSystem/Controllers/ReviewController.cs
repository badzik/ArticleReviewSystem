using ArticleReviewSystem.Enums;
using ArticleReviewSystem.Enums.ReviewEnums;
using ArticleReviewSystem.Helpers;
using ArticleReviewSystem.Models;
using ArticleReviewSystem.PartialModels;
using ArticleReviewSystem.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
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
            articlesReviewersViewModel.Articles = dbContext.Articles.Where(x => x.Reviews.Any(y => y.Reviewer.UserName == User.Identity.Name)).OrderBy(a => a.Reviews.Count).Take(10).ToList();
            articlesReviewersViewModel.CurrentPage = 1;
            articlesReviewersViewModel.ResultsForPage = 10;
            articlesReviewersViewModel.SortBy = Enums.ArticleSortBy.NumberOfAssignedReviewersAsc;
            articlesReviewersViewModel.NumberOfPages = (int)Math.Ceiling((double)dbContext.Articles.Count() / articlesReviewersViewModel.ResultsForPage);
            return View(articlesReviewersViewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ArticlesForReview(ArticlesReviewersViewModel articlesReviewersViewModel)
        {

            ApplicationDbContext dbContext = new ApplicationDbContext();
            List<Article> articles = new List<Article>();
            var userId = User.Identity.GetUserId();
            articles = dbContext.Articles.Where(x => x.Reviews.Any(y => y.Reviewer.UserName == User.Identity.Name)).OrderBy(a => a.Reviews.Count).Take(10).ToList();
            if (!String.IsNullOrEmpty(articlesReviewersViewModel.SearchPhrase))
            {
                articles = articles.Where(a => a.Title.ToLower().Contains(articlesReviewersViewModel.SearchPhrase.ToLower()) || a.Status.ToString().ToLower().Contains(articlesReviewersViewModel.SearchPhrase.ToLower()) || a.ArticleName.ToLower().Contains(articlesReviewersViewModel.SearchPhrase.ToLower())).ToList();
            }
            switch (articlesReviewersViewModel.SortBy)
            {
                case ArticleSortBy.Title:
                    articles = articles.OrderBy(r => r.Title).ToList();
                    break;
                case ArticleSortBy.Status:
                    articles = articles.OrderBy(r => r.Status).ToList();
                    break;
                case ArticleSortBy.ArticleName:
                    articles = articles.OrderBy(r => r.ArticleName).ToList();
                    break;
            }
            ArticlesForReviewPartialModel articlesForReviewPartialModel = new ArticlesForReviewPartialModel
            {
                MaxPages = (int)Math.Ceiling((double)articles.Count / (double)articlesReviewersViewModel.ResultsForPage),
                Articles = articles.Skip((articlesReviewersViewModel.CurrentPage - 1) * articlesReviewersViewModel.ResultsForPage).Take(articlesReviewersViewModel.ResultsForPage).ToList(),
            };
            return PartialView("_ArticlesForReview", articlesForReviewPartialModel);
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddReview(int? articleID)
        {
            Article article;
            try
            {
                article = dbContext.Articles.SingleOrDefault(x => x.ArticleId == articleID);
                var authorization = article.Reviews.Any(y => y.Reviewer.UserName == User.Identity.Name);
                var emptyReview = dbContext.Reviews.SingleOrDefault(x => x.Reviewer.UserName == User.Identity.Name && x.RelatedArticle.ArticleId == article.ArticleId);
                if (!authorization || emptyReview.FinalRecommendation != null)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new ReviewViewModel { ArticleTitle = article.Title, ArticleID = article.ArticleId });
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
            var reviewStatus = (ReviewStatus)Enum.Parse(typeof(ReviewStatus), model.FinalRecommendation.ToString());
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
            emptyReview.DetailComments = model.DetailComments;

            var ratedReview = article.Reviews.Where(x => x.Status != ReviewStatus.NotReviewedYet);

            var ratedPositively = ratedReview.Where(x => x.Status == ReviewStatus.ReviewedPositively);
            var ratedArticleNeedMinorChanges = ratedReview.Where(x => x.Status == ReviewStatus.ArticleNeedMinorChanges);
            var ratedRevieseAndReupload = ratedReview.Where(x => x.Status == ReviewStatus.ArticleNeedImprovement);
            var ratedNegatively = ratedReview.Where(x => x.Status == ReviewStatus.ReviewedNegatively);

            if(article.Status == ArticleStatus.NewReviewerReview)
            {
                if (emptyReview.Status == ReviewStatus.ReviewedPositively || emptyReview.Status == ReviewStatus.ArticleNeedMinorChanges)
                    article.Status = ArticleStatus.PositivelyReviewed;
                else
                    article.Status = ArticleStatus.NegativelyReviewed;
                return RedirectToAction("ArticlesForReview", "Review");
            }

            if (ratedNegatively.Count() > 1) //4 + 4
                article.Status = ArticleStatus.NegativelyReviewed;
            else
            if (ratedNegatively.Count() == 1 && ratedReview.Count() > 1) // 4 + n
                article.Status = ArticleStatus.NewReviewerNeeded;
            else
            if (ratedPositively.Count() > 1) // 1 + 1 
                article.Status = ArticleStatus.PositivelyReviewed;
            else
            if ((ratedArticleNeedMinorChanges.Count() == 1 && ratedPositively.Count() == 1) || ratedArticleNeedMinorChanges.Count() > 1) // 1+2, 2+2
                article.Status = ArticleStatus.MinorChangesWithoutNewReview;
            else
            if (ratedArticleNeedMinorChanges.Count() == 1 && ratedRevieseAndReupload.Count() == 1 || ratedRevieseAndReupload.Count() > 1) // 2+3, 3+3
                article.Status = ArticleStatus.ChangesWithNewReview;

            dbContext.SaveChanges();
            return RedirectToAction("ArticlesForReview", "Review");
        }
        [Authorize]
        public ActionResult ShowReview(int? articleID)
        {
            Review review = dbContext.Articles.SingleOrDefault(x => x.ArticleId == articleID).
                 Reviews.SingleOrDefault(y => y.Reviewer.UserName == User.Identity.Name);
            if (review == null)
            {
                return RedirectToAction("Index", "Home");
            }
            PdfBuilder builder = new PdfBuilder(review, Server.MapPath("/Views/Review/Pdf.cshtml"), Server.MapPath("/Content/pdf.css"));
            return builder.GetPdf();
        }
        [Authorize]
        public ActionResult ShowArticle(int? articleID)
        {
            var article = dbContext.Articles.SingleOrDefault(x => x.ArticleId == articleID);
            var auth = article.Reviews.SingleOrDefault(y => y.Reviewer.UserName == User.Identity.Name) != null;
            if (auth)
                return File(article.Document, "application/pdf");
            else
                return RedirectToAction("Index", "Home");
        }
    }
}