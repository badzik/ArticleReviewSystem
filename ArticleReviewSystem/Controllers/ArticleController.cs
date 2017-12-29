using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ArticleReviewSystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ArticleReviewSystem.ViewModels;
using ArticleReviewSystem.Enums;
using System.Collections;
using ArticleReviewSystem.PartialModels;

namespace ArticleReviewSystem.Controllers
{
    public class ArticleController : Controller
    {
        [Authorize]
        public ActionResult AddArticle()
        {
            AddArticleViewModel avm = new AddArticleViewModel()
            {
                MaxCoAuthors = 7,
                CoAuthorsCounter = 2,
                ArticleName = null
            };
            return View(avm);
        }

        [HttpPost]
        public ActionResult AddArticle(AddArticleViewModel avm, IEnumerable<CoAuthorViewModel> coAuthors)
        {
            avm.CoAuthors = coAuthors;
            if (String.IsNullOrEmpty(avm.Title))
            {
                ModelState.AddModelError("Title", "Title is required");
                return View(avm);
            }
            if (avm.File != null && avm.File.ContentLength > 0)
            {
                String extension = Path.GetExtension(avm.File.FileName).ToUpper();

                if (extension == ".PDF")
                {
                    ApplicationDbContext db = new ApplicationDbContext();
                    var userId = User.Identity.GetUserId();

                    Stream str = avm.File.InputStream;
                    BinaryReader Br = new BinaryReader(str);
                    Byte[] pdfFile = Br.ReadBytes((Int32)str.Length);
                    List<CoAuthor> coAuthorsList = new List<CoAuthor>();
                    if (db.Articles.Any(a => a.Title.ToUpper() == avm.Title.ToUpper()))
                    {
                        ModelState.AddModelError("Title", "There is an article with a same title");
                        return View(avm);
                    }
                    for (int i = 0; i < avm.CoAuthorsCounter; i++)
                    {
                        if (!String.IsNullOrEmpty(avm.CoAuthors.ToList()[i].Name) && !String.IsNullOrEmpty(avm.CoAuthors.ToList()[i].Surname) && !String.IsNullOrEmpty(avm.CoAuthors.ToList()[i].Affiliation))
                        {
                            coAuthorsList.Add(new CoAuthor
                            {
                                Name = avm.CoAuthors.ToList()[i].Name,
                                Surname = avm.CoAuthors.ToList()[i].Surname,
                                Affiliation = avm.CoAuthors.ToList()[i].Affiliation
                            });
                        }
                        else
                        {
                            ModelState.AddModelError("CoAuthors", "CoAuthors fields are not filled properly");
                            return View(avm);
                        }

                    }

                    db.Articles.Add(new Article
                    {
                        Document = pdfFile,
                        ArticleName = avm.File.FileName,
                        Title = avm.Title,
                        Date = DateTime.Today,
                        Status = ArticleStatus.WaitingToAssignReviewers,
                        MainAuthor = db.Users.Find(userId),
                        CoAuthors = coAuthorsList
                    });

                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewData["Extension"] = "Wrong article format, PDF is required.";
                    return View();
                }
            }
            return View(avm);

        }


        public ActionResult DisplayArticles()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            DisplayArticlesViewModel davm = new DisplayArticlesViewModel();
            var userId = User.Identity.GetUserId();
            davm.Articles = db.Articles.Where(m => m.MainAuthor.Id.Equals(userId)).OrderBy(a => a.Title).Take(10).ToList();
            davm.CurrentPage = 1;
            davm.ResultsForPage = 10;
            davm.SortBy = Enums.ArticleSortBy.Title;
            davm.NumberOfPages = (int)Math.Ceiling((double)db.Articles.Where(m => m.MainAuthor.Id.Equals(userId)).Count() / davm.ResultsForPage);
            return View(davm);
        }

        [HttpPost]
        public ActionResult DisplayArticles(DisplayArticlesViewModel davm)
        {
            DisplayArticlesPartialModel dapm = new DisplayArticlesPartialModel();
            ApplicationDbContext dbContext = new ApplicationDbContext();
            List<Article> articles = new List<Article>();
            var userId = User.Identity.GetUserId();
            articles = dbContext.Articles.Where(m => m.MainAuthor.Id.Equals(userId)).ToList();
            if (!String.IsNullOrEmpty(davm.SearchPhrase))
            {
                articles = articles.Where(a => a.Title.ToLower().Contains(davm.SearchPhrase.ToLower()) || a.Status.ToString().ToLower().Contains(davm.SearchPhrase.ToLower()) || a.ArticleName.ToLower().Contains(davm.SearchPhrase.ToLower())).ToList();
            }
            switch (davm.SortBy)
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
            dapm.MaxPages = (int)Math.Ceiling((double)articles.Count / (double)davm.ResultsForPage);
            dapm.Articles = articles.Skip((davm.CurrentPage - 1) * davm.ResultsForPage).Take(davm.ResultsForPage).ToList();
            return PartialView("_DisplayArticles", dapm);
        }

        [HttpGet]
        public ActionResult DisplayPDF(int articleId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var userId = User.Identity.GetUserId();

            byte[] byteArray = db.Articles.Where(m => m.MainAuthor.Id.Equals(userId) && m.ArticleId.Equals(articleId))
                .Select(m => m.Document).FirstOrDefault();
            MemoryStream pdfStream = new MemoryStream();
            pdfStream.Write(byteArray, 0, byteArray.Length);
            pdfStream.Position = 0;
            return new FileStreamResult(pdfStream, "application/pdf");
        }

        [HttpGet]
        public ActionResult DisplayArticleDetails(int articleId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var userId = User.Identity.GetUserId();
            ViewData["article_id"] = articleId;

            DisplayArticleViewModel model = new DisplayArticleViewModel();

            var article = db.Articles.Find(articleId);

            model.ArticleName = article.ArticleName;
            model.Document = article.Document;
            model.Date = article.Date;
            model.Status = article.Status;
            model.Title = article.Title;
            model.CoAuthors = article.CoAuthors;

            return View(model);
        }

        public ActionResult DeleteArticle(int articleId)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            ViewData["article_id"] = articleId;

            var test = db.CoAuthors.Where(m => m.CoAuthoredArticle.ArticleId == articleId);
            foreach (var item in test)
                db.CoAuthors.Remove(item);

            Article article = db.Articles.Find(articleId);
            db.Articles.Remove(article);
            //CoAuthor coAuthor = db.CoAuthors.Find(articleId);
            //db.CoAuthors.Remove(coAuthor);



            db.SaveChanges();
            return RedirectToAction("DisplayArticles");
        }

        [HttpGet]
        public ActionResult EditArticle(int articleId)
        {
            EditArticleViewModel evm = prepareEditArticleViewModel(articleId);
            return View(evm);
        }

        [HttpPost]
        public ActionResult EditArticle(EditArticleViewModel evm, IEnumerable<CoAuthorViewModel> coAuthors)
        {

            if (!ModelState.IsValid)
            {
                List<string> errors = new List<string>();
                foreach (var modelStateVal in ViewData.ModelState.Values)
                {
                    foreach (var error in modelStateVal.Errors)
                    {
                        var errorMessage = error.ErrorMessage;
                        var exception = error.Exception;
                        errors.Add(errorMessage);
                    }
                }
                foreach (string error in errors)
                {
                    ModelState.AddModelError("", error);
                }

                return View(evm);
            }

            ApplicationDbContext db = new ApplicationDbContext();

            Article article = db.Articles.Find(evm.ArticleId);
            Byte[] pdfFile = null;

            if (evm.onlyReupload && evm.File == null)
            {
                evm = prepareEditArticleViewModel(article.ArticleId);
                ModelState.AddModelError("", "You need to reupload changed article");
                return View(evm);
            }

            if (evm.File != null)
            {
                Stream str = evm.File.InputStream;
                BinaryReader Br = new BinaryReader(str);
                pdfFile = Br.ReadBytes((Int32)str.Length);
                article.Document = pdfFile;
                article.ArticleName = evm.File.FileName;
                if (evm.onlyReupload)
                {
                    switch (article.Status)
                    {
                        case ArticleStatus.MinorChangesWithoutNewReview:
                            {
                                article.Status = ArticleStatus.PositivelyReviewed;
                                break;
                            }
                        case ArticleStatus.ChangesWithNewReview:
                            {
                                article.Status = ArticleStatus.ReReviewsNeeded;
                                foreach (Review actualReview in article.Reviews)
                                {
                                    actualReview.Status = ReviewStatus.NotReviewedYet;
                                }
                                break;
                            }
                    }
                }

                article.Title = evm.Title;
                article.Date = DateTime.Now;
                List<CoAuthor> oldCoAuthors = db.CoAuthors.Where(c => c.CoAuthoredArticle.ArticleId == article.ArticleId).ToList();
                //delete old coauthors
                foreach (CoAuthor co in oldCoAuthors)
                {
                    db.CoAuthors.Remove(co);
                }

                //create new coauthors
                int count = 0;
                foreach (CoAuthorViewModel c in coAuthors)
                {
                    if (String.IsNullOrEmpty(c.Affiliation) || String.IsNullOrEmpty(c.Name) || String.IsNullOrEmpty(c.Surname))
                    {
                        evm = prepareEditArticleViewModel(article.ArticleId);
                        ModelState.AddModelError("", "All fields in co-authors info can't be empty");
                        return View(evm);
                    }
                    CoAuthor co = new CoAuthor()
                    {
                        Affiliation = c.Affiliation,
                        Name = c.Name,
                        Surname = c.Surname,
                        CoAuthoredArticle = article
                    };
                    db.CoAuthors.Add(co);
                    count++;
                    if (count >= evm.CoAuthorsCounter)
                    {
                        break;
                    }
                }
                db.SaveChanges();

                return RedirectToAction("DisplayArticleDetails", "Article", new { articleId = article.ArticleId });
            }

            private EditArticleViewModel prepareEditArticleViewModel(int articleId)
            {
                ApplicationDbContext db = new ApplicationDbContext();

                Article article = db.Articles.Find(articleId);
                EditArticleViewModel evm = new EditArticleViewModel();
                evm.ArticleId = article.ArticleId;
                evm.ArticleName = article.ArticleName;
                evm.Title = article.Title;
                evm.CoAuthorsCounter = article.CoAuthors.Count;
                evm.MaxCoAuthors = 7;
                List<CoAuthorViewModel> coAuthorsList = new List<CoAuthorViewModel>();
                foreach (CoAuthor ca in article.CoAuthors)
                {
                    coAuthorsList.Add(new CoAuthorViewModel()
                    {
                        Affiliation = ca.Affiliation,
                        Name = ca.Name,
                        Surname = ca.Surname
                    });
                }
                //preparing empty coauthor for possibility of adding new ones
                for (int i = evm.CoAuthorsCounter; i < 7; i++)
                {
                    coAuthorsList.Add(new CoAuthorViewModel()
                    {
                        Affiliation = "",
                        Name = "",
                        Surname = ""
                    });
                }
                evm.CoAuthors = coAuthorsList;
                if (article.Status == ArticleStatus.MinorChangesWithoutNewReview || article.Status == ArticleStatus.ChangesWithNewReview)
                {
                    evm.onlyReupload = true;
                }
                else
                {
                    evm.onlyReupload = false;
                }
                return evm;
            }

        }
    }