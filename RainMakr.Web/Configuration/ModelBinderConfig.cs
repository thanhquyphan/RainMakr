
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.Configuration
{
    using System.Web.Mvc;

    using RainMakr.Web.Core;
    using RainMakr.Web.Models;

    public class ModelBinderConfig
    {
        /// <summary>
        /// The register model binder.
        /// </summary>
        public static void RegisterModelBinders()
        {
            ModelBinders.Binders.Add(typeof(DayOfWeek), new DayOfWeekModelBinder());
        }
    }
}
