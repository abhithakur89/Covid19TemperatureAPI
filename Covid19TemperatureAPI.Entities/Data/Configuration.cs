using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Covid19TemperatureAPI.Entities.Data
{
    [Table("Configurations")]
    public class Configuration
    {
        [Key]
        public string ConfigKey { get; set; }
        
        [Required]
        public string ConfigValue { get; set; }
    }
}
