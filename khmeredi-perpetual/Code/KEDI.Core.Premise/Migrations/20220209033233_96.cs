using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _96 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "TypeCards",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TypeDiscount",
                table: "TypeCards",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PendingVoidItem",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderID = table.Column<int>(nullable: false),
                    OrderNo = table.Column<string>(nullable: true),
                    TableID = table.Column<int>(nullable: false),
                    ReceiptNo = table.Column<string>(nullable: true),
                    QueueNo = table.Column<string>(nullable: true),
                    DateIn = table.Column<DateTime>(type: "Date", nullable: false),
                    DateOut = table.Column<DateTime>(type: "Date", nullable: false),
                    TimeIn = table.Column<string>(nullable: true),
                    TimeOut = table.Column<string>(nullable: true),
                    WaiterID = table.Column<int>(nullable: false),
                    UserOrderID = table.Column<int>(nullable: false),
                    UserDiscountID = table.Column<int>(nullable: false),
                    CustomerID = table.Column<int>(nullable: false),
                    CustomerCount = table.Column<int>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    Sub_Total = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    TaxRate = table.Column<double>(nullable: false),
                    TaxValue = table.Column<double>(nullable: false),
                    OtherPaymentGrandTotal = table.Column<decimal>(nullable: false),
                    GrandTotal = table.Column<double>(nullable: false),
                    GrandTotal_Sys = table.Column<double>(nullable: false),
                    Tip = table.Column<double>(nullable: false),
                    Received = table.Column<double>(nullable: false),
                    Change = table.Column<double>(nullable: false),
                    CurrencyDisplay = table.Column<string>(nullable: true),
                    DisplayRate = table.Column<double>(nullable: false),
                    GrandTotal_Display = table.Column<double>(nullable: false),
                    Change_Display = table.Column<double>(nullable: false),
                    PaymentMeansID = table.Column<int>(nullable: false),
                    CheckBill = table.Column<string>(nullable: false),
                    Cancel = table.Column<bool>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    PLCurrencyID = table.Column<int>(nullable: false),
                    PLRate = table.Column<double>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false),
                    Reason = table.Column<string>(nullable: true),
                    IsVoided = table.Column<bool>(nullable: false),
                    TaxOption = table.Column<int>(nullable: false),
                    PromoCodeID = table.Column<int>(nullable: false),
                    PromoCodeDiscRate = table.Column<double>(nullable: false),
                    PromoCodeDiscValue = table.Column<double>(nullable: false),
                    RemarkDiscountID = table.Column<int>(nullable: false),
                    BuyXAmountGetXDisID = table.Column<int>(nullable: false),
                    BuyXAmGetXDisRate = table.Column<decimal>(nullable: false),
                    BuyXAmGetXDisValue = table.Column<decimal>(nullable: false),
                    BuyXAmGetXDisType = table.Column<int>(nullable: false),
                    TaxGroupID = table.Column<int>(nullable: false),
                    GrandTotalCurrenciesDisplay = table.Column<string>(nullable: true),
                    ChangeCurrenciesDisplay = table.Column<string>(nullable: true),
                    GrandTotalOtherCurrenciesDisplay = table.Column<string>(nullable: true),
                    PaymentType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingVoidItem", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PendingVoidItemDetail",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PendingVoidItemID = table.Column<int>(nullable: false),
                    OrderDetailID = table.Column<int>(nullable: false),
                    OrderID = table.Column<int>(nullable: true),
                    Line_ID = table.Column<int>(nullable: false),
                    LineID = table.Column<string>(nullable: true),
                    ItemID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    PrintQty = table.Column<double>(nullable: false),
                    UnitPrice = table.Column<double>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    TaxGroupID = table.Column<int>(nullable: false),
                    TaxRate = table.Column<decimal>(nullable: false),
                    TaxValue = table.Column<decimal>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    TotalNet = table.Column<double>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    ItemStatus = table.Column<string>(nullable: true),
                    ItemPrintTo = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    ItemType = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ParentLineID = table.Column<string>(nullable: true),
                    ParentLevel = table.Column<string>(nullable: true),
                    IsVoided = table.Column<bool>(nullable: false),
                    KSServiceSetupId = table.Column<int>(nullable: false),
                    VehicleId = table.Column<int>(nullable: false),
                    IsKsms = table.Column<bool>(nullable: false),
                    IsKsmsMaster = table.Column<bool>(nullable: false),
                    IsScale = table.Column<bool>(nullable: false),
                    IsReadonly = table.Column<bool>(nullable: false),
                    ComboSaleType = table.Column<int>(nullable: false),
                    RemarkDiscountID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingVoidItemDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PendingVoidItemDetail_PendingVoidItem_PendingVoidItemID",
                        column: x => x.PendingVoidItemID,
                        principalTable: "PendingVoidItem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PendingVoidItemDetail_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PendingVoidItemDetail_PendingVoidItemID",
                table: "PendingVoidItemDetail",
                column: "PendingVoidItemID");

            migrationBuilder.CreateIndex(
                name: "IX_PendingVoidItemDetail_UomID",
                table: "PendingVoidItemDetail",
                column: "UomID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PendingVoidItemDetail");

            migrationBuilder.DropTable(
                name: "PendingVoidItem");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "TypeCards");

            migrationBuilder.DropColumn(
                name: "TypeDiscount",
                table: "TypeCards");
        }
    }
}
