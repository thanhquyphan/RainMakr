using System;
using System.Collections.Generic;
using System.Globalization;
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

        private readonly IScheduleQueryManager scheduleQueryManager;

        public DeviceCommandManager(IDeviceCommandStore deviceCommandStore, IDeviceQueryManager deviceQueryManager, IScheduleQueryManager scheduleQueryManager)
        {
            this.deviceCommandStore = deviceCommandStore;
            this.deviceQueryManager = deviceQueryManager;
            this.scheduleQueryManager = scheduleQueryManager;
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
            if (device == null)
            {
                throw new ArgumentException("No device found.");
            }

            Execute(device.IpAddress, "Start", Method.POST, null, null);
        }

        public async Task StartDeviceByScheduleAsync(string scheduleId, string id)
        {
            var device = await this.deviceQueryManager.GetDeviceByIdAsync(id);

            if (device == null)
            {
                throw new ArgumentException("No device found with the given id.");
            }

            var schedule = await this.scheduleQueryManager.GetScheduleAsync(device.PersonId, id, scheduleId);
            if (schedule == null)
            {
                throw new ArgumentException("No schedule found with the given id.");
            }

            Action<IRestRequest> configureRequest = x =>
            {
                x.AddUrlSegment("duration", schedule.Duration.ToString(CultureInfo.InvariantCulture));
            };
            Execute(device.IpAddress, "Start/{duration}", Method.POST, configureRequest, null);
        }

        public async Task StopDeviceAsync(string personId, string id)
        {
            var device = await this.deviceQueryManager.GetDeviceAsync(personId, id);
            if (device == null)
            {
                throw new ArgumentException("No device found.");
            }

            Execute(device.IpAddress, "Stop", Method.POST, null, null);
        }

        public async Task UpdateIpAddressAsync(string macAddress, string ip)
        {
            var device = await this.deviceQueryManager.GetDeviceByMacAddressAsync(macAddress);

            if(device == null)
            {
                throw new ArgumentException("No device found with the given MAC address.");
            }

            await this.deviceCommandStore.UpdateIpAddressAsync(device.Id, ip);
        }
    }
}
