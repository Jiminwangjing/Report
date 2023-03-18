using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _109 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FreightProjCostDetails_ProjectCostAnalyses_ProjectCostAnalysisID",
                table: "FreightProjCostDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjCostAnalysisDetails_ProjectCostAnalyses_ProjectCostAnalysisID",
                table: "ProjCostAnalysisDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectCostAnalyses",
                table: "ProjectCostAnalyses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjCostAnalysisDetails",
                table: "ProjCostAnalysisDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FreightProjCostDetails",
                table: "FreightProjCostDetails");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "ProjectCostAnalyses");

            migrationBuilder.DropColumn(
                name: "SubTotalBeforeDis",
                table: "ProjectCostAnalyses");

            migrationBuilder.DropColumn(
                name: "Tax",
                table: "ProjectCostAnalyses");

            migrationBuilder.RenameTable(
                name: "ProjectCostAnalyses",
                newName: "tbProjectCostAnalysis");

            migrationBuilder.RenameTable(
                name: "ProjCostAnalysisDetails",
                newName: "tbProjCostAnalysisDetail");

            migrationBuilder.RenameTable(
                name: "FreightProjCostDetails",
                newName: "tbFreightProjCostDetail");

            migrationBuilder.RenameColumn(
                name: "SaleEmID",
                table: "tbProjectCostAnalysis",
                newName: "SaleEMID");

            migrationBuilder.RenameColumn(
                name: "Documentdate",
                table: "tbProjectCostAnalysis",
                newName: "DocumentDate");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "tbProjectCostAnalysis",
                newName: "TypeDis");

            migrationBuilder.RenameColumn(
                name: "SeriesNo",
                table: "tbProjectCostAnalysis",
                newName: "RefNo");

            migrationBuilder.RenameColumn(
                name: "CustomerRef",
                table: "tbProjectCostAnalysis",
                newName: "InvoiceNumber");

            migrationBuilder.RenameColumn(
                name: "CusContactID",
                table: "tbProjectCostAnalysis",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "Barcodereadign",
                table: "tbProjectCostAnalysis",
                newName: "InvoiceNo");

            migrationBuilder.RenameColumn(
                name: "ItemMaterDataID",
                table: "tbProjCostAnalysisDetail",
                newName: "ItemID");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "tbProjCostAnalysisDetail",
                newName: "UomName");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "tbProjCostAnalysisDetail",
                newName: "TypeDis");

            migrationBuilder.RenameColumn(
                name: "Barcode",
                table: "tbProjCostAnalysisDetail",
                newName: "Process");

            migrationBuilder.RenameIndex(
                name: "IX_ProjCostAnalysisDetails_ProjectCostAnalysisID",
                table: "tbProjCostAnalysisDetail",
                newName: "IX_tbProjCostAnalysisDetail_ProjectCostAnalysisID");

            migrationBuilder.RenameColumn(
                name: "ProjectCostAnalysisID",
                table: "tbFreightProjCostDetail",
                newName: "FreightProjectCostID");

            migrationBuilder.RenameIndex(
                name: "IX_FreightProjCostDetails_ProjectCostAnalysisID",
                table: "tbFreightProjCostDetail",
                newName: "IX_tbFreightProjCostDetail_FreightProjectCostID");

            migrationBuilder.AddColumn<int>(
                name: "BaseOnID",
                schema: "dbo",
                table: "tbSaleOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaleEmID",
                schema: "dbo",
                table: "tbSaleOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Children",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Female",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Male",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Children",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Female",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Male",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EnableCountMember",
                schema: "dbo",
                table: "tbGeneralSetting",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "EMType",
                schema: "dbo",
                table: "tbEmployee",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BaseonProjCostANID",
                table: "tbSaleQuote",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaleEmID",
                table: "tbSaleQuote",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BaseOnID",
                table: "tbSaleDelivery",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaleEmID",
                table: "tbSaleDelivery",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BaseOnID",
                table: "tbSaleAR",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaleEmID",
                table: "tbSaleAR",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaleEmID",
                table: "SaleCreditMemos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaleEmID",
                table: "ReturnDelivery",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaleEmID",
                table: "ARDownPayment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidUntilDate",
                table: "tbProjectCostAnalysis",
                type: "Date",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<double>(
                name: "TotalMargin",
                table: "tbProjectCostAnalysis",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "TotalCommission",
                table: "tbProjectCostAnalysis",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "TotalAmount",
                table: "tbProjectCostAnalysis",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "SubTotalAfterDis",
                table: "tbProjectCostAnalysis",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "tbProjectCostAnalysis",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PostingDate",
                table: "tbProjectCostAnalysis",
                type: "Date",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<double>(
                name: "OtherCost",
                table: "tbProjectCostAnalysis",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "FreightAmount",
                table: "tbProjectCostAnalysis",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "ExpectedTotalProfit",
                table: "tbProjectCostAnalysis",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "ExchangeRate",
                table: "tbProjectCostAnalysis",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DocumentDate",
                table: "tbProjectCostAnalysis",
                type: "Date",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<int>(
                name: "BranchID",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ChangeLog",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ConTactID",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "DisRate",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DisValue",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "DocTypeID",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "FreightAmountSys",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeVat",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LocalCurID",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "LocalSetRate",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "SaleCurrencyID",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "SubTotal",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SubTotalAfterDisSys",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SubTotalBefDis",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SubTotalBefDisSys",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SubTotalSys",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalAmountSys",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VatRate",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VatValue",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<double>(
                name: "UnitPrice",
                table: "tbProjCostAnalysisDetail",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "TotalWTax",
                table: "tbProjCostAnalysisDetail",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Total",
                table: "tbProjCostAnalysisDetail",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Qty",
                table: "tbProjCostAnalysisDetail",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "DisValue",
                table: "tbProjCostAnalysisDetail",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "DisRate",
                table: "tbProjCostAnalysisDetail",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Cost",
                table: "tbProjCostAnalysisDetail",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<int>(
                name: "CurrencyID",
                table: "tbProjCostAnalysisDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                table: "tbProjCostAnalysisDetail",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "Factor",
                table: "tbProjCostAnalysisDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "GUomID",
                table: "tbProjCostAnalysisDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ItemNameEN",
                table: "tbProjCostAnalysisDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemNameKH",
                table: "tbProjCostAnalysisDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemType",
                table: "tbProjCostAnalysisDetail",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OpenQty",
                table: "tbProjCostAnalysisDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalSys",
                table: "tbProjCostAnalysisDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalWTaxSys",
                table: "tbProjCostAnalysisDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VatRate",
                table: "tbProjCostAnalysisDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VatValue",
                table: "tbProjCostAnalysisDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "FreightID",
                table: "tbFreightProjCostDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TaxGroup",
                table: "tbFreightProjCostDetail",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbProjectCostAnalysis",
                table: "tbProjectCostAnalysis",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbProjCostAnalysisDetail",
                table: "tbProjCostAnalysisDetail",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbFreightProjCostDetail",
                table: "tbFreightProjCostDetail",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "tbFreightProjectCost",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjCAID = table.Column<int>(nullable: false),
                    SaleType = table.Column<int>(nullable: false),
                    AmountReven = table.Column<decimal>(nullable: false),
                    OpenAmountReven = table.Column<decimal>(nullable: false),
                    TaxSumValue = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbFreightProjectCost", x => x.ID);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_tbFreightProjCostDetail_tbFreightProjectCost_FreightProjectCostID",
                table: "tbFreightProjCostDetail",
                column: "FreightProjectCostID",
                principalTable: "tbFreightProjectCost",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbProjCostAnalysisDetail_tbProjectCostAnalysis_ProjectCostAnalysisID",
                table: "tbProjCostAnalysisDetail",
                column: "ProjectCostAnalysisID",
                principalTable: "tbProjectCostAnalysis",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbFreightProjCostDetail_tbFreightProjectCost_FreightProjectCostID",
                table: "tbFreightProjCostDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_tbProjCostAnalysisDetail_tbProjectCostAnalysis_ProjectCostAnalysisID",
                table: "tbProjCostAnalysisDetail");

            migrationBuilder.DropTable(
                name: "tbFreightProjectCost");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbProjectCostAnalysis",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbProjCostAnalysisDetail",
                table: "tbProjCostAnalysisDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbFreightProjCostDetail",
                table: "tbFreightProjCostDetail");

            migrationBuilder.DropColumn(
                name: "BaseOnID",
                schema: "dbo",
                table: "tbSaleOrder");

            migrationBuilder.DropColumn(
                name: "SaleEmID",
                schema: "dbo",
                table: "tbSaleOrder");

            migrationBuilder.DropColumn(
                name: "Children",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "Female",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "Male",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "Children",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "Female",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "Male",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "EnableCountMember",
                schema: "dbo",
                table: "tbGeneralSetting");

            migrationBuilder.DropColumn(
                name: "EMType",
                schema: "dbo",
                table: "tbEmployee");

            migrationBuilder.DropColumn(
                name: "BaseonProjCostANID",
                table: "tbSaleQuote");

            migrationBuilder.DropColumn(
                name: "SaleEmID",
                table: "tbSaleQuote");

            migrationBuilder.DropColumn(
                name: "BaseOnID",
                table: "tbSaleDelivery");

            migrationBuilder.DropColumn(
                name: "SaleEmID",
                table: "tbSaleDelivery");

            migrationBuilder.DropColumn(
                name: "BaseOnID",
                table: "tbSaleAR");

            migrationBuilder.DropColumn(
                name: "SaleEmID",
                table: "tbSaleAR");

            migrationBuilder.DropColumn(
                name: "SaleEmID",
                table: "SaleCreditMemos");

            migrationBuilder.DropColumn(
                name: "SaleEmID",
                table: "ReturnDelivery");

            migrationBuilder.DropColumn(
                name: "SaleEmID",
                table: "ARDownPayment");

            migrationBuilder.DropColumn(
                name: "BranchID",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "ConTactID",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "DisRate",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "DisValue",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "DocTypeID",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "FreightAmountSys",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "IncludeVat",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "LocalCurID",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "LocalSetRate",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "SaleCurrencyID",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "SubTotalAfterDisSys",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "SubTotalBefDis",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "SubTotalBefDisSys",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "SubTotalSys",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "TotalAmountSys",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "VatValue",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "CurrencyID",
                table: "tbProjCostAnalysisDetail");

            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "tbProjCostAnalysisDetail");

            migrationBuilder.DropColumn(
                name: "Factor",
                table: "tbProjCostAnalysisDetail");

            migrationBuilder.DropColumn(
                name: "GUomID",
                table: "tbProjCostAnalysisDetail");

            migrationBuilder.DropColumn(
                name: "ItemNameEN",
                table: "tbProjCostAnalysisDetail");

            migrationBuilder.DropColumn(
                name: "ItemNameKH",
                table: "tbProjCostAnalysisDetail");

            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "tbProjCostAnalysisDetail");

            migrationBuilder.DropColumn(
                name: "OpenQty",
                table: "tbProjCostAnalysisDetail");

            migrationBuilder.DropColumn(
                name: "TotalSys",
                table: "tbProjCostAnalysisDetail");

            migrationBuilder.DropColumn(
                name: "TotalWTaxSys",
                table: "tbProjCostAnalysisDetail");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "tbProjCostAnalysisDetail");

            migrationBuilder.DropColumn(
                name: "VatValue",
                table: "tbProjCostAnalysisDetail");

            migrationBuilder.DropColumn(
                name: "FreightID",
                table: "tbFreightProjCostDetail");

            migrationBuilder.DropColumn(
                name: "TaxGroup",
                table: "tbFreightProjCostDetail");

            migrationBuilder.RenameTable(
                name: "tbProjectCostAnalysis",
                newName: "ProjectCostAnalyses");

            migrationBuilder.RenameTable(
                name: "tbProjCostAnalysisDetail",
                newName: "ProjCostAnalysisDetails");

            migrationBuilder.RenameTable(
                name: "tbFreightProjCostDetail",
                newName: "FreightProjCostDetails");

            migrationBuilder.RenameColumn(
                name: "SaleEMID",
                table: "ProjectCostAnalyses",
                newName: "SaleEmID");

            migrationBuilder.RenameColumn(
                name: "DocumentDate",
                table: "ProjectCostAnalyses",
                newName: "Documentdate");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "ProjectCostAnalyses",
                newName: "CusContactID");

            migrationBuilder.RenameColumn(
                name: "TypeDis",
                table: "ProjectCostAnalyses",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "RefNo",
                table: "ProjectCostAnalyses",
                newName: "SeriesNo");

            migrationBuilder.RenameColumn(
                name: "InvoiceNumber",
                table: "ProjectCostAnalyses",
                newName: "CustomerRef");

            migrationBuilder.RenameColumn(
                name: "InvoiceNo",
                table: "ProjectCostAnalyses",
                newName: "Barcodereadign");

            migrationBuilder.RenameColumn(
                name: "UomName",
                table: "ProjCostAnalysisDetails",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "TypeDis",
                table: "ProjCostAnalysisDetails",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "Process",
                table: "ProjCostAnalysisDetails",
                newName: "Barcode");

            migrationBuilder.RenameColumn(
                name: "ItemID",
                table: "ProjCostAnalysisDetails",
                newName: "ItemMaterDataID");

            migrationBuilder.RenameIndex(
                name: "IX_tbProjCostAnalysisDetail_ProjectCostAnalysisID",
                table: "ProjCostAnalysisDetails",
                newName: "IX_ProjCostAnalysisDetails_ProjectCostAnalysisID");

            migrationBuilder.RenameColumn(
                name: "FreightProjectCostID",
                table: "FreightProjCostDetails",
                newName: "ProjectCostAnalysisID");

            migrationBuilder.RenameIndex(
                name: "IX_tbFreightProjCostDetail_FreightProjectCostID",
                table: "FreightProjCostDetails",
                newName: "IX_FreightProjCostDetails_ProjectCostAnalysisID");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidUntilDate",
                table: "ProjectCostAnalyses",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "Date");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalMargin",
                table: "ProjectCostAnalyses",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCommission",
                table: "ProjectCostAnalyses",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "ProjectCostAnalyses",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "SubTotalAfterDis",
                table: "ProjectCostAnalyses",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ProjectCostAnalyses",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PostingDate",
                table: "ProjectCostAnalyses",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "Date");

            migrationBuilder.AlterColumn<decimal>(
                name: "OtherCost",
                table: "ProjectCostAnalyses",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "FreightAmount",
                table: "ProjectCostAnalyses",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpectedTotalProfit",
                table: "ProjectCostAnalyses",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<string>(
                name: "ExchangeRate",
                table: "ProjectCostAnalyses",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Documentdate",
                table: "ProjectCostAnalyses",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "Date");

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "ProjectCostAnalyses",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalBeforeDis",
                table: "ProjectCostAnalyses",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Tax",
                table: "ProjectCostAnalyses",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                table: "ProjCostAnalysisDetails",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalWTax",
                table: "ProjCostAnalysisDetails",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "ProjCostAnalysisDetails",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Qty",
                table: "ProjCostAnalysisDetails",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "DisValue",
                table: "ProjCostAnalysisDetails",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "DisRate",
                table: "ProjCostAnalysisDetails",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "ProjCostAnalysisDetails",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectCostAnalyses",
                table: "ProjectCostAnalyses",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjCostAnalysisDetails",
                table: "ProjCostAnalysisDetails",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FreightProjCostDetails",
                table: "FreightProjCostDetails",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_FreightProjCostDetails_ProjectCostAnalyses_ProjectCostAnalysisID",
                table: "FreightProjCostDetails",
                column: "ProjectCostAnalysisID",
                principalTable: "ProjectCostAnalyses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjCostAnalysisDetails_ProjectCostAnalyses_ProjectCostAnalysisID",
                table: "ProjCostAnalysisDetails",
                column: "ProjectCostAnalysisID",
                principalTable: "ProjectCostAnalyses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
