using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class DB_69 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbPurchaseAPDetail_tbPurchase_AP_Purchase_APID",
                schema: "dbo",
                table: "tbPurchaseAPDetail");

            migrationBuilder.DropIndex(
                name: "IX_tbPurchaseAPDetail_Purchase_APID",
                schema: "dbo",
                table: "tbPurchaseAPDetail");

            migrationBuilder.DropColumn(
                name: "Purchase_APID",
                schema: "dbo",
                table: "tbPurchaseAPDetail");

            migrationBuilder.RenameColumn(
                name: "Total_Sys",
                schema: "dbo",
                table: "tbSaleOrderDetail",
                newName: "TotalWTaxSys");

            migrationBuilder.RenameColumn(
                name: "Total_Sys",
                schema: "dbo",
                table: "tbPurchaseOrderDetail",
                newName: "TotalWTaxSys");

            migrationBuilder.RenameColumn(
                name: "TaxValues",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "TaxValue");

            migrationBuilder.RenameColumn(
                name: "Sub_Total_Sys",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "SubTotalSys");

            migrationBuilder.RenameColumn(
                name: "Sub_Total",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "SubTotal");

            migrationBuilder.RenameColumn(
                name: "Return_Amount",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "ReturnAmount");

            migrationBuilder.RenameColumn(
                name: "Down_Payment",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "DownPaymentSys");

            migrationBuilder.RenameColumn(
                name: "DiscountValues",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "DownPayment");

            migrationBuilder.RenameColumn(
                name: "Balance_Due_Sys",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "DiscountValue");

            migrationBuilder.RenameColumn(
                name: "Balance_Due",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "BalanceDueSys");

            migrationBuilder.RenameColumn(
                name: "Applied_Amount",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "BalanceDue");

            migrationBuilder.RenameColumn(
                name: "Additional_Note",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "AdditionalNote");

            migrationBuilder.RenameColumn(
                name: "Additional_Expense",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "AppliedAmountSys");

            migrationBuilder.RenameColumn(
                name: "TaxValuse",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "TaxValue");

            migrationBuilder.RenameColumn(
                name: "Sub_Total_sys",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "SubTotalsys");

            migrationBuilder.RenameColumn(
                name: "Sub_Total",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "SubTotal");

            migrationBuilder.RenameColumn(
                name: "Return_Amount",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "ReturnAmount");

            migrationBuilder.RenameColumn(
                name: "Reff_No",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "ReffNo");

            migrationBuilder.RenameColumn(
                name: "MemoRate",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "PurRate");

            migrationBuilder.RenameColumn(
                name: "MemoCurID",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "PurCurrencyID");

            migrationBuilder.RenameColumn(
                name: "Down_Payment",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "DownPaymentSys");

            migrationBuilder.RenameColumn(
                name: "DiscountValues",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "DownPayment");

            migrationBuilder.RenameColumn(
                name: "BaseOn",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "CopyKey");

            migrationBuilder.RenameColumn(
                name: "Balance_Due_Sys",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "DiscountValue");

            migrationBuilder.RenameColumn(
                name: "Balance_Due",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "BalanceDueSys");

            migrationBuilder.RenameColumn(
                name: "Applied_Amount",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "BalanceDue");

            migrationBuilder.RenameColumn(
                name: "Additional_Node",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "BasedCopyKeys");

            migrationBuilder.RenameColumn(
                name: "Additional_Expense",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "AppliedAmountSys");

            migrationBuilder.RenameColumn(
                name: "Total_Sys",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                newName: "TotalWTaxSys");

            migrationBuilder.RenameColumn(
                name: "TaxValuse",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "TaxValue");

            migrationBuilder.RenameColumn(
                name: "Sub_Total_sys",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "SubTotalSys");

            migrationBuilder.RenameColumn(
                name: "Sub_Total",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "SubTotal");

            migrationBuilder.RenameColumn(
                name: "Return_Amount",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "ReturnAmount");

            migrationBuilder.RenameColumn(
                name: "Reff_No",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "ReffNo");

            migrationBuilder.RenameColumn(
                name: "Down_Payment",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "DownPaymentSys");

            migrationBuilder.RenameColumn(
                name: "Balance_Due_Sys",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "DownPayment");

            migrationBuilder.RenameColumn(
                name: "Balance_Due",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "BalanceDueSys");

            migrationBuilder.RenameColumn(
                name: "Applied_Amount",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "BalanceDue");

            migrationBuilder.RenameColumn(
                name: "Additional_Note",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "AdditionalNote");

            migrationBuilder.RenameColumn(
                name: "Additional_Expense",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "AppliedAmountSys");

            migrationBuilder.RenameColumn(
                name: "Total_Sys",
                schema: "dbo",
                table: "tbGoodsReciptPODatail",
                newName: "TotalWTaxSys");

            migrationBuilder.RenameColumn(
                name: "TaxValuse",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "TaxValue");

            migrationBuilder.RenameColumn(
                name: "Sub_Total_sys",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "SubTotalSys");

            migrationBuilder.RenameColumn(
                name: "Sub_Total",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "SubTotal");

            migrationBuilder.RenameColumn(
                name: "Return_Amount",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "ReturnAmount");

            migrationBuilder.RenameColumn(
                name: "Reff_No",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "ReffNo");

            migrationBuilder.RenameColumn(
                name: "Down_Payment",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "DownPaymentSys");

            migrationBuilder.RenameColumn(
                name: "Balance_Due_Sys",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "DownPayment");

            migrationBuilder.RenameColumn(
                name: "Balance_Due",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "BalanceDueSys");

            migrationBuilder.RenameColumn(
                name: "Applied_Amount",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "BalanceDue");

            migrationBuilder.RenameColumn(
                name: "Additional_Note",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "CopyKey");

            migrationBuilder.RenameColumn(
                name: "Additional_Expense",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "AppliedAmountSys");

            migrationBuilder.RenameColumn(
                name: "Total_Sys",
                table: "tbSaleQuoteDetail",
                newName: "TotalWTaxSys");

            migrationBuilder.RenameColumn(
                name: "Total_Sys",
                table: "tbSaleDeliveryDetail",
                newName: "TotalWTaxSys");

            migrationBuilder.RenameColumn(
                name: "Total_Sys",
                table: "tbSaleCreditMemoDetail",
                newName: "TotalWTaxSys");

            migrationBuilder.RenameColumn(
                name: "Total_Sys",
                table: "tbSaleARDetail",
                newName: "TotalWTaxSys");

            migrationBuilder.RenameColumn(
                name: "check",
                table: "tbPurchaseCreditMemoDetail",
                newName: "Check");

            migrationBuilder.RenameColumn(
                name: "Total_Sys",
                table: "tbPurchaseCreditMemoDetail",
                newName: "TotalWTaxSys");

            migrationBuilder.RenameColumn(
                name: "APID",
                table: "tbPurchaseCreditMemoDetail",
                newName: "TaxGroupID");

            migrationBuilder.AddColumn<DateTime>(
                name: "AdmissionDate",
                schema: "dbo",
                table: "tbWarehouseDetail",
                type: "Date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BPID",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BatchAttr1",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BatchAttr2",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BatchNo",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Details",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Direction",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GRGIID",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InStockID",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOut",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LotNumber",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MfrDate",
                schema: "dbo",
                table: "tbWarehouseDetail",
                type: "Date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MfrSerialNumber",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MfrWarDateEnd",
                schema: "dbo",
                table: "tbWarehouseDetail",
                type: "Date",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MfrWarDateStart",
                schema: "dbo",
                table: "tbWarehouseDetail",
                type: "Date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OutStockID",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProcessItem",
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

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SysNum",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TransType",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WareDetialID",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisRate",
                schema: "dbo",
                table: "tbSaleOrderDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisValue",
                schema: "dbo",
                table: "tbSaleOrderDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinTotalValue",
                schema: "dbo",
                table: "tbSaleOrderDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbSaleOrderDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxOfFinDisValue",
                schema: "dbo",
                table: "tbSaleOrderDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                schema: "dbo",
                table: "tbSaleOrderDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxValue",
                schema: "dbo",
                table: "tbSaleOrderDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "TotalSys",
                schema: "dbo",
                table: "tbSaleOrderDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalWTax",
                schema: "dbo",
                table: "tbSaleOrderDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "FreightAmount",
                schema: "dbo",
                table: "tbSaleOrder",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FreightAmountSys",
                schema: "dbo",
                table: "tbSaleOrder",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDis",
                schema: "dbo",
                table: "tbSaleOrder",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDisSys",
                schema: "dbo",
                table: "tbSaleOrder",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalBefDis",
                schema: "dbo",
                table: "tbSaleOrder",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalBefDisSys",
                schema: "dbo",
                table: "tbSaleOrder",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisRate",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisValue",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinTotalValue",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxOfFinDisValue",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxValue",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "TotalWTax",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalWTaxSys",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Applied_AmountSys",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDis",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDisSys",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisRate",
                schema: "dbo",
                table: "tbPurchaseOrderDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisValue",
                schema: "dbo",
                table: "tbPurchaseOrderDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinTotalValue",
                schema: "dbo",
                table: "tbPurchaseOrderDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbPurchaseOrderDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxOfFinDisValue",
                schema: "dbo",
                table: "tbPurchaseOrderDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                schema: "dbo",
                table: "tbPurchaseOrderDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxValue",
                schema: "dbo",
                table: "tbPurchaseOrderDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "TotalSys",
                schema: "dbo",
                table: "tbPurchaseOrderDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalWTax",
                schema: "dbo",
                table: "tbPurchaseOrderDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AdditionalExpense",
                schema: "dbo",
                table: "tbPurchaseOrder",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AppliedAmount",
                schema: "dbo",
                table: "tbPurchaseOrder",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "FrieghtAmount",
                schema: "dbo",
                table: "tbPurchaseOrder",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FrieghtAmountSys",
                schema: "dbo",
                table: "tbPurchaseOrder",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDis",
                schema: "dbo",
                table: "tbPurchaseOrder",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDisSys",
                schema: "dbo",
                table: "tbPurchaseOrder",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "AdditionalExpense",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalNode",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AppliedAmount",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "CopyType",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDis",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDisSys",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisRate",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisValue",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinTotalValue",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseAPID",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxOfFinDisValue",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxValue",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "TotalSys",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalWTax",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AdditionalExpense",
                schema: "dbo",
                table: "tbPurchase_AP",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AppliedAmount",
                schema: "dbo",
                table: "tbPurchase_AP",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "BaseOnID",
                schema: "dbo",
                table: "tbPurchase_AP",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "FrieghtAmount",
                schema: "dbo",
                table: "tbPurchase_AP",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FrieghtAmountSys",
                schema: "dbo",
                table: "tbPurchase_AP",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDis",
                schema: "dbo",
                table: "tbPurchase_AP",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDisSys",
                schema: "dbo",
                table: "tbPurchase_AP",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ManItemBy",
                schema: "dbo",
                table: "tbItemMasterData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ManMethod",
                schema: "dbo",
                table: "tbItemMasterData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaxGroupSaleID",
                schema: "dbo",
                table: "tbItemMasterData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisRate",
                schema: "dbo",
                table: "tbGoodsReciptPODatail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisValue",
                schema: "dbo",
                table: "tbGoodsReciptPODatail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinTotalValue",
                schema: "dbo",
                table: "tbGoodsReciptPODatail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbGoodsReciptPODatail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxOfFinDisValue",
                schema: "dbo",
                table: "tbGoodsReciptPODatail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                schema: "dbo",
                table: "tbGoodsReciptPODatail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxValue",
                schema: "dbo",
                table: "tbGoodsReciptPODatail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "TotalSys",
                schema: "dbo",
                table: "tbGoodsReciptPODatail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalWTax",
                schema: "dbo",
                table: "tbGoodsReciptPODatail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AdditionalExpense",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalNote",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AppliedAmount",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "BasedCopyKeys",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FrieghtAmount",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FrieghtAmountSys",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDis",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDisSys",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisRate",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisValue",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinTotalValue",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxOfFinDisValue",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxValue",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "TotalWTax",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalWTaxSys",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Applied_AmountSys",
                schema: "dbo",
                table: "tbGoodReceiptReturn",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Down_PaymentSys",
                schema: "dbo",
                table: "tbGoodReceiptReturn",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDis",
                schema: "dbo",
                table: "tbGoodReceiptReturn",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDisSys",
                schema: "dbo",
                table: "tbGoodReceiptReturn",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "InStock",
                schema: "dbo",
                table: "ServiceItemSales",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisRate",
                table: "tbSaleQuoteDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisValue",
                table: "tbSaleQuoteDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinTotalValue",
                table: "tbSaleQuoteDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TaxGroupID",
                table: "tbSaleQuoteDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxOfFinDisValue",
                table: "tbSaleQuoteDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "tbSaleQuoteDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxValue",
                table: "tbSaleQuoteDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "TotalSys",
                table: "tbSaleQuoteDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalWTax",
                table: "tbSaleQuoteDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "FreightAmount",
                table: "tbSaleQuote",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FreightAmountSys",
                table: "tbSaleQuote",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDis",
                table: "tbSaleQuote",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDisSys",
                table: "tbSaleQuote",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalBefDis",
                table: "tbSaleQuote",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalBefDisSys",
                table: "tbSaleQuote",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisRate",
                table: "tbSaleDeliveryDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisValue",
                table: "tbSaleDeliveryDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinTotalValue",
                table: "tbSaleDeliveryDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TaxGroupID",
                table: "tbSaleDeliveryDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxOfFinDisValue",
                table: "tbSaleDeliveryDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "tbSaleDeliveryDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxValue",
                table: "tbSaleDeliveryDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "TotalSys",
                table: "tbSaleDeliveryDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalWTax",
                table: "tbSaleDeliveryDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "FreightAmount",
                table: "tbSaleDelivery",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FreightAmountSys",
                table: "tbSaleDelivery",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDis",
                table: "tbSaleDelivery",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDisSys",
                table: "tbSaleDelivery",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalBefDis",
                table: "tbSaleDelivery",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalBefDisSys",
                table: "tbSaleDelivery",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisRate",
                table: "tbSaleCreditMemoDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisValue",
                table: "tbSaleCreditMemoDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinTotalValue",
                table: "tbSaleCreditMemoDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxDownPaymentValue",
                table: "tbSaleCreditMemoDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TaxGroupID",
                table: "tbSaleCreditMemoDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxOfFinDisValue",
                table: "tbSaleCreditMemoDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "tbSaleCreditMemoDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxValue",
                table: "tbSaleCreditMemoDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "TotalSys",
                table: "tbSaleCreditMemoDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalWTax",
                table: "tbSaleCreditMemoDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisRate",
                table: "tbSaleARDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisValue",
                table: "tbSaleARDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinTotalValue",
                table: "tbSaleARDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TaxGroupID",
                table: "tbSaleARDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxOfFinDisValue",
                table: "tbSaleARDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "tbSaleARDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxValue",
                table: "tbSaleARDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "TotalSys",
                table: "tbSaleARDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalWTax",
                table: "tbSaleARDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "DownPayment",
                table: "tbSaleAR",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DownPaymentSys",
                table: "tbSaleAR",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FreightAmount",
                table: "tbSaleAR",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FreightAmountSys",
                table: "tbSaleAR",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDis",
                table: "tbSaleAR",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDisSys",
                table: "tbSaleAR",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalBefDis",
                table: "tbSaleAR",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalBefDisSys",
                table: "tbSaleAR",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisRate",
                table: "tbPurchaseCreditMemoDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinDisValue",
                table: "tbPurchaseCreditMemoDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinTotalValue",
                table: "tbPurchaseCreditMemoDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxOfFinDisValue",
                table: "tbPurchaseCreditMemoDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "tbPurchaseCreditMemoDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxValue",
                table: "tbPurchaseCreditMemoDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "TotalSys",
                table: "tbPurchaseCreditMemoDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalWTax",
                table: "tbPurchaseCreditMemoDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "DPMValue",
                table: "SaleCreditMemos",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DownPayment",
                table: "SaleCreditMemos",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DownPaymentSys",
                table: "SaleCreditMemos",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FreightAmount",
                table: "SaleCreditMemos",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FreightAmountSys",
                table: "SaleCreditMemos",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "SaleType",
                table: "SaleCreditMemos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDis",
                table: "SaleCreditMemos",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAfterDisSys",
                table: "SaleCreditMemos",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalBefDis",
                table: "SaleCreditMemos",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalBefDisSys",
                table: "SaleCreditMemos",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ARDownPayment",
                columns: table => new
                {
                    ARDID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CusID = table.Column<int>(nullable: false),
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
                    ExchangeRate = table.Column<decimal>(nullable: false),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DueDate = table.Column<DateTime>(type: "Date", nullable: false),
                    ValidUntilDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Total = table.Column<double>(nullable: false),
                    SubTotal = table.Column<double>(nullable: false),
                    SubTotalSys = table.Column<double>(nullable: false),
                    SubTotalBefDis = table.Column<decimal>(nullable: false),
                    SubTotalBefDisSys = table.Column<decimal>(nullable: false),
                    SubTotalAfterDis = table.Column<decimal>(nullable: false),
                    SubTotalAfterDisSys = table.Column<decimal>(nullable: false),
                    DPMRate = table.Column<decimal>(nullable: false),
                    DPMValue = table.Column<decimal>(nullable: false),
                    DisRate = table.Column<decimal>(nullable: false),
                    DisValue = table.Column<decimal>(nullable: false),
                    VatValue = table.Column<decimal>(nullable: false),
                    VatRate = table.Column<decimal>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    TotalAmount = table.Column<double>(nullable: false),
                    AppliedAmount = table.Column<decimal>(nullable: false),
                    BalanceDue = table.Column<decimal>(nullable: false),
                    BalanceDueSys = table.Column<decimal>(nullable: false),
                    AppliedAmountSys = table.Column<decimal>(nullable: false),
                    TotalAmountSys = table.Column<double>(nullable: false),
                    CopyType = table.Column<int>(nullable: false),
                    CopyKey = table.Column<string>(nullable: true),
                    BasedCopyKeys = table.Column<string>(nullable: true),
                    ChangeLog = table.Column<DateTime>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<decimal>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    ARID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARDownPayment", x => x.ARDID);
                });

            migrationBuilder.CreateTable(
                name: "ControlAccountsReceivable",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TypeOfAccount = table.Column<string>(nullable: true),
                    CustID = table.Column<int>(nullable: false),
                    GLAID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlAccountsReceivable", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Displays",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Prices = table.Column<int>(nullable: false),
                    Amounts = table.Column<int>(nullable: false),
                    Rates = table.Column<int>(nullable: false),
                    Quantities = table.Column<int>(nullable: false),
                    Units = table.Column<int>(nullable: false),
                    Percent = table.Column<int>(nullable: false),
                    DecimalSeparator = table.Column<string>(nullable: true),
                    ThousandsSep = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Displays", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FreightPurchase",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PurID = table.Column<int>(nullable: false),
                    PurType = table.Column<int>(nullable: false),
                    ExpenceAmount = table.Column<decimal>(nullable: false),
                    OpenExpenceAmount = table.Column<decimal>(nullable: false),
                    TaxSumValue = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreightPurchase", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FreightSale",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SaleID = table.Column<int>(nullable: false),
                    SaleType = table.Column<int>(nullable: false),
                    AmountReven = table.Column<decimal>(nullable: false),
                    OpenAmountReven = table.Column<decimal>(nullable: false),
                    TaxSumValue = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreightSale", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ReturnDelivery",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CusID = table.Column<int>(nullable: false),
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
                    ExchangeRate = table.Column<double>(nullable: false),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DueDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    SubTotalBefDis = table.Column<decimal>(nullable: false),
                    SubTotalBefDisSys = table.Column<decimal>(nullable: false),
                    SubTotalAfterDis = table.Column<decimal>(nullable: false),
                    SubTotalAfterDisSys = table.Column<decimal>(nullable: false),
                    FreightAmount = table.Column<decimal>(nullable: false),
                    FreightAmountSys = table.Column<decimal>(nullable: false),
                    SubTotal = table.Column<double>(nullable: false),
                    SubTotalSys = table.Column<double>(nullable: false),
                    DisRate = table.Column<decimal>(nullable: false),
                    DisValue = table.Column<decimal>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    VatRate = table.Column<double>(nullable: false),
                    VatValue = table.Column<double>(nullable: false),
                    AppliedAmount = table.Column<double>(nullable: false),
                    TotalAmount = table.Column<double>(nullable: false),
                    TotalAmountSys = table.Column<double>(nullable: false),
                    CopyType = table.Column<int>(nullable: false),
                    CopyKey = table.Column<string>(nullable: true),
                    BasedCopyKeys = table.Column<string>(nullable: true),
                    ChangeLog = table.Column<DateTime>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<decimal>(nullable: false),
                    BasedOn = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnDelivery", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SaleGLADeter",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CusID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleGLADeter", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "StockOut",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InStockID = table.Column<int>(nullable: false),
                    OutStockID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    SyetemDate = table.Column<DateTime>(type: "Date", nullable: false),
                    TimeIn = table.Column<DateTime>(nullable: false),
                    InStock = table.Column<decimal>(nullable: false),
                    Committed = table.Column<decimal>(nullable: false),
                    Ordered = table.Column<decimal>(nullable: false),
                    Available = table.Column<decimal>(nullable: false),
                    Factor = table.Column<decimal>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: true),
                    ItemID = table.Column<int>(nullable: false),
                    Cost = table.Column<decimal>(nullable: false),
                    MfrSerialNumber = table.Column<string>(nullable: true),
                    SerialNumber = table.Column<string>(nullable: true),
                    BatchNo = table.Column<string>(nullable: true),
                    BatchAttr1 = table.Column<string>(nullable: true),
                    BatchAttr2 = table.Column<string>(nullable: true),
                    MfrDate = table.Column<DateTime>(type: "Date", nullable: true),
                    AdmissionDate = table.Column<DateTime>(type: "Date", nullable: true),
                    Location = table.Column<string>(nullable: true),
                    Details = table.Column<string>(nullable: true),
                    SysNum = table.Column<int>(nullable: false),
                    LotNumber = table.Column<string>(nullable: true),
                    MfrWarDateStart = table.Column<DateTime>(type: "Date", nullable: true),
                    MfrWarDateEnd = table.Column<DateTime>(type: "Date", nullable: true),
                    TransType = table.Column<int>(nullable: false),
                    ProcessItem = table.Column<int>(nullable: false),
                    BPID = table.Column<int>(nullable: false),
                    Direction = table.Column<int>(nullable: false),
                    WareDetialID = table.Column<int>(nullable: false),
                    TransID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockOut", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ARDownPaymentDetail",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ARDID = table.Column<int>(nullable: false),
                    SQDID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    TaxGroupID = table.Column<int>(nullable: false),
                    TaxRate = table.Column<decimal>(nullable: false),
                    TaxValue = table.Column<decimal>(nullable: false),
                    TaxOfFinDisValue = table.Column<decimal>(nullable: false),
                    TaxDownPaymentValue = table.Column<decimal>(nullable: false),
                    FinTotalValue = table.Column<decimal>(nullable: false),
                    ItemCode = table.Column<string>(nullable: true),
                    ItemNameKH = table.Column<string>(nullable: true),
                    ItemNameEN = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(nullable: false),
                    OpenQty = table.Column<decimal>(nullable: false),
                    PrintQty = table.Column<decimal>(nullable: false),
                    GUomID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    UomName = table.Column<string>(nullable: true),
                    Factor = table.Column<decimal>(nullable: false),
                    Cost = table.Column<decimal>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false),
                    DisRate = table.Column<decimal>(nullable: false),
                    DisValue = table.Column<decimal>(nullable: false),
                    FinDisRate = table.Column<decimal>(nullable: false),
                    FinDisValue = table.Column<decimal>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    TotalSys = table.Column<double>(nullable: false),
                    TotalWTax = table.Column<decimal>(nullable: false),
                    TotalWTaxSys = table.Column<decimal>(nullable: false),
                    Process = table.Column<string>(nullable: true),
                    CurrencyID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    ItemType = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARDownPaymentDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ARDownPaymentDetail_ARDownPayment_ARDID",
                        column: x => x.ARDID,
                        principalTable: "ARDownPayment",
                        principalColumn: "ARDID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FreightPurchaseDetial",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TaxGroupID = table.Column<int>(nullable: false),
                    FreightID = table.Column<int>(nullable: false),
                    FreightPurchaseID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TaxGroup = table.Column<string>(nullable: true),
                    TaxRate = table.Column<decimal>(nullable: false),
                    TotalTaxAmount = table.Column<decimal>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    AmountWithTax = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreightPurchaseDetial", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FreightPurchaseDetial_FreightPurchase_FreightPurchaseID",
                        column: x => x.FreightPurchaseID,
                        principalTable: "FreightPurchase",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FreightSaleDetail",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TaxGroupID = table.Column<int>(nullable: false),
                    FreightID = table.Column<int>(nullable: false),
                    FreightSaleID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TaxGroup = table.Column<string>(nullable: true),
                    TaxRate = table.Column<decimal>(nullable: false),
                    TotalTaxAmount = table.Column<decimal>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    AmountWithTax = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreightSaleDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FreightSaleDetail_FreightSale_FreightSaleID",
                        column: x => x.FreightSaleID,
                        principalTable: "FreightSale",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReturnDeliveryDetail",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReturnDeliveryID = table.Column<int>(nullable: false),
                    SDDID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    ItemCode = table.Column<string>(nullable: true),
                    TaxGroupID = table.Column<int>(nullable: false),
                    TaxRate = table.Column<decimal>(nullable: false),
                    TaxValue = table.Column<decimal>(nullable: false),
                    TaxOfFinDisValue = table.Column<decimal>(nullable: false),
                    FinTotalValue = table.Column<decimal>(nullable: false),
                    ItemNameKH = table.Column<string>(nullable: true),
                    ItemNameEN = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    OpenQty = table.Column<double>(nullable: false),
                    PrintQty = table.Column<double>(nullable: false),
                    GUomID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    UomName = table.Column<string>(nullable: true),
                    Factor = table.Column<double>(nullable: false),
                    UnitPrice = table.Column<double>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    DisRate = table.Column<double>(nullable: false),
                    FinDisRate = table.Column<decimal>(nullable: false),
                    FinDisValue = table.Column<decimal>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    DisValue = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    TotalSys = table.Column<double>(nullable: false),
                    TotalWTax = table.Column<double>(nullable: false),
                    TotalWTaxSys = table.Column<double>(nullable: false),
                    Process = table.Column<string>(nullable: true),
                    CurrencyID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    ItemType = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnDeliveryDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReturnDeliveryDetail_ReturnDelivery_ReturnDeliveryID",
                        column: x => x.ReturnDeliveryID,
                        principalTable: "ReturnDelivery",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleGLAccountDetermination",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TypeOfAccount = table.Column<string>(nullable: true),
                    CusID = table.Column<int>(nullable: false),
                    GLID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    SaleGLDeterminationMasterID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleGLAccountDetermination", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SaleGLAccountDetermination_SaleGLADeter_SaleGLDeterminationMasterID",
                        column: x => x.SaleGLDeterminationMasterID,
                        principalTable: "SaleGLADeter",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleGLADeterRes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TypeOfAccount = table.Column<string>(nullable: true),
                    GLAID = table.Column<int>(nullable: false),
                    SaleGLDeterminationMasterID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleGLADeterRes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SaleGLADeterRes_SaleGLADeter_SaleGLDeterminationMasterID",
                        column: x => x.SaleGLDeterminationMasterID,
                        principalTable: "SaleGLADeter",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseAPDetail_PurchaseAPID",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                column: "PurchaseAPID");

            migrationBuilder.CreateIndex(
                name: "IX_ARDownPaymentDetail_ARDID",
                table: "ARDownPaymentDetail",
                column: "ARDID");

            migrationBuilder.CreateIndex(
                name: "IX_FreightPurchaseDetial_FreightPurchaseID",
                table: "FreightPurchaseDetial",
                column: "FreightPurchaseID");

            migrationBuilder.CreateIndex(
                name: "IX_FreightSaleDetail_FreightSaleID",
                table: "FreightSaleDetail",
                column: "FreightSaleID");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnDeliveryDetail_ReturnDeliveryID",
                table: "ReturnDeliveryDetail",
                column: "ReturnDeliveryID");

            migrationBuilder.CreateIndex(
                name: "IX_SaleGLAccountDetermination_SaleGLDeterminationMasterID",
                table: "SaleGLAccountDetermination",
                column: "SaleGLDeterminationMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_SaleGLADeterRes_SaleGLDeterminationMasterID",
                table: "SaleGLADeterRes",
                column: "SaleGLDeterminationMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_tbPurchaseAPDetail_tbPurchase_AP_PurchaseAPID",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                column: "PurchaseAPID",
                principalSchema: "dbo",
                principalTable: "tbPurchase_AP",
                principalColumn: "PurchaseAPID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbPurchaseAPDetail_tbPurchase_AP_PurchaseAPID",
                schema: "dbo",
                table: "tbPurchaseAPDetail");

            migrationBuilder.DropTable(
                name: "ARDownPaymentDetail");

            migrationBuilder.DropTable(
                name: "ControlAccountsReceivable");

            migrationBuilder.DropTable(
                name: "Displays");

            migrationBuilder.DropTable(
                name: "FreightPurchaseDetial");

            migrationBuilder.DropTable(
                name: "FreightSaleDetail");

            migrationBuilder.DropTable(
                name: "ReturnDeliveryDetail");

            migrationBuilder.DropTable(
                name: "SaleGLAccountDetermination");

            migrationBuilder.DropTable(
                name: "SaleGLADeterRes");

            migrationBuilder.DropTable(
                name: "StockOut");

            migrationBuilder.DropTable(
                name: "ARDownPayment");

            migrationBuilder.DropTable(
                name: "FreightPurchase");

            migrationBuilder.DropTable(
                name: "FreightSale");

            migrationBuilder.DropTable(
                name: "ReturnDelivery");

            migrationBuilder.DropTable(
                name: "SaleGLADeter");

            migrationBuilder.DropIndex(
                name: "IX_tbPurchaseAPDetail_PurchaseAPID",
                schema: "dbo",
                table: "tbPurchaseAPDetail");

            migrationBuilder.DropColumn(
                name: "AdmissionDate",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "BPID",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "BatchAttr1",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "BatchAttr2",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "BatchNo",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "Details",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "Direction",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "GRGIID",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "InStockID",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "IsOut",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "Location",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "LotNumber",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "MfrDate",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "MfrSerialNumber",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "MfrWarDateEnd",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "MfrWarDateStart",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "OutStockID",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "ProcessItem",
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
                name: "SerialNumber",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "SysNum",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "TransType",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "WareDetialID",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "FinDisRate",
                schema: "dbo",
                table: "tbSaleOrderDetail");

            migrationBuilder.DropColumn(
                name: "FinDisValue",
                schema: "dbo",
                table: "tbSaleOrderDetail");

            migrationBuilder.DropColumn(
                name: "FinTotalValue",
                schema: "dbo",
                table: "tbSaleOrderDetail");

            migrationBuilder.DropColumn(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbSaleOrderDetail");

            migrationBuilder.DropColumn(
                name: "TaxOfFinDisValue",
                schema: "dbo",
                table: "tbSaleOrderDetail");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                schema: "dbo",
                table: "tbSaleOrderDetail");

            migrationBuilder.DropColumn(
                name: "TaxValue",
                schema: "dbo",
                table: "tbSaleOrderDetail");

            migrationBuilder.DropColumn(
                name: "TotalSys",
                schema: "dbo",
                table: "tbSaleOrderDetail");

            migrationBuilder.DropColumn(
                name: "TotalWTax",
                schema: "dbo",
                table: "tbSaleOrderDetail");

            migrationBuilder.DropColumn(
                name: "FreightAmount",
                schema: "dbo",
                table: "tbSaleOrder");

            migrationBuilder.DropColumn(
                name: "FreightAmountSys",
                schema: "dbo",
                table: "tbSaleOrder");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDis",
                schema: "dbo",
                table: "tbSaleOrder");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDisSys",
                schema: "dbo",
                table: "tbSaleOrder");

            migrationBuilder.DropColumn(
                name: "SubTotalBefDis",
                schema: "dbo",
                table: "tbSaleOrder");

            migrationBuilder.DropColumn(
                name: "SubTotalBefDisSys",
                schema: "dbo",
                table: "tbSaleOrder");

            migrationBuilder.DropColumn(
                name: "FinDisRate",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail");

            migrationBuilder.DropColumn(
                name: "FinDisValue",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail");

            migrationBuilder.DropColumn(
                name: "FinTotalValue",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail");

            migrationBuilder.DropColumn(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail");

            migrationBuilder.DropColumn(
                name: "TaxOfFinDisValue",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail");

            migrationBuilder.DropColumn(
                name: "TaxValue",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail");

            migrationBuilder.DropColumn(
                name: "TotalWTax",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail");

            migrationBuilder.DropColumn(
                name: "TotalWTaxSys",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail");

            migrationBuilder.DropColumn(
                name: "Applied_AmountSys",
                schema: "dbo",
                table: "tbPurchaseQuotation");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDis",
                schema: "dbo",
                table: "tbPurchaseQuotation");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDisSys",
                schema: "dbo",
                table: "tbPurchaseQuotation");

            migrationBuilder.DropColumn(
                name: "FinDisRate",
                schema: "dbo",
                table: "tbPurchaseOrderDetail");

            migrationBuilder.DropColumn(
                name: "FinDisValue",
                schema: "dbo",
                table: "tbPurchaseOrderDetail");

            migrationBuilder.DropColumn(
                name: "FinTotalValue",
                schema: "dbo",
                table: "tbPurchaseOrderDetail");

            migrationBuilder.DropColumn(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbPurchaseOrderDetail");

            migrationBuilder.DropColumn(
                name: "TaxOfFinDisValue",
                schema: "dbo",
                table: "tbPurchaseOrderDetail");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                schema: "dbo",
                table: "tbPurchaseOrderDetail");

            migrationBuilder.DropColumn(
                name: "TaxValue",
                schema: "dbo",
                table: "tbPurchaseOrderDetail");

            migrationBuilder.DropColumn(
                name: "TotalSys",
                schema: "dbo",
                table: "tbPurchaseOrderDetail");

            migrationBuilder.DropColumn(
                name: "TotalWTax",
                schema: "dbo",
                table: "tbPurchaseOrderDetail");

            migrationBuilder.DropColumn(
                name: "AdditionalExpense",
                schema: "dbo",
                table: "tbPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "AppliedAmount",
                schema: "dbo",
                table: "tbPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "FrieghtAmount",
                schema: "dbo",
                table: "tbPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "FrieghtAmountSys",
                schema: "dbo",
                table: "tbPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDis",
                schema: "dbo",
                table: "tbPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDisSys",
                schema: "dbo",
                table: "tbPurchaseOrder");

            migrationBuilder.DropColumn(
                name: "AdditionalExpense",
                schema: "dbo",
                table: "tbPurchaseCreditMemo");

            migrationBuilder.DropColumn(
                name: "AdditionalNode",
                schema: "dbo",
                table: "tbPurchaseCreditMemo");

            migrationBuilder.DropColumn(
                name: "AppliedAmount",
                schema: "dbo",
                table: "tbPurchaseCreditMemo");

            migrationBuilder.DropColumn(
                name: "CopyType",
                schema: "dbo",
                table: "tbPurchaseCreditMemo");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDis",
                schema: "dbo",
                table: "tbPurchaseCreditMemo");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDisSys",
                schema: "dbo",
                table: "tbPurchaseCreditMemo");

            migrationBuilder.DropColumn(
                name: "FinDisRate",
                schema: "dbo",
                table: "tbPurchaseAPDetail");

            migrationBuilder.DropColumn(
                name: "FinDisValue",
                schema: "dbo",
                table: "tbPurchaseAPDetail");

            migrationBuilder.DropColumn(
                name: "FinTotalValue",
                schema: "dbo",
                table: "tbPurchaseAPDetail");

            migrationBuilder.DropColumn(
                name: "PurchaseAPID",
                schema: "dbo",
                table: "tbPurchaseAPDetail");

            migrationBuilder.DropColumn(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbPurchaseAPDetail");

            migrationBuilder.DropColumn(
                name: "TaxOfFinDisValue",
                schema: "dbo",
                table: "tbPurchaseAPDetail");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                schema: "dbo",
                table: "tbPurchaseAPDetail");

            migrationBuilder.DropColumn(
                name: "TaxValue",
                schema: "dbo",
                table: "tbPurchaseAPDetail");

            migrationBuilder.DropColumn(
                name: "TotalSys",
                schema: "dbo",
                table: "tbPurchaseAPDetail");

            migrationBuilder.DropColumn(
                name: "TotalWTax",
                schema: "dbo",
                table: "tbPurchaseAPDetail");

            migrationBuilder.DropColumn(
                name: "AdditionalExpense",
                schema: "dbo",
                table: "tbPurchase_AP");

            migrationBuilder.DropColumn(
                name: "AppliedAmount",
                schema: "dbo",
                table: "tbPurchase_AP");

            migrationBuilder.DropColumn(
                name: "BaseOnID",
                schema: "dbo",
                table: "tbPurchase_AP");

            migrationBuilder.DropColumn(
                name: "FrieghtAmount",
                schema: "dbo",
                table: "tbPurchase_AP");

            migrationBuilder.DropColumn(
                name: "FrieghtAmountSys",
                schema: "dbo",
                table: "tbPurchase_AP");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDis",
                schema: "dbo",
                table: "tbPurchase_AP");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDisSys",
                schema: "dbo",
                table: "tbPurchase_AP");

            migrationBuilder.DropColumn(
                name: "ManItemBy",
                schema: "dbo",
                table: "tbItemMasterData");

            migrationBuilder.DropColumn(
                name: "ManMethod",
                schema: "dbo",
                table: "tbItemMasterData");

            migrationBuilder.DropColumn(
                name: "TaxGroupSaleID",
                schema: "dbo",
                table: "tbItemMasterData");

            migrationBuilder.DropColumn(
                name: "FinDisRate",
                schema: "dbo",
                table: "tbGoodsReciptPODatail");

            migrationBuilder.DropColumn(
                name: "FinDisValue",
                schema: "dbo",
                table: "tbGoodsReciptPODatail");

            migrationBuilder.DropColumn(
                name: "FinTotalValue",
                schema: "dbo",
                table: "tbGoodsReciptPODatail");

            migrationBuilder.DropColumn(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbGoodsReciptPODatail");

            migrationBuilder.DropColumn(
                name: "TaxOfFinDisValue",
                schema: "dbo",
                table: "tbGoodsReciptPODatail");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                schema: "dbo",
                table: "tbGoodsReciptPODatail");

            migrationBuilder.DropColumn(
                name: "TaxValue",
                schema: "dbo",
                table: "tbGoodsReciptPODatail");

            migrationBuilder.DropColumn(
                name: "TotalSys",
                schema: "dbo",
                table: "tbGoodsReciptPODatail");

            migrationBuilder.DropColumn(
                name: "TotalWTax",
                schema: "dbo",
                table: "tbGoodsReciptPODatail");

            migrationBuilder.DropColumn(
                name: "AdditionalExpense",
                schema: "dbo",
                table: "tbGoodsReciptPO");

            migrationBuilder.DropColumn(
                name: "AdditionalNote",
                schema: "dbo",
                table: "tbGoodsReciptPO");

            migrationBuilder.DropColumn(
                name: "AppliedAmount",
                schema: "dbo",
                table: "tbGoodsReciptPO");

            migrationBuilder.DropColumn(
                name: "BasedCopyKeys",
                schema: "dbo",
                table: "tbGoodsReciptPO");

            migrationBuilder.DropColumn(
                name: "FrieghtAmount",
                schema: "dbo",
                table: "tbGoodsReciptPO");

            migrationBuilder.DropColumn(
                name: "FrieghtAmountSys",
                schema: "dbo",
                table: "tbGoodsReciptPO");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDis",
                schema: "dbo",
                table: "tbGoodsReciptPO");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDisSys",
                schema: "dbo",
                table: "tbGoodsReciptPO");

            migrationBuilder.DropColumn(
                name: "FinDisRate",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail");

            migrationBuilder.DropColumn(
                name: "FinDisValue",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail");

            migrationBuilder.DropColumn(
                name: "FinTotalValue",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail");

            migrationBuilder.DropColumn(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail");

            migrationBuilder.DropColumn(
                name: "TaxOfFinDisValue",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail");

            migrationBuilder.DropColumn(
                name: "TaxValue",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail");

            migrationBuilder.DropColumn(
                name: "TotalWTax",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail");

            migrationBuilder.DropColumn(
                name: "TotalWTaxSys",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail");

            migrationBuilder.DropColumn(
                name: "Applied_AmountSys",
                schema: "dbo",
                table: "tbGoodReceiptReturn");

            migrationBuilder.DropColumn(
                name: "Down_PaymentSys",
                schema: "dbo",
                table: "tbGoodReceiptReturn");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDis",
                schema: "dbo",
                table: "tbGoodReceiptReturn");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDisSys",
                schema: "dbo",
                table: "tbGoodReceiptReturn");

            migrationBuilder.DropColumn(
                name: "InStock",
                schema: "dbo",
                table: "ServiceItemSales");

            migrationBuilder.DropColumn(
                name: "FinDisRate",
                table: "tbSaleQuoteDetail");

            migrationBuilder.DropColumn(
                name: "FinDisValue",
                table: "tbSaleQuoteDetail");

            migrationBuilder.DropColumn(
                name: "FinTotalValue",
                table: "tbSaleQuoteDetail");

            migrationBuilder.DropColumn(
                name: "TaxGroupID",
                table: "tbSaleQuoteDetail");

            migrationBuilder.DropColumn(
                name: "TaxOfFinDisValue",
                table: "tbSaleQuoteDetail");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "tbSaleQuoteDetail");

            migrationBuilder.DropColumn(
                name: "TaxValue",
                table: "tbSaleQuoteDetail");

            migrationBuilder.DropColumn(
                name: "TotalSys",
                table: "tbSaleQuoteDetail");

            migrationBuilder.DropColumn(
                name: "TotalWTax",
                table: "tbSaleQuoteDetail");

            migrationBuilder.DropColumn(
                name: "FreightAmount",
                table: "tbSaleQuote");

            migrationBuilder.DropColumn(
                name: "FreightAmountSys",
                table: "tbSaleQuote");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDis",
                table: "tbSaleQuote");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDisSys",
                table: "tbSaleQuote");

            migrationBuilder.DropColumn(
                name: "SubTotalBefDis",
                table: "tbSaleQuote");

            migrationBuilder.DropColumn(
                name: "SubTotalBefDisSys",
                table: "tbSaleQuote");

            migrationBuilder.DropColumn(
                name: "FinDisRate",
                table: "tbSaleDeliveryDetail");

            migrationBuilder.DropColumn(
                name: "FinDisValue",
                table: "tbSaleDeliveryDetail");

            migrationBuilder.DropColumn(
                name: "FinTotalValue",
                table: "tbSaleDeliveryDetail");

            migrationBuilder.DropColumn(
                name: "TaxGroupID",
                table: "tbSaleDeliveryDetail");

            migrationBuilder.DropColumn(
                name: "TaxOfFinDisValue",
                table: "tbSaleDeliveryDetail");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "tbSaleDeliveryDetail");

            migrationBuilder.DropColumn(
                name: "TaxValue",
                table: "tbSaleDeliveryDetail");

            migrationBuilder.DropColumn(
                name: "TotalSys",
                table: "tbSaleDeliveryDetail");

            migrationBuilder.DropColumn(
                name: "TotalWTax",
                table: "tbSaleDeliveryDetail");

            migrationBuilder.DropColumn(
                name: "FreightAmount",
                table: "tbSaleDelivery");

            migrationBuilder.DropColumn(
                name: "FreightAmountSys",
                table: "tbSaleDelivery");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDis",
                table: "tbSaleDelivery");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDisSys",
                table: "tbSaleDelivery");

            migrationBuilder.DropColumn(
                name: "SubTotalBefDis",
                table: "tbSaleDelivery");

            migrationBuilder.DropColumn(
                name: "SubTotalBefDisSys",
                table: "tbSaleDelivery");

            migrationBuilder.DropColumn(
                name: "FinDisRate",
                table: "tbSaleCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "FinDisValue",
                table: "tbSaleCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "FinTotalValue",
                table: "tbSaleCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "TaxDownPaymentValue",
                table: "tbSaleCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "TaxGroupID",
                table: "tbSaleCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "TaxOfFinDisValue",
                table: "tbSaleCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "tbSaleCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "TaxValue",
                table: "tbSaleCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "TotalSys",
                table: "tbSaleCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "TotalWTax",
                table: "tbSaleCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "FinDisRate",
                table: "tbSaleARDetail");

            migrationBuilder.DropColumn(
                name: "FinDisValue",
                table: "tbSaleARDetail");

            migrationBuilder.DropColumn(
                name: "FinTotalValue",
                table: "tbSaleARDetail");

            migrationBuilder.DropColumn(
                name: "TaxGroupID",
                table: "tbSaleARDetail");

            migrationBuilder.DropColumn(
                name: "TaxOfFinDisValue",
                table: "tbSaleARDetail");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "tbSaleARDetail");

            migrationBuilder.DropColumn(
                name: "TaxValue",
                table: "tbSaleARDetail");

            migrationBuilder.DropColumn(
                name: "TotalSys",
                table: "tbSaleARDetail");

            migrationBuilder.DropColumn(
                name: "TotalWTax",
                table: "tbSaleARDetail");

            migrationBuilder.DropColumn(
                name: "DownPayment",
                table: "tbSaleAR");

            migrationBuilder.DropColumn(
                name: "DownPaymentSys",
                table: "tbSaleAR");

            migrationBuilder.DropColumn(
                name: "FreightAmount",
                table: "tbSaleAR");

            migrationBuilder.DropColumn(
                name: "FreightAmountSys",
                table: "tbSaleAR");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDis",
                table: "tbSaleAR");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDisSys",
                table: "tbSaleAR");

            migrationBuilder.DropColumn(
                name: "SubTotalBefDis",
                table: "tbSaleAR");

            migrationBuilder.DropColumn(
                name: "SubTotalBefDisSys",
                table: "tbSaleAR");

            migrationBuilder.DropColumn(
                name: "FinDisRate",
                table: "tbPurchaseCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "FinDisValue",
                table: "tbPurchaseCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "FinTotalValue",
                table: "tbPurchaseCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "TaxOfFinDisValue",
                table: "tbPurchaseCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "tbPurchaseCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "TaxValue",
                table: "tbPurchaseCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "TotalSys",
                table: "tbPurchaseCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "TotalWTax",
                table: "tbPurchaseCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "DPMValue",
                table: "SaleCreditMemos");

            migrationBuilder.DropColumn(
                name: "DownPayment",
                table: "SaleCreditMemos");

            migrationBuilder.DropColumn(
                name: "DownPaymentSys",
                table: "SaleCreditMemos");

            migrationBuilder.DropColumn(
                name: "FreightAmount",
                table: "SaleCreditMemos");

            migrationBuilder.DropColumn(
                name: "FreightAmountSys",
                table: "SaleCreditMemos");

            migrationBuilder.DropColumn(
                name: "SaleType",
                table: "SaleCreditMemos");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDis",
                table: "SaleCreditMemos");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDisSys",
                table: "SaleCreditMemos");

            migrationBuilder.DropColumn(
                name: "SubTotalBefDis",
                table: "SaleCreditMemos");

            migrationBuilder.DropColumn(
                name: "SubTotalBefDisSys",
                table: "SaleCreditMemos");

            migrationBuilder.RenameColumn(
                name: "TotalWTaxSys",
                schema: "dbo",
                table: "tbSaleOrderDetail",
                newName: "Total_Sys");

            migrationBuilder.RenameColumn(
                name: "TotalWTaxSys",
                schema: "dbo",
                table: "tbPurchaseOrderDetail",
                newName: "Total_Sys");

            migrationBuilder.RenameColumn(
                name: "TaxValue",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "TaxValues");

            migrationBuilder.RenameColumn(
                name: "SubTotalSys",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "Sub_Total_Sys");

            migrationBuilder.RenameColumn(
                name: "SubTotal",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "Sub_Total");

            migrationBuilder.RenameColumn(
                name: "ReturnAmount",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "Return_Amount");

            migrationBuilder.RenameColumn(
                name: "DownPaymentSys",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "Down_Payment");

            migrationBuilder.RenameColumn(
                name: "DownPayment",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "DiscountValues");

            migrationBuilder.RenameColumn(
                name: "DiscountValue",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "Balance_Due_Sys");

            migrationBuilder.RenameColumn(
                name: "BalanceDueSys",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "Balance_Due");

            migrationBuilder.RenameColumn(
                name: "BalanceDue",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "Applied_Amount");

            migrationBuilder.RenameColumn(
                name: "AppliedAmountSys",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "Additional_Expense");

            migrationBuilder.RenameColumn(
                name: "AdditionalNote",
                schema: "dbo",
                table: "tbPurchaseOrder",
                newName: "Additional_Note");

            migrationBuilder.RenameColumn(
                name: "TaxValue",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "TaxValuse");

            migrationBuilder.RenameColumn(
                name: "SubTotalsys",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "Sub_Total_sys");

            migrationBuilder.RenameColumn(
                name: "SubTotal",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "Sub_Total");

            migrationBuilder.RenameColumn(
                name: "ReturnAmount",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "Return_Amount");

            migrationBuilder.RenameColumn(
                name: "ReffNo",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "Reff_No");

            migrationBuilder.RenameColumn(
                name: "PurRate",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "MemoRate");

            migrationBuilder.RenameColumn(
                name: "PurCurrencyID",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "MemoCurID");

            migrationBuilder.RenameColumn(
                name: "DownPaymentSys",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "Down_Payment");

            migrationBuilder.RenameColumn(
                name: "DownPayment",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "DiscountValues");

            migrationBuilder.RenameColumn(
                name: "DiscountValue",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "Balance_Due_Sys");

            migrationBuilder.RenameColumn(
                name: "CopyKey",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "BaseOn");

            migrationBuilder.RenameColumn(
                name: "BasedCopyKeys",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "Additional_Node");

            migrationBuilder.RenameColumn(
                name: "BalanceDueSys",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "Balance_Due");

            migrationBuilder.RenameColumn(
                name: "BalanceDue",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "Applied_Amount");

            migrationBuilder.RenameColumn(
                name: "AppliedAmountSys",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "Additional_Expense");

            migrationBuilder.RenameColumn(
                name: "TotalWTaxSys",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                newName: "Total_Sys");

            migrationBuilder.RenameColumn(
                name: "TaxValue",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "TaxValuse");

            migrationBuilder.RenameColumn(
                name: "SubTotalSys",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "Sub_Total_sys");

            migrationBuilder.RenameColumn(
                name: "SubTotal",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "Sub_Total");

            migrationBuilder.RenameColumn(
                name: "ReturnAmount",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "Return_Amount");

            migrationBuilder.RenameColumn(
                name: "ReffNo",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "Reff_No");

            migrationBuilder.RenameColumn(
                name: "DownPaymentSys",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "Down_Payment");

            migrationBuilder.RenameColumn(
                name: "DownPayment",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "Balance_Due_Sys");

            migrationBuilder.RenameColumn(
                name: "BalanceDueSys",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "Balance_Due");

            migrationBuilder.RenameColumn(
                name: "BalanceDue",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "Applied_Amount");

            migrationBuilder.RenameColumn(
                name: "AppliedAmountSys",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "Additional_Expense");

            migrationBuilder.RenameColumn(
                name: "AdditionalNote",
                schema: "dbo",
                table: "tbPurchase_AP",
                newName: "Additional_Note");

            migrationBuilder.RenameColumn(
                name: "TotalWTaxSys",
                schema: "dbo",
                table: "tbGoodsReciptPODatail",
                newName: "Total_Sys");

            migrationBuilder.RenameColumn(
                name: "TaxValue",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "TaxValuse");

            migrationBuilder.RenameColumn(
                name: "SubTotalSys",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "Sub_Total_sys");

            migrationBuilder.RenameColumn(
                name: "SubTotal",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "Sub_Total");

            migrationBuilder.RenameColumn(
                name: "ReturnAmount",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "Return_Amount");

            migrationBuilder.RenameColumn(
                name: "ReffNo",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "Reff_No");

            migrationBuilder.RenameColumn(
                name: "DownPaymentSys",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "Down_Payment");

            migrationBuilder.RenameColumn(
                name: "DownPayment",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "Balance_Due_Sys");

            migrationBuilder.RenameColumn(
                name: "CopyKey",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "Additional_Note");

            migrationBuilder.RenameColumn(
                name: "BalanceDueSys",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "Balance_Due");

            migrationBuilder.RenameColumn(
                name: "BalanceDue",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "Applied_Amount");

            migrationBuilder.RenameColumn(
                name: "AppliedAmountSys",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                newName: "Additional_Expense");

            migrationBuilder.RenameColumn(
                name: "TotalWTaxSys",
                table: "tbSaleQuoteDetail",
                newName: "Total_Sys");

            migrationBuilder.RenameColumn(
                name: "TotalWTaxSys",
                table: "tbSaleDeliveryDetail",
                newName: "Total_Sys");

            migrationBuilder.RenameColumn(
                name: "TotalWTaxSys",
                table: "tbSaleCreditMemoDetail",
                newName: "Total_Sys");

            migrationBuilder.RenameColumn(
                name: "TotalWTaxSys",
                table: "tbSaleARDetail",
                newName: "Total_Sys");

            migrationBuilder.RenameColumn(
                name: "Check",
                table: "tbPurchaseCreditMemoDetail",
                newName: "check");

            migrationBuilder.RenameColumn(
                name: "TotalWTaxSys",
                table: "tbPurchaseCreditMemoDetail",
                newName: "Total_Sys");

            migrationBuilder.RenameColumn(
                name: "TaxGroupID",
                table: "tbPurchaseCreditMemoDetail",
                newName: "APID");

            migrationBuilder.AddColumn<int>(
                name: "Purchase_APID",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseAPDetail_Purchase_APID",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                column: "Purchase_APID");

            migrationBuilder.AddForeignKey(
                name: "FK_tbPurchaseAPDetail_tbPurchase_AP_Purchase_APID",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                column: "Purchase_APID",
                principalSchema: "dbo",
                principalTable: "tbPurchase_AP",
                principalColumn: "PurchaseAPID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
