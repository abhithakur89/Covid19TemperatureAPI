using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Covid19TemperatureAPI.Entities.Migrations
{
    public partial class TemperatureRecordAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TemperatureRecords",
                columns: table => new
                {
                    TemperatureRecordId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PersonUID = table.Column<string>(nullable: true),
                    PersonName = table.Column<string>(nullable: true),
                    DeviceId = table.Column<string>(nullable: true),
                    Temperature = table.Column<decimal>(type: "decimal(5,3)", nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    ImagePath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemperatureRecords", x => x.TemperatureRecordId);
                    table.ForeignKey(
                        name: "FK_TemperatureRecords_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "DeviceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemperatureRecords_DeviceId",
                table: "TemperatureRecords",
                column: "DeviceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemperatureRecords");
        }
    }
}
