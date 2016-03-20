using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RainMakr.Web.Interfaces.Manager.Command;
using RainMakr.Web.Interfaces.Store.Command;
using RainMakr.Web.Models;

namespace RainMakr.Web.BusinessLogics.Command
{
    public class DeviceEventCommandManager : IDeviceEventCommandManager
    {
        private readonly IDeviceEventCommandStore deviceEventCommandStore;

        public DeviceEventCommandManager(IDeviceEventCommandStore deviceEventCommandStore)
        {
            this.deviceEventCommandStore = deviceEventCommandStore;
        }

        public Task AddEventAsync(DeviceEvent deviceEvent)
        {
            return this.deviceEventCommandStore.AddEventAsync(deviceEvent);
        }
    }
}
