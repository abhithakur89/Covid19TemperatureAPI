using Covid19TemperatureAPI.Entities.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19TemperatureAPI.Helper
{
    public static class ConfigReader
    {
        public const string VisitorUID = "0";
        public const int NoMaskValue = 2;
        public const string TemperatureThresholdConfigSettingName = "TemperatureThreshold";

        public static decimal GetTemperatureThreshold(ApplicationDbContext DbContext, IConfiguration Configuration)
        {
            bool res = decimal.TryParse(DbContext.Configurations
                   .Where(x => x.ConfigKey == TemperatureThresholdConfigSettingName)
                   .Select(x => x.ConfigValue).FirstOrDefault(), out decimal thresholdTemperature);

            if (!res)
                decimal.TryParse(Configuration["DefaultTemperatureThreshold"], out thresholdTemperature);

            return thresholdTemperature;
        }

        public static string GetTemperatureAlertHeader(ApplicationDbContext DbContext, IConfiguration Configuration)
        {
            string res = DbContext.Configurations
                   .Where(x => x.ConfigKey == "TemperatureAlertHeader")
                   .Select(x => x.ConfigValue).FirstOrDefault();

            if (string.IsNullOrEmpty(res))
                res = Configuration["DefaultTemperatureAlertHeader"];

            return res;
        }

        public static string GetMaskAlertHeader(ApplicationDbContext DbContext, IConfiguration Configuration)
        {
            string res = DbContext.Configurations
                   .Where(x => x.ConfigKey == "MaskAlertHeader")
                   .Select(x => x.ConfigValue).FirstOrDefault();

            if (string.IsNullOrEmpty(res))
                res = Configuration["DefaultMaskAlertHeader"];

            return res;
        }


        public static string GetTemperatureAlertEmailSubject(ApplicationDbContext DbContext, IConfiguration Configuration)
        {
            string res = DbContext.Configurations
                   .Where(x => x.ConfigKey == "TemperatureAlertEmailSubject")
                   .Select(x => x.ConfigValue).FirstOrDefault();

            if (string.IsNullOrEmpty(res))
                res = Configuration["DefaultTemperatureAlertHeader"];

            return res;
        }

        public static string GetMaskAlertEmailSubject(ApplicationDbContext DbContext, IConfiguration Configuration)
        {
            string res = DbContext.Configurations
                   .Where(x => x.ConfigKey == "MaskAlertEmailSubject")
                   .Select(x => x.ConfigValue).FirstOrDefault();

            if (string.IsNullOrEmpty(res))
                res = Configuration["DefaultMaskAlertHeader"];

            return res;
        }


        public static bool GetSendAlertForTemperatureSMSEnabled(ApplicationDbContext DbContext, IConfiguration Configuration)
        {
            string res = DbContext.Configurations
                   .Where(x => x.ConfigKey == "SendSMSAlertForTemperature")
                   .Select(x => x.ConfigValue).FirstOrDefault();

            if (string.IsNullOrEmpty(res))
            {
                res = Configuration["SMSAlertForTemperatureEnabled"];
            }

            if (res == "0") return false;
            if (res == "1") return true;

            return false;
        }

        public static bool GetSendAlertForMaskSMSEnabled(ApplicationDbContext DbContext, IConfiguration Configuration)
        {
            string res = DbContext.Configurations
                   .Where(x => x.ConfigKey == "SendSMSAlertForMask")
                   .Select(x => x.ConfigValue).FirstOrDefault();

            if (string.IsNullOrEmpty(res))
            {
                res = Configuration["SMSAlertForMaskEnabled"];
            }

            if (res == "0") return false;
            if (res == "1") return true;

            return false;
        }


        public static bool GetSendAlertForTemperatureEmailEnabled(ApplicationDbContext DbContext, IConfiguration Configuration)
        {
            string res = DbContext.Configurations
                   .Where(x => x.ConfigKey == "SendEmailAlertForTemperature")
                   .Select(x => x.ConfigValue).FirstOrDefault();

            if (string.IsNullOrEmpty(res))
            {
                res = Configuration["EmailAlertForTemperatureEnabled"];
            }

            if (res == "0") return false;
            if (res == "1") return true;

            return false;
        }

        public static bool GetSendAlertForMaskEmailEnabled(ApplicationDbContext DbContext, IConfiguration Configuration)
        {
            string res = DbContext.Configurations
                   .Where(x => x.ConfigKey == "SendEmailAlertForMask")
                   .Select(x => x.ConfigValue).FirstOrDefault();

            if (string.IsNullOrEmpty(res))
            {
                res = Configuration["EmailAlertForMaskEnabled"];
            }

            if (res == "0") return false;
            if (res == "1") return true;

            return false;
        }


        public static string GetSMSSender(ApplicationDbContext DbContext, IConfiguration Configuration)
        {
            string res = DbContext.Configurations
                   .Where(x => x.ConfigKey == "SMSSender")
                   .Select(x => x.ConfigValue).FirstOrDefault();

            if (string.IsNullOrEmpty(res))
                res = Configuration["DefaultSMSSender"];

            return res;
        }

        public static string GetEmailSenderName(ApplicationDbContext DbContext, IConfiguration Configuration)
        {
            string res = DbContext.Configurations
                   .Where(x => x.ConfigKey == "EmailSenderName")
                   .Select(x => x.ConfigValue).FirstOrDefault();

            if (string.IsNullOrEmpty(res))
                res = Configuration["DefaultEmailSenderName"];

            return res;
        }

        public static string GetSensetimeEmployeeGroupId(ApplicationDbContext DbContext, IConfiguration Configuration)
        {
            string res = DbContext.Configurations
                   .Where(x => x.ConfigKey == "SensetimeEmployeeGroupId")
                   .Select(x => x.ConfigValue).FirstOrDefault();

            if (string.IsNullOrEmpty(res))
                res = Configuration["DefaultSensetimeEmployeeGroupId"];

            return res;
        }

        public static string GetSensetimeToken(ApplicationDbContext DbContext, IConfiguration Configuration)
        {
            string res = DbContext.Configurations
                   .Where(x => x.ConfigKey == "SensetimeToken")
                   .Select(x => x.ConfigValue).FirstOrDefault();

            return res;
        }

        public static void SaveSensetimeToken(ApplicationDbContext DbContext, IConfiguration Configuration, string sensetimeToken)
        {
            try
            {
                var res = DbContext.Configurations
                       .Where(x => x.ConfigKey == "SensetimeToken")
                       ?.Select(x => x).FirstOrDefault();

                res.ConfigValue = sensetimeToken;

                DbContext.Configurations.Update(res);
                DbContext.SaveChanges();
            }
            catch (Exception) 
            { 
                // Suppressing. If cannot save the token then may be next time get a new token and save that.
            }
        }

    }
}
