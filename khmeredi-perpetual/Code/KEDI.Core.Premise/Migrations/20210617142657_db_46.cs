using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_46 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbSaleCreditMemoDetail_tbSaleCreditMemo_SCMOID",
                table: "tbSaleCreditMemoDetail");

            migrationBuilder.DropTable(
                name: "tbReceiptDetailMemoKvms",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbReceiptMemoKvms",
                schema: "dbo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbSaleCreditMemo",
                table: "tbSaleCreditMemo");

            migrationBuilder.RenameTable(
                name: "tbSaleCreditMemo",
                newName: "SaleCreditMemos");

            migrationBuilder.RenameColumn(
                name: "TotalAmount_Sys",
                table: "SaleCreditMemos",
                newName: "TotalAmountSys");

            migrationBuilder.RenameColumn(
                name: "SubTotal_Sys",
                table: "SaleCreditMemos",
                newName: "SubTotalSys");

            migrationBuilder.AddColumn<int>(
                name: "BasedOn",
                table: "SaleCreditMemos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SaleCreditMemos",
                table: "SaleCreditMemos",
                column: "SCMOID");

            migrationBuilder.CreateTable(
                name: "ReceiptDetailMemoKvms",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReceiptMemoID = table.Column<int>(nullable: false),
                    OrderDetailID = table.Column<int>(nullable: false),
                    OrderID = table.Column<int>(nullable: true),
                    Line_ID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    UnitPrice = table.Column<double>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    DisRate = table.Column<double>(nullable: false),
                    DisValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    Total = table.Column<double>(nullable: false),
                    TotalSys = table.Column<double>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    ItemStatus = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true),
                    ItemType = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ParentLevel = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptDetailMemoKvms", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptMemo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReceiptKvmsID = table.Column<int>(nullable: false),
                    OrderID = table.Column<int>(nullable: false),
                    OrderNo = table.Column<string>(nullable: false),
                    TableID = table.Column<int>(nullable: false),
                    ReceiptMemoNo = table.Column<string>(nullable: false),
                    ReceiptNo = table.Column<string>(nullable: false),
                    QueueNo = table.Column<string>(nullable: false),
                    DateIn = table.Column<DateTime>(type: "Date", nullable: false),
                    DateOut = table.Column<DateTime>(type: "Date", nullable: false),
                    TimeIn = table.Column<string>(nullable: false),
                    TimeOut = table.Column<string>(nullable: false),
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
                    SubTotal = table.Column<double>(nullable: false),
                    DisRate = table.Column<double>(nullable: false),
                    DisValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: false),
                    TaxRate = table.Column<double>(nullable: false),
                    TaxValue = table.Column<double>(nullable: false),
                    GrandTotal = table.Column<double>(nullable: false),
                    GrandTotalSys = table.Column<double>(nullable: false),
                    Tip = table.Column<double>(nullable: false),
                    Received = table.Column<double>(nullable: false),
                    Change = table.Column<double>(nullable: false),
                    CurrencyDisplay = table.Column<string>(nullable: true),
                    DisplayRate = table.Column<double>(nullable: false),
                    GrandTotalDisplay = table.Column<double>(nullable: false),
                    ChangeDisplay = table.Column<double>(nullable: false),
                    PaymentMeansID = table.Column<int>(nullable: false),
                    CheckBill = table.Column<string>(nullable: false),
                    Cancel = table.Column<bool>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    Return = table.Column<bool>(nullable: false),
                    PLCurrencyID = table.Column<int>(nullable: false),
                    PLRate = table.Column<double>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false),
                    AppliedAmount = table.Column<double>(nullable: false),
                    BalanceDue = table.Column<double>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    SeriesID = table.Column<int>(nullable: false),
                    SeriesDID = table.Column<int>(nullable: false),
                    DocTypeID = table.Column<int>(nullable: false),
                    BasedOn = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptMemo", x => x.ID);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_tbSaleCreditMemoDetail_SaleCreditMemos_SCMOID",
                table: "tbSaleCreditMemoDetail",
                column: "SCMOID",
                principalTable: "SaleCreditMemos",
                principalColumn: "SCMOID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbSaleCreditMemoDetail_SaleCreditMemos_SCMOID",
                table: "tbSaleCreditMemoDetail");

            migrationBuilder.DropTable(
                name: "ReceiptDetailMemoKvms");

            migrationBuilder.DropTable(
                name: "ReceiptMemo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SaleCreditMemos",
                table: "SaleCreditMemos");

            migrationBuilder.DropColumn(
                name: "BasedOn",
                table: "SaleCreditMemos");

            migrationBuilder.RenameTable(
                name: "SaleCreditMemos",
                newName: "tbSaleCreditMemo");

            migrationBuilder.RenameColumn(
                name: "TotalAmountSys",
                table: "tbSaleCreditMemo",
                newName: "TotalAmount_Sys");

            migrationBuilder.RenameColumn(
                name: "SubTotalSys",
                table: "tbSaleCreditMemo",
                newName: "SubTotal_Sys");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbSaleCreditMemo",
                table: "tbSaleCreditMemo",
                column: "SCMOID");

            migrationBuilder.CreateTable(
                name: "tbReceiptMemoKvms",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AppliedAmount = table.Column<double>(nullable: false),
                    BalanceDue = table.Column<double>(nullable: false),
                    BasedOn = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    Cancel = table.Column<bool>(nullable: false),
                    Change = table.Column<double>(nullable: false),
                    Change_Display = table.Column<double>(nullable: false),
                    CheckBill = table.Column<string>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    CurrencyDisplay = table.Column<string>(nullable: true),
                    CustomerCount = table.Column<int>(nullable: false),
                    CustomerID = table.Column<int>(nullable: false),
                    DateIn = table.Column<DateTime>(type: "Date", nullable: false),
                    DateOut = table.Column<DateTime>(type: "Date", nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    DisplayRate = table.Column<double>(nullable: false),
                    DocTypeID = table.Column<int>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    GrandTotal = table.Column<double>(nullable: false),
                    GrandTotal_Display = table.Column<double>(nullable: false),
                    GrandTotal_Sys = table.Column<double>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false),
                    OrderID = table.Column<int>(nullable: false),
                    OrderNo = table.Column<string>(nullable: false),
                    PLCurrencyID = table.Column<int>(nullable: false),
                    PLRate = table.Column<double>(nullable: false),
                    PaymentMeansID = table.Column<int>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    QueueNo = table.Column<string>(nullable: false),
                    ReceiptKvmsID = table.Column<int>(nullable: false),
                    ReceiptMemoNo = table.Column<string>(nullable: false),
                    ReceiptNo = table.Column<string>(nullable: false),
                    Received = table.Column<double>(nullable: false),
                    Return = table.Column<bool>(nullable: false),
                    SeriesDID = table.Column<int>(nullable: false),
                    SeriesID = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Sub_Total = table.Column<double>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    TableID = table.Column<int>(nullable: false),
                    TaxRate = table.Column<double>(nullable: false),
                    TaxValue = table.Column<double>(nullable: false),
                    TimeIn = table.Column<string>(nullable: false),
                    TimeOut = table.Column<string>(nullable: false),
                    Tip = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: false),
                    UserDiscountID = table.Column<int>(nullable: false),
                    UserOrderID = table.Column<int>(nullable: false),
                    WaiterID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbReceiptMemoKvms", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbReceiptMemoKvms_tbCurrency_LocalCurrencyID",
                        column: x => x.LocalCurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbReceiptMemoKvms_tbTable_TableID",
                        column: x => x.TableID,
                        principalSchema: "dbo",
                        principalTable: "tbTable",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbReceiptMemoKvms_tbUserAccount_UserOrderID",
                        column: x => x.UserOrderID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbReceiptDetailMemoKvms",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Cost = table.Column<double>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    EnglishName = table.Column<string>(nullable: true),
                    ItemID = table.Column<int>(nullable: false),
                    ItemStatus = table.Column<string>(nullable: true),
                    ItemType = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    Line_ID = table.Column<int>(nullable: false),
                    OrderDetailID = table.Column<int>(nullable: false),
                    OrderID = table.Column<int>(nullable: true),
                    ParentLevel = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    ReceiptMemoKvmsID = table.Column<int>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    UnitPrice = table.Column<double>(nullable: false),
                    UomID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbReceiptDetailMemoKvms", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbReceiptDetailMemoKvms_tbReceiptMemoKvms_ReceiptMemoKvmsID",
                        column: x => x.ReceiptMemoKvmsID,
                        principalSchema: "dbo",
                        principalTable: "tbReceiptMemoKvms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbReceiptDetailMemoKvms_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbReceiptDetailMemoKvms_ReceiptMemoKvmsID",
                schema: "dbo",
                table: "tbReceiptDetailMemoKvms",
                column: "ReceiptMemoKvmsID");

            migrationBuilder.CreateIndex(
                name: "IX_tbReceiptDetailMemoKvms_UomID",
                schema: "dbo",
                table: "tbReceiptDetailMemoKvms",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbReceiptMemoKvms_LocalCurrencyID",
                schema: "dbo",
                table: "tbReceiptMemoKvms",
                column: "LocalCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbReceiptMemoKvms_TableID",
                schema: "dbo",
                table: "tbReceiptMemoKvms",
                column: "TableID");

            migrationBuilder.CreateIndex(
                name: "IX_tbReceiptMemoKvms_UserOrderID",
                schema: "dbo",
                table: "tbReceiptMemoKvms",
                column: "UserOrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_tbSaleCreditMemoDetail_tbSaleCreditMemo_SCMOID",
                table: "tbSaleCreditMemoDetail",
                column: "SCMOID",
                principalTable: "tbSaleCreditMemo",
                principalColumn: "SCMOID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
