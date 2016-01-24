namespace RainMakr.Web.UI.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using RainMakr.Web.Models;

    using DayOfWeek = RainMakr.Web.Models.DayOfWeek;

    public class ScheduleSubmitModel : IValidatableObject
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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.Recurrence)
            {
                if (this.Days.GetValueOrDefault(DayOfWeek.Undefined) == DayOfWeek.Undefined)
                {
                    yield return new ValidationResult("You must select days for the schedule.", new []{ "Days" });
                }
            }
            else
            {
                if (this.StartDate.GetValueOrDefault(DateTime.MinValue) == DateTime.MinValue)
                {
                    yield return new ValidationResult("You must select a date for the schedule.", new[] { "StartDate" });
                }

                if (this.StartDate.HasValue && this.StartDate.Value < DateTime.Today)
                {
                    yield return new ValidationResult("Date must not be in the past.", new[] { "StartDate" });
                }

                if (this.StartDate.HasValue && new DateTime(this.StartDate.Value.Year, this.StartDate.Value.Month, this.StartDate.Value.Day, this.Hours, this.Minutes, 0) <= DateTime.Now)
                {
                    yield return new ValidationResult("Schedule date and time must not be in the past.", new[] { "StartDate", "Hours", "Minutes" });
                }
            }
        }
    }
}