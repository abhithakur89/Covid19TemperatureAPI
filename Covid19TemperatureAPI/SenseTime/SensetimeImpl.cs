using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19TemperatureAPI.SenseTime
{
    public class SensetimeImpl : ISensetime
    {
        public string Login(string username, string password, string apiType)
        {
            return $"{username}:{password} ({apiType})";
        }
    }
}
