using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Covid19TemperatureAPI.Entities.Data
{
    [Table("AlertMobileNumbers")]
    public class AlertMobileNumber
    {
        [Key]
        public string MobileNumber { get; set; }
        public string Name { get; set; }

        [ForeignKey("Site")]
        public int SiteId { get; set; }
        public virtual Site Site { get; set; }

    }
}
