using ArticleReviewSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.ViewModels
{
    public class RegisterConfirmationViewModel
    {
        public List<ApplicationUser> UnconfirmedUsers { get; set; }
    }
}