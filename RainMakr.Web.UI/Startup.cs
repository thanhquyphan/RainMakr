using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RainMakr.Web.UI.Startup))]
namespace RainMakr.Web.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
