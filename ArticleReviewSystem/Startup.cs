using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ArticleReviewSystem.Startup))]
namespace ArticleReviewSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
