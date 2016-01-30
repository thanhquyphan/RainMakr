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

    using RestSharp;

    public class DeviceCommandManager : WebServiceBase, IDeviceCommandManager
    {
        private readonly IDeviceCommandStore deviceCommandStore;

        private readonly IDeviceQueryManager deviceQueryManager;

        public DeviceCommandManager(RestClient client, IDeviceCommandStore deviceCommandStore, IDeviceQueryManager deviceQueryManager)
            : base(client)
        {
            this.deviceCommandStore = deviceCommandStore;
            this.deviceQueryManager = deviceQueryManager;
        }

        public async Task AddDeviceAsync(string personId, Device device)
        {
            var devices = await this.deviceQueryManager.GetDevicesAsync(personId);

            if (devices.Any(x => x.MacAddress == device.MacAddress || x.Name == device.Name))
            {
                throw new ArgumentException("You already have a device with the same name or MAC address.");
            }

            device.PersonId = personId;

            await this.deviceCommandStore.AddDeviceAsync(device);
        }

        public async Task RemoveDeviceAsync(string personId, string id)
        {
            var device = await this.deviceQueryManager.GetDeviceAsync(personId, id);
            
            await this.deviceCommandStore.RemoveDeviceAsync(device);
        }

        public async Task StartDeviceAsync(string personId, string id)
        {
            var device = await this.deviceQueryManager.GetDeviceAsync(personId, id);

            Execute("Start", Method.POST, null, null);
        }

        public async Task StopDeviceAsync(string personId, string id)
        {
            var device = await this.deviceQueryManager.GetDeviceAsync(personId, id);
            Execute("Stop", Method.POST, null, null);
        }
    }
}
