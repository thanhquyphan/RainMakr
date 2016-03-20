using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RainMakr.Web.Models;

namespace RainMakr.Web.Interfaces.Store.Command
{
    public interface IDeviceEventCommandStore : ICommandStore
    {
        Task AddEventAsync(DeviceEvent deviceEvent);
    }
}
