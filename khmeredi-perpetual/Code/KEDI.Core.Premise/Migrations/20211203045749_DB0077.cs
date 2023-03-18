using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class DB0077 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbWarehouseDetail_tbWarhouse_WarehouseID",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropIndex(
                name: "IX_tbWarehouseDetail_WarehouseID",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "Available",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "Committed",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "Factor",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "InStockID",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "IsOut",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "Ordered",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "OutStockID",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "PurID",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "SaleID",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "Available",
                table: "StockOut");

            migrationBuilder.DropColumn(
                name: "Committed",
                table: "StockOut");

            migrationBuilder.DropColumn(
                name: "Factor",
                table: "StockOut");

            migrationBuilder.DropColumn(
                name: "InStockID",
                table: "StockOut");

            migrationBuilder.DropColumn(
                name: "Ordered",
                table: "StockOut");

            migrationBuilder.RenameColumn(
                name: "WareDetialID",
                schema: "dbo",
                table: "tbWarehouseDetail",
                newName: "InStockFrom");

            migrationBuilder.RenameColumn(
                name: "WareDetialID",
                table: "StockOut",
                newName: "OutStockFrom");

            migrationBuilder.RenameColumn(
                name: "OutStockID",
                table: "StockOut",
                newName: "FromWareDetialID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InStockFrom",
                schema: "dbo",
                table: "tbWarehouseDetail",
                newName: "WareDetialID");

            migrationBuilder.RenameColumn(
                name: "OutStockFrom",
                table: "StockOut",
                newName: "WareDetialID");

            migrationBuilder.RenameColumn(
                name: "FromWareDetialID",
                table: "StockOut",
                newName: "OutStockID");

            migrationBuilder.AddColumn<double>(
                name: "Available",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Committed",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Factor",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "InStockID",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsOut",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Ordered",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "OutStockID",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PurID",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaleID",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Available",
                table: "StockOut",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Committed",
                table: "StockOut",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Factor",
                table: "StockOut",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "InStockID",
                table: "StockOut",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Ordered",
                table: "StockOut",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_tbWarehouseDetail_WarehouseID",
                schema: "dbo",
                table: "tbWarehouseDetail",
                column: "WarehouseID");

            migrationBuilder.AddForeignKey(
                name: "FK_tbWarehouseDetail_tbWarhouse_WarehouseID",
                schema: "dbo",
                table: "tbWarehouseDetail",
                column: "WarehouseID",
                principalSchema: "dbo",
                principalTable: "tbWarhouse",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
