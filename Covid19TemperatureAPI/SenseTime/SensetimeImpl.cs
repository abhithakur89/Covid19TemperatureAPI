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
        const string SensetimeLoginUrl = "https://www.cyrus-secure.com/GUNS/mgr/login";

        const string AccoutType = "2";

        private static string AuthToken = string.Empty;

        public string Login(string username, string password)
        {
            var req = new { username, password, AccoutType };

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(SensetimeLoginUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(req);

                streamWriter.Write(json);
            }

            // expected result
            var resposeObj = new { code = string.Empty, msg = string.Empty, data = string.Empty, success = false };

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var responseStr = streamReader.ReadToEnd();

                resposeObj = JsonConvert.DeserializeAnonymousType(responseStr, resposeObj);
            }

            if (resposeObj.success)
                AuthToken = resposeObj.data;

            return AuthToken;
        }
    }
}
