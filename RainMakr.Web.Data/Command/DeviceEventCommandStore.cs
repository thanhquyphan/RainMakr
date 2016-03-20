using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RainMakr.Web.Interfaces.Store.Command;
using RainMakr.Web.Models;

namespace RainMakr.Web.Data.Command
{
    public class DeviceEventCommandStore : IDeviceEventCommandStore
    {
        public Task AddEventAsync(DeviceEvent deviceEvent)
        {
            using (var context = new ApplicationDatabaseContext())
            {
                context.DeviceEvents.Add(deviceEvent);
                return context.SaveChangesAsync();
            }
        }
    }
}
