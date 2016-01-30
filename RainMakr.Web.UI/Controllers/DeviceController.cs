using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RainMakr.Web.UI.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNet.Identity;

    using RainMakr.Web.Core;
    using RainMakr.Web.Interfaces.Manager.Command;
    using RainMakr.Web.Interfaces.Manager.Query;
    using RainMakr.Web.Models;
    using RainMakr.Web.UI.ViewModels;
    
    [Authorize]
    [RoutePrefix("Device")]
    public class DeviceController : BaseController
    {
        private readonly IDeviceQueryManager deviceQueryManager;

        private readonly IDeviceCommandManager deviceCommandManager;

        private readonly IScheduleQueryManager scheduleQueryManager;

        public DeviceController(IDeviceQueryManager deviceQueryManager, IDeviceCommandManager deviceCommandManager, IScheduleQueryManager scheduleQueryManager)
        {
            this.deviceQueryManager = deviceQueryManager;
            this.deviceCommandManager = deviceCommandManager;
            this.scheduleQueryManager = scheduleQueryManager;
        }

        [Route("{id}")]
        public async Task<ActionResult> Index(string id)
        {
            var personId = this.User.Identity.GetUserId();
            var device = await this.deviceQueryManager.GetDeviceAsync(personId, id);
            device.Schedules = await
                this.scheduleQueryManager.GetSchedulesAsync(personId, id);
            var status = await this.deviceQueryManager.GetDeviceStatusAsync(personId, id);

            var model = new DeviceViewModel{Device = device, Status = status};
            return View("Index", model);
        }

        [Route("Add")]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Route("Add")]
        public async Task<ActionResult> AddDevice()
        {
            var device = new Device();

            if (!this.TryUpdateModel(device))
            {
                return this.View("Add");
            }

            await this.deviceCommandManager.AddDeviceAsync(this.HttpContext.User.Identity.GetUserId(), device);
            return RedirectToAction("Index", new { Id = device.Id});
        }

        [HttpPost]
        [Route("Start")]
        public async Task<ActionResult> Start(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                ModelState.AddModelError("id", "Invalid id provided.");
                return await this.Index(id);
            }

            await this.deviceCommandManager.StartDeviceAsync(this.HttpContext.User.Identity.GetUserId(), id);
            return await this.Index(id);
        }

        [HttpPost]
        [Route("Stop")]
        public async Task<ActionResult> Stop(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                ModelState.AddModelError("id", "Invalid id provided.");
                return await this.Index(id);
            }

            await this.deviceCommandManager.StopDeviceAsync(this.HttpContext.User.Identity.GetUserId(), id);
            return await this.Index(id);
        }

        [HttpPost]
        public async Task<ActionResult> Remove(string id)
        {
            await this.deviceCommandManager.RemoveDeviceAsync(this.HttpContext.User.Identity.GetUserId(), id);
            return RedirectToAction("Index", "Home");
        } 
    }
}