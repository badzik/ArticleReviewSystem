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

namespace ArticleReviewSystem.Controllers
{
    public class ArticleController : Controller
    {
        [Authorize]
        public ActionResult AddArticle()
        {
            //AddArticleViewModel avm = new AddArticleViewModel(2,7);
            AddArticleViewModel avm = new AddArticleViewModel()
            {
                MaxCoAuthors = 7,
                CoAuthorsCounter = 2
            };
            return View(avm);
        }

        [HttpPost]
        public ActionResult AddArticle(AddArticleViewModel avm,IEnumerable<CoAuthorViewModel> coAuthors)
        {
            avm.CoAuthors = coAuthors;
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
                    for (int i = 0; i < avm.CoAuthorsCounter; i++)
                    {
                        if (avm.CoAuthors.ToList()[i].Name!="" && avm.CoAuthors.ToList()[i].Surname != "" && avm.CoAuthors.ToList()[i].Affiliation != "")
                        {
                            coAuthorsList.Add(new CoAuthor
                            {
                                Name = avm.CoAuthors.ToList()[i].Name,
                                Surname = avm.CoAuthors.ToList()[i].Surname,
                                Affiliation = avm.CoAuthors.ToList()[i].Affiliation
                            });
                        }

                    }

                    db.Articles.Add(new Article
                    {
                        Document = pdfFile,
                        ArticleName = avm.File.FileName,
                        Title = avm.Title,
                        Date = DateTime.Today,
                        Status = "awaiting",
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