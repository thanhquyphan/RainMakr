using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainMakr.Web.UI.ViewModels
{
    using RainMakr.Web.Models;

    public class HomeViewModel
    {
        public IEnumerable<Device> Devices { get; set; }
    }
}