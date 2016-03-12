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

    using RestSharp;

    public class DeviceQueryManager : WebServiceBase, IDeviceQueryManager
    {
        private readonly IDeviceQueryStore deviceQueryStore;

        public DeviceQueryManager(IDeviceQueryStore deviceQueryStore)
        {
            this.deviceQueryStore = deviceQueryStore;
        }

        public async Task<Device> GetDeviceAsync(string personId, string id)
        {
            var device = await this.deviceQueryStore.GetDeviceAsync(id);
            if (device == null || device.PersonId != personId)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, "No device found."); 
            }

            return device;
        }

        public Task<Device> GetDeviceByIdAsync(string id)
        {
            return this.deviceQueryStore.GetDeviceAsync(id);
        }

        public Task<Device> GetDeviceByMacAddressAsync(string macAddress)
        {
            return this.deviceQueryStore.GetDeviceByMacAddressAsync(macAddress);
        }

        public Task<List<Device>> GetDevicesAsync(string personId)
        {
            return this.deviceQueryStore.GetDevicesAsync(personId);
        }

        public async Task<DeviceStatus> GetDeviceStatusAsync(string personId, string id)
        {
            var device = await this.GetDeviceAsync(personId, id);

            if (string.IsNullOrWhiteSpace(device.IpAddress))
            {
                return DeviceStatus.Undefined;
            }

            Action<IRestResponse<DeviceStatus>> processResponse = x =>
                { };

            var result = Execute(device.IpAddress, "Status", Method.POST, null, processResponse);

            return result;
        }
    }
}
