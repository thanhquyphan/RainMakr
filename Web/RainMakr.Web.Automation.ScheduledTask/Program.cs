using System;
using System.Web.Http;
using Nito.AsyncEx;
using RainMakr.Web.Configuration;
using RainMakr.Web.Interfaces.Manager.Command;
using RainMakr.Web.Interfaces.Manager.Query;
using RainMakr.Web.Models;

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
            var deviceEventCommandManager =
                (IDeviceEventCommandManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IDeviceEventCommandManager));

            var schedules = await scheduleQueryManager.GetElapsedSchedulesAsync();
            foreach (var schedule in schedules)
            {
                if (schedule.CheckForRain)
                {
                    try
                    {
                        var raining = await weatherQueryManager.CurrentWeatherIsRainingForDeviceAsync(schedule.DeviceId);

                        if (raining)
                        {
                            continue;
                        }
                    }
                    catch
                    {
                    }
                }
                await deviceCommandManager.StartDeviceByScheduleAsync(schedule.Id, schedule.DeviceId);
                await
                    deviceEventCommandManager.AddEventAsync(new DeviceEvent
                    {
                        DateCreated = DateTime.Now,
                        DeviceId = schedule.DeviceId,
                        ScheduleId = schedule.Id,
                        Status = DeviceStatus.On
                    });
            }
        }
    }
}
