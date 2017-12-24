using ArticleReviewSystem.Enums;
using ArticleReviewSystem.Models;
using ArticleReviewSystem.PartialModels;
using ArticleReviewSystem.ViewModels;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

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
            rcVM.UnconfirmedUsers = dbContext.Users.Where(m => !m.EmailConfirmed).OrderBy(a => a.Surname).Take(10).ToList();
            rcVM.CurrentPage = 1;
            rcVM.ResultsForPage = 10;
            rcVM.SortBy = Enums.UserSortBy.Surname;
            rcVM.NumberOfPages = (int)Math.Ceiling((double)dbContext.Users.Where(m => !m.EmailConfirmed).Count() / rcVM.ResultsForPage);
            return View(rcVM);
        }

        [HttpPost]
        public ActionResult RegisterConfirmation(RegisterConfirmationViewModel rcvm)
        {
            RegisterConfirmationPartialModel rcpm = new RegisterConfirmationPartialModel();
            ApplicationDbContext dbContext = new ApplicationDbContext();
            List<ApplicationUser> unconfirmedUsers = new List<ApplicationUser>();
            unconfirmedUsers = dbContext.Users.Where(m => !m.EmailConfirmed).ToList();
            if (!String.IsNullOrEmpty(rcvm.SearchPhrase))
            {
                unconfirmedUsers = unconfirmedUsers.Where(a => a.Name.ToLower().Contains(rcvm.SearchPhrase.ToLower()) || a.Surname.ToLower().Contains(rcvm.SearchPhrase.ToLower()) || a.Affiliation.ToLower().Contains(rcvm.SearchPhrase.ToLower())).ToList();
            }
            switch (rcvm.SortBy)
            {
                case UserSortBy.Name:
                    unconfirmedUsers = unconfirmedUsers.OrderBy(r => r.Name).ToList();
                    break;
                case UserSortBy.Surname:
                    unconfirmedUsers = unconfirmedUsers.OrderBy(r => r.Surname).ToList();
                    break;
                case UserSortBy.Affiliation:
                    unconfirmedUsers = unconfirmedUsers.OrderBy(r => r.Affiliation).ToList();
                    break;
            }
            rcpm.MaxPages = (int)Math.Ceiling((double)unconfirmedUsers.Count / (double)rcvm.ResultsForPage);
            rcpm.UnconfirmedUsers = unconfirmedUsers.Skip((rcvm.CurrentPage - 1) * rcvm.ResultsForPage).Take(rcvm.ResultsForPage).ToList();
            return PartialView("_RegisterConfirmation", rcpm);
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
            arvm.Articles = dbContext.Articles.Where(a=>a.Status!=ArticleStatus.PositivelyReviewed && a.Status!=ArticleStatus.NegativelyReviewed && a.Status!=ArticleStatus.MinorChangesWithoutNewReview).OrderBy(a => a.Reviews.Count).Take(10).ToList();
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
            articles = dbContext.Articles.Where(a => a.Status != ArticleStatus.PositivelyReviewed && a.Status != ArticleStatus.NegativelyReviewed && a.Status != ArticleStatus.MinorChangesWithoutNewReview).ToList();
            if (!String.IsNullOrEmpty(arvm.SearchPhrase))
            {
                articles = dbContext.Articles.Where(a => a.Title.ToLower().Contains(arvm.SearchPhrase.ToLower()) || a.MainAuthor.Name.ToLower().Contains(arvm.SearchPhrase.ToLower()) || a.MainAuthor.Surname.ToLower().Contains(arvm.SearchPhrase.ToLower())).ToList();
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
            ReviewersAssignViewModel ravm = getActualReviewrsAssignModel(articleId);
            return View(ravm);
        }

        [HttpPost]
        public ActionResult ReviewersAssignAdd(ReviewersAssignViewModel ram, string reviewerId, int articleId)
        {
            ModelState.Clear();
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var article = dbContext.Articles.Find(ram.Article.ArticleId);
            List<SimpleReviewer> assigned = new List<SimpleReviewer>();
            List<ApplicationUser> available = new List<ApplicationUser>();
            ApplicationUser reviewer;
            ReviewStatus status;
            //add previous assigned reviewers
            if (ram.AssignedReviewers != null)
            {
                foreach (SimpleReviewer a in ram.AssignedReviewers)
                {
                    reviewer = dbContext.Users.Find(a.Id);
                    if (reviewer.AssignedReviews.Any(r => r.Reviewer.Id == reviewer.Id))
                    {
                        status = reviewer.AssignedReviews.First(r => r.Reviewer.Id == reviewer.Id).Status;
                    }
                    else
                    {
                        status = ReviewStatus.NotReviewedYet;
                    }
                    assigned.Add(new SimpleReviewer() {
                        Affiliation = reviewer.Affiliation,
                        Id = reviewer.Id,
                        Name = reviewer.Name,
                        Surname = reviewer.Surname,
                        ReviewStatus = status
                    });
                }
            }

            //add new reviewer
            reviewer = dbContext.Users.Find(reviewerId);
            assigned.Add(new SimpleReviewer() { Affiliation = reviewer.Affiliation, Id = reviewer.Id, Name = reviewer.Name, Surname = reviewer.Surname });


            var role = dbContext.Roles.SingleOrDefault(m => m.Name == "Admin");
            available = dbContext.Users.Where(m => m.Roles.All(r => r.RoleId != role.Id) && !m.Affiliation.ToLower().Equals(article.MainAuthor.Affiliation.ToLower()) && m.EmailConfirmed).ToList();
            foreach (SimpleReviewer s in assigned)
            {
                var toRemove = available.FirstOrDefault(a => a.Id == s.Id);
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

            return View("ReviewersAssign", ram);
        }

        [HttpPost]
        public ActionResult ReviewersAssignDelete(ReviewersAssignViewModel ram, string reviewerId, int articleId)
        {
            ModelState.Clear();
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var article = dbContext.Articles.Find(ram.Article.ArticleId);
            List<SimpleReviewer> assigned = new List<SimpleReviewer>();
            List<ApplicationUser> available = new List<ApplicationUser>();
            ApplicationUser tempUser;

            //prepare assigned list
            foreach (SimpleReviewer a in ram.AssignedReviewers)
            {
                if (a.Id != reviewerId)
                {
                    tempUser = dbContext.Users.Find(a.Id);
                    assigned.Add(new SimpleReviewer()
                    {
                        Affiliation = tempUser.Affiliation,
                        Id = tempUser.Id,
                        Name = tempUser.Name,
                        Surname = tempUser.Surname,
                        ReviewStatus = tempUser.AssignedReviews.First(r => r.Reviewer.Id == tempUser.Id).Status
                    });
                }
            }

            var role = dbContext.Roles.SingleOrDefault(m => m.Name == "Admin");
            available = dbContext.Users.Where(m => m.Roles.All(r => r.RoleId != role.Id) && !m.Affiliation.ToLower().Equals(article.MainAuthor.Affiliation.ToLower()) && m.EmailConfirmed).ToList();
            foreach (SimpleReviewer s in assigned)
            {
                var toRemove = available.FirstOrDefault(a => a.Id == s.Id);
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

            return View("ReviewersAssign", ram);
        }

        [HttpPost]
        public ActionResult ReviewersAssign(ReviewersAssignViewModel ram)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var article = dbContext.Articles.Find(ram.Article.ArticleId);
            List<ApplicationUser> assignedReviewers = new List<ApplicationUser>();
            if(ram.AssignedReviewers!=null)
            {
                foreach (SimpleReviewer user in ram.AssignedReviewers)
                {
                    assignedReviewers.Add(dbContext.Users.Find(user.Id));
                }
            }
            switch (article.Status)
            {
                case ArticleStatus.WaitingToAssignReviewers:
                    {
                        if (assignedReviewers.Count != 2)
                        {
                            ram = getActualReviewrsAssignModel(article.ArticleId);
                            ModelState.AddModelError("AssignedReviewers", "You must assign two reviewers for the article.");
                            return View(ram);
                        }
                        foreach (ApplicationUser reviewer in assignedReviewers)
                        {
                            article.Reviews.Add(new Review()
                            {
                                Reviewer = reviewer,
                                Status = ReviewStatus.NotReviewedYet,
                                RelatedArticle = article
                            });
                        }
                        article.Status = ArticleStatus.ReviewersAssigned;
                        dbContext.SaveChangesAsync();
                        break;
                    }
                case ArticleStatus.ReviewersAssigned:
                    {
                        if (assignedReviewers.Count != 2)
                        {
                            ram = getActualReviewrsAssignModel(article.ArticleId);
                            ModelState.AddModelError("AssignedReviewers", "You must assign two reviewers for the article.");
                            return View(ram);
                        }

                        //Delete previous reviewers
                        List<Review> tempReviewsList = article.Reviews.ToList();
                        foreach (Review review in tempReviewsList)
                        {
                            dbContext.Reviews.Remove(review);
                        }
                        //Add new reviewers
                        foreach (ApplicationUser reviewer in assignedReviewers)
                        {
                            article.Reviews.Add(new Review()
                            {
                                Reviewer = reviewer,
                                Status = ReviewStatus.NotReviewedYet,
                                RelatedArticle = article
                            });
                        }
                        break;
                    }
                case ArticleStatus.NewReviewerNeeded:
                    {
                        if (assignedReviewers.Count != 3)
                        {
                            ram = getActualReviewrsAssignModel(article.ArticleId);
                            ModelState.AddModelError("AssignedReviewers", "You must only one additional reviewer");
                            return View(ram);
                        }

                        foreach (ApplicationUser reviewer in assignedReviewers)
                        {
                            if (article.Reviews.Any(r => r.Reviewer.Id != reviewer.Id))
                            {
                                article.Reviews.Add(new Review()
                                {
                                    Reviewer = reviewer,
                                    Status = ReviewStatus.NotReviewedYet,
                                    RelatedArticle = article
                                });
                            }
                        }
                        article.Status = ArticleStatus.ChangesWithNewReview;
                        dbContext.SaveChangesAsync();
                        break;
                    }
            }
            return RedirectToAction("ReviewersAssign", new { articleId = ram.Article.ArticleId });
        }

        [HttpPost]
        public ActionResult ReviewersSearchAssign(ReviewersAssignViewModel ram, int articleId, string[] assignedIds)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            List<ApplicationUser> reviewers = new List<ApplicationUser>();
            ReviewersAssignPartialModel rapm = new ReviewersAssignPartialModel();
            Article article = dbContext.Articles.Find(articleId);

            rapm.ArticleId = articleId;
            var role = dbContext.Roles.SingleOrDefault(m => m.Name == "Admin");
            reviewers = dbContext.Users.Where(m => m.Roles.All(r => r.RoleId != role.Id) && !m.Affiliation.ToLower().Equals(article.MainAuthor.Affiliation.ToLower()) && m.EmailConfirmed).ToList();

            if (assignedIds != null)
            {
                foreach (string id in assignedIds)
                {
                    var toRemove = reviewers.FirstOrDefault(a => a.Id == id);
                    reviewers.Remove(toRemove);
                }
            }


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
            string link = "https://localhost:44330\\Account\\ConfirmEmail?id=" + user.Id + "&token=" + token;
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

        private ReviewersAssignViewModel getActualReviewrsAssignModel(int articleId)
        {
            ReviewersAssignViewModel ravm = new ReviewersAssignViewModel();

            ApplicationDbContext dbContext = new ApplicationDbContext();

            ravm.Article = dbContext.Articles.Find(articleId);
            ravm.AssignedReviewers = new List<SimpleReviewer>();

            foreach (Review r in ravm.Article.Reviews)
            {
                ravm.AssignedReviewers.Add(new SimpleReviewer() {
                    Affiliation = r.Reviewer.Affiliation,
                    Id = r.Reviewer.Id,
                    Name = r.Reviewer.Name,
                    Surname = r.Reviewer.Surname,
                    ReviewStatus = r.Status
                });
            }

            var role = dbContext.Roles.SingleOrDefault(m => m.Name == "Admin");
            ravm.AvailableReviewers = dbContext.Users.Where(m => m.Roles.All(r => r.RoleId != role.Id) && !m.Affiliation.ToLower().Equals(ravm.Article.MainAuthor.Affiliation.ToLower()) && m.EmailConfirmed).ToList();

            foreach (SimpleReviewer s in ravm.AssignedReviewers)
            {
                var toRemove = ravm.AvailableReviewers.FirstOrDefault(a => a.Id == s.Id);
                ravm.AvailableReviewers.Remove(toRemove);
            }

            ravm.AvailableReviewers = ravm.AvailableReviewers.OrderBy(r => r.Surname).ToList();
            ravm.SortBy = UserSortBy.Name;
            ravm.CurrentPage = 1;
            ravm.NumberOfPages = (int)Math.Ceiling((double)ravm.AvailableReviewers.Count / (double)10);
            ravm.ResultsForPage = 10;
            return ravm;
        }
    }
}