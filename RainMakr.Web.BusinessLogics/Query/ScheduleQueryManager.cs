using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.BusinessLogics.Query
{
    using System.Net;
    using System.Web;

    using RainMakr.Web.Interfaces.Manager.Query;
    using RainMakr.Web.Interfaces.Store.Query;
    using RainMakr.Web.Models;

    public class ScheduleQueryManager : IScheduleQueryManager
    {
        private readonly IScheduleQueryStore scheduleQueryStore;

        private readonly IDeviceQueryManager deviceQueryManager;

        public ScheduleQueryManager(IScheduleQueryStore scheduleQueryStore, IDeviceQueryManager deviceQueryManager)
        {
            this.scheduleQueryStore = scheduleQueryStore;
            this.deviceQueryManager = deviceQueryManager;
        }

        public async Task<Schedule> GetScheduleAsync(string personId, string deviceId, string id)
        {
            var device = await this.deviceQueryManager.GetDeviceAsync(personId, deviceId);
            var schedule = await this.scheduleQueryStore.GetScheduleAsync(id);

            if (schedule.DeviceId == device.Id)
            {
                return schedule;
            }

            throw new HttpException((int)HttpStatusCode.NotFound, "No schedule found."); 
        }

        public async Task<List<Schedule>> GetSchedulesAsync(string personId, string deviceId)
        {
            var device = await this.deviceQueryManager.GetDeviceAsync(personId, deviceId);

            return await this.scheduleQueryStore.GetSchedulesAsync(device.Id);
        }

        public Task<List<Schedule>> GetElapsedSchedulesAsync()
        {
            return this.scheduleQueryStore.GetElapsedSchedulesAsync();
        }
    }
}
