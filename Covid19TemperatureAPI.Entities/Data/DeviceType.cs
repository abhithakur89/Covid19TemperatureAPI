using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Covid19TemperatureAPI.Entities.Data
{
    [Table("DeviceTypes")]
    public class DeviceType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeviceTypeId { get; set; }
        public string DeviceTypeDescription { get; set; }
    }
}
