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
        const string DefaultTemperatureThresholdConfig = "DefaultTemperatureThreshold";
        const string DefaultTemperatureAlertHeaderConfig = "DefaultTemperatureAlertHeader";

        const string DefaultMaskAlertHeaderConfig = "DefaultMaskAlertHeader";

        const string DefaultSendAlertForTemperatureSMSConfig = "SMSAlertForTemperatureEnabled";
        const string DefaultSMSSenderConfig = "DefaultSMSSender";

        const string DefaultSendAlertForTemperatureEmailConfig = "EmailAlertForTemperatureEnabled";
        const string DefaultSendAlertForMaskEmailConfig = "EmailAlertForMaskEnabled";

        const string DefaultEmailSenderConfig = "DefaultEmailSenderName";

        const string DefaultSendAlertForMaskSMSConfig = "SMSAlertForMaskEnabled";

        public const string VisitorUID = "0";
        public const int NoMaskValue = 2;

        public static decimal GetTemperatureThreshold(ApplicationDbContext DbContext, IConfiguration Configuration)
        {
            bool res = decimal.TryParse(DbContext.Configurations
                   .Where(x => x.ConfigKey == "TemperatureThreshold")
                   .Select(x => x.ConfigValue).FirstOrDefault(), out decimal thresholdTemperature);

            if (!res)
                decimal.TryParse(Configuration[DefaultTemperatureThresholdConfig], out thresholdTemperature);

            return thresholdTemperature;
        }

        public static string GetTemperatureAlertHeader(ApplicationDbContext DbContext, IConfiguration Configuration)
        {
            string res = DbContext.Configurations
                   .Where(x => x.ConfigKey == "TemperatureAlertHeader")
                   .Select(x => x.ConfigValue).FirstOrDefault();

            if (string.IsNullOrEmpty(res))
                res = Configuration[DefaultTemperatureAlertHeaderConfig];

            return res;
        }

        public static string GetMaskAlertHeader(ApplicationDbContext DbContext, IConfiguration Configuration)
        {
            string res = DbContext.Configurations
                   .Where(x => x.ConfigKey == "MaskAlertHeader")
                   .Select(x => x.ConfigValue).FirstOrDefault();

            if (string.IsNullOrEmpty(res))
                res = Configuration[DefaultMaskAlertHeaderConfig];

            return res;
        }


        public static string GetTemperatureAlertEmailSubject(ApplicationDbContext DbContext, IConfiguration Configuration)
        {
            string res = DbContext.Configurations
                   .Where(x => x.ConfigKey == "TemperatureAlertEmailSubject")
                   .Select(x => x.ConfigValue).FirstOrDefault();

            if (string.IsNullOrEmpty(res))
                res = Configuration[DefaultTemperatureAlertHeaderConfig];

            return res;
        }

        public static string GetMaskAlertEmailSubject(ApplicationDbContext DbContext, IConfiguration Configuration)
        {
            string res = DbContext.Configurations
                   .Where(x => x.ConfigKey == "MaskAlertEmailSubject")
                   .Select(x => x.ConfigValue).FirstOrDefault();

            if (string.IsNullOrEmpty(res))
                res = Configuration[DefaultMaskAlertHeaderConfig];

            return res;
        }


        public static bool GetSendAlertForTemperatureSMSEnabled(ApplicationDbContext DbContext, IConfiguration Configuration)
        {
            string res = DbContext.Configurations
                   .Where(x => x.ConfigKey == "SendSMSAlertForTemperature")
                   .Select(x => x.ConfigValue).FirstOrDefault();

            if (string.IsNullOrEmpty(res))
            {
                res = Configuration[DefaultSendAlertForTemperatureSMSConfig];
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
                res = Configuration[DefaultSendAlertForMaskSMSConfig];
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
                res = Configuration[DefaultSendAlertForTemperatureEmailConfig];
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
                res = Configuration[DefaultSendAlertForMaskEmailConfig];
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
                res = Configuration[DefaultSMSSenderConfig];

            return res;
        }

        public static string GetEmailSenderName(ApplicationDbContext DbContext, IConfiguration Configuration)
        {
            string res = DbContext.Configurations
                   .Where(x => x.ConfigKey == "EmailSenderName")
                   .Select(x => x.ConfigValue).FirstOrDefault();

            if (string.IsNullOrEmpty(res))
                res = Configuration[DefaultEmailSenderConfig];

            return res;
        }

    }
}
