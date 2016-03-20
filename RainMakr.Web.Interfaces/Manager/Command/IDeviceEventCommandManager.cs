using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RainMakr.Web.Models;

namespace RainMakr.Web.Interfaces.Manager.Command
{
    public interface IDeviceEventCommandManager : ICommandManager
    {
        Task AddEventAsync(DeviceEvent deviceEvent);
    }
}
