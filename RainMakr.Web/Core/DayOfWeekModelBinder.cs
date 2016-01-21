namespace RainMakr.Web.Core
{
    using System;
    using System.Web.Mvc;

    using DayOfWeek = RainMakr.Web.Models.DayOfWeek;

    public class DayOfWeekModelBinder : DefaultModelBinder
    {
        /// <summary>
        /// The bind model.
        /// </summary>
        /// <param name="controllerContext">
        /// The controller context.
        /// </param>
        /// <param name="bindingContext">
        /// The binding context.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value == null)
            {
                return DayOfWeek.Undefined;
            }

            var rawValues = value.RawValue as string[];

            if (rawValues == null)
            {
                return DayOfWeek.Undefined;
            }

            DayOfWeek result;

            if (Enum.TryParse(string.Join(",", rawValues), out result))
            {
                return result;
            }

            throw new FormatException(string.Format("{0} is not in the correct format.", string.Join(",", rawValues)));
        }
    }
}