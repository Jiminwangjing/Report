using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _156 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LineID",
                schema: "dbo",
                table: "tbSaleOrderDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaleCopyType",
                schema: "dbo",
                table: "tbSaleOrderDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "AIAJ",
                schema: "dbo",
                table: "tbItemMasterData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Portraite",
                schema: "dbo",
                table: "tbGeneralSetting",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "tbSaleQuoteDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaleCopyType",
                table: "tbSaleQuoteDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BaseOnID",
                table: "tbSaleQuote",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "tbSaleDeliveryDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaleCopyType",
                table: "tbSaleDeliveryDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "tbSaleCreditMemoDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaleCopyType",
                table: "tbSaleCreditMemoDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "tbSaleARDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaleCopyType",
                table: "tbSaleARDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "SaleAREditeDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaleCopyType",
                table: "SaleAREditeDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "SaleAREditeDetailHistory",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaleCopyType",
                table: "SaleAREditeDetailHistory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "ReturnDeliveryDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaleCopyType",
                table: "ReturnDeliveryDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "DraftReserveInvoiceEditableDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaleCopyType",
                table: "DraftReserveInvoiceEditableDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "DraftReserveInvoiceDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaleCopyType",
                table: "DraftReserveInvoiceDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "DraftDeliveryDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaleCopyType",
                table: "DraftDeliveryDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "DraftARDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaleCopyType",
                table: "DraftARDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "ARReserveInvoiceEditableDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaleCopyType",
                table: "ARReserveInvoiceEditableDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "ARReserveInvoiceDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaleCopyType",
                table: "ARReserveInvoiceDetail",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LineID",
                schema: "dbo",
                table: "tbSaleOrderDetail");

            migrationBuilder.DropColumn(
                name: "SaleCopyType",
                schema: "dbo",
                table: "tbSaleOrderDetail");

            migrationBuilder.DropColumn(
                name: "AIAJ",
                schema: "dbo",
                table: "tbItemMasterData");

            migrationBuilder.DropColumn(
                name: "Portraite",
                schema: "dbo",
                table: "tbGeneralSetting");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "tbSaleQuoteDetail");

            migrationBuilder.DropColumn(
                name: "SaleCopyType",
                table: "tbSaleQuoteDetail");

            migrationBuilder.DropColumn(
                name: "BaseOnID",
                table: "tbSaleQuote");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "tbSaleDeliveryDetail");

            migrationBuilder.DropColumn(
                name: "SaleCopyType",
                table: "tbSaleDeliveryDetail");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "tbSaleCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "SaleCopyType",
                table: "tbSaleCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "tbSaleARDetail");

            migrationBuilder.DropColumn(
                name: "SaleCopyType",
                table: "tbSaleARDetail");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "SaleAREditeDetails");

            migrationBuilder.DropColumn(
                name: "SaleCopyType",
                table: "SaleAREditeDetails");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "SaleAREditeDetailHistory");

            migrationBuilder.DropColumn(
                name: "SaleCopyType",
                table: "SaleAREditeDetailHistory");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "ReturnDeliveryDetail");

            migrationBuilder.DropColumn(
                name: "SaleCopyType",
                table: "ReturnDeliveryDetail");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "DraftReserveInvoiceEditableDetails");

            migrationBuilder.DropColumn(
                name: "SaleCopyType",
                table: "DraftReserveInvoiceEditableDetails");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "DraftReserveInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "SaleCopyType",
                table: "DraftReserveInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "DraftDeliveryDetails");

            migrationBuilder.DropColumn(
                name: "SaleCopyType",
                table: "DraftDeliveryDetails");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "DraftARDetail");

            migrationBuilder.DropColumn(
                name: "SaleCopyType",
                table: "DraftARDetail");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "ARReserveInvoiceEditableDetails");

            migrationBuilder.DropColumn(
                name: "SaleCopyType",
                table: "ARReserveInvoiceEditableDetails");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "ARReserveInvoiceDetail");

            migrationBuilder.DropColumn(
                name: "SaleCopyType",
                table: "ARReserveInvoiceDetail");
        }
    }
}
