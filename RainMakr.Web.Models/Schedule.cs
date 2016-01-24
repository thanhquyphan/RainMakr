using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    using RainMakr.Web.Models.Core.Extensions;

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

        public DayOfWeek? Days { get; set; }

        public string DeviceId { get; set; }

        public virtual Device Device { get; set; }

        public override string ToString()
        {
            if (this.Recurrence)
            {

                string daysText;
                if (this.Days == DayOfWeek.Everyday)
                {
                    daysText = "Everyday";
                }
                else
                {
                    var test = this.Days.Value.GetFlags();
                    var values = this.Days.Value.GetFlags().Where(i => !i.Equals(DayOfWeek.Undefined)).Select(x => x.ToString()).ToArray();
                    daysText = string.Join(", ", values, 0, values.Length - 1);
                    daysText = daysText + " and " + values.Last();
                }
                return string.Format("{0} at {1:00}:{2:00} for {3} minutes{4}", daysText, this.Offset.Hours, this.Offset.Minutes, this.Duration, this.CheckForRain ? " with checking for rain." : string.Empty);
            }
            else
            {
                return string.Format(
                    "{0} at {1:00}:{2:00} for {3} minutes{4}",
                    this.StartDate.Value.ToShortDateString(),
                    this.Offset.Hours,
                    this.Offset.Minutes,
                    this.Duration, 
                    this.CheckForRain ? " with checking for rain." : string.Empty);
            }
        }
    }
}
