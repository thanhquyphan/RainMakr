using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RainMakr.Web.Models;

namespace RainMakr.Web.UI.ViewModels
{
    public class DeviceViewModel
    {
        public Device Device { get; set; }

        public DeviceStatus Status { get; set; }
    }
}