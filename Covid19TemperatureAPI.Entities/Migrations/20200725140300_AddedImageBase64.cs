using Microsoft.EntityFrameworkCore.Migrations;

namespace Covid19TemperatureAPI.Entities.Migrations
{
    public partial class AddedImageBase64 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageBase64",
                table: "TemperatureRecords",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageBase64",
                table: "MaskRecords",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageBase64",
                table: "TemperatureRecords");

            migrationBuilder.DropColumn(
                name: "ImageBase64",
                table: "MaskRecords");
        }
    }
}
