using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ArticleReviewSystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ArticleReviewSystem.Controllers
{
    public class ArticleController : Controller
    {
        private ApplicationUser ApplicationUser;


        // GET: Article
        public ActionResult AddArticle()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddArticle(AddArticleViewModel avm)
        {
            if (avm.files != null && avm.files.ContentLength > 0)
            {
                String extension = Path.GetExtension(avm.files.FileName).ToUpper();

                if (extension == ".PDF")
                {
                    ApplicationDbContext db = new ApplicationDbContext();
                    var userId = User.Identity.GetUserId();

                    Stream str = avm.files.InputStream;
                    BinaryReader Br = new BinaryReader(str);
                    Byte[] pdfFile = Br.ReadBytes((Int32)str.Length);

                    db.AspNetArticles.Add(new Articles
                    {
                        Article = pdfFile,
                        ArticleName = avm.files.FileName,
                        Title = avm.Title,
                        Date = DateTime.Today,
                        Status = "oczekiwanie",
                        UserId = userId

                    });
                    for (int i = 0; i < 2; i++)
                    {
                        db.AspNetCoAuthors.Add(new CoAuthors
                        {
                            Name = avm.Name,
                            Surname = avm.Surname,
                            Affiliation = "affiliation",
                            ArticleId = 1
                        });
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewData["Extension"] = "niewłaściwy format pliku";
                    return View();
                }
            }
            return View(avm);

        }
    }
}