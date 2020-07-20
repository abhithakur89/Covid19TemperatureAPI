using Microsoft.EntityFrameworkCore.Migrations;

namespace Covid19TemperatureAPI.Entities.Migrations
{
    public partial class FloorAdded2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalDetails",
                table: "Floors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FloorNumber",
                table: "Floors",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalDetails",
                table: "Floors");

            migrationBuilder.DropColumn(
                name: "FloorNumber",
                table: "Floors");
        }
    }
}
