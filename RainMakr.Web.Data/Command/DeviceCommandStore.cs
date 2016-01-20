using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.Data.Command
{
    using RainMakr.Web.Interfaces.Store.Command;
    using RainMakr.Web.Models;

    public class DeviceCommandStore : IDeviceCommandStore
    {
        /// <summary>
        /// The database context.
        /// </summary>
        private readonly ApplicationDatabaseContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceCommandStore"/> class.
        /// </summary>
        /// <param name="databaseContext">
        /// The database context.
        /// </param>
        public DeviceCommandStore(ApplicationDatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public Task AddDeviceAsync(Device device)
        {
            this.databaseContext.Devices.Add(device);
            return this.databaseContext.SaveChangesAsync();
        }

        public Task RemoveDeviceAsync(Device device)
        {
            this.databaseContext.Devices.Attach(device);
            this.databaseContext.Devices.Remove(device);
            return this.databaseContext.SaveChangesAsync();
        }
    }
}
