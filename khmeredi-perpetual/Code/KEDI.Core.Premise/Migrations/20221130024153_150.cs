using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _150 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PurCopyType",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PurCopyType",
                schema: "dbo",
                table: "tbPurchaseOrderDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PurCopyType",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Split",
                schema: "dbo",
                table: "tbPrinterName",
                nullable: false,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<int>(
                name: "PurCopyType",
                schema: "dbo",
                table: "tbGoodsReciptPODatail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PurCopyType",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PurCopyType",
                table: "tbPurchaseCreditMemoDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PurCopyType",
                table: "PurchaseAPReserveDetail",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurCopyType",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail");

            migrationBuilder.DropColumn(
                name: "PurCopyType",
                schema: "dbo",
                table: "tbPurchaseOrderDetail");

            migrationBuilder.DropColumn(
                name: "PurCopyType",
                schema: "dbo",
                table: "tbPurchaseAPDetail");

            migrationBuilder.DropColumn(
                name: "PurCopyType",
                schema: "dbo",
                table: "tbGoodsReciptPODatail");

            migrationBuilder.DropColumn(
                name: "PurCopyType",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail");

            migrationBuilder.DropColumn(
                name: "PurCopyType",
                table: "tbPurchaseCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "PurCopyType",
                table: "PurchaseAPReserveDetail");

            migrationBuilder.AlterColumn<bool>(
                name: "Split",
                schema: "dbo",
                table: "tbPrinterName",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
