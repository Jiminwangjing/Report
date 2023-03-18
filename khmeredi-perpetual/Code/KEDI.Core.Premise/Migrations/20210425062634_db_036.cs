using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_036 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cash",
                schema: "dbo",
                table: "tbIncomingPaymentDetail");

            migrationBuilder.DropColumn(
                name: "Cash",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer");

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                schema: "dbo",
                table: "tbSaleOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DocTypeID",
                schema: "dbo",
                table: "tbSaleOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                schema: "dbo",
                table: "tbSaleOrder",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeriesDID",
                schema: "dbo",
                table: "tbSaleOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesID",
                schema: "dbo",
                table: "tbSaleOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "CheckPay",
                schema: "dbo",
                table: "tbIncomingPaymentDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyName",
                schema: "dbo",
                table: "tbIncomingPaymentDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocNo",
                schema: "dbo",
                table: "tbIncomingPaymentDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DocTypeID",
                schema: "dbo",
                table: "tbIncomingPaymentDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                schema: "dbo",
                table: "tbIncomingPaymentDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemInvoice",
                schema: "dbo",
                table: "tbIncomingPaymentDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DocTypeID",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemInvoice",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeriesDID",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesID",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                schema: "dbo",
                table: "tbIncomingPayment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DocTypeID",
                schema: "dbo",
                table: "tbIncomingPayment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                schema: "dbo",
                table: "tbIncomingPayment",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMeanID",
                schema: "dbo",
                table: "tbIncomingPayment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesDID",
                schema: "dbo",
                table: "tbIncomingPayment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesID",
                schema: "dbo",
                table: "tbIncomingPayment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "dbo",
                table: "tbIncomingPayment",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                table: "tbSaleQuote",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DocTypeID",
                table: "tbSaleQuote",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "tbSaleQuote",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeriesDID",
                table: "tbSaleQuote",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesID",
                table: "tbSaleQuote",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                table: "tbSaleDelivery",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DocTypeID",
                table: "tbSaleDelivery",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "tbSaleDelivery",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeriesDID",
                table: "tbSaleDelivery",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesID",
                table: "tbSaleDelivery",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                table: "tbSaleCreditMemo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DocTypeID",
                table: "tbSaleCreditMemo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "tbSaleCreditMemo",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeriesDID",
                table: "tbSaleCreditMemo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesID",
                table: "tbSaleCreditMemo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                table: "tbSaleAR",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DocTypeID",
                table: "tbSaleAR",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "tbSaleAR",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeriesDID",
                table: "tbSaleAR",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesID",
                table: "tbSaleAR",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyID",
                schema: "dbo",
                table: "tbSaleOrder");

            migrationBuilder.DropColumn(
                name: "DocTypeID",
                schema: "dbo",
                table: "tbSaleOrder");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                schema: "dbo",
                table: "tbSaleOrder");

            migrationBuilder.DropColumn(
                name: "SeriesDID",
                schema: "dbo",
                table: "tbSaleOrder");

            migrationBuilder.DropColumn(
                name: "SeriesID",
                schema: "dbo",
                table: "tbSaleOrder");

            migrationBuilder.DropColumn(
                name: "CheckPay",
                schema: "dbo",
                table: "tbIncomingPaymentDetail");

            migrationBuilder.DropColumn(
                name: "CurrencyName",
                schema: "dbo",
                table: "tbIncomingPaymentDetail");

            migrationBuilder.DropColumn(
                name: "DocNo",
                schema: "dbo",
                table: "tbIncomingPaymentDetail");

            migrationBuilder.DropColumn(
                name: "DocTypeID",
                schema: "dbo",
                table: "tbIncomingPaymentDetail");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                schema: "dbo",
                table: "tbIncomingPaymentDetail");

            migrationBuilder.DropColumn(
                name: "ItemInvoice",
                schema: "dbo",
                table: "tbIncomingPaymentDetail");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer");

            migrationBuilder.DropColumn(
                name: "DocTypeID",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer");

            migrationBuilder.DropColumn(
                name: "ItemInvoice",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer");

            migrationBuilder.DropColumn(
                name: "SeriesDID",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer");

            migrationBuilder.DropColumn(
                name: "SeriesID",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                schema: "dbo",
                table: "tbIncomingPayment");

            migrationBuilder.DropColumn(
                name: "DocTypeID",
                schema: "dbo",
                table: "tbIncomingPayment");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                schema: "dbo",
                table: "tbIncomingPayment");

            migrationBuilder.DropColumn(
                name: "PaymentMeanID",
                schema: "dbo",
                table: "tbIncomingPayment");

            migrationBuilder.DropColumn(
                name: "SeriesDID",
                schema: "dbo",
                table: "tbIncomingPayment");

            migrationBuilder.DropColumn(
                name: "SeriesID",
                schema: "dbo",
                table: "tbIncomingPayment");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "dbo",
                table: "tbIncomingPayment");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                table: "tbSaleQuote");

            migrationBuilder.DropColumn(
                name: "DocTypeID",
                table: "tbSaleQuote");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "tbSaleQuote");

            migrationBuilder.DropColumn(
                name: "SeriesDID",
                table: "tbSaleQuote");

            migrationBuilder.DropColumn(
                name: "SeriesID",
                table: "tbSaleQuote");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                table: "tbSaleDelivery");

            migrationBuilder.DropColumn(
                name: "DocTypeID",
                table: "tbSaleDelivery");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "tbSaleDelivery");

            migrationBuilder.DropColumn(
                name: "SeriesDID",
                table: "tbSaleDelivery");

            migrationBuilder.DropColumn(
                name: "SeriesID",
                table: "tbSaleDelivery");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                table: "tbSaleCreditMemo");

            migrationBuilder.DropColumn(
                name: "DocTypeID",
                table: "tbSaleCreditMemo");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "tbSaleCreditMemo");

            migrationBuilder.DropColumn(
                name: "SeriesDID",
                table: "tbSaleCreditMemo");

            migrationBuilder.DropColumn(
                name: "SeriesID",
                table: "tbSaleCreditMemo");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                table: "tbSaleAR");

            migrationBuilder.DropColumn(
                name: "DocTypeID",
                table: "tbSaleAR");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "tbSaleAR");

            migrationBuilder.DropColumn(
                name: "SeriesDID",
                table: "tbSaleAR");

            migrationBuilder.DropColumn(
                name: "SeriesID",
                table: "tbSaleAR");

            migrationBuilder.AddColumn<double>(
                name: "Cash",
                schema: "dbo",
                table: "tbIncomingPaymentDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Cash",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
