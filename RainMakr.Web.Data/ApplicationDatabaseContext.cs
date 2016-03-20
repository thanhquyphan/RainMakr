namespace RainMakr.Web.Data
{
    using System.Data.Entity;

    using Microsoft.AspNet.Identity.EntityFramework;

    using RainMakr.Web.Models;

    public class ApplicationDatabaseContext : IdentityDbContext<Person>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDatabaseContext" /> class.
        /// </summary>
        public ApplicationDatabaseContext()
            : base("DefaultConnection", false)
        {
        }

        public DbSet<Device> Devices { get; set; }

        public DbSet<Schedule> Schedules { get; set; }

        public DbSet<DeviceEvent> DeviceEvents { get; set; }

        /// <inheritdoc />
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // This needs to go before the other rules!

            var personConfig = modelBuilder.Entity<Person>().ToTable("Person");
            personConfig.HasMany(x => x.Devices);

            modelBuilder.Entity<IdentityRole>().ToTable("Role");
            modelBuilder.Entity<IdentityUserRole>().ToTable("PersonRole");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("PersonClaim");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("PersonLogin");
           
            var deviceConfig = modelBuilder.Entity<Device>().HasKey(x => x.Id).ToTable("Device");
            deviceConfig.HasMany(x => x.Schedules);
            modelBuilder.Entity<Schedule>().HasKey(x => x.Id).ToTable("Schedule");
            modelBuilder.Entity<DeviceEvent>().ToTable("DeviceEvent");
        }
    }
}