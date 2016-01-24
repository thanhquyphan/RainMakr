namespace RainMakr.Web.Models.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumExtensions
    {
        public static IEnumerable<Enum> GetFlags(this Enum input)
        {
            return Enum.GetValues(input.GetType()).Cast<Enum>().Where(value => input.HasFlag(value));
        }
    }
}
