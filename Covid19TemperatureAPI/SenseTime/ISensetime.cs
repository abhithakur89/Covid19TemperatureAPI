using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19TemperatureAPI.SenseTime
{
    public interface ISensetime
    {
        //string Login();
        string UploadBase64(string imageContent, string imageExtension);
        string CreatePerson(string imageURI, string personName, string idNumber);
        bool AddMemberToEmployeeGroup(string employeeUID);
    }
}
