using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Covid19TemperatureAPI.Entities.Migrations
{
    public partial class MaskRecordsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaskRecords",
                columns: table => new
                {
                    MaskRecordId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PersonUID = table.Column<string>(nullable: true),
                    PersonName = table.Column<string>(nullable: true),
                    DeviceId = table.Column<string>(nullable: true),
                    MaskValue = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    ImagePath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaskRecords", x => x.MaskRecordId);
                    table.ForeignKey(
                        name: "FK_MaskRecords_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "DeviceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaskRecords_DeviceId",
                table: "MaskRecords",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_MaskRecords_Timestamp",
                table: "MaskRecords",
                column: "Timestamp");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaskRecords");
        }
    }
}
