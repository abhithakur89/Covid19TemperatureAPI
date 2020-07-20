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

        public string DeviceDetails { get; set; }

        [ForeignKey("Gate")]
        public int GateId { get; set; }
        public virtual Gate Gate { get; set; }
    }
}
