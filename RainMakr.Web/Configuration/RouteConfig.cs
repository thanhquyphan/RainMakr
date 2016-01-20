// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RouteConfig.cs" company="BringDream">
//   BringDream 2016
// </copyright>
// <summary>
//   Defines the RouteConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RainMakr.Web.Configuration
{
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// The route config.
    /// </summary>
    public static class RouteConfig
    {
        /// <summary>
        /// The register routes.
        /// </summary>
        /// <param name="routes">
        /// The routes.
        /// </param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}
