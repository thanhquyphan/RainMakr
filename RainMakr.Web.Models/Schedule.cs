using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Schedule
    {
        public Schedule()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }

        public bool CheckForRain { get; set; }

        public bool Recurrence { get; set; }

        public DateTime? StartDate { get; set; }

        [Required]
        public TimeSpan Offset { get; set; }

        [Required]
        public int Duration { get; set; }

        public ScheduleDay? Days { get; set; }

        public string DeviceId { get; set; }

        public virtual Device Device { get; set; }
    }
}
