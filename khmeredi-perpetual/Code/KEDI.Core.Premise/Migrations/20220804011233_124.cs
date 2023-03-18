using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _124 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartTime",
                schema: "dbo",
                table: "tbPromotion");

            migrationBuilder.DropColumn(
                name: "StopTime",
                schema: "dbo",
                table: "tbPromotion");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StopDate",
                schema: "dbo",
                table: "tbPromotion",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "Date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                schema: "dbo",
                table: "tbPromotion",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "Date");

            migrationBuilder.AddColumn<bool>(
                name: "Cash",
                schema: "dbo",
                table: "tbGeneralSetting",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "MultipayMeansSetting",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SettingID = table.Column<int>(nullable: false),
                    PaymentID = table.Column<int>(nullable: false),
                    Check = table.Column<bool>(nullable: false),
                    Changed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultipayMeansSetting", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MultiPaymentMean",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReceiptID = table.Column<int>(nullable: false),
                    PaymentMeanID = table.Column<int>(nullable: false),
                    AltCurrencyID = table.Column<int>(nullable: false),
                    AltCurrency = table.Column<string>(nullable: true),
                    AltRate = table.Column<int>(nullable: false),
                    PLCurrencyID = table.Column<int>(nullable: false),
                    PLCurrency = table.Column<string>(nullable: true),
                    PLRate = table.Column<double>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(36,18)", nullable: false),
                    OpenAmount = table.Column<decimal>(type: "decimal(36,18)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(36,18)", nullable: false),
                    SCRate = table.Column<decimal>(type: "decimal(36,18)", nullable: false),
                    LCRate = table.Column<decimal>(type: "decimal(36,18)", nullable: false),
                    ReturnStatus = table.Column<bool>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Exceed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultiPaymentMean", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SaleAREditReport",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CusID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    TimeIn = table.Column<string>(nullable: false),
                    SaleCurrencyID = table.Column<int>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    InvoiceNumber = table.Column<string>(nullable: true),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DisValue = table.Column<double>(nullable: false),
                    SubTotal = table.Column<double>(nullable: false),
                    VatRate = table.Column<double>(nullable: false),
                    VatValue = table.Column<double>(nullable: false),
                    AppliedAmount = table.Column<double>(nullable: false),
                    TotalAmount = table.Column<double>(nullable: false),
                    TotalAmountSys = table.Column<double>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleAREditReport", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SettingPayment",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PaymentMeansID = table.Column<int>(nullable: false),
                    SettingID = table.Column<int>(nullable: false),
                    Payment = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingPayment", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SaleAREditDetailReport",
                columns: table => new
                {
                    SARDID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ID = table.Column<int>(nullable: false),
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
                    EditeQty = table.Column<double>(nullable: false),
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
                    table.PrimaryKey("PK_SaleAREditDetailReport", x => x.SARDID);
                    table.ForeignKey(
                        name: "FK_SaleAREditDetailReport_SaleAREditReport_ID",
                        column: x => x.ID,
                        principalTable: "SaleAREditReport",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleAREditDetailReport_ID",
                table: "SaleAREditDetailReport",
                column: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MultipayMeansSetting");

            migrationBuilder.DropTable(
                name: "MultiPaymentMean");

            migrationBuilder.DropTable(
                name: "SaleAREditDetailReport");

            migrationBuilder.DropTable(
                name: "SettingPayment");

            migrationBuilder.DropTable(
                name: "SaleAREditReport");

            migrationBuilder.DropColumn(
                name: "Cash",
                schema: "dbo",
                table: "tbGeneralSetting");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StopDate",
                schema: "dbo",
                table: "tbPromotion",
                type: "Date",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                schema: "dbo",
                table: "tbPromotion",
                type: "Date",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                schema: "dbo",
                table: "tbPromotion",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StopTime",
                schema: "dbo",
                table: "tbPromotion",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
