using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.Data.Query
{
    using System.Data.Entity;

    using RainMakr.Web.Interfaces.Store.Query;
    using RainMakr.Web.Models;

    public class DeviceQueryStore : IDeviceQueryStore
    {
        /// <summary>
        /// The database context.
        /// </summary>
        private readonly ApplicationDatabaseContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceQueryStore"/> class.
        /// </summary>
        /// <param name="databaseContext">
        /// The database context.
        /// </param>
        public DeviceQueryStore(ApplicationDatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public Task<List<Device>> GetDevicesAsync(string personId)
        {
            return this.databaseContext.Devices.Where(x => x.PersonId == personId).ToListAsync();
        }

        public Task<Device> GetDeviceAsync(string id)
        {
            return this.databaseContext.Devices.FindAsync(id);
        }

        public Task<Device> GetDeviceByMacAddressAsync(string macAddress)
        {
            return this.databaseContext.Devices.FirstOrDefaultAsync(x => x.MacAddress == macAddress);
        }
    }
}
