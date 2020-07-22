using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19TemperatureAPI.SignalRHub
{
    public interface ICovidHubClient
    {
        Task ReceiveMessage(string message);
    }
}
