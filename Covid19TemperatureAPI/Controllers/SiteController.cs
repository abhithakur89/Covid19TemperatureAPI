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
    public class SiteController : ControllerBase
    {
        private IConfiguration Configuration;
        private readonly ILogger<SiteController> _logger;
        private ApplicationDbContext Context { get; set; }

        private enum ResponseCodes
        {
            [Display(Name = "Successful")]
            Successful = 1200,
            [Display(Name = "Error")]
            SystemError = 1201,
        }

        public SiteController(IConfiguration configuration, ApplicationDbContext applicationDbContext, ILogger<SiteController> logger)
        {
            Configuration = configuration;
            Context = applicationDbContext;
            _logger = logger;
        }

        /// <summary>
        /// GetAllSites API. Returns all sites info.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /c19server/getallsites
        ///      
        /// Sample response:
        /// 
        ///     {
        ///         "respcode": 1200,
        ///         "description": "Successful",
        ///         "sites": [
        ///             {
        ///                 "siteId": 1,
        ///                 "siteName": "Singapore",
        ///                 "siteDescription": "Chai Chee Office"
        ///             },
        ///             {
        ///                 ...
        ///             }
        ///             ...
        ///         ]
        ///     }
        ///     
        /// Response codes:
        ///     1200 = "Successful"
        ///     1201 = "Error"
        /// </remarks>
        /// <returns>
        /// </returns>
        [HttpPost]
        [Route("getallsites")]
        public ActionResult GetAllSites()
        {
            try
            {
                _logger.LogInformation("GetAllSites() called from: " + HttpContext.Connection.RemoteIpAddress.ToString());

                var sites = Context.Sites
                    .Select(x => new { x.SiteId, x.SiteName, x.SiteDescription });

                _logger.LogInformation($"Returning sites: {JsonConvert.SerializeObject(sites)}");
                return new JsonResult(new
                {
                    respcode = ResponseCodes.Successful,
                    description = ResponseCodes.Successful.DisplayName(),
                    sites
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
        /// GetSiteDevices API. Returns all devices for the site id specified.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /c19server/getsitedevices
        ///     {
        ///         "siteid":"1"
        ///     }
        ///      
        /// Sample response:
        /// 
        ///     {
        ///         "respcode": 1200,
        ///         "description": "Successful",
        ///         "res": [
        ///             {
        ///                 "buildingId": 1,
        ///                 "buildingName": "750, Chai Chee Road",
        ///                 "floorId": 1,
        ///                 "floorNumber": "3",
        ///                 "floorDetails": "OBS Office",
        ///                 "gateId": 1,
        ///                 "gateNumber": "1",
        ///                 "deviceId": "1234567890",
        ///                 "deviceDetails": "Test device"
        ///             },
        ///             {
        ///                 ...
        ///             }
        ///             ...
        ///         ]
        ///     }
        ///     
        /// Response codes:
        ///     1200 = "Successful"
        ///     1201 = "Error"
        /// </remarks>
        /// <returns>
        /// </returns>
        [HttpPost]
        [Route("getsitedevices")]
        public ActionResult GetSiteDevices([FromBody]JObject jsiteId)
        {
            try
            {
                _logger.LogInformation("GetSiteDevices() called from: " + HttpContext.Connection.RemoteIpAddress.ToString());

                var received = new { SiteId = string.Empty };

                received = JsonConvert.DeserializeAnonymousType(jsiteId.ToString(Formatting.None), received);

                _logger.LogInformation($"Paramerters: {received.SiteId}");

                int.TryParse(received.SiteId, out int nSiteId);


                var devices = from a in Context.Buildings
                          join b in Context.Floors on a.BuildingId equals b.BuildingId
                          join c in Context.Gates on b.FloorId equals c.FloorId
                          join d in Context.Devices on c.GateId equals d.GateId
                          where a.SiteId == nSiteId
                          select new
                          {
                              a.BuildingId,
                              a.BuildingName,
                              b.FloorId,
                              b.FloorNumber,
                              b.FloorDetails,
                              c.GateId,
                              c.GateNumber,
                              d.DeviceId,
                              d.DeviceDetails
                          };


                return new JsonResult(new
                {
                    respcode = ResponseCodes.Successful,
                    description = ResponseCodes.Successful.DisplayName(),
                    devices
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
