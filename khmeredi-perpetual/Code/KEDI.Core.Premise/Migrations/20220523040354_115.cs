using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _115 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbPurchaseRequest_tbBranch_BranchID",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_tbPurchaseRequest_tbCurrency_SystemCurrencyID",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_tbPurchaseRequest_tbUserAccount_UserID",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_tbPurchaseRequest_tbWarhouse_WarehoueseID",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_tbPurchaseRequiredDetail_tbItemMasterData_ItemID",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_tbPurchaseRequiredDetail_tbPurchaseRequest_PurchaseRequestID",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_tbPurchaseRequiredDetail_tbCurrency_SystemCurrencyID",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_tbPurchaseRequiredDetail_tbUnitofMeasure_UomID",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail");

            migrationBuilder.DropIndex(
                name: "IX_tbPurchaseRequest_BranchID",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropIndex(
                name: "IX_tbPurchaseRequest_SystemCurrencyID",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropIndex(
                name: "IX_tbPurchaseRequest_UserID",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropIndex(
                name: "IX_tbPurchaseRequest_WarehoueseID",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbPurchaseRequiredDetail",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail");

            migrationBuilder.DropIndex(
                name: "IX_tbPurchaseRequiredDetail_ItemID",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail");

            migrationBuilder.DropIndex(
                name: "IX_tbPurchaseRequiredDetail_SystemCurrencyID",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail");

            migrationBuilder.DropIndex(
                name: "IX_tbPurchaseRequiredDetail_UomID",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail");

            migrationBuilder.DropColumn(
                name: "RequiredDate",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "QuotedDate",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail");

            migrationBuilder.DropColumn(
                name: "RequiredDate",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail");

            migrationBuilder.DropColumn(
                name: "RequiredQty",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail");

            migrationBuilder.DropColumn(
                name: "RequiredDate",
                schema: "dbo",
                table: "tbPurchaseQuotation");

            migrationBuilder.RenameTable(
                name: "tbPurchaseRequiredDetail",
                schema: "dbo",
                newName: "tbPurchaseRequestDetail",
                newSchema: "dbo");

            migrationBuilder.RenameColumn(
                name: "WarehoueseID",
                schema: "dbo",
                table: "tbPurchaseRequest",
                newName: "SysCurrencyID");

            migrationBuilder.RenameColumn(
                name: "ValidUntil",
                schema: "dbo",
                table: "tbPurchaseRequest",
                newName: "DeliveryDate");

            migrationBuilder.RenameColumn(
                name: "SystemCurrencyID",
                schema: "dbo",
                table: "tbPurchaseRequest",
                newName: "SeriesID");

            migrationBuilder.RenameColumn(
                name: "InvoiceNo",
                schema: "dbo",
                table: "tbPurchaseRequest",
                newName: "ReffNo");

            migrationBuilder.RenameColumn(
                name: "ExchangeRate",
                schema: "dbo",
                table: "tbPurchaseRequest",
                newName: "TaxValue");

            migrationBuilder.RenameColumn(
                name: "Balance_Due",
                schema: "dbo",
                table: "tbPurchaseRequest",
                newName: "TaxRate");

            migrationBuilder.RenameColumn(
                name: "PurchaseRequestID",
                schema: "dbo",
                table: "tbPurchaseRequest",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "requestID",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                newName: "QuotationID");

            migrationBuilder.RenameColumn(
                name: "Total_Sys",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                newName: "TotalSys");

            migrationBuilder.RenameColumn(
                name: "PurchaseQuotaionDetailID",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "ValidUntil",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "DeliveryDate");

            migrationBuilder.RenameColumn(
                name: "TaxValues",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "TaxValue");

            migrationBuilder.RenameColumn(
                name: "Sub_Total_Sys",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "SubTotalSys");

            migrationBuilder.RenameColumn(
                name: "Sub_Total",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "SubTotal");

            migrationBuilder.RenameColumn(
                name: "Reff_No",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "ReffNo");

            migrationBuilder.RenameColumn(
                name: "LocalCurrencyID",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "SeriesID");

            migrationBuilder.RenameColumn(
                name: "ExchangeRate",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "ReturnAmount");

            migrationBuilder.RenameColumn(
                name: "DiscountValues",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "PurRate");

            migrationBuilder.RenameColumn(
                name: "Balance_Due_Sys",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "LocalSetRate");

            migrationBuilder.RenameColumn(
                name: "Balance_Due",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "DownPaymentSys");

            migrationBuilder.RenameColumn(
                name: "Applied_AmountSys",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "DownPayment");

            migrationBuilder.RenameColumn(
                name: "Applied_Amount",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "DiscountValue");

            migrationBuilder.RenameColumn(
                name: "PurchaseQuotationID",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Reff_No",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "ReffNo");

            migrationBuilder.RenameColumn(
                name: "check",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail",
                newName: "Check");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                newName: "TotalWTaxSys");

            migrationBuilder.RenameColumn(
                name: "SystemCurrencyID",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                newName: "TaxGroupID");

            migrationBuilder.RenameColumn(
                name: "LineID",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                newName: "QuotationID");

            migrationBuilder.RenameColumn(
                name: "ExchangRate",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                newName: "TotalWTax");

            migrationBuilder.RenameColumn(
                name: "RequiredDetailID",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_tbPurchaseRequiredDetail_PurchaseRequestID",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                newName: "IX_tbPurchaseRequestDetail_PurchaseRequestID");

            migrationBuilder.AddColumn<int>(
                name: "FWarehouseID",
                schema: "dbo",
                table: "tbTarnsferDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TWarehouseID",
                schema: "dbo",
                table: "tbTarnsferDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReceivedBy",
                schema: "dbo",
                table: "tbSaleOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequestedBy",
                schema: "dbo",
                table: "tbSaleOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShippedBy",
                schema: "dbo",
                table: "tbSaleOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "AdditionalExpense",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalNote",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AppliedAmount",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AppliedAmountSys",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BalanceDue",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BalanceDueSys",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "DiscountRate",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DiscountValue",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "DocumentTypeID",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "DownPayment",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DownPaymentSys",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "FrieghtAmount",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FrieghtAmountSys",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "LocalCurID",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "LocalSetRate",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Number",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurCurrencyID",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "PurRate",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "RequesterID",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "ReturnAmount",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesDetailID",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "SubTotal",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDis",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDisSys",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "SubTotalSys",
                schema: "dbo",
                table: "tbPurchaseRequest",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpireDate",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "Date");

            migrationBuilder.AddColumn<string>(
                name: "Check",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AdditionalExpense",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalNote",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AppliedAmount",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AppliedAmountSys",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BalanceDue",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BalanceDueSys",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "BaseOnID",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CopyKey",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CopyType",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DocumentTypeID",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "FrieghtAmount",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FrieghtAmountSys",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "LocalCurID",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Number",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurCurrencyID",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesDetailID",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BaseOnID",
                schema: "dbo",
                table: "tbPurchaseOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CopyKey",
                schema: "dbo",
                table: "tbPurchaseOrder",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CopyType",
                schema: "dbo",
                table: "tbPurchaseOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TypePurchase",
                schema: "dbo",
                table: "tbOutgoingPaymentVendor",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TypePurchase",
                schema: "dbo",
                table: "tbOutgoingpayment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Types",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OpenQty",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "WarehouseID",
                schema: "dbo",
                table: "tbGoodReceitpDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WarehouseID",
                schema: "dbo",
                table: "tbGoodIssuesDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "RememberCustomer",
                schema: "dbo",
                table: "tbGeneralSetting",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "DisplayRate",
                schema: "dbo",
                table: "tbExchangeRate",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "CustomerSourceID",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StoreName",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TerritoryID",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                schema: "dbo",
                table: "rp_Cashout",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReceivedBy",
                table: "tbSaleQuote",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequestedBy",
                table: "tbSaleQuote",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShippedBy",
                table: "tbSaleQuote",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReceivedBy",
                table: "tbSaleDelivery",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequestedBy",
                table: "tbSaleDelivery",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShippedBy",
                table: "tbSaleDelivery",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReceivedBy",
                table: "tbSaleAR",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequestedBy",
                table: "tbSaleAR",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShippedBy",
                table: "tbSaleAR",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "OpenQty",
                table: "tbPurchaseCreditMemoDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ReceivedBy",
                table: "SaleCreditMemos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequestedBy",
                table: "SaleCreditMemos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShippedBy",
                table: "SaleCreditMemos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReceivedBy",
                table: "ReturnDelivery",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequestedBy",
                table: "ReturnDelivery",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShippedBy",
                table: "ReturnDelivery",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "PredictedClosingInTime",
                table: "PotentialDetail",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "ReceivedBy",
                table: "ARDownPayment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequestedBy",
                table: "ARDownPayment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShippedBy",
                table: "ARDownPayment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AssignByName",
                table: "Activity",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequiredDate",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "Date");

            migrationBuilder.AddColumn<double>(
                name: "AlertStock",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Check",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DiscountRate",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DiscountValue",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisRate",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisValue",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinTotalValue",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "LocalCurrencyID",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "OldQty",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PurchasPrice",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxOfFinDisValue",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxValue",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "TotalSys",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbPurchaseRequestDetail",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "ARReserveInvoice",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CusID = table.Column<int>(nullable: false),
                    BaseOnID = table.Column<int>(nullable: false),
                    RequestedBy = table.Column<int>(nullable: false),
                    ShippedBy = table.Column<int>(nullable: false),
                    ReceivedBy = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    SaleCurrencyID = table.Column<int>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    DocTypeID = table.Column<int>(nullable: false),
                    SeriesID = table.Column<int>(nullable: false),
                    SeriesDID = table.Column<int>(nullable: false),
                    InvoiceNumber = table.Column<string>(nullable: true),
                    RefNo = table.Column<string>(nullable: true),
                    InvoiceNo = table.Column<string>(nullable: true),
                    ExchangeRate = table.Column<double>(nullable: false),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DueDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    IncludeVat = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    SubTotalBefDis = table.Column<decimal>(nullable: false),
                    SubTotalBefDisSys = table.Column<decimal>(nullable: false),
                    SubTotalAfterDis = table.Column<decimal>(nullable: false),
                    SubTotalAfterDisSys = table.Column<decimal>(nullable: false),
                    FreightAmount = table.Column<decimal>(nullable: false),
                    FreightAmountSys = table.Column<decimal>(nullable: false),
                    DownPayment = table.Column<decimal>(nullable: false),
                    DownPaymentSys = table.Column<decimal>(nullable: false),
                    SubTotal = table.Column<double>(nullable: false),
                    SubTotalSys = table.Column<double>(nullable: false),
                    DisRate = table.Column<double>(nullable: false),
                    DisValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    VatRate = table.Column<double>(nullable: false),
                    VatValue = table.Column<double>(nullable: false),
                    AppliedAmount = table.Column<double>(nullable: false),
                    FeeNote = table.Column<string>(nullable: true),
                    FeeAmount = table.Column<double>(nullable: false),
                    TotalAmount = table.Column<double>(nullable: false),
                    TotalAmountSys = table.Column<double>(nullable: false),
                    CopyType = table.Column<int>(nullable: false),
                    CopyKey = table.Column<string>(nullable: true),
                    BasedCopyKeys = table.Column<string>(nullable: true),
                    ChangeLog = table.Column<DateTime>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    Types = table.Column<string>(nullable: true),
                    LocalSetRate = table.Column<double>(nullable: false),
                    SaleEmID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARReserveInvoice", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CanRing",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ItemID = table.Column<int>(nullable: false),
                    ItemChangeID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    UomChangeID = table.Column<int>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "Date", nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(nullable: false),
                    ChangeQty = table.Column<decimal>(nullable: false),
                    ChargePrice = table.Column<decimal>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanRing", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CanRingMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Number = table.Column<string>(nullable: true),
                    DocTypeID = table.Column<int>(nullable: false),
                    SeriesID = table.Column<int>(nullable: false),
                    SeriesDID = table.Column<int>(nullable: false),
                    CusId = table.Column<int>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    PaymentMeanID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "Date", nullable: false),
                    ExchangeRate = table.Column<decimal>(nullable: false),
                    TotalSystem = table.Column<decimal>(nullable: false),
                    Total = table.Column<decimal>(nullable: false),
                    TotalAlt = table.Column<decimal>(nullable: false),
                    Change = table.Column<decimal>(nullable: false),
                    ChangeAlt = table.Column<decimal>(nullable: false),
                    Received = table.Column<decimal>(nullable: false),
                    ReceivedAlt = table.Column<decimal>(nullable: false),
                    OtherPaymentGrandTotal = table.Column<decimal>(nullable: false),
                    GrandTotalCurrenciesDisplay = table.Column<string>(nullable: true),
                    ChangeCurrenciesDisplay = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanRingMaster", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ContractBillingss",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BusinessPartnerID = table.Column<int>(nullable: false),
                    ContractID = table.Column<int>(nullable: false),
                    BPID = table.Column<int>(nullable: false),
                    SaleID = table.Column<int>(nullable: false),
                    SeriesDID = table.Column<int>(nullable: false),
                    DocumentNo = table.Column<string>(nullable: true),
                    Amount = table.Column<string>(nullable: true),
                    NumExpiresOfDay = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ConfrimRenew = table.Column<int>(nullable: false),
                    Payment = table.Column<int>(nullable: false),
                    NewContractStartDate = table.Column<DateTime>(nullable: false),
                    NewContractEndDate = table.Column<DateTime>(nullable: false),
                    NextOpenRenewalDate = table.Column<DateTime>(nullable: false),
                    Renewalstartdate = table.Column<DateTime>(nullable: false),
                    Renewalenddate = table.Column<DateTime>(nullable: false),
                    TerminateDate = table.Column<DateTime>(nullable: false),
                    ContractType = table.Column<string>(nullable: true),
                    ContractNameTemplate = table.Column<string>(nullable: true),
                    SubContractTypeTemplate = table.Column<string>(nullable: true),
                    Activities = table.Column<int>(nullable: false),
                    EstimateSupportCost = table.Column<decimal>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    Attachement = table.Column<int>(nullable: false),
                    Path = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractBillingss", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ContractBillingss_tbBusinessPartner_BusinessPartnerID",
                        column: x => x.BusinessPartnerID,
                        principalSchema: "dbo",
                        principalTable: "tbBusinessPartner",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractTemplate",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    ContracType = table.Column<int>(nullable: false),
                    Expired = table.Column<bool>(nullable: false),
                    ResponseTime = table.Column<string>(nullable: true),
                    ResponseTimeDH = table.Column<int>(nullable: false),
                    ResultionTime = table.Column<string>(nullable: true),
                    ResultionTimeDH = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    StarttimeWed = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractTemplate", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CustomerSource",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerSource", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeCanRingMasters",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Number = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "Date", nullable: false),
                    DocTypeID = table.Column<int>(nullable: false),
                    SeriesID = table.Column<int>(nullable: false),
                    SeriesDID = table.Column<int>(nullable: false),
                    CusId = table.Column<int>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    PaymentMeanID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    ExchangeRate = table.Column<decimal>(nullable: false),
                    TotalSystem = table.Column<decimal>(nullable: false),
                    Total = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeCanRingMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Menu",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseAPReserve",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VendorID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    PurCurrencyID = table.Column<int>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    DocumentTypeID = table.Column<int>(nullable: false),
                    SeriesID = table.Column<int>(nullable: false),
                    SeriesDetailID = table.Column<int>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    ReffNo = table.Column<string>(nullable: true),
                    InvoiceNo = table.Column<string>(nullable: true),
                    PurRate = table.Column<double>(nullable: false),
                    BalanceDueSys = table.Column<double>(nullable: false),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DueDate = table.Column<DateTime>(type: "Date", nullable: false),
                    SubTotal = table.Column<double>(nullable: false),
                    SubTotalSys = table.Column<double>(nullable: false),
                    SubTotalAfterDis = table.Column<decimal>(nullable: false),
                    SubTotalAfterDisSys = table.Column<decimal>(nullable: false),
                    FrieghtAmount = table.Column<decimal>(nullable: false),
                    FrieghtAmountSys = table.Column<decimal>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    TaxRate = table.Column<double>(nullable: false),
                    TaxValue = table.Column<double>(nullable: false),
                    DownPayment = table.Column<double>(nullable: false),
                    DownPaymentSys = table.Column<double>(nullable: false),
                    AppliedAmount = table.Column<double>(nullable: false),
                    AppliedAmountSys = table.Column<double>(nullable: false),
                    ReturnAmount = table.Column<double>(nullable: false),
                    BalanceDue = table.Column<double>(nullable: false),
                    AdditionalExpense = table.Column<double>(nullable: false),
                    AdditionalNote = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    LocalSetRate = table.Column<double>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    BaseOnID = table.Column<int>(nullable: false),
                    CopyToNote = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseAPReserve", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PurchaseAPReserve_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseAPReserve_tbUserAccount_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseAPReserve_tbBusinessPartner_VendorID",
                        column: x => x.VendorID,
                        principalSchema: "dbo",
                        principalTable: "tbBusinessPartner",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseAPReserve_tbWarhouse_WarehouseID",
                        column: x => x.WarehouseID,
                        principalSchema: "dbo",
                        principalTable: "tbWarhouse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceContract",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CusID = table.Column<int>(nullable: false),
                    BaseOnID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    SaleCurrencyID = table.Column<int>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    DocTypeID = table.Column<int>(nullable: false),
                    AppliedAmount = table.Column<double>(nullable: false),
                    SeriesID = table.Column<int>(nullable: false),
                    SeriesDID = table.Column<int>(nullable: false),
                    InvoiceNumber = table.Column<string>(nullable: true),
                    AdditionalContractNo = table.Column<string>(nullable: true),
                    ContractStartDate = table.Column<DateTime>(nullable: false),
                    ContractENDate = table.Column<DateTime>(nullable: false),
                    ContractRenewalDate = table.Column<DateTime>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    ContractTemplateID = table.Column<int>(nullable: false),
                    RefNo = table.Column<string>(nullable: true),
                    InvoiceNo = table.Column<string>(nullable: true),
                    ExchangeRate = table.Column<double>(nullable: false),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DueDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    IncludeVat = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    SubTotalBefDis = table.Column<decimal>(nullable: false),
                    SubTotalBefDisSys = table.Column<decimal>(nullable: false),
                    SubTotalAfterDis = table.Column<decimal>(nullable: false),
                    SubTotalAfterDisSys = table.Column<decimal>(nullable: false),
                    FreightAmount = table.Column<decimal>(nullable: false),
                    FreightAmountSys = table.Column<decimal>(nullable: false),
                    DownPayment = table.Column<decimal>(nullable: false),
                    DownPaymentSys = table.Column<decimal>(nullable: false),
                    SubTotal = table.Column<double>(nullable: false),
                    SubTotalSys = table.Column<double>(nullable: false),
                    DisRate = table.Column<double>(nullable: false),
                    DisValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    VatRate = table.Column<double>(nullable: false),
                    VatValue = table.Column<double>(nullable: false),
                    Types = table.Column<string>(nullable: true),
                    FeeNote = table.Column<string>(nullable: true),
                    FeeAmount = table.Column<double>(nullable: false),
                    TotalAmount = table.Column<double>(nullable: false),
                    TotalAmountSys = table.Column<double>(nullable: false),
                    CopyType = table.Column<int>(nullable: false),
                    CopyKey = table.Column<string>(nullable: true),
                    BasedCopyKeys = table.Column<string>(nullable: true),
                    ChangeLog = table.Column<DateTime>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false),
                    ContractType = table.Column<string>(nullable: true),
                    SaleEmID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceContract", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceContractAlerts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BPID = table.Column<int>(nullable: false),
                    InvoiceID = table.Column<int>(nullable: false),
                    SeriesDID = table.Column<int>(nullable: false),
                    InvoiceNumber = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    IsRead = table.Column<bool>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DueDateType = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    TimeLeft = table.Column<string>(nullable: true),
                    CompanyID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceContractAlerts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SetupContractName",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContractName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetupContractName", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SetupContractType",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContractType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetupContractType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Territories",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    LoationId = table.Column<int>(nullable: false),
                    ParentTerID = table.Column<int>(nullable: false),
                    ParentId = table.Column<int>(nullable: false),
                    Level = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Territories", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ARReserveInvoiceDetail",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ARReserveInvoiceID = table.Column<int>(nullable: false),
                    SQDID = table.Column<int>(nullable: false),
                    SODID = table.Column<int>(nullable: false),
                    SDDID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    TaxGroupID = table.Column<int>(nullable: false),
                    TaxRate = table.Column<decimal>(nullable: false),
                    TaxValue = table.Column<decimal>(nullable: false),
                    TaxOfFinDisValue = table.Column<decimal>(nullable: false),
                    FinTotalValue = table.Column<decimal>(nullable: false),
                    ItemCode = table.Column<string>(nullable: true),
                    ItemNameKH = table.Column<string>(nullable: true),
                    ItemNameEN = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    OpenQty = table.Column<double>(nullable: false),
                    PrintQty = table.Column<double>(nullable: false),
                    GUomID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    UomName = table.Column<string>(nullable: true),
                    Factor = table.Column<double>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    UnitPrice = table.Column<double>(nullable: false),
                    DisRate = table.Column<double>(nullable: false),
                    DisValue = table.Column<double>(nullable: false),
                    FinDisRate = table.Column<decimal>(nullable: false),
                    FinDisValue = table.Column<decimal>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    VatRate = table.Column<double>(nullable: false),
                    VatValue = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    TotalSys = table.Column<double>(nullable: false),
                    TotalWTax = table.Column<double>(nullable: false),
                    TotalWTaxSys = table.Column<double>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    ItemType = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARReserveInvoiceDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ARReserveInvoiceDetail_ARReserveInvoice_ARReserveInvoiceID",
                        column: x => x.ARReserveInvoiceID,
                        principalTable: "ARReserveInvoice",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CanRingDetail",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ItemID = table.Column<int>(nullable: false),
                    ItemChangeID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    UomChangeID = table.Column<int>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "Date", nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(nullable: false),
                    ChangeQty = table.Column<decimal>(nullable: false),
                    ChargePrice = table.Column<decimal>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CanRingID = table.Column<int>(nullable: false),
                    CanRingMasterID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanRingDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CanRingDetail_CanRingMaster_CanRingMasterID",
                        column: x => x.CanRingMasterID,
                        principalTable: "CanRingMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttachmentFileOfContractTemplate",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContractTemplateID = table.Column<int>(nullable: false),
                    TargetPath = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    AttachmentDate = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentFileOfContractTemplate", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AttachmentFileOfContractTemplate_ContractTemplate_ContractTemplateID",
                        column: x => x.ContractTemplateID,
                        principalTable: "ContractTemplate",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Converage",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContractTemplateID = table.Column<int>(nullable: false),
                    Part = table.Column<bool>(nullable: false),
                    Labor = table.Column<bool>(nullable: false),
                    Travel = table.Column<bool>(nullable: false),
                    Holiday = table.Column<bool>(nullable: false),
                    Expired = table.Column<bool>(nullable: false),
                    Monthday = table.Column<bool>(nullable: false),
                    Thuesday = table.Column<bool>(nullable: false),
                    Wednesday = table.Column<bool>(nullable: false),
                    Thursday = table.Column<bool>(nullable: false),
                    Friday = table.Column<bool>(nullable: false),
                    Saturday = table.Column<bool>(nullable: false),
                    Sunday = table.Column<bool>(nullable: false),
                    StarttimeMon = table.Column<string>(nullable: true),
                    StarttimeThu = table.Column<string>(nullable: true),
                    StarttimeWed = table.Column<string>(nullable: true),
                    StarttimeThur = table.Column<string>(nullable: true),
                    StarttimeFri = table.Column<string>(nullable: true),
                    StarttimeSat = table.Column<string>(nullable: true),
                    StarttimeSun = table.Column<string>(nullable: true),
                    EndtimeMon = table.Column<string>(nullable: true),
                    EndtimeThu = table.Column<string>(nullable: true),
                    EndtimeWed = table.Column<string>(nullable: true),
                    EndtimeThur = table.Column<string>(nullable: true),
                    EndtimeFri = table.Column<string>(nullable: true),
                    EndtimeSat = table.Column<string>(nullable: true),
                    EndtimeSun = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Converage", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Converage_ContractTemplate_ContractTemplateID",
                        column: x => x.ContractTemplateID,
                        principalTable: "ContractTemplate",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Remark",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContractTemplateID = table.Column<int>(nullable: false),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Remark", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Remark_ContractTemplate_ContractTemplateID",
                        column: x => x.ContractTemplateID,
                        principalTable: "ContractTemplate",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeCanRingDetails",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ItemID = table.Column<int>(nullable: false),
                    ItemChangeID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    UomChangeID = table.Column<int>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "Date", nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(nullable: false),
                    ChangeQty = table.Column<decimal>(nullable: false),
                    ChargePrice = table.Column<decimal>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CanRingID = table.Column<int>(nullable: false),
                    ExchangeCanRingMasterID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeCanRingDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ExchangeCanRingDetails_ExchangeCanRingMasters_ExchangeCanRingMasterID",
                        column: x => x.ExchangeCanRingMasterID,
                        principalTable: "ExchangeCanRingMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseAPReserveDetail",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PurchaseAPReserveID = table.Column<int>(nullable: false),
                    LineID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    TaxGroupID = table.Column<int>(nullable: false),
                    TaxRate = table.Column<decimal>(nullable: false),
                    TaxValue = table.Column<decimal>(nullable: false),
                    TaxOfFinDisValue = table.Column<decimal>(nullable: false),
                    FinTotalValue = table.Column<decimal>(nullable: false),
                    FinDisRate = table.Column<decimal>(nullable: false),
                    FinDisValue = table.Column<decimal>(nullable: false),
                    TotalWTax = table.Column<double>(nullable: false),
                    TotalWTaxSys = table.Column<double>(nullable: false),
                    TotalSys = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    OpenQty = table.Column<double>(nullable: false),
                    PurchasPrice = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    OrderID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: false),
                    AlertStock = table.Column<double>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    Check = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseAPReserveDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PurchaseAPReserveDetail_tbItemMasterData_ItemID",
                        column: x => x.ItemID,
                        principalSchema: "dbo",
                        principalTable: "tbItemMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseAPReserveDetail_tbCurrency_LocalCurrencyID",
                        column: x => x.LocalCurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseAPReserveDetail_PurchaseAPReserve_PurchaseAPReserveID",
                        column: x => x.PurchaseAPReserveID,
                        principalTable: "PurchaseAPReserve",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseAPReserveDetail_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttchmentFile",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServiceContractID = table.Column<int>(nullable: false),
                    TargetPath = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    AttachmentDate = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false),
                    ContractBilingID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttchmentFile", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AttchmentFile_ContractBillingss_ContractBilingID",
                        column: x => x.ContractBilingID,
                        principalTable: "ContractBillingss",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttchmentFile_ServiceContract_ServiceContractID",
                        column: x => x.ServiceContractID,
                        principalTable: "ServiceContract",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceContractDetail",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServiceContractID = table.Column<int>(nullable: false),
                    SQDID = table.Column<int>(nullable: false),
                    SODID = table.Column<int>(nullable: false),
                    SDDID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    TaxGroupID = table.Column<int>(nullable: false),
                    TaxRate = table.Column<decimal>(nullable: false),
                    TaxValue = table.Column<decimal>(nullable: false),
                    TaxOfFinDisValue = table.Column<decimal>(nullable: false),
                    FinTotalValue = table.Column<decimal>(nullable: false),
                    ItemCode = table.Column<string>(nullable: true),
                    ItemNameKH = table.Column<string>(nullable: true),
                    ItemNameEN = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    OpenQty = table.Column<double>(nullable: false),
                    PrintQty = table.Column<double>(nullable: false),
                    GUomID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    UomName = table.Column<string>(nullable: true),
                    Factor = table.Column<double>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    UnitPrice = table.Column<double>(nullable: false),
                    DisRate = table.Column<double>(nullable: false),
                    DisValue = table.Column<double>(nullable: false),
                    FinDisRate = table.Column<decimal>(nullable: false),
                    FinDisValue = table.Column<decimal>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    VatRate = table.Column<double>(nullable: false),
                    VatValue = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    TotalSys = table.Column<double>(nullable: false),
                    TotalWTax = table.Column<double>(nullable: false),
                    TotalWTaxSys = table.Column<double>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    ItemType = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceContractDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ServiceContractDetail_ServiceContract_ServiceContractID",
                        column: x => x.ServiceContractID,
                        principalTable: "ServiceContract",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ARReserveInvoiceDetail_ARReserveInvoiceID",
                table: "ARReserveInvoiceDetail",
                column: "ARReserveInvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentFileOfContractTemplate_ContractTemplateID",
                table: "AttachmentFileOfContractTemplate",
                column: "ContractTemplateID");

            migrationBuilder.CreateIndex(
                name: "IX_AttchmentFile_ContractBilingID",
                table: "AttchmentFile",
                column: "ContractBilingID");

            migrationBuilder.CreateIndex(
                name: "IX_AttchmentFile_ServiceContractID",
                table: "AttchmentFile",
                column: "ServiceContractID");

            migrationBuilder.CreateIndex(
                name: "IX_CanRingDetail_CanRingMasterID",
                table: "CanRingDetail",
                column: "CanRingMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_ContractBillingss_BusinessPartnerID",
                table: "ContractBillingss",
                column: "BusinessPartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_Converage_ContractTemplateID",
                table: "Converage",
                column: "ContractTemplateID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeCanRingDetails_ExchangeCanRingMasterID",
                table: "ExchangeCanRingDetails",
                column: "ExchangeCanRingMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseAPReserve_BranchID",
                table: "PurchaseAPReserve",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseAPReserve_UserID",
                table: "PurchaseAPReserve",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseAPReserve_VendorID",
                table: "PurchaseAPReserve",
                column: "VendorID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseAPReserve_WarehouseID",
                table: "PurchaseAPReserve",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseAPReserveDetail_ItemID",
                table: "PurchaseAPReserveDetail",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseAPReserveDetail_LocalCurrencyID",
                table: "PurchaseAPReserveDetail",
                column: "LocalCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseAPReserveDetail_PurchaseAPReserveID",
                table: "PurchaseAPReserveDetail",
                column: "PurchaseAPReserveID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseAPReserveDetail_UomID",
                table: "PurchaseAPReserveDetail",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_Remark_ContractTemplateID",
                table: "Remark",
                column: "ContractTemplateID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceContractDetail_ServiceContractID",
                table: "ServiceContractDetail",
                column: "ServiceContractID");

            migrationBuilder.AddForeignKey(
                name: "FK_tbPurchaseRequestDetail_tbPurchaseRequest_PurchaseRequestID",
                schema: "dbo",
                table: "tbPurchaseRequestDetail",
                column: "PurchaseRequestID",
                principalSchema: "dbo",
                principalTable: "tbPurchaseRequest",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbPurchaseRequestDetail_tbPurchaseRequest_PurchaseRequestID",
                schema: "dbo",
                table: "tbPurchaseRequestDetail");

            migrationBuilder.DropTable(
                name: "ARReserveInvoiceDetail");

            migrationBuilder.DropTable(
                name: "AttachmentFileOfContractTemplate");

            migrationBuilder.DropTable(
                name: "AttchmentFile");

            migrationBuilder.DropTable(
                name: "CanRing");

            migrationBuilder.DropTable(
                name: "CanRingDetail");

            migrationBuilder.DropTable(
                name: "Converage");

            migrationBuilder.DropTable(
                name: "CustomerSource");

            migrationBuilder.DropTable(
                name: "ExchangeCanRingDetails");

            migrationBuilder.DropTable(
                name: "Menu");

            migrationBuilder.DropTable(
                name: "PurchaseAPReserveDetail");

            migrationBuilder.DropTable(
                name: "Remark");

            migrationBuilder.DropTable(
                name: "ServiceContractAlerts");

            migrationBuilder.DropTable(
                name: "ServiceContractDetail");

            migrationBuilder.DropTable(
                name: "SetupContractName");

            migrationBuilder.DropTable(
                name: "SetupContractType");

            migrationBuilder.DropTable(
                name: "Territories");

            migrationBuilder.DropTable(
                name: "ARReserveInvoice");

            migrationBuilder.DropTable(
                name: "ContractBillingss");

            migrationBuilder.DropTable(
                name: "CanRingMaster");

            migrationBuilder.DropTable(
                name: "ExchangeCanRingMasters");

            migrationBuilder.DropTable(
                name: "PurchaseAPReserve");

            migrationBuilder.DropTable(
                name: "ContractTemplate");

            migrationBuilder.DropTable(
                name: "ServiceContract");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbPurchaseRequestDetail",
                schema: "dbo",
                table: "tbPurchaseRequestDetail");

            migrationBuilder.DropColumn(
                name: "FWarehouseID",
                schema: "dbo",
                table: "tbTarnsferDetail");

            migrationBuilder.DropColumn(
                name: "TWarehouseID",
                schema: "dbo",
                table: "tbTarnsferDetail");

            migrationBuilder.DropColumn(
                name: "ReceivedBy",
                schema: "dbo",
                table: "tbSaleOrder");

            migrationBuilder.DropColumn(
                name: "RequestedBy",
                schema: "dbo",
                table: "tbSaleOrder");

            migrationBuilder.DropColumn(
                name: "ShippedBy",
                schema: "dbo",
                table: "tbSaleOrder");

            migrationBuilder.DropColumn(
                name: "AdditionalExpense",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "AdditionalNote",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "AppliedAmount",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "AppliedAmountSys",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "BalanceDue",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "BalanceDueSys",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "DiscountRate",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "DiscountValue",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "DocumentTypeID",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "DownPayment",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "DownPaymentSys",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "FrieghtAmount",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "FrieghtAmountSys",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "LocalCurID",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "LocalSetRate",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "Number",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "PurCurrencyID",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "PurRate",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "RequesterID",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "ReturnAmount",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "SeriesDetailID",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDis",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDisSys",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "SubTotalSys",
                schema: "dbo",
                table: "tbPurchaseRequest");

            migrationBuilder.DropColumn(
                name: "Check",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail");

            migrationBuilder.DropColumn(
                name: "Remark",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail");

            migrationBuilder.DropColumn(
                name: "AdditionalExpense",
                schema: "dbo",
                table: "tbPurchaseQuotation");

            migrationBuilder.DropColumn(
                name: "AdditionalNote",
                schema: "dbo",
                table: "tbPurchaseQuotation");

            migrationBuilder.DropColumn(
                name: "AppliedAmount",
                schema: "dbo",
                table: "tbPurchaseQuotation");

            migrationBuilder.DropColumn(
                name: "AppliedAmountSys",
                schema: "dbo",
                table: "tbPurchaseQuotation");

            migrationBuilder.DropColumn(
                name: "BalanceDue",
                schema: "dbo",
                table: "tbPurchaseQuotation");

            migrationBuilder.DropColumn(
                name: "BalanceDueSys",
                schema: "dbo",
                table: "tbPurchaseQuotation");

            migrationBuilder.DropColumn(
                name: "BaseOnID",
                schema: "dbo",
                table: "tbPurchaseQuotation");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                schema: "dbo",
                table: "tbPurchaseQuotation");

            migrationBuilder.DropColumn(
                name: "CopyKey",
                schema: "dbo",
                table: "tbPurchaseQuotation");

            migrationBuilder.DropColumn(
                name: "CopyType",
                schema: "dbo",
                table: "tbPurchaseQuotation");

            migrationBuilder.DropColumn(
                name: "DocumentTypeID",
                schema: "dbo",
                table: "tbPurchaseQuotation");

            migrationBuilder.DropColumn(
                name: "FrieghtAmount",
                schema: "dbo",
                table: "tbPurchaseQuotation");

            migrationBuilder.DropColumn(
                name: "FrieghtAmountSys",
                schema: "dbo",
                table: "tbPurchaseQuotation");

            migrationBuilder.DropColumn(
                name: "LocalCurID",
                schema: "dbo",
                table: "tbPurchaseQuotation");

            migrationBuilder.DropColumn(
                name: "Number",
                schema: "dbo",
                table: "tbPurchaseQuotation");

            migrationBuilder.DropColumn(
                name: "PurCurrencyID",
                schema: "dbo",
                table: "tbPurchaseQuotation");

            migrationBuilder.DropColumn(
                name: "SeriesDetailID",
                schema: "dbo",
                table: "tbPurchaseQuotation");

            migrationBuilder.DropColumn(
                name: "BaseOnID",
                schema: "dbo",
                table: "tbPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "CopyKey",
                schema: "dbo",
                table: "tbPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "CopyType",
                schema: "dbo",
                table: "tbPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "TypePurchase",
                schema: "dbo",
                table: "tbOutgoingPaymentVendor");

            migrationBuilder.DropColumn(
                name: "TypePurchase",
                schema: "dbo",
                table: "tbOutgoingpayment");

            migrationBuilder.DropColumn(
                name: "Types",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer");

            migrationBuilder.DropColumn(
                name: "OpenQty",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail");

            migrationBuilder.DropColumn(
                name: "WarehouseID",
                schema: "dbo",
                table: "tbGoodReceitpDetail");

            migrationBuilder.DropColumn(
                name: "WarehouseID",
                schema: "dbo",
                table: "tbGoodIssuesDetail");

            migrationBuilder.DropColumn(
                name: "RememberCustomer",
                schema: "dbo",
                table: "tbGeneralSetting");

            migrationBuilder.DropColumn(
                name: "DisplayRate",
                schema: "dbo",
                table: "tbExchangeRate");

            migrationBuilder.DropColumn(
                name: "CustomerSourceID",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "StoreName",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "TerritoryID",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "ItemId",
                schema: "dbo",
                table: "rp_Cashout");

            migrationBuilder.DropColumn(
                name: "ReceivedBy",
                table: "tbSaleQuote");

            migrationBuilder.DropColumn(
                name: "RequestedBy",
                table: "tbSaleQuote");

            migrationBuilder.DropColumn(
                name: "ShippedBy",
                table: "tbSaleQuote");

            migrationBuilder.DropColumn(
                name: "ReceivedBy",
                table: "tbSaleDelivery");

            migrationBuilder.DropColumn(
                name: "RequestedBy",
                table: "tbSaleDelivery");

            migrationBuilder.DropColumn(
                name: "ShippedBy",
                table: "tbSaleDelivery");

            migrationBuilder.DropColumn(
                name: "ReceivedBy",
                table: "tbSaleAR");

            migrationBuilder.DropColumn(
                name: "RequestedBy",
                table: "tbSaleAR");

            migrationBuilder.DropColumn(
                name: "ShippedBy",
                table: "tbSaleAR");

            migrationBuilder.DropColumn(
                name: "OpenQty",
                table: "tbPurchaseCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "ReceivedBy",
                table: "SaleCreditMemos");

            migrationBuilder.DropColumn(
                name: "RequestedBy",
                table: "SaleCreditMemos");

            migrationBuilder.DropColumn(
                name: "ShippedBy",
                table: "SaleCreditMemos");

            migrationBuilder.DropColumn(
                name: "ReceivedBy",
                table: "ReturnDelivery");

            migrationBuilder.DropColumn(
                name: "RequestedBy",
                table: "ReturnDelivery");

            migrationBuilder.DropColumn(
                name: "ShippedBy",
                table: "ReturnDelivery");

            migrationBuilder.DropColumn(
                name: "ReceivedBy",
                table: "ARDownPayment");

            migrationBuilder.DropColumn(
                name: "RequestedBy",
                table: "ARDownPayment");

            migrationBuilder.DropColumn(
                name: "ShippedBy",
                table: "ARDownPayment");

            migrationBuilder.DropColumn(
                name: "AssignByName",
                table: "Activity");

            migrationBuilder.DropColumn(
                name: "AlertStock",
                schema: "dbo",
                table: "tbPurchaseRequestDetail");

            migrationBuilder.DropColumn(
                name: "Check",
                schema: "dbo",
                table: "tbPurchaseRequestDetail");

            migrationBuilder.DropColumn(
                name: "DiscountRate",
                schema: "dbo",
                table: "tbPurchaseRequestDetail");

            migrationBuilder.DropColumn(
                name: "DiscountValue",
                schema: "dbo",
                table: "tbPurchaseRequestDetail");

            migrationBuilder.DropColumn(
                name: "ExpireDate",
                schema: "dbo",
                table: "tbPurchaseRequestDetail");

            migrationBuilder.DropColumn(
                name: "FinDisRate",
                schema: "dbo",
                table: "tbPurchaseRequestDetail");

            migrationBuilder.DropColumn(
                name: "FinDisValue",
                schema: "dbo",
                table: "tbPurchaseRequestDetail");

            migrationBuilder.DropColumn(
                name: "FinTotalValue",
                schema: "dbo",
                table: "tbPurchaseRequestDetail");

            migrationBuilder.DropColumn(
                name: "LocalCurrencyID",
                schema: "dbo",
                table: "tbPurchaseRequestDetail");

            migrationBuilder.DropColumn(
                name: "OldQty",
                schema: "dbo",
                table: "tbPurchaseRequestDetail");

            migrationBuilder.DropColumn(
                name: "PurchasPrice",
                schema: "dbo",
                table: "tbPurchaseRequestDetail");

            migrationBuilder.DropColumn(
                name: "TaxOfFinDisValue",
                schema: "dbo",
                table: "tbPurchaseRequestDetail");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                schema: "dbo",
                table: "tbPurchaseRequestDetail");

            migrationBuilder.DropColumn(
                name: "TaxValue",
                schema: "dbo",
                table: "tbPurchaseRequestDetail");

            migrationBuilder.DropColumn(
                name: "TotalSys",
                schema: "dbo",
                table: "tbPurchaseRequestDetail");

            migrationBuilder.RenameTable(
                name: "tbPurchaseRequestDetail",
                schema: "dbo",
                newName: "tbPurchaseRequiredDetail",
                newSchema: "dbo");

            migrationBuilder.RenameColumn(
                name: "TaxValue",
                schema: "dbo",
                table: "tbPurchaseRequest",
                newName: "ExchangeRate");

            migrationBuilder.RenameColumn(
                name: "TaxRate",
                schema: "dbo",
                table: "tbPurchaseRequest",
                newName: "Balance_Due");

            migrationBuilder.RenameColumn(
                name: "SysCurrencyID",
                schema: "dbo",
                table: "tbPurchaseRequest",
                newName: "WarehoueseID");

            migrationBuilder.RenameColumn(
                name: "SeriesID",
                schema: "dbo",
                table: "tbPurchaseRequest",
                newName: "SystemCurrencyID");

            migrationBuilder.RenameColumn(
                name: "ReffNo",
                schema: "dbo",
                table: "tbPurchaseRequest",
                newName: "InvoiceNo");

            migrationBuilder.RenameColumn(
                name: "DeliveryDate",
                schema: "dbo",
                table: "tbPurchaseRequest",
                newName: "ValidUntil");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "dbo",
                table: "tbPurchaseRequest",
                newName: "PurchaseRequestID");

            migrationBuilder.RenameColumn(
                name: "TotalSys",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                newName: "Total_Sys");

            migrationBuilder.RenameColumn(
                name: "QuotationID",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                newName: "requestID");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                newName: "PurchaseQuotaionDetailID");

            migrationBuilder.RenameColumn(
                name: "TaxValue",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "TaxValues");

            migrationBuilder.RenameColumn(
                name: "SubTotalSys",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "Sub_Total_Sys");

            migrationBuilder.RenameColumn(
                name: "SubTotal",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "Sub_Total");

            migrationBuilder.RenameColumn(
                name: "SeriesID",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "LocalCurrencyID");

            migrationBuilder.RenameColumn(
                name: "ReturnAmount",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "ExchangeRate");

            migrationBuilder.RenameColumn(
                name: "ReffNo",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "Reff_No");

            migrationBuilder.RenameColumn(
                name: "PurRate",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "DiscountValues");

            migrationBuilder.RenameColumn(
                name: "LocalSetRate",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "Balance_Due_Sys");

            migrationBuilder.RenameColumn(
                name: "DownPaymentSys",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "Balance_Due");

            migrationBuilder.RenameColumn(
                name: "DownPayment",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "Applied_AmountSys");

            migrationBuilder.RenameColumn(
                name: "DiscountValue",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "Applied_Amount");

            migrationBuilder.RenameColumn(
                name: "DeliveryDate",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "ValidUntil");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                newName: "PurchaseQuotationID");

            migrationBuilder.RenameColumn(
                name: "ReffNo",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "Reff_No");

            migrationBuilder.RenameColumn(
                name: "Check",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail",
                newName: "check");

            migrationBuilder.RenameColumn(
                name: "TotalWTaxSys",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "TotalWTax",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail",
                newName: "ExchangRate");

            migrationBuilder.RenameColumn(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail",
                newName: "SystemCurrencyID");

            migrationBuilder.RenameColumn(
                name: "QuotationID",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail",
                newName: "LineID");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail",
                newName: "RequiredDetailID");

            migrationBuilder.RenameIndex(
                name: "IX_tbPurchaseRequestDetail_PurchaseRequestID",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail",
                newName: "IX_tbPurchaseRequiredDetail_PurchaseRequestID");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequiredDate",
                schema: "dbo",
                table: "tbPurchaseRequest",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpireDate",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                type: "Date",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<DateTime>(
                name: "QuotedDate",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "RequiredDate",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "RequiredQty",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "RequiredDate",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "PredictedClosingInTime",
                table: "PotentialDetail",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequiredDate",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail",
                type: "Date",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbPurchaseRequiredDetail",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail",
                column: "RequiredDetailID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseRequest_BranchID",
                schema: "dbo",
                table: "tbPurchaseRequest",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseRequest_SystemCurrencyID",
                schema: "dbo",
                table: "tbPurchaseRequest",
                column: "SystemCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseRequest_UserID",
                schema: "dbo",
                table: "tbPurchaseRequest",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseRequest_WarehoueseID",
                schema: "dbo",
                table: "tbPurchaseRequest",
                column: "WarehoueseID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseRequiredDetail_ItemID",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseRequiredDetail_SystemCurrencyID",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail",
                column: "SystemCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseRequiredDetail_UomID",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail",
                column: "UomID");

            migrationBuilder.AddForeignKey(
                name: "FK_tbPurchaseRequest_tbBranch_BranchID",
                schema: "dbo",
                table: "tbPurchaseRequest",
                column: "BranchID",
                principalSchema: "dbo",
                principalTable: "tbBranch",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbPurchaseRequest_tbCurrency_SystemCurrencyID",
                schema: "dbo",
                table: "tbPurchaseRequest",
                column: "SystemCurrencyID",
                principalSchema: "dbo",
                principalTable: "tbCurrency",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbPurchaseRequest_tbUserAccount_UserID",
                schema: "dbo",
                table: "tbPurchaseRequest",
                column: "UserID",
                principalSchema: "dbo",
                principalTable: "tbUserAccount",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbPurchaseRequest_tbWarhouse_WarehoueseID",
                schema: "dbo",
                table: "tbPurchaseRequest",
                column: "WarehoueseID",
                principalSchema: "dbo",
                principalTable: "tbWarhouse",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbPurchaseRequiredDetail_tbItemMasterData_ItemID",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail",
                column: "ItemID",
                principalSchema: "dbo",
                principalTable: "tbItemMasterData",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbPurchaseRequiredDetail_tbPurchaseRequest_PurchaseRequestID",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail",
                column: "PurchaseRequestID",
                principalSchema: "dbo",
                principalTable: "tbPurchaseRequest",
                principalColumn: "PurchaseRequestID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbPurchaseRequiredDetail_tbCurrency_SystemCurrencyID",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail",
                column: "SystemCurrencyID",
                principalSchema: "dbo",
                principalTable: "tbCurrency",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbPurchaseRequiredDetail_tbUnitofMeasure_UomID",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail",
                column: "UomID",
                principalSchema: "dbo",
                principalTable: "tbUnitofMeasure",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
