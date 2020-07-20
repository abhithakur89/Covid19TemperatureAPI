using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Covid19TemperatureAPI.Helper
{
    public static class EnumExtention
    {
        public static string DisplayName(this Enum enumValue)
        {
            var enumType = enumValue.GetType();

            var enumString = enumType.GetField(enumValue.ToString()).Name;

            return (enumType.GetField(enumValue.ToString()).GetCustomAttribute<DisplayAttribute>()?.Name) ?? enumString;
        }
    }
}
