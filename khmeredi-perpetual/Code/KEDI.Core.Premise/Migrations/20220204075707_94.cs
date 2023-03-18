using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _94 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalNet",
                schema: "dbo",
                table: "tbVoidOrderDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "ChangeCurrenciesDisplay",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GrandTotalCurrenciesDisplay",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GrandTotalOtherCurrenciesDisplay",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherPaymentGrandTotal",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "PromoCodeDiscRate",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PromoCodeDiscValue",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "PromoCodeID",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "TotalNet",
                schema: "dbo",
                table: "tbReceiptDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "ChangeCurrenciesDisplay",
                schema: "dbo",
                table: "tbReceipt",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GrandTotalCurrenciesDisplay",
                schema: "dbo",
                table: "tbReceipt",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GrandTotalOtherCurrenciesDisplay",
                schema: "dbo",
                table: "tbReceipt",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OpenOtherPaymentGrandTotal",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherPaymentGrandTotal",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RemarkDiscount",
                schema: "dbo",
                table: "tbReceipt",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderCount",
                schema: "dbo",
                table: "tbPrinterName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Split",
                schema: "dbo",
                table: "tbPrinterName",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "TotalNet",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "ChangeCurrenciesDisplay",
                schema: "dbo",
                table: "tbOrder",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GrandTotalCurrenciesDisplay",
                schema: "dbo",
                table: "tbOrder",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GrandTotalOtherCurrenciesDisplay",
                schema: "dbo",
                table: "tbOrder",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherPaymentGrandTotal",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TypeItem",
                schema: "dbo",
                table: "tbInventoryAudit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrintBillName",
                schema: "dbo",
                table: "tbGeneralSetting",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrintLabelName",
                schema: "dbo",
                table: "tbGeneralSetting",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrintOrderCount",
                schema: "dbo",
                table: "tbGeneralSetting",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrintReceiptOption",
                schema: "dbo",
                table: "tbGeneralSetting",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PrinterOrder",
                schema: "dbo",
                table: "tbGeneralSetting",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QueueOption",
                schema: "dbo",
                table: "tbGeneralSetting",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "tbDisplayCurrency",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShowCurrency",
                schema: "dbo",
                table: "tbDisplayCurrency",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShowOtherCurrency",
                schema: "dbo",
                table: "tbDisplayCurrency",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "PLDisplayRate",
                schema: "dbo",
                table: "tbDisplayCurrency",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalNet",
                table: "VoidItemDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "ChangeCurrenciesDisplay",
                table: "VoidItem",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GrandTotalCurrenciesDisplay",
                table: "VoidItem",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GrandTotalOtherCurrenciesDisplay",
                table: "VoidItem",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherPaymentGrandTotal",
                table: "VoidItem",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                table: "VoidItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "PromoCodeDiscRate",
                table: "VoidItem",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PromoCodeDiscValue",
                table: "VoidItem",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "PromoCodeID",
                table: "VoidItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ChangeCurrenciesDisplay",
                table: "ReceiptMemo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GrandTotalCurrenciesDisplay",
                table: "ReceiptMemo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GrandTotalOtherCurrenciesDisplay",
                table: "ReceiptMemo",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherPaymentGrandTotal",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "PromoCodeDiscRate",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PromoCodeDiscValue",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "PromoCodeID",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "TotalNet",
                table: "ReceiptDetailMemoKvms",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "CardMemberTemplate",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Option = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardMemberTemplate", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardMemberTemplate");

            migrationBuilder.DropColumn(
                name: "TotalNet",
                schema: "dbo",
                table: "tbVoidOrderDetail");

            migrationBuilder.DropColumn(
                name: "ChangeCurrenciesDisplay",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "GrandTotalCurrenciesDisplay",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "GrandTotalOtherCurrenciesDisplay",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "OtherPaymentGrandTotal",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "PromoCodeDiscRate",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "PromoCodeDiscValue",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "PromoCodeID",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "TotalNet",
                schema: "dbo",
                table: "tbReceiptDetail");

            migrationBuilder.DropColumn(
                name: "ChangeCurrenciesDisplay",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "GrandTotalCurrenciesDisplay",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "GrandTotalOtherCurrenciesDisplay",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "OpenOtherPaymentGrandTotal",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "OtherPaymentGrandTotal",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "RemarkDiscount",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "OrderCount",
                schema: "dbo",
                table: "tbPrinterName");

            migrationBuilder.DropColumn(
                name: "Split",
                schema: "dbo",
                table: "tbPrinterName");

            migrationBuilder.DropColumn(
                name: "TotalNet",
                schema: "dbo",
                table: "tbOrderDetail");

            migrationBuilder.DropColumn(
                name: "ChangeCurrenciesDisplay",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "GrandTotalCurrenciesDisplay",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "GrandTotalOtherCurrenciesDisplay",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "OtherPaymentGrandTotal",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "TypeItem",
                schema: "dbo",
                table: "tbInventoryAudit");

            migrationBuilder.DropColumn(
                name: "PrintBillName",
                schema: "dbo",
                table: "tbGeneralSetting");

            migrationBuilder.DropColumn(
                name: "PrintLabelName",
                schema: "dbo",
                table: "tbGeneralSetting");

            migrationBuilder.DropColumn(
                name: "PrintOrderCount",
                schema: "dbo",
                table: "tbGeneralSetting");

            migrationBuilder.DropColumn(
                name: "PrintReceiptOption",
                schema: "dbo",
                table: "tbGeneralSetting");

            migrationBuilder.DropColumn(
                name: "PrinterOrder",
                schema: "dbo",
                table: "tbGeneralSetting");

            migrationBuilder.DropColumn(
                name: "QueueOption",
                schema: "dbo",
                table: "tbGeneralSetting");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "dbo",
                table: "tbDisplayCurrency");

            migrationBuilder.DropColumn(
                name: "IsShowCurrency",
                schema: "dbo",
                table: "tbDisplayCurrency");

            migrationBuilder.DropColumn(
                name: "IsShowOtherCurrency",
                schema: "dbo",
                table: "tbDisplayCurrency");

            migrationBuilder.DropColumn(
                name: "PLDisplayRate",
                schema: "dbo",
                table: "tbDisplayCurrency");

            migrationBuilder.DropColumn(
                name: "TotalNet",
                table: "VoidItemDetail");

            migrationBuilder.DropColumn(
                name: "ChangeCurrenciesDisplay",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "GrandTotalCurrenciesDisplay",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "GrandTotalOtherCurrenciesDisplay",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "OtherPaymentGrandTotal",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "PromoCodeDiscRate",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "PromoCodeDiscValue",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "PromoCodeID",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "ChangeCurrenciesDisplay",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "GrandTotalCurrenciesDisplay",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "GrandTotalOtherCurrenciesDisplay",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "OtherPaymentGrandTotal",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "PromoCodeDiscRate",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "PromoCodeDiscValue",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "PromoCodeID",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "TotalNet",
                table: "ReceiptDetailMemoKvms");
        }
    }
}
