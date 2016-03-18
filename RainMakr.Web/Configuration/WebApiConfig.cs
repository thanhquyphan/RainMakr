using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Practices.Unity;
using RainMakr.Web.BusinessLogics;
using RainMakr.Web.Data;
using RainMakr.Web.Interfaces.Manager;
using RainMakr.Web.Interfaces.Manager.Command;
using RainMakr.Web.Interfaces.Store;
using RainMakr.Web.Models;
using RainMakr.Web.Unity;

namespace RainMakr.Web.Configuration
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
           var  container = new UnityContainer();

            var applicationDbContext = new ApplicationDatabaseContext();
            var userStore = new UserStore<Person>(applicationDbContext);
            container.RegisterInstance(
                typeof(ApplicationDatabaseContext), applicationDbContext);

            container.RegisterTypes(
                AllClasses.FromLoadedAssemblies()
                    .Where(
                        type =>
                        (typeof(IManager).IsAssignableFrom(type) || typeof(IStore).IsAssignableFrom(type))
                        && !type.IsAbstract && !type.IsInterface),
                WithMappings.FromMatchingInterface,
                WithName.Default,
                WithLifetime.PerResolve);
            container.RegisterType<IEmailManager, EmailManager>();
            var userManager = AuthConfig.ConfigureUserManager(userStore, null);
            container.RegisterInstance(typeof(UserManager<Person>), userManager);
            container.RegisterInstance(typeof(IUserStore<Person>), userStore);
            // Web API configuration and services
            config.DependencyResolver = new UnityDependencyResolver(container);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
