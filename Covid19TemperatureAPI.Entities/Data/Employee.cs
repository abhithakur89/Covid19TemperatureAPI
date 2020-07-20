using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Covid19TemperatureAPI.Entities.Data
{
    [Table("Employees")]
    public class Employee
    {
        [Key]
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Mobile { get; set; }

        [ForeignKey("SiteId")]
        public int SiteId { get; set; }
        public virtual Site Site { get; set; }

        [ForeignKey("DepartmentId")]
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public string ImageBase64 { get; set; }

        public string Role { get; set; }

        public string UID { get; set; }
    }
}
