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
    }
}