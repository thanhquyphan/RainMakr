namespace RainMakr.Web.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    using Microsoft.AspNet.Identity.EntityFramework;

    internal sealed class Configuration : DbMigrationsConfiguration<RainMakr.Web.Data.ApplicationDatabaseContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "RainMakr.Web.Data.ApplicationDatabaseContext";
        }

        protected override void Seed(RainMakr.Web.Data.ApplicationDatabaseContext context)
        {
            context.Roles.AddOrUpdate(
                r => r.Name,
                new IdentityRole("Administrator"),
                new IdentityRole("User"));

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
