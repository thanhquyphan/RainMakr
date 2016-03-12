using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using RainMakr.Web.Interfaces.Manager.Query;
using RainMakr.Web.Models;
using RestSharp;

namespace RainMakr.Web.BusinessLogics.Query
{
    public class WeatherQueryManager : WebServiceBase, IWeatherQueryManager
    {
        private readonly IDeviceQueryManager deviceQueryManager;

        private readonly string apiKey = WebConfigurationManager.AppSettings["WeatherApiKey"];

        private readonly string serviceLocation = WebConfigurationManager.AppSettings["WeatherServiceLocation"];

        public WeatherQueryManager(IDeviceQueryManager deviceQueryManager)
        {
            this.deviceQueryManager = deviceQueryManager;
        }

        public async Task<bool> CurrentWeatherIsRainingForDeviceAsync(string deviceId)
        {
            var device = await this.deviceQueryManager.GetDeviceByIdAsync(deviceId);
            if (device == null)
            {
                throw new ArgumentException("No device found.");
            }
            if (string.IsNullOrWhiteSpace(device.City) || string.IsNullOrWhiteSpace(device.CountryCode))
            {
                return false;
            }

            Action<IRestRequest> configureRequest = x =>
            {
                x.AddQueryParameter("q", string.Format("{0},{1}", device.City, device.CountryCode));
                x.AddQueryParameter("appid", this.apiKey);
            };

            Action<IRestResponse<Dictionary<string, object>>> processResponse = x =>
            { };

            var result = Execute(this.serviceLocation, "weather", Method.GET, configureRequest, processResponse);
            var rain = ((Dictionary<string, object>)((JsonArray)result["weather"]).First())["main"].ToString();
            return rain.Equals("Rain", StringComparison.OrdinalIgnoreCase);
        }
    }
}
