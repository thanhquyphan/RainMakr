using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RainMakr.Web.UI.Startup))]
namespace RainMakr.Web.UI
{
    using System.Web.Mvc;

    using RainMakr.Web.Configuration;
    using RainMakr.Web.Unity;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            UnityConfig.SetupUnityContainer(app);
            var unityFilterAttributeFilterProvider = new UnityFilterAttributeFilterProvider(UnityConfig.Container);
            FilterProviders.Providers.Add(unityFilterAttributeFilterProvider);
            AuthConfig.ConfigureAuth(app);
        }
    }
}
