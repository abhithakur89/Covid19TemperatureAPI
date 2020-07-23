using Microsoft.EntityFrameworkCore.Migrations;

namespace Covid19TemperatureAPI.Entities.Migrations
{
    public partial class AddedMobileForVisitors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IC",
                table: "TemperatureRecords",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "TemperatureRecords",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IC",
                table: "MaskRecords",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "MaskRecords",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IC",
                table: "TemperatureRecords");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "TemperatureRecords");

            migrationBuilder.DropColumn(
                name: "IC",
                table: "MaskRecords");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "MaskRecords");
        }
    }
}
