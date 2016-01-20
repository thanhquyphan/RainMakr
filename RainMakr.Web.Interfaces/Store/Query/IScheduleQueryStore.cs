using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.Interfaces.Store.Query
{
    using RainMakr.Web.Models;

    public interface IScheduleQueryStore : IQueryStore
    {
        Task<Schedule> GetScheduleAsync(string id);

        Task<List<Schedule>> GetSchedulesAsync(string deviceId);
    }
}
