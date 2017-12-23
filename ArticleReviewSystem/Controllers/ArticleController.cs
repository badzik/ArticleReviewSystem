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
                ArticleName=null
            };
            return View(avm);
        }

        [HttpPost]
        public ActionResult AddArticle(AddArticleViewModel avm,IEnumerable<CoAuthorViewModel> coAuthors)
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
                    if (db.Articles.Any(a => a.Title.ToUpper() == avm.Title.ToUpper())){
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
                        CoAuthors=coAuthorsList
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
            var userId = User.Identity.GetUserId();

            return View(db.Articles.Where(m => m.MainAuthor.Id.Equals(userId)).ToList());
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
            ApplicationDbContext db = new ApplicationDbContext();

            Article article = db.Articles.Find(articleId);
            EditArticleViewModel evm = new EditArticleViewModel();
            evm.ArticleId = article.ArticleId;
            evm.ArticleName = article.ArticleName;
            evm.Title = article.Title;

            return View(evm);
        }

        [HttpPost]
        public ActionResult EditArticle(EditArticleViewModel evm)
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

            if (evm.File != null)
            {
                Stream str = evm.File.InputStream;
                BinaryReader Br = new BinaryReader(str);
                pdfFile = Br.ReadBytes((Int32)str.Length);
                article.Document = pdfFile;
                article.ArticleName = evm.File.FileName;
            }
            article.Title = evm.Title;
            article.Date = DateTime.Now;

            db.SaveChanges();

            return RedirectToAction("DisplayArticles");
        }



    }
}