// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnityConfig.cs" company="BringDream">
//   BringDream 2016
// </copyright>
// <summary>
//   The unity config.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Practices.Unity;

namespace RainMakr.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Mvc;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    using Owin;

    using RainMakr.Web.BusinessLogics;
    using RainMakr.Web.Data;
    using RainMakr.Web.Interfaces.Manager;
    using RainMakr.Web.Interfaces.Manager.Command;
    using RainMakr.Web.Interfaces.Store;
    using RainMakr.Web.Models;
    using RainMakr.Web.Unity;

    using RestSharp;

    /// <summary>
    /// The unity config.
    /// </summary>
    public class UnityConfig
    {
        /// <summary>
        /// The container.
        /// </summary>
        private static UnityContainer container;

        /// <summary>
        /// Gets the container.
        /// </summary>
        public static UnityContainer Container
        {
            get
            {
                return container;
            }
        }

        public static void SetupUnityContainer()
        {
            container = new UnityContainer();

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
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }

        /// <summary>
        /// Registers services and configures unity.
        /// </summary>
        /// <param name="app">
        /// The app builder.
        /// </param>
        public static void SetupUnityContainer(IAppBuilder app)
        {
            container = new UnityContainer();

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
            var userManager = AuthConfig.ConfigureUserManager(userStore, app);
            container.RegisterInstance(typeof(UserManager<Person>), userManager);
            container.RegisterInstance(typeof(IUserStore<Person>), userStore);
            ControllerBuilder.Current.SetControllerFactory(new UnityControllerFactory(container));
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}
