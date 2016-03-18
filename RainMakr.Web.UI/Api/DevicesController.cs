using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using RainMakr.Web.Interfaces.Manager.Command;

namespace RainMakr.Web.UI.Api
{
    public class DevicesController : ApiController
    {
        private readonly IDeviceCommandManager deviceCommandManager;

        public DevicesController(IDeviceCommandManager deviceCommandManager)
        {
            this.deviceCommandManager = deviceCommandManager;
        }
        
        [HttpPost]
        [ActionName("updateIpAddress")]
        public async Task<HttpResponseMessage> UpdateIpAddressAsync([FromBody] string macAddress, string ip)
        {
            // POST: api/devices/{ip}/updateIpAddress
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            await this.deviceCommandManager.UpdateIpAddressAsync(macAddress, ip);
            
            return this.Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [ActionName("start")]
        public async Task<HttpResponseMessage> StartDeviceAsync([FromBody] string id, string scheduleId)
        {
            // POST: api/devices/{scheduleId}/start
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            await this.deviceCommandManager.StartDeviceByScheduleAsync(scheduleId, id);

            return this.Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}