﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Covid19TemperatureAPI.Entities.Data
{
    [Table("Sites")]
    public class Site
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SiteId { get; set; }
        public string SiteName { get; set; }

        public string SiteDescription { get; set; }

        public virtual ICollection<Building> Buildings { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<AlertEmailAddress> EmailAddresses { get; set; }
        public virtual ICollection<AlertMobileNumber> AlertMobileNumbers { get; set; }

    }
}
