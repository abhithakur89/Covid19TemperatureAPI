﻿// <auto-generated />
using System;
using Covid19TemperatureAPI.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Covid19TemperatureAPI.Entities.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200722073357_AddedConfiguration")]
    partial class AddedConfiguration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Data.AlertEmailAddress", b =>
                {
                    b.Property<string>("EmailId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("SiteId");

                    b.HasKey("EmailId");

                    b.HasIndex("SiteId");

                    b.ToTable("AlertEmailAddresses");
                });

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Data.AlertMobileNumber", b =>
                {
                    b.Property<string>("MobileNumber")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("SiteId");

                    b.HasKey("MobileNumber");

                    b.HasIndex("SiteId");

                    b.ToTable("AlertMobileNumbers");
                });

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Data.Building", b =>
                {
                    b.Property<int>("BuildingId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BuildingDescription");

                    b.Property<string>("BuildingName");

                    b.Property<int>("SiteId");

                    b.HasKey("BuildingId");

                    b.HasIndex("SiteId");

                    b.ToTable("Buildings");
                });

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Data.Configuration", b =>
                {
                    b.Property<string>("ConfigKey")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConfigValue")
                        .IsRequired();

                    b.HasKey("ConfigKey");

                    b.ToTable("Configurations");
                });

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Data.Department", b =>
                {
                    b.Property<int>("DepartmentId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DepartmentCode");

                    b.Property<string>("DepartmentName");

                    b.HasKey("DepartmentId");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Data.Device", b =>
                {
                    b.Property<string>("DeviceId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DeviceDetails");

                    b.Property<int>("GateId");

                    b.HasKey("DeviceId");

                    b.HasIndex("GateId");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Data.Employee", b =>
                {
                    b.Property<string>("EmployeeId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DepartmentId");

                    b.Property<string>("EmployeeName");

                    b.Property<string>("ImageBase64");

                    b.Property<string>("Mobile");

                    b.Property<string>("Role");

                    b.Property<int>("SiteId");

                    b.Property<string>("UID");

                    b.HasKey("EmployeeId");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("SiteId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Data.Floor", b =>
                {
                    b.Property<int>("FloorId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AdditionalDetails");

                    b.Property<int>("BuildingId");

                    b.Property<string>("FloorDetails");

                    b.Property<string>("FloorNumber");

                    b.HasKey("FloorId");

                    b.HasIndex("BuildingId");

                    b.ToTable("Floors");
                });

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Data.Gate", b =>
                {
                    b.Property<int>("GateId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AdditionalDetails");

                    b.Property<int>("FloorId");

                    b.Property<string>("GateNumber");

                    b.HasKey("GateId");

                    b.HasIndex("FloorId");

                    b.ToTable("Gates");
                });

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Data.MaskRecord", b =>
                {
                    b.Property<int>("MaskRecordId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DeviceId");

                    b.Property<string>("ImagePath");

                    b.Property<int>("MaskValue");

                    b.Property<string>("PersonName");

                    b.Property<string>("PersonUID");

                    b.Property<DateTime>("Timestamp");

                    b.HasKey("MaskRecordId");

                    b.HasIndex("DeviceId");

                    b.ToTable("MaskRecords");
                });

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Data.Site", b =>
                {
                    b.Property<int>("SiteId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("SiteDescription");

                    b.Property<string>("SiteName");

                    b.HasKey("SiteId");

                    b.ToTable("Sites");
                });

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Data.TemperatureRecord", b =>
                {
                    b.Property<int>("TemperatureRecordId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DeviceId");

                    b.Property<string>("ImagePath");

                    b.Property<string>("PersonName");

                    b.Property<string>("PersonUID");

                    b.Property<decimal>("Temperature")
                        .HasColumnType("decimal(5,3)");

                    b.Property<DateTime>("Timestamp");

                    b.HasKey("TemperatureRecordId");

                    b.HasIndex("DeviceId");

                    b.ToTable("TemperatureRecords");
                });

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Data.AlertEmailAddress", b =>
                {
                    b.HasOne("Covid19TemperatureAPI.Entities.Data.Site", "Site")
                        .WithMany("EmailAddresses")
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Data.AlertMobileNumber", b =>
                {
                    b.HasOne("Covid19TemperatureAPI.Entities.Data.Site", "Site")
                        .WithMany("AlertMobileNumbers")
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Data.Building", b =>
                {
                    b.HasOne("Covid19TemperatureAPI.Entities.Data.Site", "Site")
                        .WithMany("Buildings")
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Data.Device", b =>
                {
                    b.HasOne("Covid19TemperatureAPI.Entities.Data.Gate", "Gate")
                        .WithMany("Devices")
                        .HasForeignKey("GateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Data.Employee", b =>
                {
                    b.HasOne("Covid19TemperatureAPI.Entities.Data.Department", "Department")
                        .WithMany("Employees")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Covid19TemperatureAPI.Entities.Data.Site", "Site")
                        .WithMany("Employees")
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Data.Floor", b =>
                {
                    b.HasOne("Covid19TemperatureAPI.Entities.Data.Building", "Building")
                        .WithMany("Floors")
                        .HasForeignKey("BuildingId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Data.Gate", b =>
                {
                    b.HasOne("Covid19TemperatureAPI.Entities.Data.Floor", "Floor")
                        .WithMany("Gates")
                        .HasForeignKey("FloorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Data.MaskRecord", b =>
                {
                    b.HasOne("Covid19TemperatureAPI.Entities.Data.Device", "Device")
                        .WithMany("MaskRecords")
                        .HasForeignKey("DeviceId");
                });

            modelBuilder.Entity("Covid19TemperatureAPI.Entities.Data.TemperatureRecord", b =>
                {
                    b.HasOne("Covid19TemperatureAPI.Entities.Data.Device", "Device")
                        .WithMany("TemperatureRecords")
                        .HasForeignKey("DeviceId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Covid19TemperatureAPI.Entities.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Covid19TemperatureAPI.Entities.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Covid19TemperatureAPI.Entities.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Covid19TemperatureAPI.Entities.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
