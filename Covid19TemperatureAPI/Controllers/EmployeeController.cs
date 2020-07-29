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
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger<EmployeeController> _logger;
        private ApplicationDbContext Context { get; set; }
        private readonly ISensetime Sensetime;

        private enum ResponseCodes
        {
            [Display(Name = "Successful")]
            Successful = 1200,
            [Display(Name = "Error")]
            SystemError = 1201,
        }

        public EmployeeController(IConfiguration configuration, ApplicationDbContext applicationDbContext, 
            ILogger<EmployeeController> logger, ISensetime sensetime)
        {
            Configuration = configuration;
            Context = applicationDbContext;
            _logger = logger;
            Sensetime = sensetime;
        }

        /// <summary>
        /// AddNewEmployee API. Adds new employee.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /c19server/addnewemployee
        ///     {
        ///         "EmployeeId" : "E5456",
        ///         "EmployeeName" : "Bill Gates",
        ///         "Mobile" : "",
        ///         "SiteId" : "1",
        ///         "DepartmentId" : "1",
        ///         "ImageBase64" : "data:image/jpeg;base64,/9j/4...gICA=",
        ///         "ImageExtension" : "jpg",
        ///         "Role" : "Test Role"
        ///     }
        ///      
        /// Sample response:
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
        [Route("addnewemployee")]
        public ActionResult AddNewEmployee([FromBody]JObject jemployeedata)
        {
            try
            {
                _logger.LogInformation("AddNewEmployee() called from: " + HttpContext.Connection.RemoteIpAddress.ToString());

                var received = new
                {
                    EmployeeId = string.Empty,
                    EmployeeName = string.Empty,
                    Mobile = string.Empty,
                    SiteId = string.Empty,
                    DepartmentId = string.Empty,
                    ImageBase64 = string.Empty,
                    ImageExtension = string.Empty,
                    Role = string.Empty
                };

                received = JsonConvert.DeserializeAnonymousType(jemployeedata.ToString(Formatting.None), received);

                _logger.LogInformation($"Paramerters: {received.EmployeeId}, {received.EmployeeName}, {received.Mobile}, " +
                    $"{received.SiteId}, {received.DepartmentId}, {received.ImageExtension}, {received.Role}");

                if (!int.TryParse(received.SiteId, out int nSiteId)) throw new Exception("Invalid Site Id");
                if (!int.TryParse(received.DepartmentId, out int nDepartmentId)) throw new Exception("Invalid Department Id");

                var imageUri = Sensetime.UploadBase64(received.ImageBase64, received.ImageExtension);

                if (string.IsNullOrEmpty(imageUri)) throw new Exception("Could not upload the image.");

                var employeeUID = Sensetime.CreatePerson(imageUri, received.EmployeeName, received.EmployeeId);

                if (string.IsNullOrEmpty(employeeUID)) throw new Exception("Could not add employee to sensetime");

                bool res = Sensetime.AddMemberToEmployeeGroup(employeeUID);

                if (!res) throw new Exception("Could not add employee to group.");

                var employee = new Employee()
                {
                    EmployeeId = received.EmployeeId,
                    EmployeeName = received.EmployeeName,
                    Mobile = received.Mobile,
                    SiteId = nSiteId,
                    DepartmentId = nDepartmentId,
                    ImageBase64 = received.ImageBase64,
                    Role = received.Role,
                    UID = employeeUID
                };

                Context.Employees.Add(employee);

                Context.SaveChanges();

                return new JsonResult(new
                {
                    respcode = ResponseCodes.Successful,
                    description = ResponseCodes.Successful.DisplayName(),
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
        /// GetAllEmployees API. Get all employees in the system based on site id.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /c19server/getallemployees
        ///     {
        ///         "siteId" : "1"
        ///     }
        ///      
        /// Sample response:
        ///     {
        ///         "respcode": 1200,
        ///         "description": "Successful",
        ///         "employees": [
        ///             {
        ///                 "employeeId": "E0001",
        ///                 "departmentId": 1,
        ///                 "departmentCode": "DI",
        ///                 "departmentName": "Digital Integration",
        ///                 "employeeName": "Abhishek",
        ///                 "image": "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD.../9k=",
        ///                 "mobile": "98812345",
        ///                 "role": "Tester",
        ///                 "siteId": 1
        ///             },
        ///             {
        ///                 ...
        ///             },
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
        [Route("getallemployees")]
        public ActionResult GetAllEmployees([FromBody]JObject jsiteId)
        {
            try
            {
                _logger.LogInformation("GetAllEmployees() called from: " + HttpContext.Connection.RemoteIpAddress.ToString());

                var received = new { SiteId = string.Empty };

                received = JsonConvert.DeserializeAnonymousType(jsiteId.ToString(Formatting.None), received);

                _logger.LogInformation($"Paramerters: {received.SiteId}");

                if (!int.TryParse(received.SiteId, out int nSiteId)) throw new Exception("Invalid Site Id");

                var employees = Context.Employees
                    .Where(x => x.SiteId == nSiteId)
                    ?.Select(x => new
                    {
                        x.EmployeeId,
                        x.DepartmentId,
                        x.Department.DepartmentCode,
                        x.Department.DepartmentName,
                        x.EmployeeName,
                        Image = x.ImageBase64,
                        x.Mobile,
                        x.Role,
                        x.SiteId
                    });

                return new JsonResult(new
                {
                    respcode = ResponseCodes.Successful,
                    description = ResponseCodes.Successful.DisplayName(),
                    employees
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
        /// EditEmployee API. Edit employees in the system based on employeeId.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /c19server/editemployee
        ///     {
        ///         "EmployeeId" : "12345",
        ///         "EmployeeName" : "E. Musk",
        ///         "Mobile" : "12345",
        ///         "SiteId" : "1",
        ///         "DepartmentId" : "1",
        ///         "Image" : "data:image/jpeg;base64,/9j/4AA...P/2Q==",
        ///         "Role" : "Test role"
        ///     }
        ///      
        /// Sample response:
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
        [Route("editemployee")]
        public ActionResult EditEmployee([FromBody]JObject jemployeeparams)
        {
            try
            {
                _logger.LogInformation("EditEmployee() called from: " + HttpContext.Connection.RemoteIpAddress.ToString());

                var received = new
                {
                    EmployeeId = string.Empty,
                    EmployeeName = string.Empty,
                    Mobile=string.Empty,
                    SiteId=string.Empty,
                    DepartmentId=string.Empty,
                    Image=string.Empty,
                    Role=string.Empty
                };

                received = JsonConvert.DeserializeAnonymousType(jemployeeparams.ToString(Formatting.None), received);

                _logger.LogInformation($"Paramerters: {received.EmployeeId}, {received.EmployeeName}, {received.Mobile}, " +
                    $"{received.SiteId}, {received.DepartmentId}, {received.Image}, {received.Role}");

                var employee = Context.Employees
                    .Where(x => x.EmployeeId == received.EmployeeId)
                    ?.Select(x => x)?.FirstOrDefault();

                employee.EmployeeName = received.EmployeeName;
                employee.Mobile = received.Mobile;

                int.TryParse(received.SiteId, out int nSiteId);
                employee.SiteId = nSiteId;

                int.TryParse(received.DepartmentId, out int nDepartmentId);
                employee.DepartmentId = nDepartmentId;

                employee.ImageBase64 = received.Image;
                employee.Role = received.Role;

                Context.Employees.Update(employee);
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
