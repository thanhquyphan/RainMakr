using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.Interfaces.Manager.Query
{
    using RainMakr.Web.Models;

    public interface IDeviceQueryManager : IQueryManager
    {
        Task<Device> GetDeviceAsync(string personId, string id);

        Task<Device> GetDeviceByIdAsync(string id);

        Task<List<Device>> GetDevicesAsync(string personId);

        Task<DeviceStatus> GetDeviceStatusAsync(string personId, string id);

        Task<Device> GetDeviceByMacAddressAsync(string macAddress);
    }
}
