namespace RainMakr.Web.Data
{
    using System.Data.Entity;

    using Microsoft.AspNet.Identity.EntityFramework;

    using RainMakr.Web.Models;

    public class ApplicationDatabaseContext : IdentityDbContext<Person>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ApplicationDatabaseContext" /> class.
        /// </summary>
        public ApplicationDatabaseContext()
            : base("DefaultConnection", false)
        {

        }

        /// <inheritdoc />
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // This needs to go before the other rules!

            modelBuilder.Entity<Person>().ToTable("Person");
            modelBuilder.Entity<IdentityRole>().ToTable("Role");
            modelBuilder.Entity<IdentityUserRole>().ToTable("PersonRole");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("PersonClaim");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("PersonLogin");
        }
    }
}