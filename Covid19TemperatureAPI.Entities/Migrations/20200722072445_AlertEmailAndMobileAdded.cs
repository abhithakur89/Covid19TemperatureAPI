using Microsoft.EntityFrameworkCore.Migrations;

namespace Covid19TemperatureAPI.Entities.Migrations
{
    public partial class AlertEmailAndMobileAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlertEmailAddresses",
                columns: table => new
                {
                    EmailId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertEmailAddresses", x => x.EmailId);
                    table.ForeignKey(
                        name: "FK_AlertEmailAddresses_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "SiteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlertMobileNumbers",
                columns: table => new
                {
                    MobileNumber = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertMobileNumbers", x => x.MobileNumber);
                    table.ForeignKey(
                        name: "FK_AlertMobileNumbers_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "SiteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertEmailAddresses_SiteId",
                table: "AlertEmailAddresses",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertMobileNumbers_SiteId",
                table: "AlertMobileNumbers",
                column: "SiteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertEmailAddresses");

            migrationBuilder.DropTable(
                name: "AlertMobileNumbers");
        }
    }
}
