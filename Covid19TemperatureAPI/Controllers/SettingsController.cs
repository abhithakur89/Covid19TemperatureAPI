using Covid19TemperatureAPI.Entities.Data;
using Covid19TemperatureAPI.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19TemperatureAPI.Controllers
{
    [Route("c19server")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class SettingsController: ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger<SettingsController> _logger;
        private ApplicationDbContext Context { get; set; }

        private enum ResponseCodes
        {
            [Display(Name = "Successful")]
            Successful = 1200,
            [Display(Name = "Error")]
            SystemError = 1201,
        }

        public SettingsController(IConfiguration configuration, ApplicationDbContext applicationDbContext, ILogger<SettingsController> logger)
        {
            Configuration = configuration;
            Context = applicationDbContext;
            _logger = logger;
        }

        /// <summary>
        /// GetTemperatureThreshold API. Returns current temperature threshold value.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /c19server/gettemperaturethreshold
        ///      
        /// Sample response:
        /// 
        ///     {
        ///         "respcode": 1200,
        ///         "description": "Successful",
        ///         "temperatureThreshold": "37.5"
        ///     }
        ///     
        /// Response codes:
        ///     1200 = "Successful"
        ///     1201 = "Error"
        /// </remarks>
        /// <returns>
        /// </returns>
        [HttpPost]
        [Route("gettemperaturethreshold")]
        public ActionResult GetTemperatureThreshold()
        {
            try
            {
                _logger.LogInformation("GetTemperatureThreshold() called from: " + HttpContext.Connection.RemoteIpAddress.ToString());

                var temperatureThreshold = ConfigReader.GetTemperatureThreshold(Context, Configuration).ToString("#.#");

                _logger.LogInformation($"Returning temperatureThreshold: {temperatureThreshold}");
                return new JsonResult(new
                {
                    respcode = ResponseCodes.Successful,
                    description = ResponseCodes.Successful.DisplayName(),
                    temperatureThreshold
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"Generic exception handler invoked. {e.Message}: {e.StackTrace}");

                return new JsonResult(new
                {
                    respcode = ResponseCodes.SystemError,
                    description = ResponseCodes.SystemError.DisplayName(),
                    Error = e.Message
                });
            }
        }

        /// <summary>
        /// SetTemperatureThreshold API. Sets the temperature threshold value.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /c19server/settemperaturethreshold
        ///      
        /// Sample response:
        /// 
        ///     {
        ///         "respcode": 1200,
        ///         "description": "Successful"
        ///     }
        ///     
        /// Response codes:
        ///     1200 = "Successful"
        ///     1201 = "Error"
        /// </remarks>
        /// <returns>
        /// </returns>
        [HttpPost]
        [Route("settemperaturethreshold")]
        public ActionResult SetTemperatureThreshold([FromBody]JObject jthreshold)
        {
            try
            {
                _logger.LogInformation("SetTemperatureThreshold() called from: " + HttpContext.Connection.RemoteIpAddress.ToString());

                var received = new { Threshold = string.Empty };

                received = JsonConvert.DeserializeAnonymousType(jthreshold.ToString(Formatting.None), received);

                _logger.LogInformation($"Paramerters: {received.Threshold}");

                if (!decimal.TryParse(received.Threshold, out decimal dThreshold)) throw new Exception("Invalid threshold value");

                var thresholdRecord = Context.Configurations
                    .Where(x => x.ConfigKey == ConfigReader.TemperatureThresholdConfigSettingName)
                    .Select(x => x)?.FirstOrDefault();

                thresholdRecord.ConfigValue = received.Threshold;

                Context.Update(thresholdRecord);
                Context.SaveChanges();

                // Update device table
                var devices = Context.Devices;

                foreach(var device in devices )
                {
                    device.UpdatedThreshold = false;
                    Context.Devices.Update(device);
                }

                Context.SaveChanges();

                return new JsonResult(new
                {
                    respcode = ResponseCodes.Successful,
                    description = ResponseCodes.Successful.DisplayName()
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"Generic exception handler invoked. {e.Message}: {e.StackTrace}");

                return new JsonResult(new
                {
                    respcode = ResponseCodes.SystemError,
                    description = ResponseCodes.SystemError.DisplayName(),
                    Error = e.Message
                });
            }
        }

    }
}
