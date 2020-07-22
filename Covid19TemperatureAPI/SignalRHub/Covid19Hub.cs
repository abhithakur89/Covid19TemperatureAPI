using Covid19TemperatureAPI.Entities.Data;
using Covid19TemperatureAPI.Mobile;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19TemperatureAPI.SignalRHub
{
    public class Covid19Hub : Hub<ICovidHubClient>
    {
        private IConfiguration Configuration;
        private readonly ILogger<Covid19Hub> _logger;
        private ApplicationDbContext DbContext { get; set; }

        public Covid19Hub(IConfiguration configuration, ApplicationDbContext applicationDbContext, ILogger<Covid19Hub> logger)
        {
            Configuration = configuration;
            DbContext = applicationDbContext;
            _logger = logger;
        }

        public async Task BroadcastMessage(string message)
        {
            var notifiationMsgRecieved = new { Notification = string.Empty };
            notifiationMsgRecieved = JsonConvert.DeserializeAnonymousType(message, notifiationMsgRecieved);

            string notificationMessageToSend = message;

            // Prepare push message
            if (notifiationMsgRecieved.Notification == "Abnormal Temperature")
            {
                var abnormalTemperatureNotifiationMsg = new {
                    Notification = string.Empty,
                    Temperature = string.Empty,
                    DeviceId = string.Empty,
                    EmployeeId = string.Empty,
                    PersonName = string.Empty,
                    PersonId = string.Empty,
                    Desc = string.Empty,
                    Timestamp = string.Empty
                };

                abnormalTemperatureNotifiationMsg = JsonConvert.DeserializeAnonymousType(message, abnormalTemperatureNotifiationMsg);

                var details = DbContext.Employees
                    .Where(x => x.UID == abnormalTemperatureNotifiationMsg.PersonId.Trim())
                    ?.Select(x => new { x.EmployeeId })
                    ?.FirstOrDefault();

                var abnormalTemperatureNotifiationMsgWithEmployeeInfo = new
                {
                    abnormalTemperatureNotifiationMsg.Notification,
                    abnormalTemperatureNotifiationMsg.Temperature,
                    abnormalTemperatureNotifiationMsg.DeviceId,
                    EmployeeId = details?.EmployeeId ?? string.Empty,
                    abnormalTemperatureNotifiationMsg.PersonName,
                    abnormalTemperatureNotifiationMsg.PersonId,
                    abnormalTemperatureNotifiationMsg.Desc,
                    abnormalTemperatureNotifiationMsg.Timestamp
                };


                // Final Push Message
                notificationMessageToSend = JsonConvert.SerializeObject(abnormalTemperatureNotifiationMsgWithEmployeeInfo);

                // Push all clients
                await Clients.All.ReceiveMessage(notificationMessageToSend);

                // Prepare SMS
                string smsBody = DbContext.Configurations
                    .Where(x => x.ConfigKey == "Abnormal_Temperature_SMS_Body")
                    .Select(x => x.ConfigValue)
                    .FirstOrDefault();

                // Get mobile numbers for this site 
                var v = from a in DbContext.Devices
                                    join b in DbContext.Gates on a.GateId equals b.GateId
                                    join c in DbContext.AlertMobileNumbers on b.Floor.Building.SiteId equals c.SiteId
                                    where a.DeviceId == abnormalTemperatureNotifiationMsg.DeviceId
                                    select new
                                    {
                                        c.MobileNumber, b.Floor.Building.BuildingName, b.GateNumber
                                    };

                // Send SMS 
                foreach (var v1 in v)
                {
                    var body = smsBody + $"\nName - {abnormalTemperatureNotifiationMsg.PersonName}\n" +
                        $"Temperature - {abnormalTemperatureNotifiationMsg.Temperature}\n" +
                        $"Building - {v1.BuildingName}\n" +
                        $"Gate - {v1.GateNumber}\n" +
                        $"Time - {Convert.ToDateTime(abnormalTemperatureNotifiationMsg.Timestamp).ToString("MMMM dd hh:mm tt")}";

                    SMSSender.SendSMS(ApiKey: Configuration["NexmmoApiKey"],
                        ApiSecret: Configuration["NexmoApiSecret"],
                        from: Configuration["SMSSender"],
                        to: v1.MobileNumber,
                        msg: body);
                }
            }
        }
    }
}
