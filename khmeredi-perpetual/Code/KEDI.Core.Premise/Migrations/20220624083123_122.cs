using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _122 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Point",
                schema: "dbo",
                table: "tbOrder",
                newName: "ReceivedPoint");

            migrationBuilder.RenameColumn(
                name: "OutStandingPoint",
                schema: "dbo",
                table: "tbOrder",
                newName: "CumulativePoint");

            migrationBuilder.AddColumn<double>(
                name: "CumulativePoint",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ReceivedPoint",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CumulativePoint",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "ReceivedPoint",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.RenameColumn(
                name: "ReceivedPoint",
                schema: "dbo",
                table: "tbOrder",
                newName: "Point");

            migrationBuilder.RenameColumn(
                name: "CumulativePoint",
                schema: "dbo",
                table: "tbOrder",
                newName: "OutStandingPoint");
        }
    }
}
