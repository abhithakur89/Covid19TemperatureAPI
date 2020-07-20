using Covid19TemperatureAPI.Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19TemperatureAPI.Entities.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Site> Sites { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Gate> Gates { get; set; }
        public DbSet<Device> Devices { get; set; }
    }
}
