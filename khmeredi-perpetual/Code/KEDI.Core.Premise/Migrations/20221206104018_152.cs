using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _152 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BaseOnID",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PurCopyType",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BaseOnID",
                table: "StockOut",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PurCopyType",
                table: "StockOut",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseOnID",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "PurCopyType",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "BaseOnID",
                table: "StockOut");

            migrationBuilder.DropColumn(
                name: "PurCopyType",
                table: "StockOut");
        }
    }
}
