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

    public class DeviceQueryManager : IDeviceQueryManager
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

        public Task<List<Device>> GetDevicesAsync(string personId)
        {
            return this.deviceQueryStore.GetDevicesAsync(personId);
        }
    }
}
