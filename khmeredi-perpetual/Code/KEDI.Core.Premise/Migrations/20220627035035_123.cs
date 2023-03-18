using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _123 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CumulativePoint",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "ReceivedPoint",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "OutStanding",
                table: "Redeem");

            migrationBuilder.AddColumn<int>(
                name: "OpenShiftID",
                table: "Redeem",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpenShiftID",
                table: "Redeem");

            migrationBuilder.AddColumn<double>(
                name: "CumulativePoint",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ReceivedPoint",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "OutStanding",
                table: "Redeem",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
