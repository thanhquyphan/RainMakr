namespace RainMakr.Web.Interfaces.Store.Query
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RainMakr.Web.Models;

    public interface IDeviceQueryStore : IQueryStore
    {
        Task<List<Device>> GetDevicesAsync(string personId);

        Task<Device> GetDeviceAsync(string id);

        Task<Device> GetDeviceByMacAddressAsync(string macAddress);
    }
}