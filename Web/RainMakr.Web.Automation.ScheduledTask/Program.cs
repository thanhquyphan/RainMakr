using System.Web.Http;
using Nito.AsyncEx;
using RainMakr.Web.Configuration;
using RainMakr.Web.Interfaces.Manager.Command;
using RainMakr.Web.Interfaces.Manager.Query;

namespace RainMakr.Web.Automation.ScheduledTask
{
    class Program
    {
        static void Main(string[] args)
        {
            UnityConfig.SetupUnityContainer();

            AsyncContext.Run(() => MainAsync());
        }

        private static async void MainAsync()
        {
            var scheduleQueryManager =
                (IScheduleQueryManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IScheduleQueryManager));
            var deviceCommandManager =
                (IDeviceCommandManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IDeviceCommandManager));
            var weatherQueryManager = 
                (IWeatherQueryManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IWeatherQueryManager));

            var schedules = await scheduleQueryManager.GetElapsedSchedulesAsync();
            foreach (var schedule in schedules)
            {
                if (schedule.CheckForRain)
                {
                    var raining = await weatherQueryManager.CurrentWeatherIsRainingForDeviceAsync(schedule.DeviceId);

                    if (raining)
                    {
                        continue;
                    }
                }
                await deviceCommandManager.StartDeviceByScheduleAsync(schedule.Id, schedule.DeviceId);
            }
        }
    }
}
