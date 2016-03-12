using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.Interfaces.Manager.Query
{
    public interface IWeatherQueryManager : IQueryManager
    {
        Task<bool> CurrentWeatherIsRainingForDeviceAsync(string deviceId);
    }
}
