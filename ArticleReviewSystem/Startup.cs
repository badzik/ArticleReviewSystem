using ArticleReviewSystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using System;

[assembly: OwinStartupAttribute(typeof(ArticleReviewSystem.Startup))]
namespace ArticleReviewSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesandUsers();
        }
        private void CreateRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var provider = new DpapiDataProtectionProvider("ArticleReviewSystem");
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);
                var user = new ApplicationUser
                {
                    UserName = "SuperAdmin",
                    Email = "dssj@dssj.com",
                    Name = "Super",
                    Surname = "Admin",
                    RegistrationDate = DateTime.Now
                };

            string password = "superokoń";
                var superAdminAccount = UserManager.Create(user, password);
                if (superAdminAccount.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");
                    UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
                        provider.Create("EmailConfirmation"));
                    var token = UserManager.GenerateEmailConfirmationToken(user.Id);
                    var result = UserManager.ConfirmEmail(user.Id, token);
                }
            }
   
            if (!roleManager.RoleExists("User"))
            {
                var role = new IdentityRole();
                role.Name = "User";
                roleManager.Create(role);
            }
}
    }
}
