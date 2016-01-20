using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.Interfaces.Store.Command
{
    using RainMakr.Web.Models;

    public interface IScheduleCommandStore : ICommandStore
    {
        Task AddScheduleAsync(Schedule schedule);

        Task RemoveScheduleAsync(Schedule schedule);
    }
}
