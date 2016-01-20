using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.BusinessLogics.Command
{
    using RainMakr.Web.Interfaces.Manager.Command;
    using RainMakr.Web.Interfaces.Manager.Query;
    using RainMakr.Web.Interfaces.Store.Command;
    using RainMakr.Web.Models;

    public class ScheduleCommandManager : IScheduleCommandManager
    {
        private readonly IScheduleCommandStore scheduleCommandStore;

        private readonly IScheduleQueryManager scheduleQueryManager;

        private readonly IDeviceQueryManager deviceQueryManager;

        public ScheduleCommandManager(IScheduleCommandStore scheduleCommandStore, IScheduleQueryManager scheduleQueryManager, IDeviceQueryManager deviceQueryManager)
        {
            this.scheduleCommandStore = scheduleCommandStore;
            this.scheduleQueryManager = scheduleQueryManager;
            this.deviceQueryManager = deviceQueryManager;
        }

        public async Task AddScheduleAsync(string personId, Schedule schedule)
        {
            var device = await this.deviceQueryManager.GetDeviceAsync(personId, schedule.DeviceId);

            var schedules = await this.scheduleQueryManager.GetSchedulesAsync(personId, device.Id);
            if (schedules.Any(x => x.StartDate == schedule.StartDate && x.Days == schedule.Days && x.Offset == schedule.Offset))
            {
                throw new ArgumentException("This device already has a schedule with the same trigger.");
            }

            schedule.DeviceId = device.Id;
            await this.scheduleCommandStore.AddScheduleAsync(schedule);
        }

        public async Task RemoveScheduleAsync(string personId, string deviceId, string id)
        {
            var schedule = await this.scheduleQueryManager.GetScheduleAsync(personId, deviceId, id);
            await this.scheduleCommandStore.RemoveScheduleAsync(schedule);
        }
    }
}
