using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.Data.Query
{
    using System.Data.Entity;

    using RainMakr.Web.Interfaces.Store.Query;
    using RainMakr.Web.Models;

    public class ScheduleQueryStore : IScheduleQueryStore
    {
        /// <summary>
        /// The database context.
        /// </summary>
        private readonly ApplicationDatabaseContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleQueryStore"/> class.
        /// </summary>
        /// <param name="databaseContext">
        /// The database context.
        /// </param>
        public ScheduleQueryStore(ApplicationDatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        
        public Task<Schedule> GetScheduleAsync(string id)
        {
            return this.databaseContext.Schedules.FindAsync(id);
        }

        public Task<List<Schedule>> GetSchedulesAsync(string deviceId)
        {
            return this.databaseContext.Schedules.Where(x => x.DeviceId == deviceId).ToListAsync();
        }
    }
}
