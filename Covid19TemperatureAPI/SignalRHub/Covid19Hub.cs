using Covid19TemperatureAPI.Entities.Data;
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

                notificationMessageToSend = JsonConvert.SerializeObject(abnormalTemperatureNotifiationMsgWithEmployeeInfo);
            }

            await Clients.All.ReceiveMessage(notificationMessageToSend);
        }

        public async Task SendToCaller(string message)
        {
            await Clients.Caller.ReceiveMessage(message);
        }

        public async Task SendToOthers(string message)
        {
            await Clients.Others.ReceiveMessage(message);
        }

        public async Task SendToGroup(string groupName, string message)
        {
            await Clients.Group(groupName).ReceiveMessage(message);
        }

        public async Task AddUserToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.ReceiveMessage($"Current user added to {groupName} group");
            await Clients.Others.ReceiveMessage($"User {Context.ConnectionId} added to {groupName} group");
        }
        public async Task RemoveUserFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.ReceiveMessage($"Current user removed from {groupName} group");
            await Clients.Others.ReceiveMessage($"User {Context.ConnectionId} removed from {groupName} group");
        }
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "HubUsers");
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "HubUsers");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
