using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19TemperatureAPI.SenseTime
{
    public interface ISensetime
    {
        string Login(string username, string password);
    }
}
