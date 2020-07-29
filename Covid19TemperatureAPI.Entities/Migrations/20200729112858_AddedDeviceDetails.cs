using Microsoft.EntityFrameworkCore.Migrations;

namespace Covid19TemperatureAPI.Entities.Migrations
{
    public partial class AddedDeviceDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeviceProductTypeUuid",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceTypeUuid",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceUUID",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperatePerson",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UpdatedThreshold",
                table: "Devices",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceProductTypeUuid",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "DeviceTypeUuid",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "DeviceUUID",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "OperatePerson",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "UpdatedThreshold",
                table: "Devices");
        }
    }
}
