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
    public class ContactTracingController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger<ContactTracingController> _logger;
        private ApplicationDbContext Context { get; set; }

        private enum ResponseCodes
        {
            [Display(Name = "Successful")]
            Successful = 1200,
            [Display(Name = "Error")]
            SystemError = 1201,
        }

        public ContactTracingController(IConfiguration configuration, ApplicationDbContext applicationDbContext, ILogger<ContactTracingController> logger)
        {
            Configuration = configuration;
            Context = applicationDbContext;
            _logger = logger;
        }

        /// <summary>
        /// GetQueriedPersonDetails API. Returns the person details by an alert timestamp.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /c19server/getqueriedpersondetails
        ///     {
        ///         "alertTimestamp":"2020-07-28 11:57:56"
        ///     }
        ///      
        /// Sample response:
        /// 
        ///     {
        ///         "respcode": 1200,
        ///         "description": "Successful",
        ///         "queriedPerson": {
        ///             "personName": "Abhishek",
        ///             "personImage": "data:image/jpeg;base64,/9j/4A...uf/9k=",
        ///             "personAlertImage": "data:image/jpeg;base64,/9j/4AAQSkZ...j//Z",
        ///             "isVisitor": false
        ///         }
        ///     }
        /// Response codes:
        ///     1200 = "Successful"
        ///     1201 = "Error"
        /// </remarks>
        /// <returns>
        /// </returns>

        [HttpPost]
        [Route("getqueriedpersondetails")]
        public ActionResult GetQueriedPersonDetails([FromBody]JObject alertTimestamp)
        {
            try
            {
                _logger.LogInformation("GetQueriedPersonDetails() called from: " + HttpContext.Connection.RemoteIpAddress.ToString());

                var received = new { AlertTimestamp = string.Empty };

                received = JsonConvert.DeserializeAnonymousType(alertTimestamp.ToString(Formatting.None), received);

                _logger.LogInformation($"Paramerters: {received.AlertTimestamp}");

                DateTime.TryParse(received.AlertTimestamp, out DateTime dtAlertTimestamp);

                // Check if it's a temperature alert
                var temperatureRecord = Context.TemperatureRecords
                    .Where(x => x.Timestamp.AddMilliseconds(-x.Timestamp.Millisecond) == dtAlertTimestamp)
                    ?.Select(x => new { x.PersonName, x.PersonUID, x.ImageBase64 })
                    ?.FirstOrDefault();

                // It is a temperature alert
                if (temperatureRecord != null)
                {
                    // Find employee details
                    var PersonImage = Context.Employees
                        .Where(x => x.UID == temperatureRecord.PersonUID)
                        ?.Select(x => x.ImageBase64)
                        ?.FirstOrDefault();

                    var queriedPerson = new
                    {
                        temperatureRecord.PersonName,
                        PersonImage = PersonImage ?? temperatureRecord.ImageBase64,
                        PersonAlertImage = temperatureRecord.ImageBase64,
                        IsVisitor = PersonImage == null ? true : false
                    };

                    return new JsonResult(new
                    {
                        respcode = ResponseCodes.Successful,
                        description = ResponseCodes.Successful.DisplayName(),
                        queriedPerson
                    });
                }
                else
                {
                    // Check if it's a mask alert
                    var maskRecord = Context.MaskRecords
                        .Where(x => x.Timestamp.AddMilliseconds(-x.Timestamp.Millisecond) == dtAlertTimestamp)
                        ?.Select(x => new { x.PersonName, x.PersonUID, x.ImageBase64 })
                        ?.FirstOrDefault();

                    // Find employee details
                    var PersonImage = Context.Employees
                        .Where(x => x.UID == maskRecord.PersonUID)
                        ?.Select(x => x.ImageBase64)
                        ?.FirstOrDefault();

                    var queriedPerson = new
                    {
                        maskRecord.PersonName,
                        PersonImage = PersonImage ?? maskRecord.ImageBase64,
                        PersonAlertImage = maskRecord.ImageBase64,
                        IsVisitor = PersonImage == null ? true : false
                    };

                    return new JsonResult(new
                    {
                        respcode = ResponseCodes.Successful,
                        description = ResponseCodes.Successful.DisplayName(),
                        queriedPerson
                    });
                }
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
        /// GetPersonRecord API. Returns the person trace records using and alert timestamp.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /c19server/getpersonrecord
        ///     {
        ///         "alertTimestamp":"2020-07-28 13:04:22",
        ///         "startTimestamp":"2020-07-23 00:00:00"
        ///         }
        ///      
        /// Sample response:
        /// 
        ///     {
        ///         "respcode": 1200,
        ///         "description": "Successful",
        ///         "personReords": [
        ///             {
        ///                 "visitor": false,
        ///                 "person": "Abhishek",
        ///                 "location": "Floor 3 Reception Gate",
        ///                 "temperature": "36.7",
        ///                 "mask": false,
        ///                 "timestamp": "2020-07-28 13:04:22",
        ///                 "image": "data:image/jpeg;base64,/9j/4AAQS...VH/2Q=="
        ///             },
        ///             {
        ///                 ...
        ///             },
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
        [Route("getpersonrecord")]
        public ActionResult GetPersonRecord([FromBody]JObject jparams)
        {
            try
            {
                _logger.LogInformation("GetPersonRecord() called from: " + HttpContext.Connection.RemoteIpAddress.ToString());

                var received = new { AlertTimestamp = string.Empty, StartTimestamp = string.Empty };

                received = JsonConvert.DeserializeAnonymousType(jparams.ToString(Formatting.None), received);

                _logger.LogInformation($"Paramerters: {received.AlertTimestamp}, {received.StartTimestamp}");

                DateTime.TryParse(received.AlertTimestamp, out DateTime dtAlertTimestamp);
                DateTime.TryParse(received.StartTimestamp, out DateTime dtStartTimestamp);

                // Check if it's a temperature alert
                var temperatureAlertRecord = Context.TemperatureRecords
                    .Where(x => x.Timestamp.AddMilliseconds(-x.Timestamp.Millisecond) == dtAlertTimestamp)
                    ?.Select(x => new
                    {
                        x.PersonUID,
                        x.Mobile,
                        x.DeviceId
                    })?.FirstOrDefault();

                // It is a temperature alert
                if (temperatureAlertRecord != null)
                {
                    // if visitor, search using mobile. For employee, mobile can change.
                    if(temperatureAlertRecord.PersonUID==ConfigReader.VisitorUID)
                    {
                        var temperatureRecords = (from a in Context.TemperatureRecords
                                                  where a.Timestamp >= dtStartTimestamp && a.Mobile == temperatureAlertRecord.Mobile
                                                  select new
                                                  {
                                                      a.PersonUID,
                                                      a.PersonName,
                                                      Location = a.Device.Gate.AdditionalDetails,
                                                      Temperature = a.Temperature.ToString("#.#"),
                                                      Timestamp = a.Timestamp.ToString(),
                                                      a.ImagePath,
                                                      a.ImageBase64,
                                                  }).Distinct();

                        var maskRecords = (from a in Context.MaskRecords
                                           where a.Timestamp >= dtStartTimestamp && a.Mobile == temperatureAlertRecord.Mobile
                                           select new
                                           {
                                               a.PersonUID,
                                               a.PersonName,
                                               Location = a.Device.Gate.AdditionalDetails,
                                               Mask = false,
                                               Timestamp = a.Timestamp.ToString(),
                                               a.ImagePath,
                                               a.ImageBase64,
                                           }).Distinct();

                        var leftOuterJoin = from a in temperatureRecords
                                            join b in maskRecords on a.Timestamp equals b.Timestamp into bb
                                            from c in bb.DefaultIfEmpty()
                                            select new
                                            {
                                                Visitor = true,
                                                Person = a.PersonName,
                                                a.Location,
                                                a.Temperature,
                                                Mask = c != null ? false : true,   // false means no mask
                                                Timestamp = a.Timestamp.Remove(a.Timestamp.Length - 8),
                                                Image = !string.IsNullOrEmpty(a.ImageBase64) ? a.ImageBase64 : a.ImagePath.ConvertImageUrlToBase64().Result,
                                            };

                        var rightOuterJoin = from a in maskRecords
                                             join b in temperatureRecords on a.Timestamp equals b.Timestamp into bb
                                             from c in bb.DefaultIfEmpty()
                                             select new
                                             {
                                                 Visitor = true,
                                                 Person = a.PersonName,
                                                 a.Location,
                                                 Temperature = c != null ? c.Temperature.ToString() : string.Empty,
                                                 a.Mask,
                                                 Timestamp = a.Timestamp.Remove(a.Timestamp.Length - 8),
                                                 Image = !string.IsNullOrEmpty(a.ImageBase64) ? a.ImageBase64 : a.ImagePath.ConvertImageUrlToBase64().Result,
                                             };

                        var personReords = from a in leftOuterJoin.Union(rightOuterJoin)
                                                  orderby a.Timestamp descending
                                                  select a;

                        return new JsonResult(new
                        {
                            respcode = ResponseCodes.Successful,
                            description = ResponseCodes.Successful.DisplayName(),
                            personReords
                        });
                    }
                    else
                    {
                        var temperatureRecords = (from a in Context.TemperatureRecords
                                                  where a.Timestamp >= dtStartTimestamp && a.PersonUID == temperatureAlertRecord.PersonUID
                                                  select new
                                                  {
                                                      a.PersonUID,
                                                      a.PersonName,
                                                      Location = a.Device.Gate.AdditionalDetails,
                                                      Temperature = a.Temperature.ToString("#.#"),
                                                      Timestamp = a.Timestamp.ToString(),
                                                      a.ImagePath,
                                                      a.ImageBase64,
                                                  }).Distinct();

                        var maskRecords = (from a in Context.MaskRecords
                                           where a.Timestamp >= dtStartTimestamp && a.PersonUID == temperatureAlertRecord.PersonUID
                                           select new
                                           {
                                               a.PersonUID,
                                               a.PersonName,
                                               Location = a.Device.Gate.AdditionalDetails,
                                               Mask = false,
                                               Timestamp = a.Timestamp.ToString(),
                                               a.ImagePath,
                                               a.ImageBase64,
                                           }).Distinct();

                        var leftOuterJoin = from a in temperatureRecords
                                            join b in maskRecords on a.Timestamp equals b.Timestamp into bb
                                            from c in bb.DefaultIfEmpty()
                                            select new
                                            {
                                                Visitor = false,
                                                Person = a.PersonName,
                                                a.Location,
                                                a.Temperature,
                                                Mask = c != null ? false : true,   // false means no mask
                                                Timestamp = a.Timestamp.Remove(a.Timestamp.Length - 8),
                                                Image = !string.IsNullOrEmpty(a.ImageBase64) ? a.ImageBase64 : a.ImagePath.ConvertImageUrlToBase64().Result,
                                            };

                        var rightOuterJoin = from a in maskRecords
                                             join b in temperatureRecords on a.Timestamp equals b.Timestamp into bb
                                             from c in bb.DefaultIfEmpty()
                                             select new
                                             {
                                                 Visitor = true,
                                                 Person = a.PersonName,
                                                 a.Location,
                                                 Temperature = c != null ? c.Temperature.ToString() : string.Empty,
                                                 a.Mask,
                                                 Timestamp = a.Timestamp.Remove(a.Timestamp.Length - 8),
                                                 Image = !string.IsNullOrEmpty(a.ImageBase64) ? a.ImageBase64 : a.ImagePath.ConvertImageUrlToBase64().Result,
                                             };

                        var personReords = from a in leftOuterJoin.Union(rightOuterJoin)
                                           orderby a.Timestamp descending
                                           select a;

                        return new JsonResult(new
                        {
                            respcode = ResponseCodes.Successful,
                            description = ResponseCodes.Successful.DisplayName(),
                            personReords
                        });
                    }
                }
                else
                {
                    // If not temperature, then mask
                    // Check if it's a temperature alert
                    var maskAlertRecord = Context.MaskRecords
                        .Where(x => x.Timestamp.AddMilliseconds(-x.Timestamp.Millisecond) == dtAlertTimestamp)
                        ?.Select(x => new
                        {
                            x.PersonUID,
                            x.Mobile,
                            x.DeviceId
                        })?.FirstOrDefault();
                    // if visitor, search using mobile. For employee, mobile can change.
                    if (maskAlertRecord.PersonUID == ConfigReader.VisitorUID)
                    {
                        var temperatureRecords = (from a in Context.TemperatureRecords
                                                  where a.Timestamp >= dtStartTimestamp && a.Mobile == maskAlertRecord.Mobile
                                                  select new
                                                  {
                                                      a.PersonUID,
                                                      a.PersonName,
                                                      Location = a.Device.Gate.AdditionalDetails,
                                                      Temperature = a.Temperature.ToString("#.#"),
                                                      Timestamp = a.Timestamp.ToString(),
                                                      a.ImagePath,
                                                      a.ImageBase64,
                                                  }).Distinct();

                        var maskRecords = (from a in Context.MaskRecords
                                           where a.Timestamp >= dtStartTimestamp && a.Mobile == maskAlertRecord.Mobile
                                           select new
                                           {
                                               a.PersonUID,
                                               a.PersonName,
                                               Location = a.Device.Gate.AdditionalDetails,
                                               Mask = false,
                                               Timestamp = a.Timestamp.ToString(),
                                               a.ImagePath,
                                               a.ImageBase64,
                                           }).Distinct();

                        var leftOuterJoin = from a in temperatureRecords
                                            join b in maskRecords on a.Timestamp equals b.Timestamp into bb
                                            from c in bb.DefaultIfEmpty()
                                            select new
                                            {
                                                Visitor = true,
                                                Person = a.PersonName,
                                                a.Location,
                                                a.Temperature,
                                                Mask = c != null ? false : true,   // false means no mask
                                                Timestamp = a.Timestamp.Remove(a.Timestamp.Length - 8),
                                                Image = !string.IsNullOrEmpty(a.ImageBase64) ? a.ImageBase64 : a.ImagePath.ConvertImageUrlToBase64().Result,
                                            };

                        var rightOuterJoin = from a in maskRecords
                                             join b in temperatureRecords on a.Timestamp equals b.Timestamp into bb
                                             from c in bb.DefaultIfEmpty()
                                             select new
                                             {
                                                 Visitor = true,
                                                 Person = a.PersonName,
                                                 a.Location,
                                                 Temperature = c != null ? c.Temperature.ToString() : string.Empty,
                                                 a.Mask,
                                                 Timestamp = a.Timestamp.Remove(a.Timestamp.Length - 8),
                                                 Image = !string.IsNullOrEmpty(a.ImageBase64) ? a.ImageBase64 : a.ImagePath.ConvertImageUrlToBase64().Result,
                                             };

                        var personReords = from a in leftOuterJoin.Union(rightOuterJoin)
                                           orderby a.Timestamp descending
                                           select a;

                        return new JsonResult(new
                        {
                            respcode = ResponseCodes.Successful,
                            description = ResponseCodes.Successful.DisplayName(),
                            personReords
                        });
                    }
                    else
                    {
                        var temperatureRecords = (from a in Context.TemperatureRecords
                                                  where a.Timestamp >= dtStartTimestamp && a.PersonUID == maskAlertRecord.PersonUID
                                                  select new
                                                  {
                                                      a.PersonUID,
                                                      a.PersonName,
                                                      Location = a.Device.Gate.AdditionalDetails,
                                                      Temperature = a.Temperature.ToString("#.#"),
                                                      Timestamp = a.Timestamp.ToString(),
                                                      a.ImagePath,
                                                      a.ImageBase64,
                                                  }).Distinct();

                        var maskRecords = (from a in Context.MaskRecords
                                           where a.Timestamp >= dtStartTimestamp && a.PersonUID == maskAlertRecord.PersonUID
                                           select new
                                           {
                                               a.PersonUID,
                                               a.PersonName,
                                               Location = a.Device.Gate.AdditionalDetails,
                                               Mask = false,
                                               Timestamp = a.Timestamp.ToString(),
                                               a.ImagePath,
                                               a.ImageBase64,
                                           }).Distinct();

                        var leftOuterJoin = from a in temperatureRecords
                                            join b in maskRecords on a.Timestamp equals b.Timestamp into bb
                                            from c in bb.DefaultIfEmpty()
                                            select new
                                            {
                                                Visitor = false,
                                                Person = a.PersonName,
                                                a.Location,
                                                a.Temperature,
                                                Mask = c != null ? false : true,   // false means no mask
                                                Timestamp = a.Timestamp.Remove(a.Timestamp.Length - 8),
                                                Image = !string.IsNullOrEmpty(a.ImageBase64) ? a.ImageBase64 : a.ImagePath.ConvertImageUrlToBase64().Result,
                                            };

                        var rightOuterJoin = from a in maskRecords
                                             join b in temperatureRecords on a.Timestamp equals b.Timestamp into bb
                                             from c in bb.DefaultIfEmpty()
                                             select new
                                             {
                                                 Visitor = true,
                                                 Person = a.PersonName,
                                                 a.Location,
                                                 Temperature = c != null ? c.Temperature.ToString() : string.Empty,
                                                 a.Mask,
                                                 Timestamp = a.Timestamp.Remove(a.Timestamp.Length - 8),
                                                 Image = !string.IsNullOrEmpty(a.ImageBase64) ? a.ImageBase64 : a.ImagePath.ConvertImageUrlToBase64().Result,
                                             };

                        var personReords = from a in leftOuterJoin.Union(rightOuterJoin)
                                           orderby a.Timestamp descending
                                           select a;

                        return new JsonResult(new
                        {
                            respcode = ResponseCodes.Successful,
                            description = ResponseCodes.Successful.DisplayName(),
                            personReords
                        });
                    }

                }

                throw new Exception("Undefined");
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

        [HttpPost]
        [Route("getpotentialview")]
        public ActionResult GetPotentialView([FromBody]JObject jparams)
        {
            try
            {
                _logger.LogInformation("GetPotentialView() called from: " + HttpContext.Connection.RemoteIpAddress.ToString());

                var received = new { AlertTimestamp = string.Empty, StartTimestamp = string.Empty, EndTimestamp = string.Empty };

                received = JsonConvert.DeserializeAnonymousType(jparams.ToString(Formatting.None), received);

                _logger.LogInformation($"Paramerters: {received.AlertTimestamp}, {received.StartTimestamp}, {received.EndTimestamp}");

                DateTime.TryParse(received.AlertTimestamp, out DateTime dtAlertTimestamp);
                DateTime.TryParse(received.StartTimestamp, out DateTime dtStartTimestamp);
                DateTime.TryParse(received.StartTimestamp, out DateTime dtEndTimestamp);


                
                throw new Exception("Undefined");
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
