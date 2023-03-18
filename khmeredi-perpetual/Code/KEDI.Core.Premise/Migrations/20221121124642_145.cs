using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _145 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Brand",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Condition",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Power",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Year",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "TmpSerialNumberSelectedDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "TmpSerialNumberSelectedDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Condition",
                table: "TmpSerialNumberSelectedDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Power",
                table: "TmpSerialNumberSelectedDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "TmpSerialNumberSelectedDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "TmpSerialNumberSelectedDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "StockOut",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "StockOut",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Condition",
                table: "StockOut",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Power",
                table: "StockOut",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "StockOut",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "StockOut",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "Color",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "Condition",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "Power",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "Year",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "Brand",
                table: "TmpSerialNumberSelectedDetail");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "TmpSerialNumberSelectedDetail");

            migrationBuilder.DropColumn(
                name: "Condition",
                table: "TmpSerialNumberSelectedDetail");

            migrationBuilder.DropColumn(
                name: "Power",
                table: "TmpSerialNumberSelectedDetail");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "TmpSerialNumberSelectedDetail");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "TmpSerialNumberSelectedDetail");

            migrationBuilder.DropColumn(
                name: "Brand",
                table: "StockOut");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "StockOut");

            migrationBuilder.DropColumn(
                name: "Condition",
                table: "StockOut");

            migrationBuilder.DropColumn(
                name: "Power",
                table: "StockOut");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "StockOut");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "StockOut");
        }
    }
}
