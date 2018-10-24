using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyLink.Startup))]
namespace MyLink
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
