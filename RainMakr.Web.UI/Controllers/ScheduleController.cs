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
    [RoutePrefix("Schedule")]
    public class ScheduleController : BaseController
    {
        private readonly IScheduleQueryManager scheduleQueryManager;

        private readonly IScheduleCommandManager scheduleCommandManager;

        public ScheduleController(IScheduleQueryManager scheduleQueryManager, IScheduleCommandManager scheduleCommandManager)
        {
            this.scheduleQueryManager = scheduleQueryManager;
            this.scheduleCommandManager = scheduleCommandManager;
        }

        [Route("{id}/Device/{deviceId}")]
        public async Task<ActionResult> Index(string deviceId, string id)
        {
            var model =
                await
                this.scheduleQueryManager.GetScheduleAsync(this.HttpContext.User.Identity.GetUserId(), deviceId, id);

            return View(model);
        }

        [HttpPost]
        [Route("Add")]
        public async Task<ActionResult> AddSchedule()
        {
            var schedule = new Schedule();

            if (!this.TryUpdateModel(schedule))
            {
                return this.View("Add");
            }

            await this.scheduleCommandManager.AddScheduleAsync(this.HttpContext.User.Identity.GetUserId(), schedule);
            return RedirectToAction("Index", new { Id = schedule.Id, DeviceId = schedule.DeviceId });
        }

        [Route("Add/{deviceId}")]
        public async Task<ActionResult> Add(string deviceId)
        {
            var model = new Schedule { DeviceId = deviceId };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Remove(string deviceId, string id)
        {
            await this.scheduleCommandManager.RemoveScheduleAsync(this.HttpContext.User.Identity.GetUserId(), deviceId, id);
            return RedirectToAction("Index", "Device", new { Id = deviceId });
        } 
    }
}