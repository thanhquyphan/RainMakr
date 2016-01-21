namespace RainMakr.Web.UI.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using RainMakr.Web.Models;

    using DayOfWeek = RainMakr.Web.Models.DayOfWeek;

    public class ScheduleSubmitModel
    {
        public bool CheckForRain { get; set; }

        public bool Recurrence { get; set; }

        public DateTime? StartDate { get; set; }

        [Required]
        [Range(0, 23)]
        public int Hours { get; set; }

        [Required]
        [Range(0, 59)]
        public int Minutes { get; set; }

        [Required]
        [Range(1, 60)]
        public int Duration { get; set; }

        public DayOfWeek? Days { get; set; }

        [Required]
        public string DeviceId { get; set; }
    }
}