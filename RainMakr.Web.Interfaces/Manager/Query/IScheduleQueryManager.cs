using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.Interfaces.Manager.Query
{
    using RainMakr.Web.Models;

    public interface IScheduleQueryManager : IQueryManager
    {
        Task<Schedule> GetScheduleAsync(string personId, string deviceId, string id);

        Task<List<Schedule>> GetSchedulesAsync(string personId, string deviceId);

        Task<List<Schedule>> GetElapsedSchedulesAsync();
    }
}
