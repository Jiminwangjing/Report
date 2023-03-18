using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class dB_049 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                schema: "dbo",
                table: "tbOrder",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "Reason",
                schema: "dbo",
                table: "tbOrder");
        }
    }
}
