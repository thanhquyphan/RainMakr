using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.Data.Command
{
    using RainMakr.Web.Interfaces.Store.Command;
    using RainMakr.Web.Models;

    public class ScheduleCommandStore : IScheduleCommandStore
    {
        /// <summary>
        /// The database context.
        /// </summary>
        private readonly ApplicationDatabaseContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleCommandStore"/> class.
        /// </summary>
        /// <param name="databaseContext">
        /// The database context.
        /// </param>
        public ScheduleCommandStore(ApplicationDatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public Task AddScheduleAsync(Schedule schedule)
        {
            this.databaseContext.Schedules.Add(schedule);
            return this.databaseContext.SaveChangesAsync();
        }

        public Task RemoveScheduleAsync(Schedule schedule)
        {
            this.databaseContext.Schedules.Attach(schedule);
            this.databaseContext.Schedules.Remove(schedule);
            return this.databaseContext.SaveChangesAsync();
        }
    }
}
