using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _80 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                schema: "dbo",
                table: "tbVoidOrderDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                schema: "dbo",
                table: "tbReceiptDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsLimitOrder",
                schema: "dbo",
                table: "tbItemMasterData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "MaxOrderQty",
                schema: "dbo",
                table: "tbItemMasterData",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MinOrderQty",
                schema: "dbo",
                table: "tbItemMasterData",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "VoidItemDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "ReceiptDetailMemoKvms",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxRate",
                schema: "dbo",
                table: "tbVoidOrderDetail");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                schema: "dbo",
                table: "tbReceiptDetail");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                schema: "dbo",
                table: "tbOrderDetail");

            migrationBuilder.DropColumn(
                name: "IsLimitOrder",
                schema: "dbo",
                table: "tbItemMasterData");

            migrationBuilder.DropColumn(
                name: "MaxOrderQty",
                schema: "dbo",
                table: "tbItemMasterData");

            migrationBuilder.DropColumn(
                name: "MinOrderQty",
                schema: "dbo",
                table: "tbItemMasterData");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "VoidItemDetail");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "ReceiptDetailMemoKvms");
        }
    }
}
