using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _004 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAddon",
                schema: "dbo",
                table: "ServiceItemSales");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAddon",
                schema: "dbo",
                table: "ServiceItemSales",
                nullable: false,
                defaultValue: false);
        }
    }
}
