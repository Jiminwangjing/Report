using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _66 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LinePosition",
                schema: "dbo",
                table: "tbReceiptDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LinePosition",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinePosition",
                schema: "dbo",
                table: "tbReceiptDetail");

            migrationBuilder.DropColumn(
                name: "LinePosition",
                schema: "dbo",
                table: "tbOrderDetail");
        }
    }
}
