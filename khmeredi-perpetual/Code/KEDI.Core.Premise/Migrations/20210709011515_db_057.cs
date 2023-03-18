using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_057 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Staus",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.AddColumn<int>(
                name: "Staus",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0);
        }
    }
}
