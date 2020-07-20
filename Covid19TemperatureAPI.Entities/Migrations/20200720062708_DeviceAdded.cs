using Microsoft.EntityFrameworkCore.Migrations;

namespace Covid19TemperatureAPI.Entities.Migrations
{
    public partial class DeviceAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    DeviceId = table.Column<string>(nullable: false),
                    DeviceDetails = table.Column<string>(nullable: true),
                    GateId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.DeviceId);
                    table.ForeignKey(
                        name: "FK_Devices_Gates_GateId",
                        column: x => x.GateId,
                        principalTable: "Gates",
                        principalColumn: "GateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_GateId",
                table: "Devices",
                column: "GateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Devices");
        }
    }
}
