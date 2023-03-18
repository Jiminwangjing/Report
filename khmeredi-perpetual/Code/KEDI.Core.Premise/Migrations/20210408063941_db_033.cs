using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_033 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cash",
                schema: "dbo",
                table: "tbOutgoingpaymnetDetail");

            migrationBuilder.DropColumn(
                name: "Cash",
                schema: "dbo",
                table: "tbOutgoingPaymentVendor");

            migrationBuilder.RenameColumn(
                name: "DocumentType",
                schema: "dbo",
                table: "tbOutgoingpaymnetDetail",
                newName: "NumberInvioce");

            migrationBuilder.RenameColumn(
                name: "DocumentNo",
                schema: "dbo",
                table: "tbOutgoingpaymnetDetail",
                newName: "ItemInvoice");

            migrationBuilder.RenameColumn(
                name: "DocumentType",
                schema: "dbo",
                table: "tbOutgoingPaymentVendor",
                newName: "Number");

            migrationBuilder.RenameColumn(
                name: "DocumentNo",
                schema: "dbo",
                table: "tbOutgoingPaymentVendor",
                newName: "ItemInvoice");

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                schema: "dbo",
                table: "tbPurchaseOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DocumentTypeID",
                schema: "dbo",
                table: "tbPurchaseOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Number",
                schema: "dbo",
                table: "tbPurchaseOrder",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeriesDetailID",
                schema: "dbo",
                table: "tbPurchaseOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesID",
                schema: "dbo",
                table: "tbPurchaseOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DocumentTypeID",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Number",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeriesDetailID",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesID",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                schema: "dbo",
                table: "tbPurchase_AP",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DocumentTypeID",
                schema: "dbo",
                table: "tbPurchase_AP",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Number",
                schema: "dbo",
                table: "tbPurchase_AP",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeriesDetailID",
                schema: "dbo",
                table: "tbPurchase_AP",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesID",
                schema: "dbo",
                table: "tbPurchase_AP",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "CheckPay",
                schema: "dbo",
                table: "tbOutgoingpaymnetDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyName",
                schema: "dbo",
                table: "tbOutgoingpaymnetDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocNo",
                schema: "dbo",
                table: "tbOutgoingpaymnetDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DocTypeID",
                schema: "dbo",
                table: "tbOutgoingpaymnetDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                schema: "dbo",
                table: "tbOutgoingPaymentVendor",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DocumentID",
                schema: "dbo",
                table: "tbOutgoingPaymentVendor",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesDetailID",
                schema: "dbo",
                table: "tbOutgoingPaymentVendor",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                schema: "dbo",
                table: "tbOutgoingpayment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DocumentID",
                schema: "dbo",
                table: "tbOutgoingpayment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NumberInvioce",
                schema: "dbo",
                table: "tbOutgoingpayment",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMeanID",
                schema: "dbo",
                table: "tbOutgoingpayment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesDetailID",
                schema: "dbo",
                table: "tbOutgoingpayment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesID",
                schema: "dbo",
                table: "tbOutgoingpayment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "dbo",
                table: "tbOutgoingpayment",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DocumentTypeID",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Number",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeriesDetailID",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesID",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Number",
                schema: "dbo",
                table: "ReportPurchaseOrder",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prefix",
                schema: "dbo",
                table: "ReportPurchaseOrder",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Number",
                schema: "dbo",
                table: "ReportPurchasCreditMemo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prefix",
                schema: "dbo",
                table: "ReportPurchasCreditMemo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Number",
                schema: "dbo",
                table: "ReportPurchasAP",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prefix",
                schema: "dbo",
                table: "ReportPurchasAP",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyID",
                schema: "dbo",
                table: "tbPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "DocumentTypeID",
                schema: "dbo",
                table: "tbPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "Number",
                schema: "dbo",
                table: "tbPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "SeriesDetailID",
                schema: "dbo",
                table: "tbPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "SeriesID",
                schema: "dbo",
                table: "tbPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                schema: "dbo",
                table: "tbPurchaseCreditMemo");

            migrationBuilder.DropColumn(
                name: "DocumentTypeID",
                schema: "dbo",
                table: "tbPurchaseCreditMemo");

            migrationBuilder.DropColumn(
                name: "Number",
                schema: "dbo",
                table: "tbPurchaseCreditMemo");

            migrationBuilder.DropColumn(
                name: "SeriesDetailID",
                schema: "dbo",
                table: "tbPurchaseCreditMemo");

            migrationBuilder.DropColumn(
                name: "SeriesID",
                schema: "dbo",
                table: "tbPurchaseCreditMemo");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                schema: "dbo",
                table: "tbPurchase_AP");

            migrationBuilder.DropColumn(
                name: "DocumentTypeID",
                schema: "dbo",
                table: "tbPurchase_AP");

            migrationBuilder.DropColumn(
                name: "Number",
                schema: "dbo",
                table: "tbPurchase_AP");

            migrationBuilder.DropColumn(
                name: "SeriesDetailID",
                schema: "dbo",
                table: "tbPurchase_AP");

            migrationBuilder.DropColumn(
                name: "SeriesID",
                schema: "dbo",
                table: "tbPurchase_AP");

            migrationBuilder.DropColumn(
                name: "CheckPay",
                schema: "dbo",
                table: "tbOutgoingpaymnetDetail");

            migrationBuilder.DropColumn(
                name: "CurrencyName",
                schema: "dbo",
                table: "tbOutgoingpaymnetDetail");

            migrationBuilder.DropColumn(
                name: "DocNo",
                schema: "dbo",
                table: "tbOutgoingpaymnetDetail");

            migrationBuilder.DropColumn(
                name: "DocTypeID",
                schema: "dbo",
                table: "tbOutgoingpaymnetDetail");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                schema: "dbo",
                table: "tbOutgoingPaymentVendor");

            migrationBuilder.DropColumn(
                name: "DocumentID",
                schema: "dbo",
                table: "tbOutgoingPaymentVendor");

            migrationBuilder.DropColumn(
                name: "SeriesDetailID",
                schema: "dbo",
                table: "tbOutgoingPaymentVendor");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                schema: "dbo",
                table: "tbOutgoingpayment");

            migrationBuilder.DropColumn(
                name: "DocumentID",
                schema: "dbo",
                table: "tbOutgoingpayment");

            migrationBuilder.DropColumn(
                name: "NumberInvioce",
                schema: "dbo",
                table: "tbOutgoingpayment");

            migrationBuilder.DropColumn(
                name: "PaymentMeanID",
                schema: "dbo",
                table: "tbOutgoingpayment");

            migrationBuilder.DropColumn(
                name: "SeriesDetailID",
                schema: "dbo",
                table: "tbOutgoingpayment");

            migrationBuilder.DropColumn(
                name: "SeriesID",
                schema: "dbo",
                table: "tbOutgoingpayment");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "dbo",
                table: "tbOutgoingpayment");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                schema: "dbo",
                table: "tbGoodsReciptPO");

            migrationBuilder.DropColumn(
                name: "DocumentTypeID",
                schema: "dbo",
                table: "tbGoodsReciptPO");

            migrationBuilder.DropColumn(
                name: "Number",
                schema: "dbo",
                table: "tbGoodsReciptPO");

            migrationBuilder.DropColumn(
                name: "SeriesDetailID",
                schema: "dbo",
                table: "tbGoodsReciptPO");

            migrationBuilder.DropColumn(
                name: "SeriesID",
                schema: "dbo",
                table: "tbGoodsReciptPO");

            migrationBuilder.DropColumn(
                name: "Number",
                schema: "dbo",
                table: "ReportPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "Prefix",
                schema: "dbo",
                table: "ReportPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "Number",
                schema: "dbo",
                table: "ReportPurchasCreditMemo");

            migrationBuilder.DropColumn(
                name: "Prefix",
                schema: "dbo",
                table: "ReportPurchasCreditMemo");

            migrationBuilder.DropColumn(
                name: "Number",
                schema: "dbo",
                table: "ReportPurchasAP");

            migrationBuilder.DropColumn(
                name: "Prefix",
                schema: "dbo",
                table: "ReportPurchasAP");

            migrationBuilder.RenameColumn(
                name: "NumberInvioce",
                schema: "dbo",
                table: "tbOutgoingpaymnetDetail",
                newName: "DocumentType");

            migrationBuilder.RenameColumn(
                name: "ItemInvoice",
                schema: "dbo",
                table: "tbOutgoingpaymnetDetail",
                newName: "DocumentNo");

            migrationBuilder.RenameColumn(
                name: "Number",
                schema: "dbo",
                table: "tbOutgoingPaymentVendor",
                newName: "DocumentType");

            migrationBuilder.RenameColumn(
                name: "ItemInvoice",
                schema: "dbo",
                table: "tbOutgoingPaymentVendor",
                newName: "DocumentNo");

            migrationBuilder.AddColumn<double>(
                name: "Cash",
                schema: "dbo",
                table: "tbOutgoingpaymnetDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Cash",
                schema: "dbo",
                table: "tbOutgoingPaymentVendor",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
