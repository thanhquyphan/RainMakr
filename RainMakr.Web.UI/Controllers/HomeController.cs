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
    using RainMakr.Web.Interfaces.Manager.Query;
    using RainMakr.Web.UI.ViewModels;

    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IDeviceQueryManager deviceQueryManager;

        public HomeController(IDeviceQueryManager deviceQueryManager)
        {
            this.deviceQueryManager = deviceQueryManager;
        }

        public async Task<ActionResult> Index()
        {
            var model = new HomeViewModel{Devices = await this.deviceQueryManager.GetDevicesAsync(this.User.Identity.GetUserId())};

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}