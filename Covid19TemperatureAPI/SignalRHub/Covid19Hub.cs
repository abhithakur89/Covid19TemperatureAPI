﻿using Covid19TemperatureAPI.Entities.Data;
using Covid19TemperatureAPI.Helper;
using Covid19TemperatureAPI.Mobile;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
            var notifiationMsgRecieved = new
            {
                Notification = string.Empty,
                Value = string.Empty,
                DeviceId = string.Empty,
                EmployeeId = string.Empty,
                PersonName = string.Empty,
                PersonId = string.Empty,
                Desc = string.Empty,
                Timestamp = string.Empty
            };
            notifiationMsgRecieved = JsonConvert.DeserializeAnonymousType(message, notifiationMsgRecieved);

            //string notificationMessageToSend = message;

            // Prepare push message
            if (notifiationMsgRecieved.Notification == Configuration["TemperaturePolicyName"])
            {
                // Get Config setting for temperature threshold
                decimal thresholdTemperature = ConfigReader.GetTemperatureThreshold(DbContext, Configuration);

                // Parse received teperature
                bool res = decimal.TryParse(notifiationMsgRecieved.Value, out decimal receivedTeperature);

                if (res)
                {
                    // Check threshold
                    if(receivedTeperature>= thresholdTemperature)
                    {
                        // Notify everyone
                        await Clients.All.ReceiveMessage(Configuration["HighTemperatureAlertNotificationString"]);

                        #region SMS
                        // Notify SMS
                        var sendAlertSMSConfig = ConfigReader.GetSendAlertForTemperatureSMSEnabled(DbContext, Configuration);
                        if(sendAlertSMSConfig)
                        {
                            // Prepare SMS
                            string smsBody = ConfigReader.GetTemperatureAlertHeader(DbContext, Configuration);

                            // Get SMS sender
                            string smsSender = ConfigReader.GetSMSSender(DbContext, Configuration);

                            // Get mobile numbers for this site 
                            var v = from a in DbContext.Devices
                                    join b in DbContext.Gates on a.GateId equals b.GateId
                                    join c in DbContext.AlertMobileNumbers on b.Floor.Building.SiteId equals c.SiteId
                                    where a.DeviceId == notifiationMsgRecieved.DeviceId
                                    select new
                                    {
                                        c.MobileNumber,
                                        b.Floor.Building.BuildingName,
                                        b.GateNumber
                                    };

                            // Send SMS 
                            foreach (var v1 in v)
                            {
                                var body = smsBody + $"\nName - {notifiationMsgRecieved.PersonName}\n" +
                                    $"Temperature - {notifiationMsgRecieved.Value}\n" +
                                    $"Building - {v1.BuildingName}\n" +
                                    $"Gate - {v1.GateNumber}\n" +
                                    $"Time - {Convert.ToDateTime(notifiationMsgRecieved.Timestamp).ToString("MMMM dd hh:mm tt")}";

                                try
                                {
                                    SMSSender.SendSMS(ApiKey: Configuration["NexmmoApiKey"],
                                        ApiSecret: Configuration["NexmoApiSecret"],
                                        from: smsSender,
                                        to: v1.MobileNumber,
                                        msg: body);
                                }
                                catch(Exception e)
                                {
                                    _logger.LogError($"Error sending SMS to {v1.MobileNumber}. Error: {e.Message}");
                                }
                            }
                        }
                        #endregion

                        #region Email
                        // Notify email
                        var sendAlertEmailConfig = ConfigReader.GetSendAlertForTemperatureEmailEnabled(DbContext, Configuration);
                        if (sendAlertEmailConfig)
                        {
                            var fromAddress = new MailAddress(Configuration["EmailSenderId"], 
                                ConfigReader.GetEmailSenderName(DbContext, Configuration));

                            string fromPassword = Configuration["EmailSenderPassword"];

                            // Prepare Email
                            string emailSubject = ConfigReader.GetTemperatureAlertEmailSubject(DbContext, Configuration);

                            // Get Email addresses for this site 
                            var v = from a in DbContext.Devices
                                    join b in DbContext.Gates on a.GateId equals b.GateId
                                    join c in DbContext.EmailAddresses on b.Floor.Building.SiteId equals c.SiteId
                                    where a.DeviceId == notifiationMsgRecieved.DeviceId
                                    select new
                                    {
                                        c.EmailId,
                                        c.Name,
                                        b.Floor.Building.BuildingName,
                                        b.GateNumber
                                    };

                            // Send Email 
                            foreach (var v1 in v)
                            {
                                var body = $"\nName - {notifiationMsgRecieved.PersonName}\n" +
                                    $"Temperature - {notifiationMsgRecieved.Value}\n" +
                                    $"Building - {v1.BuildingName}\n" +
                                    $"Gate - {v1.GateNumber}\n" +
                                    $"Time - {Convert.ToDateTime(notifiationMsgRecieved.Timestamp).ToString("MMMM dd hh:mm tt")}";

                                var smtp = new SmtpClient
                                {
                                    Host = Configuration["SMTPHost"],
                                    Port = Convert.ToInt32(Configuration["SMTPPort"]),
                                    EnableSsl = true,
                                    DeliveryMethod = SmtpDeliveryMethod.Network,
                                    UseDefaultCredentials = true,
                                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                                };

                                var toAddress = new MailAddress(v1.EmailId, v1.Name);

                                using (var mailMessage = new MailMessage(fromAddress, toAddress)
                                {
                                    Subject = emailSubject,
                                    Body = body
                                })
                                {
                                    try
                                    {
                                        smtp.Send(mailMessage);
                                    }
                                    catch(Exception e)
                                    {
                                        _logger.LogError($"Error while sending email: {v1.EmailId}. Error: {e.Message}");
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }

                await Clients.All.ReceiveMessage(Configuration["TemperatureEventNotificationString"]);
            }

            if (notifiationMsgRecieved.Notification == Configuration["NoMaskPolicyName"])
            {
                await Clients.All.ReceiveMessage(Configuration["NoMaskAlertNotificationString"]);

                #region SMS
                // Notify SMS
                var sendAlertSMSConfig = ConfigReader.GetSendAlertForMaskSMSEnabled(DbContext, Configuration);
                if (sendAlertSMSConfig)
                {
                    // Prepare SMS
                    string smsBody = ConfigReader.GetMaskAlertHeader(DbContext, Configuration);

                    // Get SMS sender
                    string smsSender = ConfigReader.GetSMSSender(DbContext, Configuration);

                    // Get mobile numbers for this site 
                    var v = from a in DbContext.Devices
                            join b in DbContext.Gates on a.GateId equals b.GateId
                            join c in DbContext.AlertMobileNumbers on b.Floor.Building.SiteId equals c.SiteId
                            where a.DeviceId == notifiationMsgRecieved.DeviceId
                            select new
                            {
                                c.MobileNumber,
                                b.Floor.Building.BuildingName,
                                b.GateNumber
                            };

                    // Send SMS 
                    foreach (var v1 in v)
                    {
                        var body = smsBody + $"\nName - {notifiationMsgRecieved.PersonName}\n" +
                            $"Building - {v1.BuildingName}\n" +
                            $"Gate - {v1.GateNumber}\n" +
                            $"Time - {Convert.ToDateTime(notifiationMsgRecieved.Timestamp).ToString("MMMM dd hh:mm tt")}";

                        try
                        {
                            SMSSender.SendSMS(ApiKey: Configuration["NexmmoApiKey"],
                                ApiSecret: Configuration["NexmoApiSecret"],
                                from: smsSender,
                                to: v1.MobileNumber,
                                msg: body);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError($"Error sending SMS to {v1.MobileNumber}. Error: {e.Message}");
                        }
                    }
                }
                #endregion

                #region Email
                // Notify email
                var sendAlertEmailConfig = ConfigReader.GetSendAlertForMaskEmailEnabled(DbContext, Configuration);
                if (sendAlertEmailConfig)
                {
                    var fromAddress = new MailAddress(Configuration["EmailSenderId"],
                        ConfigReader.GetEmailSenderName(DbContext, Configuration));

                    string fromPassword = Configuration["EmailSenderPassword"];

                    // Prepare Email
                    string emailSubject = ConfigReader.GetMaskAlertEmailSubject(DbContext, Configuration);

                    // Get Email addresses for this site 
                    var v = from a in DbContext.Devices
                            join b in DbContext.Gates on a.GateId equals b.GateId
                            join c in DbContext.EmailAddresses on b.Floor.Building.SiteId equals c.SiteId
                            where a.DeviceId == notifiationMsgRecieved.DeviceId
                            select new
                            {
                                c.EmailId,
                                c.Name,
                                b.Floor.Building.BuildingName,
                                b.GateNumber
                            };

                    // Send Email 
                    foreach (var v1 in v)
                    {
                        var body = $"\nName - {notifiationMsgRecieved.PersonName}\n" +
                            $"Building - {v1.BuildingName}\n" +
                            $"Gate - {v1.GateNumber}\n" +
                            $"Time - {Convert.ToDateTime(notifiationMsgRecieved.Timestamp).ToString("MMMM dd hh:mm tt")}";

                        var smtp = new SmtpClient
                        {
                            Host = Configuration["SMTPHost"],
                            Port = Convert.ToInt32(Configuration["SMTPPort"]),
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = true,
                            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                        };

                        var toAddress = new MailAddress(v1.EmailId, v1.Name);

                        using (var mailMessage = new MailMessage(fromAddress, toAddress)
                        {
                            Subject = emailSubject,
                            Body = body
                        })
                        {
                            try
                            {
                                smtp.Send(mailMessage);
                            }
                            catch (Exception e)
                            {
                                _logger.LogError($"Error while sending email: {v1.EmailId}. Error: {e.Message}");
                            }
                        }
                    }
                }
            }
            #endregion
        }
    }
}
