using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Covid19TemperatureAPI.Helper
{
    public static class ExtentionMethods
    {
        public static string DisplayName(this Enum enumValue)
        {
            var enumType = enumValue.GetType();

            var enumString = enumType.GetField(enumValue.ToString()).Name;

            return (enumType.GetField(enumValue.ToString()).GetCustomAttribute<DisplayAttribute>()?.Name) ?? enumString;
        }

        public async static Task<string> ConvertImageUrlToBase64(this string url)
        {
            var credentials = new NetworkCredential();
            using (var handler = new HttpClientHandler { Credentials = credentials })
            using (var client = new HttpClient(handler))
            {
                var bytes = await client.GetByteArrayAsync(url);
                return "data:image/jpeg;base64," + Convert.ToBase64String(bytes);
            }
        }
    }
}
