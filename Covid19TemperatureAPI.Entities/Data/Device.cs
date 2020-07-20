using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Covid19TemperatureAPI.Entities.Data
{
    [Table("Devices")]
    public class Device
    {
        [Key]
        public string DeviceId { get; set; }

        [ForeignKey("DeviceTypeId")]
        public int DeviceTypeId { get; set; }
        public virtual DeviceType DeviceType { get; set; }

    }
}
