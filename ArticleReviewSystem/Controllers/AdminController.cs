using ArticleReviewSystem.Enums;
using ArticleReviewSystem.Models;
using ArticleReviewSystem.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ArticleReviewSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationUserManager userManager;
        private string mailLogin = "articleReviewerTeam@gmail.com";
        private string mailPassword = "articleReview007";

        public AdminController()
        {

        }

        public ActionResult RegisterConfirmation()
        {
            RegisterConfirmationViewModel rcVM = new RegisterConfirmationViewModel();
            ApplicationDbContext dbContext = new ApplicationDbContext();
            rcVM.UnconfirmedUsers = dbContext.Users.Where(m => !m.EmailConfirmed).ToList();
            return View(rcVM);
        }

        public async Task<ActionResult> ConfirmRegistration(string userId)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var user = dbContext.Users.Find(userId);
            await prepareAndSendConfirmationEmail(user);
            user.ConfirmRegistrationDate = DateTime.Now;
            dbContext.SaveChanges();
            return RedirectToAction("RegisterConfirmation");
        }

        public ActionResult DeleteAccount(string userId)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var user = dbContext.Users.Find(userId);
            dbContext.Users.Remove(user);
            dbContext.SaveChanges();
            return RedirectToAction("RegisterConfirmation");
        }

        public ActionResult ArticlesReviewers()
        {
            ArticlesReviewersViewModel arvm = new ArticlesReviewersViewModel();
            ApplicationDbContext dbContext = new ApplicationDbContext();
            arvm.Articles = dbContext.Articles.OrderBy(a => a.Reviews.Count).Take(10).ToList();
            arvm.CurrentPage = 1;
            arvm.ResultsForPage = 10;
            arvm.SortBy = Enums.ArticleSortBy.NumberOfAssignedReviewersAsc;
            arvm.NumberOfPages = (int)Math.Ceiling((double)dbContext.Articles.Count() / arvm.ResultsForPage);
            return View(arvm);
        }

        [HttpPost]
        public ActionResult ArticlesReviewers(ArticlesReviewersViewModel arvm)
        {
            ArticlesReviewersPartialModel nArpm = new ArticlesReviewersPartialModel();
            ApplicationDbContext dbContext = new ApplicationDbContext();
            List<Article> articles = new List<Article>();
            if (!String.IsNullOrEmpty(arvm.SearchPhrase))
            {
                articles = dbContext.Articles.Where(a => a.Title.ToLower().Contains(arvm.SearchPhrase.ToLower()) || a.MainAuthor.Name.ToLower().Contains(arvm.SearchPhrase.ToLower()) || a.MainAuthor.Surname.ToLower().Contains(arvm.SearchPhrase.ToLower())).ToList();
            }
            else
            {
                articles = dbContext.Articles.ToList();
            }
            switch (arvm.SortBy)
            {
                case ArticleSortBy.AddDate:
                    articles = articles.OrderBy(a => a.Date).ToList();
                    break;
                case ArticleSortBy.Author:
                    articles = articles.OrderBy(a => a.MainAuthor.Surname).ToList();
                    break;
                case ArticleSortBy.NumberOfAssignedReviewersAsc:
                    articles = articles.OrderBy(a => a.Reviews.Count).ToList();
                    break;
                case ArticleSortBy.NumberOfAssignedReviewersDsc:
                    articles = articles.OrderByDescending(a => a.Reviews.Count).ToList();
                    break;
                case ArticleSortBy.Title:
                    articles = articles.OrderBy(a => a.Title).ToList();
                    break;
            }
            nArpm.MaxPages = (int)Math.Ceiling((double)articles.Count / (double)arvm.ResultsForPage);
            nArpm.Articles = articles.Skip((arvm.CurrentPage - 1) * arvm.ResultsForPage).Take(arvm.ResultsForPage).ToList();
            return PartialView("_ArticlesReviewers", nArpm);
        }

        public ActionResult ReviewersAssign(int articleId)
        {
            ReviewersAssignViewModel ravm = new ReviewersAssignViewModel();

            ApplicationDbContext dbContext = new ApplicationDbContext();

            ravm.Article = dbContext.Articles.Find(articleId);
            ravm.AssignedReviewers = new List<SimpleUser>();

            foreach (Review r in ravm.Article.Reviews)
            {
                ravm.AssignedReviewers.Add(new SimpleUser() { Affiliation=r.Reviewer.Affiliation , Id=r.Reviewer.Id, Name=r.Reviewer.Name, Surname=r.Reviewer.Surname});
            }

            var role = dbContext.Roles.SingleOrDefault(m => m.Name == "Admin");
            ravm.AvailableReviewers = dbContext.Users.Where(m => m.Roles.All(r => r.RoleId != role.Id) && !m.Affiliation.ToLower().Equals(ravm.Article.MainAuthor.Affiliation.ToLower())).ToList();


            ravm.AvailableReviewers = ravm.AvailableReviewers.OrderBy(r => r.Surname).ToList();
            ravm.SortBy = UserSortBy.Name;
            ravm.CurrentPage = 1;
            ravm.NumberOfPages = (int)Math.Ceiling((double)ravm.AvailableReviewers.Count / (double)10);
            ravm.ResultsForPage = 10;
            return View(ravm);
        }

        [HttpPost]
        public ActionResult ReviewersAssignAdd(ReviewersAssignViewModel ram, string reviewerId)
        {
            //add reviewer to model
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var article = dbContext.Articles.Find(ram.Article.ArticleId);
            List<SimpleUser> assigned = new List<SimpleUser>();
            List<ApplicationUser> available = new List<ApplicationUser>();
            ApplicationUser reviewer;

            //add previous assigned reviewers
            if (ram.AssignedReviewers != null)
            {
                foreach (SimpleUser a in ram.AssignedReviewers)
                {
                    reviewer = dbContext.Users.Find(a.Id);
                    assigned.Add(new SimpleUser() { Affiliation = reviewer.Affiliation, Id = reviewer.Id, Name = reviewer.Name, Surname = reviewer.Surname });
                }
            }

            //add new reviewer
            reviewer = dbContext.Users.Find(reviewerId);
            assigned.Add(new SimpleUser() { Affiliation = reviewer.Affiliation, Id = reviewer.Id, Name = reviewer.Name, Surname = reviewer.Surname });


            var role = dbContext.Roles.SingleOrDefault(m => m.Name == "Admin");
            available = dbContext.Users.Where(m => m.Roles.All(r => r.RoleId != role.Id) && !m.Affiliation.ToLower().Equals(article.MainAuthor.Affiliation.ToLower())).ToList();
            foreach (SimpleUser s in assigned)
            {
                var toRemove = available.FirstOrDefault(a=>a.Id==s.Id);
                available.Remove(toRemove);
            }
            if (!String.IsNullOrEmpty(ram.SearchPhrase))
            {
                available = available.Where(r => r.Name.ToLower().Contains(ram.SearchPhrase.ToLower()) || r.Surname.ToLower().Contains(ram.SearchPhrase.ToLower()) || r.Affiliation.ToLower().Contains(ram.SearchPhrase.ToLower())).ToList();
            }
            switch (ram.SortBy)
            {
                case UserSortBy.Name:
                    available = available.OrderBy(r => r.Name).ToList();
                    break;
                case UserSortBy.Surname:
                    available = available.OrderBy(r => r.Surname).ToList();
                    break;
                case UserSortBy.Affiliation:
                    available = available.OrderBy(r => r.Affiliation).ToList();
                    break;
            }
            ram.Article = article;
            ram.AssignedReviewers = assigned;
            ram.AvailableReviewers = available;

            return View("ReviewersAssign",ram);
        }

        [HttpPost]
        public ActionResult ReviewersAssignDelete(ReviewersAssignViewModel ram, string reviewerId)
        {
            //TODO:delete reviewer from model
            return RedirectToAction("ArticlesReviewers", "Admin");
        }

        [HttpPost]
        public ActionResult ReviewersAssign(ReviewersAssignViewModel ram)
        {
            //TODO:check number of reviewers and save to db
            return RedirectToAction("ArticlesReviewers", "Admin");
        }

        [HttpPost]
        public ActionResult ReviewersSearchAssign(ReviewersAssignViewModel ram, int articleId)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            List<ApplicationUser> reviewers = new List<ApplicationUser>();
            ReviewersAssignPartialModel rapm = new ReviewersAssignPartialModel();
            Article article= dbContext.Articles.Find(articleId);

            var role = dbContext.Roles.SingleOrDefault(m => m.Name == "Admin");
            reviewers = dbContext.Users.Where(m => m.Roles.All(r => r.RoleId != role.Id) && !m.Affiliation.ToLower().Equals(article.MainAuthor.Affiliation.ToLower())).ToList();

            if (!String.IsNullOrEmpty(ram.SearchPhrase))
            {
                reviewers = reviewers.Where(r => r.Name.ToLower().Contains(ram.SearchPhrase.ToLower()) || r.Surname.ToLower().Contains(ram.SearchPhrase.ToLower()) || r.Affiliation.ToLower().Contains(ram.SearchPhrase.ToLower())).ToList();
            }

            switch (ram.SortBy)
            {
                case UserSortBy.Name:
                    reviewers = reviewers.OrderBy(r => r.Name).ToList();
                    break;
                case UserSortBy.Surname:
                    reviewers = reviewers.OrderBy(r => r.Surname).ToList();
                    break;
                case UserSortBy.Affiliation:
                    reviewers = reviewers.OrderBy(r => r.Affiliation).ToList();
                    break;
            }
            rapm.AvailableReviewers = reviewers.Skip((ram.CurrentPage - 1) * ram.ResultsForPage).Take(ram.ResultsForPage).ToList();
            rapm.MaxPages = (int)Math.Ceiling((double)reviewers.Count / (double)ram.ResultsForPage);

            return PartialView("_ReviewersAssign", rapm);
        }

        private async Task prepareAndSendConfirmationEmail(ApplicationUser user)
        {
            userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            MailMessage message;
            string token = await userManager.GenerateEmailConfirmationTokenAsync(user.Id);
            token = HttpUtility.UrlEncode(token);
            string link = "https://localhost:44330\\Account\\ConfirmEmail?userId=" + user.Id + "&code=" + token;
            var fromAddress = new MailAddress(mailLogin, "ArticleReviewerSystem");
            var toAddress = new MailAddress(user.Email, "New User");
            const string subject = "Email confirmation - Article Review System";
            string body = "Hello,</br> Your registration request on Article Review System has been resolved positively</br></br>"
                + "To activate your account please visit link:</br>" + "<a>" + link + "</a>";
            message = new MailMessage(fromAddress, toAddress);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(mailLogin, mailPassword),
                EnableSsl = true
            };
            client.Send(message);
        }
    }
}