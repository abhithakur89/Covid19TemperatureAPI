using Covid19TemperatureAPI.Entities.Data;
using Covid19TemperatureAPI.Helper;
using Covid19TemperatureAPI.SenseTime;
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
        ISensetime Sensetime;

        private enum ResponseCodes
        {
            [Display(Name = "Successful")]
            Successful = 1200,
            [Display(Name = "Error")]
            SystemError = 1201,
        }

        public SiteController(IConfiguration configuration, ApplicationDbContext applicationDbContext, ILogger<SiteController> logger, ISensetime sensetime)
        {
            Configuration = configuration;
            Context = applicationDbContext;
            _logger = logger;
            Sensetime = sensetime;
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

        /// <summary>
        /// GetSiteSummary API. Returns the summary for the site.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /c19server/getsitesummary
        ///     {
        ///         "siteid":"1"
        ///     }
        ///      
        /// Sample response:
        /// 
        ///     {
        ///         "respcode": 1200,
        ///         "description": "Successful",
        ///         "employees": 2,
        ///         "visitors": 4,
        ///         "alerts": 6,
        ///         "abnormalTemperatureAlerts": 3,
        ///         "noMaskAlerts": 3
        ///     }
        ///     
        /// Response codes:
        ///     1200 = "Successful"
        ///     1201 = "Error"
        /// </remarks>
        /// <returns>
        /// </returns>

        [HttpPost]
        [Route("getsitesummary")]
        public ActionResult GetSiteSummary([FromBody]JObject jsiteId)
        {
            try
            {
                _logger.LogInformation("GetSiteSummary() called from: " + HttpContext.Connection.RemoteIpAddress.ToString());

                var received = new { SiteId = string.Empty };

                received = JsonConvert.DeserializeAnonymousType(jsiteId.ToString(Formatting.None), received);

                _logger.LogInformation($"Paramerters: {received.SiteId}");

                int.TryParse(received.SiteId, out int nSiteId);

                var devices = (Context.Devices
                    .Where(x => x.Gate.Floor.Building.SiteId == nSiteId)
                    .Select(x => x.DeviceId)).Distinct();

                var Employees = (from a in Context.Employees
                                 join b in Context.TemperatureRecords on a.UID equals b.PersonUID into bb
                                 from c in bb.DefaultIfEmpty()
                                 join d in Context.MaskRecords on a.UID equals d.PersonUID into dd
                                 from e in dd.DefaultIfEmpty()
                                 where (devices.Contains(c.DeviceId) && c.Timestamp.Date == DateTime.Today)
                                 || (devices.Contains(e.DeviceId) && e.Timestamp.Date == DateTime.Today)
                                 select new { a.EmployeeId }).Distinct().Count();

                var visitorsInTemperatureTable = (Context.TemperatureRecords
                    .Where(x => x.PersonUID == ConfigReader.VisitorUID
                    && x.Timestamp.Date == DateTime.Today
                    && devices.Contains(x.DeviceId)
                    && !string.IsNullOrEmpty(x.Mobile))
                    .Select(x => x.Mobile)).Distinct();

                var visitorsInMaskTable = (Context.MaskRecords
                    .Where(x => x.PersonUID == ConfigReader.VisitorUID
                    && x.Timestamp.Date == DateTime.Today
                    && devices.Contains(x.DeviceId)
                    && !string.IsNullOrEmpty(x.Mobile))
                    .Select(x => x.Mobile)).Distinct();

                var Visitors = visitorsInTemperatureTable.Concat(visitorsInMaskTable)
                    .Distinct().Count();

                // Temperature Threshold
                decimal thresholdTemperature = ConfigReader.GetTemperatureThreshold(Context, Configuration);

                // Count Temperature Alerts. First count employees alerts
                int employeeTemperatureAlerts = (from a in Context.Employees
                                                 join b in Context.TemperatureRecords on a.UID equals b.PersonUID
                                                 where b.Temperature > thresholdTemperature
                                                 && devices.Contains(b.DeviceId)
                                                 && b.Timestamp.Date == DateTime.Today
                                                 select new { a.EmployeeId }).Distinct().Count();

                // Count Visitor temperature alerts
                int visitorTemperatureAlerts = (Context.TemperatureRecords
                    .Where(x => x.PersonUID == ConfigReader.VisitorUID
                        && x.Timestamp.Date == DateTime.Today
                        && x.Temperature > thresholdTemperature
                        && devices.Contains(x.DeviceId)
                        && !string.IsNullOrEmpty(x.Mobile))
                    .Select(x => x.Mobile)).Distinct().Count();

                int AbnormalTemperatureAlerts = employeeTemperatureAlerts + visitorTemperatureAlerts;

                // Count Mask Alerts. First count employees alerts
                int employeeMaskAlerts = (from a in Context.Employees
                                          join b in Context.MaskRecords on a.UID equals b.PersonUID
                                          where b.MaskValue == ConfigReader.NoMaskValue
                                          && b.Timestamp.Date == DateTime.Today
                                          && devices.Contains(b.DeviceId)
                                          select new { a.EmployeeId }).Distinct().Count();

                // Count Visitor mask alerts
                int visitorMaskAlerts = (Context.MaskRecords
                    .Where(x => x.PersonUID == ConfigReader.VisitorUID
                        && x.Timestamp.Date == DateTime.Today 
                        && devices.Contains(x.DeviceId)
                        && !string.IsNullOrEmpty(x.Mobile))
                    .Select(x => x.Mobile)).Distinct().Count();

                int NoMaskAlerts = employeeMaskAlerts + visitorMaskAlerts;

                return new JsonResult(new
                {
                    respcode = ResponseCodes.Successful,
                    description = ResponseCodes.Successful.DisplayName(),
                    Employees,
                    Visitors,
                    Alerts = AbnormalTemperatureAlerts + NoMaskAlerts,
                    AbnormalTemperatureAlerts,
                    NoMaskAlerts
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
        /// GetBuildingSummary API. Returns the summary for the building.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /c19server/getbuildingsummary
        ///     {
        ///         "buildingid":"1"
        ///     }
        ///      
        /// Sample response:
        /// 
        ///     {
        ///         "respcode": 1200,
        ///         "description": "Successful",
        ///         "employees": 2,
        ///         "visitors": 4,
        ///         "alerts": 6,
        ///         "abnormalTemperatureAlerts": 3,
        ///         "noMaskAlerts": 3
        ///     }
        ///     
        /// Response codes:
        ///     1200 = "Successful"
        ///     1201 = "Error"
        /// </remarks>
        /// <returns>
        /// </returns>

        [HttpPost]
        [Route("getbuildingsummary")]
        public ActionResult GetBuildingSummary([FromBody]JObject jbuildingid)
        {
            try
            {
                _logger.LogInformation("GetBuildingSummary() called from: " + HttpContext.Connection.RemoteIpAddress.ToString());

                var received = new { BuildingId = string.Empty };

                received = JsonConvert.DeserializeAnonymousType(jbuildingid.ToString(Formatting.None), received);

                _logger.LogInformation($"Paramerters: {received.BuildingId}");

                int.TryParse(received.BuildingId, out int nBuildingId);

                var devices = (Context.Devices
                    .Where(x => x.Gate.Floor.BuildingId == nBuildingId)
                    .Select(x => x.DeviceId)).Distinct();
                
                var Employees = (from a in Context.Employees
                                 join b in Context.TemperatureRecords on a.UID equals b.PersonUID into bb
                                 from c in bb.DefaultIfEmpty()
                                 join d in Context.MaskRecords on a.UID equals d.PersonUID into dd
                                 from e in dd.DefaultIfEmpty()
                                 where (devices.Contains(c.DeviceId) && c.Timestamp.Date == DateTime.Today)
                                 || (devices.Contains(e.DeviceId) && e.Timestamp.Date == DateTime.Today)
                                 select new { a.EmployeeId }).Distinct().Count();
                
                var visitorsInTemperatureTable = (Context.TemperatureRecords
                    .Where(x => x.PersonUID == ConfigReader.VisitorUID
                    && x.Timestamp.Date == DateTime.Today
                    && devices.Contains(x.DeviceId)
                    && !string.IsNullOrEmpty(x.Mobile))
                    .Select(x => x.Mobile)).Distinct();

                var visitorsInMaskTable = (Context.MaskRecords
                    .Where(x => x.PersonUID == ConfigReader.VisitorUID
                    && x.Timestamp.Date == DateTime.Today
                    && devices.Contains(x.DeviceId)
                    && !string.IsNullOrEmpty(x.Mobile))
                    .Select(x => x.Mobile)).Distinct();

                var Visitors = visitorsInTemperatureTable.Concat(visitorsInMaskTable)
                    .Distinct().Count();

                // Temperature Threshold
                decimal thresholdTemperature = ConfigReader.GetTemperatureThreshold(Context, Configuration);

                // Count Temperature Alerts. First count employees alerts
                int employeeTemperatureAlerts = (from a in Context.Employees
                                                 join b in Context.TemperatureRecords on a.UID equals b.PersonUID
                                                 where b.Temperature > thresholdTemperature
                                                 && b.Timestamp.Date == DateTime.Today
                                                 && devices.Contains(b.DeviceId)
                                                 select new { a.EmployeeId }).Distinct().Count();

                // Count Visitor temperature alerts
                int visitorTemperatureAlerts = (Context.TemperatureRecords
                    .Where(x => x.PersonUID == ConfigReader.VisitorUID
                        && x.Timestamp.Date == DateTime.Today
                        && x.Temperature > thresholdTemperature
                        && devices.Contains(x.DeviceId)
                        && !string.IsNullOrEmpty(x.Mobile))
                    .Select(x => x.Mobile)).Distinct().Count();

                int AbnormalTemperatureAlerts = employeeTemperatureAlerts + visitorTemperatureAlerts;

                // Count Mask Alerts. First count employees alerts
                int employeeMaskAlerts = (from a in Context.Employees
                                          join b in Context.MaskRecords on a.UID equals b.PersonUID
                                          where b.MaskValue == ConfigReader.NoMaskValue
                                          && devices.Contains(b.DeviceId)
                                          && b.Timestamp.Date == DateTime.Today
                                          select new { a.EmployeeId }).Distinct().Count();

                // Count Visitor mask alerts
                var visitorMaskAlerts = (Context.MaskRecords
                    .Where(x => x.PersonUID == ConfigReader.VisitorUID
                        && x.Timestamp.Date == DateTime.Today
                        && devices.Contains(x.DeviceId)
                        && !string.IsNullOrEmpty(x.Mobile))
                    .Select(x => x.Mobile)).Distinct().Count();

                int NoMaskAlerts = employeeMaskAlerts + visitorMaskAlerts;

                return new JsonResult(new
                {
                    respcode = ResponseCodes.Successful,
                    description = ResponseCodes.Successful.DisplayName(),
                    Employees,
                    Visitors,
                    Alerts = AbnormalTemperatureAlerts + NoMaskAlerts,
                    AbnormalTemperatureAlerts,
                    NoMaskAlerts
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
        /// GetFloorSummary API. Returns the summary for the floor.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /c19server/getfloorsummary
        ///     {
        ///         "floorid":"1"
        ///     }
        ///      
        /// Sample response:
        /// 
        ///     {
        ///         "respcode": 1200,
        ///         "description": "Successful",
        ///         "employees": 2,
        ///         "visitors": 4,
        ///         "alerts": 6,
        ///         "abnormalTemperatureAlerts": 3,
        ///         "noMaskAlerts": 3
        ///     }
        ///     
        /// Response codes:
        ///     1200 = "Successful"
        ///     1201 = "Error"
        /// </remarks>
        /// <returns>
        /// </returns>

        [HttpPost]
        [Route("getfloorsummary")]
        public ActionResult GetFloorSummary([FromBody]JObject jfloorid)
        {
            try
            {
                _logger.LogInformation("GetFloorSummary() called from: " + HttpContext.Connection.RemoteIpAddress.ToString());

                var received = new { FloorId = string.Empty };

                received = JsonConvert.DeserializeAnonymousType(jfloorid.ToString(Formatting.None), received);

                _logger.LogInformation($"Paramerters: {received.FloorId}");

                int.TryParse(received.FloorId, out int nFloorId);

                var devices = (Context.Devices
                    .Where(x => x.Gate.FloorId == nFloorId)
                    .Select(x => x.DeviceId)).Distinct();
                
                var Employees = (from a in Context.Employees
                                 join b in Context.TemperatureRecords on a.UID equals b.PersonUID into bb
                                 from c in bb.DefaultIfEmpty()
                                 join d in Context.MaskRecords on a.UID equals d.PersonUID into dd
                                 from e in dd.DefaultIfEmpty()
                                 where (devices.Contains(c.DeviceId) && c.Timestamp.Date == DateTime.Today)
                                 || (devices.Contains(e.DeviceId) && e.Timestamp.Date == DateTime.Today)
                                 select new { a.EmployeeId }).Distinct().Count();
                
                var visitorsInTemperatureTable = (Context.TemperatureRecords
                    .Where(x => x.PersonUID == ConfigReader.VisitorUID
                    && x.Timestamp.Date == DateTime.Today
                    && devices.Contains(x.DeviceId)
                    && !string.IsNullOrEmpty(x.Mobile))
                    .Select(x => x.Mobile)).Distinct();

                var visitorsInMaskTable = (Context.MaskRecords
                    .Where(x => x.PersonUID == ConfigReader.VisitorUID
                    && x.Timestamp.Date == DateTime.Today
                    && devices.Contains(x.DeviceId)
                    && !string.IsNullOrEmpty(x.Mobile))
                    .Select(x => x.Mobile)).Distinct();

                var Visitors = visitorsInTemperatureTable.Concat(visitorsInMaskTable)
                    .Distinct().Count();

                // Temperature Threshold
                decimal thresholdTemperature = ConfigReader.GetTemperatureThreshold(Context, Configuration);

                // Count Temperature Alerts. First count employees alerts
                int employeeTemperatureAlerts = (from a in Context.Employees
                                                 join b in Context.TemperatureRecords on a.UID equals b.PersonUID
                                                 where b.Temperature > thresholdTemperature
                                                 && b.Timestamp.Date == DateTime.Today
                                                 && devices.Contains(b.DeviceId)
                                                 select new { a.EmployeeId }).Distinct().Count();

                // Count Visitor temperature alerts
                int visitorTemperatureAlerts = (Context.TemperatureRecords
                    .Where(x => x.PersonUID == ConfigReader.VisitorUID
                        && x.Timestamp.Date == DateTime.Today
                        && x.Temperature > thresholdTemperature
                        && devices.Contains(x.DeviceId)
                        && !string.IsNullOrEmpty(x.Mobile))
                    .Select(x => x.Mobile)).Distinct().Count();

                int AbnormalTemperatureAlerts = employeeTemperatureAlerts + visitorTemperatureAlerts;

                // Count Mask Alerts. First count employees alerts
                int employeeMaskAlerts = (from a in Context.Employees
                                          join b in Context.MaskRecords on a.UID equals b.PersonUID
                                          where b.MaskValue == ConfigReader.NoMaskValue
                                          && devices.Contains(b.DeviceId)
                                          && b.Timestamp.Date == DateTime.Today
                                          select new { a.EmployeeId }).Distinct().Count();

                // Count Visitor mask alerts
                int visitorMaskAlerts = (Context.MaskRecords
                    .Where(x => x.PersonUID == ConfigReader.VisitorUID
                        && x.Timestamp.Date == DateTime.Today
                        && devices.Contains(x.DeviceId)
                        && !string.IsNullOrEmpty(x.Mobile))
                    .Select(x => x.Mobile)).Distinct().Count();

                int NoMaskAlerts = employeeMaskAlerts + visitorMaskAlerts;

                return new JsonResult(new
                {
                    respcode = ResponseCodes.Successful,
                    description = ResponseCodes.Successful.DisplayName(),
                    Employees,
                    Visitors,
                    Alerts = AbnormalTemperatureAlerts + NoMaskAlerts,
                    AbnormalTemperatureAlerts,
                    NoMaskAlerts
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
        /// GetGateSummary API. Returns the summary for the gate.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /c19server/getgatesummary
        ///     {
        ///         "gateid":"1"
        ///     }
        ///      
        /// Sample response:
        /// 
        ///     {
        ///         "respcode": 1200,
        ///         "description": "Successful",
        ///         "employees": 2,
        ///         "visitors": 4,
        ///         "alerts": 6,
        ///         "abnormalTemperatureAlerts": 3,
        ///         "noMaskAlerts": 3
        ///     }
        ///     
        /// Response codes:
        ///     1200 = "Successful"
        ///     1201 = "Error"
        /// </remarks>
        /// <returns>
        /// </returns>

        [HttpPost]
        [Route("getgatesummary")]
        public ActionResult GetGateSummary([FromBody]JObject jgateid)
        {
            try
            {
                _logger.LogInformation("GetGateSummary() called from: " + HttpContext.Connection.RemoteIpAddress.ToString());

                var received = new { GateId = string.Empty };

                received = JsonConvert.DeserializeAnonymousType(jgateid.ToString(Formatting.None), received);

                _logger.LogInformation($"Paramerters: {received.GateId}");

                int.TryParse(received.GateId, out int nGateId);

                var devices = (Context.Devices
                    .Where(x => x.GateId == nGateId)
                    .Select(x => x.DeviceId)).Distinct();

                var Employees = (from a in Context.Employees
                                 join b in Context.TemperatureRecords on a.UID equals b.PersonUID into bb
                                 from c in bb.DefaultIfEmpty()
                                 join d in Context.MaskRecords on a.UID equals d.PersonUID into dd
                                 from e in dd.DefaultIfEmpty()
                                 where (devices.Contains(c.DeviceId) && c.Timestamp.Date == DateTime.Today)
                                 || (devices.Contains(e.DeviceId) && e.Timestamp.Date == DateTime.Today)
                                 select new { a.EmployeeId }).Distinct().Count();

                var visitorsInTemperatureTable = (Context.TemperatureRecords
                    .Where(x => x.PersonUID == ConfigReader.VisitorUID
                    && x.Timestamp.Date == DateTime.Today
                    && devices.Contains(x.DeviceId)
                    && !string.IsNullOrEmpty(x.Mobile))
                    .Select(x => x.Mobile)).Distinct();

                var visitorsInMaskTable = (Context.MaskRecords
                    .Where(x => x.PersonUID == ConfigReader.VisitorUID
                    && x.Timestamp.Date == DateTime.Today
                    && devices.Contains(x.DeviceId)
                    && !string.IsNullOrEmpty(x.Mobile))
                    .Select(x => x.Mobile)).Distinct();

                var Visitors = visitorsInTemperatureTable.Concat(visitorsInMaskTable)
                    .Distinct().Count();

                // Temperature Threshold
                decimal thresholdTemperature = ConfigReader.GetTemperatureThreshold(Context, Configuration);

                // Count Temperature Alerts. First count employees alerts
                int employeeTemperatureAlerts = (from a in Context.Employees
                                                 join b in Context.TemperatureRecords on a.UID equals b.PersonUID
                                                 where b.Temperature > thresholdTemperature
                                                 && b.Timestamp.Date == DateTime.Today
                                                 && devices.Contains(b.DeviceId)
                                                 select new { a.EmployeeId }).Distinct().Count();

                // Count Visitor temperature alerts
                int visitorTemperatureAlerts = (Context.TemperatureRecords
                    .Where(x => x.PersonUID == ConfigReader.VisitorUID
                        && x.Timestamp.Date == DateTime.Today
                        && x.Temperature > thresholdTemperature
                        && devices.Contains(x.DeviceId)
                        && !string.IsNullOrEmpty(x.Mobile))
                    .Select(x => x.Mobile)).Distinct().Count();

                int AbnormalTemperatureAlerts = employeeTemperatureAlerts + visitorTemperatureAlerts;

                // Count Mask Alerts. First count employees alerts
                int employeeMaskAlerts = (from a in Context.Employees
                                          join b in Context.MaskRecords on a.UID equals b.PersonUID
                                          where b.MaskValue == ConfigReader.NoMaskValue
                                          && devices.Contains(b.DeviceId)
                                          && b.Timestamp.Date == DateTime.Today
                                          select new { a.EmployeeId }).Distinct().Count();

                // Count Visitor mask alerts
                int visitorMaskAlerts = (Context.MaskRecords
                    .Where(x => x.PersonUID == ConfigReader.VisitorUID
                        && x.Timestamp.Date == DateTime.Today
                        && devices.Contains(x.DeviceId)
                        && !string.IsNullOrEmpty(x.Mobile))
                    .Select(x => x.Mobile)).Distinct().Count();

                int NoMaskAlerts = employeeMaskAlerts + visitorMaskAlerts;

                return new JsonResult(new
                {
                    respcode = ResponseCodes.Successful,
                    description = ResponseCodes.Successful.DisplayName(),
                    Employees,
                    Visitors,
                    Alerts = AbnormalTemperatureAlerts + NoMaskAlerts,
                    AbnormalTemperatureAlerts,
                    NoMaskAlerts
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
        /// GetEntranceLogForToday API. Returns the  Entrance log for Today.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /c19server/getentrancelogfortoday
        ///     {
        ///         "siteid":"1"
        ///     }
        ///      
        /// Sample response:
        /// 
        ///     {
        ///         "respcode": 1200,
        ///         "description": "Successful",
        ///         "entranceLogForToday": [
        ///             {
        ///                 "visitor": false,
        ///                 "person": "Abhishek",
        ///                 "location": "Floor 3 Reception Gate",
        ///                 "temperature": "36.7",
        ///                 "mask": false,
        ///                 "timestamp": "2020-07-25 23:13:44",
        ///                 "image": "data:image/jpeg;base64,/9j/4AA...SQT/9k="
        ///             },
        ///             {
        ///                 ...
        ///             }
        ///             ...
        ///         ]
        ///     }        
        /// Response codes:
        ///     1200 = "Successful"
        ///     1201 = "Error"
        /// </remarks>
        /// <returns>
        /// </returns>


        [HttpPost]
        [Route("getentrancelogfortoday")]
        public ActionResult GetEntranceLogForToday([FromBody]JObject jsiteId)
        {
            try
            {
                _logger.LogInformation("GetEntranceLogForToday() called from: " + HttpContext.Connection.RemoteIpAddress.ToString());

                var received = new { SiteId = string.Empty };

                received = JsonConvert.DeserializeAnonymousType(jsiteId.ToString(Formatting.None), received);

                _logger.LogInformation($"Paramerters: {received.SiteId}");

                int.TryParse(received.SiteId, out int nSiteId);

                var devices = (Context.Devices
                    .Where(x => x.Gate.Floor.Building.SiteId == nSiteId)
                    .Select(x => x.DeviceId)).Distinct();

                var temperatureTimestamps = (Context.TemperatureRecords
                    .Where(x => x.Timestamp.Date == DateTime.Today && devices.Contains(x.DeviceId))
                    .GroupBy(x => x.ImagePath)
                    .Select(x => new { Image = x.Key, LatestTimestamp = x.Max(y => y.Timestamp).ToString() })).Distinct();

                var maskTimestamps = (Context.MaskRecords
                    .Where(x => x.Timestamp.Date == DateTime.Today && devices.Contains(x.DeviceId))
                    .GroupBy(x => x.ImagePath)
                    .Select(x => new { Image = x.Key, LatestTimestamp = x.Max(y => y.Timestamp).ToString() })).Distinct();

                var temperatureRecords = (from a in Context.TemperatureRecords
                                          join b in temperatureTimestamps on a.Timestamp.ToString() equals b.LatestTimestamp
                                          select new
                                          {
                                              a.PersonUID,
                                              a.PersonName,
                                              //a.DeviceId,
                                              //a.Device.Gate.GateNumber,
                                              Location = a.Device.Gate.AdditionalDetails,
                                              Temperature = a.Temperature.ToString("#.#"),
                                              Timestamp = a.Timestamp.ToString(),
                                              a.ImagePath,
                                              a.ImageBase64,
                                              //a.IC,
                                              //a.Mobile
                                          }).Distinct();

                var maskRecords = (from a in Context.MaskRecords
                                   join b in maskTimestamps on a.Timestamp.ToString() equals b.LatestTimestamp
                                   select new
                                   {
                                       a.PersonUID,
                                       a.PersonName,
                                       //a.DeviceId,
                                       //a.Device.Gate.GateNumber,
                                       Location = a.Device.Gate.AdditionalDetails,
                                       //MaskValue = a.MaskValue.ToString(),
                                       Mask = false,
                                       Timestamp = a.Timestamp.ToString(),
                                       a.ImagePath,
                                       a.ImageBase64,
                                       //a.IC,
                                       //a.Mobile
                                   }).Distinct();

                var leftOuterJoin = from a in temperatureRecords
                                    join b in maskRecords on a.Timestamp equals b.Timestamp into bb
                                    from c in bb.DefaultIfEmpty()
                                    select new
                                    {
                                        Visitor = a.PersonUID == ConfigReader.VisitorUID,
                                        Person = a.PersonName,
                                        //a.DeviceId,
                                        //a.GateNumber,
                                        a.Location,
                                        a.Temperature,
                                        //MaskValue = c != null ? c.MaskValue.ToString() : string.Empty,
                                        Mask = c != null ? false : true,   // false means no mask
                                        DateTime = a.Timestamp.Remove(a.Timestamp.Length - 8),
                                        //Image =  a.ImagePath.ConvertImageUrlToBase64().Result,
                                        Captured = !string.IsNullOrEmpty(a.ImageBase64) ? a.ImageBase64 : a.ImagePath.ConvertImageUrlToBase64().Result,
                                        //a.IC,
                                        //a.Mobile
                                    };

                var rightOuterJoin = from a in maskRecords
                                     join b in temperatureRecords on a.Timestamp equals b.Timestamp into bb
                                     from c in bb.DefaultIfEmpty()
                                     select new
                                     {
                                         Visitor = a.PersonUID == ConfigReader.VisitorUID,
                                         Person = a.PersonName,
                                         //a.DeviceId,
                                         //a.GateNumber,
                                         a.Location,
                                         Temperature = c != null ? c.Temperature.ToString() : string.Empty,
                                         a.Mask,
                                         DateTime = a.Timestamp.Remove(a.Timestamp.Length - 8),
                                         //Image = a.ImagePath.ConvertImageUrlToBase64().Result,
                                         Captured = !string.IsNullOrEmpty(a.ImageBase64) ? a.ImageBase64 : a.ImagePath.ConvertImageUrlToBase64().Result,
                                         //a.IC,
                                         //a.Mobile
                                     };

                var entranceLogForToday = from a in leftOuterJoin.Union(rightOuterJoin)
                                    orderby a.DateTime descending
                                    select a;


                return new JsonResult(new
                {
                    respcode = ResponseCodes.Successful,
                    description = ResponseCodes.Successful.DisplayName(),
                    entranceLogForToday
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
