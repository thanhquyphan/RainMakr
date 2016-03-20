using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.Models
{
    public class DeviceEvent
    {
        public DeviceEvent()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }

        public string DeviceId { get; set; }

        public DateTime DateCreated { get; set; }

        public DeviceStatus Status { get; set; }

        public string ScheduleId { get; set; }
    }
}
