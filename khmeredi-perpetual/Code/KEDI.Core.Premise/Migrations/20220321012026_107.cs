using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _107 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTablePriceList",
                schema: "dbo",
                table: "tbTable",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PriceListID",
                schema: "dbo",
                table: "tbTable",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTablePriceList",
                schema: "dbo",
                table: "tbTable");

            migrationBuilder.DropColumn(
                name: "PriceListID",
                schema: "dbo",
                table: "tbTable");
        }
    }
}
