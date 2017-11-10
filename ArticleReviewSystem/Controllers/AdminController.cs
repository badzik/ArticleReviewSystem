using ArticleReviewSystem.Models;
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

        private async Task prepareAndSendConfirmationEmail(ApplicationUser user)
        {
            userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            MailMessage message;
            string token = await userManager.GenerateEmailConfirmationTokenAsync(user.Id);
            token = HttpUtility.UrlEncode(token);
            string link = "localhost:2686\\Account\\ConfirmEmail?userId=" + user.Id + "&code=" + token;
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