using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.Interfaces.Manager.Command
{
    using RainMakr.Web.Models;

    public interface IDeviceCommandManager:ICommandManager
    {
        Task AddDeviceAsync(string personId, Device device);

        Task RemoveDeviceAsync(string personId, string id);

        Task StartDeviceAsync(string personId, string id);

        Task StartDeviceByScheduleAsync(string scheduleId, string id);

        Task StopDeviceAsync(string personId, string id);

        Task UpdateIpAddressAsync(string macAddress, string ip);
    }
}
