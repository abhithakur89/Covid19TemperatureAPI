using Covid19TemperatureAPI.Entities.Data;
using Covid19TemperatureAPI.Helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Covid19TemperatureAPI.SenseTime
{
    public class SensetimeImpl : ISensetime
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger<SensetimeImpl> _logger;
        private ApplicationDbContext Context { get; set; }

        private static string AuthToken = string.Empty;
        private const string AuthTokenType = "Basic";

        public SensetimeImpl(IConfiguration configuration, ApplicationDbContext applicationDbContext,
            ILogger<SensetimeImpl> logger)
        {
            Configuration = configuration;
            Context = applicationDbContext;
            _logger = logger;
        }

        public string Login()
        {
            try
            {
                _logger.LogInformation($"SensetimeImpl.Login() called.");

                var req = new
                {
                    username = Configuration["SensetimeUsername"],
                    password = Configuration["SensetimePassword"],
                    accountType = Configuration["SensetimeAccontType"]
                };

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(Configuration["SensetimeLoginUrl"]);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(req);

                    streamWriter.Write(json);
                }

                // expected result
                var resposeObj = new { data = string.Empty, success = false };

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseStr = streamReader.ReadToEnd();

                    resposeObj = JsonConvert.DeserializeAnonymousType(responseStr, resposeObj);
                }

                if (resposeObj.success)
                    AuthToken = resposeObj.data;
            }
            catch(Exception e)
            {
                _logger.LogInformation($"SensetimeImpl.Login() error: {e.Message}");
                AuthToken = string.Empty;
                throw e;
            }
            return AuthToken;
        }

        public string UploadBase64(string imageContent,string imageExtension)
        {
            string imageUri = string.Empty;

            try
            {

                int attempt = 0;
                do
                {
                    attempt++;

                    var req = new
                    {
                        imageName = $"{DateTime.Now.Ticks.ToString()}{(imageExtension.StartsWith(".") ? imageExtension : $".{imageExtension}")}",
                        imageContent = imageContent.StartsWith("data:image") ? imageContent.Split(",")[1] : imageContent
                    };

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(Configuration["SensetimeUploadImageBase64Url"]);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, $"{AuthTokenType} {AuthToken}");

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = JsonConvert.SerializeObject(req);

                        streamWriter.Write(json);
                    }

                    // expected result
                    var resposeObj = new { data = string.Empty, success = false };

                    HttpWebResponse httpResponse = null;
                    try
                    {
                        httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    }
                    catch (Exception e)
                    {
                        if (attempt < 2)
                            Login();
                        else
                            throw e;
                    }

                    if (httpResponse != null)
                    {
                        // If unauthorized get a new auth token
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var responseStr = streamReader.ReadToEnd();

                            resposeObj = JsonConvert.DeserializeAnonymousType(responseStr, resposeObj);
                        }

                        if (resposeObj.success)
                        {
                            imageUri = resposeObj.data;
                            break;
                        }
                    }
                } while (attempt <= 1);
            }
            catch(Exception e)
            {
                _logger.LogError($"SensetimeImpl.UploadBase64() error: {e.Message}");
                throw e;
            }
            return imageUri;
        }

        public string CreatePerson(string imageURI, string personName, string idNumber)
        {
            string personUID = string.Empty;

            try
            {

                int attempt = 0;
                do
                {
                    attempt++;

                    var req = new
                    {
                        imageURI,
                        cnName=personName,
                        idNumber,
                        operatePerson=Configuration["SensetimeOperatePersonConfig"]
                    };

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(Configuration["SensetimeCreatePersonUrl"]);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, $"{AuthTokenType} {AuthToken}");

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = JsonConvert.SerializeObject(req);

                        streamWriter.Write(json);
                    }

                    // expected result
                    var resposeObj = new { data = string.Empty, success = false };

                    HttpWebResponse httpResponse = null;
                    try
                    {
                        httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    }
                    catch (Exception e)
                    {
                        if (attempt < 2)
                            Login();
                        else
                            throw e;
                    }
                    if (httpResponse != null)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var responseStr = streamReader.ReadToEnd();

                            resposeObj = JsonConvert.DeserializeAnonymousType(responseStr, resposeObj);
                        }

                        if (resposeObj.success)
                        {
                            personUID = resposeObj.data;
                            break;
                        }
                    }
                } while (attempt <= 1);
            }
            catch (Exception e)
            {
                _logger.LogError($"SensetimeImpl.CreatePerson() error: {e.Message}");
                throw e;
            }
            return personUID;
        }

        public bool AddMemberToEmployeeGroup(string employeeUID)
        {
            bool success = false;

            try
            {

                int attempt = 0;
                do
                {
                    attempt++;

                    var groupId = ConfigReader.GetSensetimeEmployeeGroupId(Context, Configuration);

                    var req = new
                    {
                        groupId,
                        operatePerson = Configuration["SensetimeOperatePersonConfig"],
                        uidList = new[] { employeeUID }
                    };

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(Configuration["SensetimeAddGroupMemberUrl"]);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, $"{AuthTokenType} {AuthToken}");

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = JsonConvert.SerializeObject(req);

                        streamWriter.Write(json);
                    }

                    // expected result
                    var resposeObj = new { success = false };

                    HttpWebResponse httpResponse = null;
                    try
                    {
                        httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    }
                    catch (Exception e)
                    {
                        if (attempt < 2)
                            Login();
                        else
                            throw e;
                    }
                    if (httpResponse != null)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var responseStr = streamReader.ReadToEnd();

                            resposeObj = JsonConvert.DeserializeAnonymousType(responseStr, resposeObj);
                        }

                        success = resposeObj.success;
                        break;
                    }
                } while (attempt <= 1);
            }
            catch (Exception e)
            {
                _logger.LogError($"SensetimeImpl.CreatePerson() error: {e.Message}");
                throw e;
            }
            return success;
        }

    }
}
