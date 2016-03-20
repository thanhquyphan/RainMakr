using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
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
        public async Task<HttpResponseMessage> UpdateIpAddressAsync(string macAddress)
        {
            // POST: api/devices/{ip}/updateIpAddress
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            var ip = HttpContext.Current.Request.UserHostAddress;
            await this.deviceCommandManager.UpdateIpAddressAsync(macAddress, ip);
            
            return this.Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}