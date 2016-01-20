using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.Interfaces.Manager.Command
{
    using RainMakr.Web.Models;

    public interface IScheduleCommandManager : ICommandManager
    {
        Task AddScheduleAsync(string personId, Schedule schedule);

        Task RemoveScheduleAsync(string personId, string deviceId, string id);
    }
}
