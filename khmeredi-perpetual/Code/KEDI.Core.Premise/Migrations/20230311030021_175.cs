using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _175 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SelfSyncEntity");

            migrationBuilder.AddColumn<bool>(
                name: "CusConsignment",
                schema: "dbo",
                table: "tbWarhouse",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "VenderConsignment",
                schema: "dbo",
                table: "tbWarhouse",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                schema: "dbo",
                table: "tbPromotionDetail",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StopTime",
                schema: "dbo",
                table: "tbPromotionDetail",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "BaseOnID",
                schema: "dbo",
                table: "tbOutgoingpayment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BasedOnID",
                schema: "dbo",
                table: "tbIncomingPayment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CopyType",
                schema: "dbo",
                table: "tbIncomingPayment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LoanPartnerID",
                table: "tbSaleARDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LoanPartnerID",
                table: "DraftARDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "IncomingPaymentOrders",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<int>(nullable: false),
                    SeriesID = table.Column<int>(nullable: false),
                    SeriesDID = table.Column<int>(nullable: false),
                    DocTypeID = table.Column<int>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    PaymentMeanID = table.Column<int>(nullable: false),
                    InvoiceNumber = table.Column<string>(nullable: true),
                    CustomerID = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    BranchID = table.Column<int>(nullable: false),
                    Ref_No = table.Column<string>(nullable: true),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    TotalAmountDue = table.Column<double>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false),
                    IcoPayCusID = table.Column<int>(nullable: false),
                    TotalApplied = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingPaymentOrders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_IncomingPaymentOrders_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncomingPaymentOrders_tbBusinessPartner_CustomerID",
                        column: x => x.CustomerID,
                        principalSchema: "dbo",
                        principalTable: "tbBusinessPartner",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncomingPaymentOrders_tbUserAccount_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbInventoryCounting",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DocTypeID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    SeriesID = table.Column<int>(nullable: false),
                    SeriesDetailID = table.Column<int>(nullable: false),
                    InvioceNumber = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Ref_No = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(type: "Date", nullable: false),
                    Time = table.Column<TimeSpan>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbInventoryCounting", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbOutgoingpaymentOrder",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<int>(nullable: false),
                    SeriesID = table.Column<int>(nullable: false),
                    SeriesDetailID = table.Column<int>(nullable: false),
                    DocumentID = table.Column<int>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    NumberInvioce = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    PaymentMeanID = table.Column<int>(nullable: false),
                    VendorID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    Ref_No = table.Column<string>(nullable: true),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    TotalAmountDue = table.Column<double>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false),
                    TypePurchase = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbOutgoingpaymentOrder", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbOutgoingpaymentOrder_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbOutgoingpaymentOrder_tbUserAccount_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbOutgoingpaymentOrder_tbBusinessPartner_VendorID",
                        column: x => x.VendorID,
                        principalSchema: "dbo",
                        principalTable: "tbBusinessPartner",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncomingPaymentOrderDetails",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IncomingPaymentOrderID = table.Column<int>(nullable: false),
                    InvoiceNumber = table.Column<string>(nullable: true),
                    ItemInvoice = table.Column<string>(nullable: true),
                    DocNo = table.Column<string>(nullable: true),
                    CheckPay = table.Column<bool>(nullable: false),
                    DocTypeID = table.Column<int>(nullable: false),
                    DocumentNo = table.Column<string>(nullable: true),
                    DocumentType = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(type: "Date", nullable: false),
                    OverdueDays = table.Column<double>(nullable: false),
                    CurrencyName = table.Column<string>(nullable: true),
                    Total = table.Column<double>(nullable: false),
                    BalanceDue = table.Column<double>(nullable: false),
                    Applied_Amount = table.Column<double>(nullable: false),
                    CashDiscount = table.Column<double>(nullable: false),
                    TotalDiscount = table.Column<double>(nullable: false),
                    Totalpayment = table.Column<double>(nullable: false),
                    OpenTotalpayment = table.Column<double>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false),
                    IcoPayCusID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingPaymentOrderDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_IncomingPaymentOrderDetails_tbCurrency_CurrencyID",
                        column: x => x.CurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncomingPaymentOrderDetails_IncomingPaymentOrders_IncomingPaymentOrderID",
                        column: x => x.IncomingPaymentOrderID,
                        principalTable: "IncomingPaymentOrders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MultiIncomingPaymentOrders",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PaymentMeanID = table.Column<int>(nullable: false),
                    SCRate = table.Column<decimal>(nullable: false),
                    IncomingPaymentOrderID = table.Column<int>(nullable: false),
                    OpenAmount = table.Column<decimal>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    AmmountSys = table.Column<decimal>(nullable: false),
                    GLAccID = table.Column<int>(nullable: false),
                    CurrID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultiIncomingPaymentOrders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MultiIncomingPaymentOrders_IncomingPaymentOrders_IncomingPaymentOrderID",
                        column: x => x.IncomingPaymentOrderID,
                        principalTable: "IncomingPaymentOrders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbInventoryCountingDetail",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InventoryCountingID = table.Column<int>(nullable: false),
                    EmployeeID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    InstockQty = table.Column<double>(nullable: false),
                    Counted = table.Column<bool>(nullable: false),
                    UomCountQty = table.Column<double>(nullable: false),
                    CountedQty = table.Column<double>(nullable: false),
                    Varaince = table.Column<double>(nullable: false),
                    UomName = table.Column<string>(nullable: true),
                    EmName = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbInventoryCountingDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbInventoryCountingDetail_tbInventoryCounting_InventoryCountingID",
                        column: x => x.InventoryCountingID,
                        principalSchema: "dbo",
                        principalTable: "tbInventoryCounting",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbOutgoingpaymnetOrderDetail",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OutgoingPaymentOrderID = table.Column<int>(nullable: false),
                    NumberInvioce = table.Column<string>(nullable: true),
                    ItemInvoice = table.Column<string>(nullable: true),
                    DocNo = table.Column<string>(nullable: true),
                    CheckPay = table.Column<bool>(nullable: false),
                    DocTypeID = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(type: "Date", nullable: false),
                    OverdueDays = table.Column<double>(nullable: false),
                    CurrencyName = table.Column<string>(nullable: true),
                    Total = table.Column<double>(nullable: false),
                    BalanceDue = table.Column<double>(nullable: false),
                    CashDiscount = table.Column<double>(nullable: false),
                    TotalDiscount = table.Column<double>(nullable: false),
                    Totalpayment = table.Column<double>(nullable: false),
                    Applied_Amount = table.Column<double>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    BasedOnID = table.Column<int>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbOutgoingpaymnetOrderDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbOutgoingpaymnetOrderDetail_tbCurrency_CurrencyID",
                        column: x => x.CurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbOutgoingpaymnetOrderDetail_tbOutgoingpaymentOrder_OutgoingPaymentOrderID",
                        column: x => x.OutgoingPaymentOrderID,
                        principalSchema: "dbo",
                        principalTable: "tbOutgoingpaymentOrder",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncomingPaymentOrderDetails_CurrencyID",
                table: "IncomingPaymentOrderDetails",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingPaymentOrderDetails_IncomingPaymentOrderID",
                table: "IncomingPaymentOrderDetails",
                column: "IncomingPaymentOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingPaymentOrders_BranchID",
                table: "IncomingPaymentOrders",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingPaymentOrders_CustomerID",
                table: "IncomingPaymentOrders",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingPaymentOrders_UserID",
                table: "IncomingPaymentOrders",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_MultiIncomingPaymentOrders_IncomingPaymentOrderID",
                table: "MultiIncomingPaymentOrders",
                column: "IncomingPaymentOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_tbInventoryCountingDetail_InventoryCountingID",
                schema: "dbo",
                table: "tbInventoryCountingDetail",
                column: "InventoryCountingID");

            migrationBuilder.CreateIndex(
                name: "IX_tbOutgoingpaymentOrder_BranchID",
                schema: "dbo",
                table: "tbOutgoingpaymentOrder",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_tbOutgoingpaymentOrder_UserID",
                schema: "dbo",
                table: "tbOutgoingpaymentOrder",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbOutgoingpaymentOrder_VendorID",
                schema: "dbo",
                table: "tbOutgoingpaymentOrder",
                column: "VendorID");

            migrationBuilder.CreateIndex(
                name: "IX_tbOutgoingpaymnetOrderDetail_CurrencyID",
                schema: "dbo",
                table: "tbOutgoingpaymnetOrderDetail",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbOutgoingpaymnetOrderDetail_OutgoingPaymentOrderID",
                schema: "dbo",
                table: "tbOutgoingpaymnetOrderDetail",
                column: "OutgoingPaymentOrderID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncomingPaymentOrderDetails");

            migrationBuilder.DropTable(
                name: "MultiIncomingPaymentOrders");

            migrationBuilder.DropTable(
                name: "tbInventoryCountingDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbOutgoingpaymnetOrderDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "IncomingPaymentOrders");

            migrationBuilder.DropTable(
                name: "tbInventoryCounting",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbOutgoingpaymentOrder",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "CusConsignment",
                schema: "dbo",
                table: "tbWarhouse");

            migrationBuilder.DropColumn(
                name: "VenderConsignment",
                schema: "dbo",
                table: "tbWarhouse");

            migrationBuilder.DropColumn(
                name: "StartTime",
                schema: "dbo",
                table: "tbPromotionDetail");

            migrationBuilder.DropColumn(
                name: "StopTime",
                schema: "dbo",
                table: "tbPromotionDetail");

            migrationBuilder.DropColumn(
                name: "BaseOnID",
                schema: "dbo",
                table: "tbOutgoingpayment");

            migrationBuilder.DropColumn(
                name: "BasedOnID",
                schema: "dbo",
                table: "tbIncomingPayment");

            migrationBuilder.DropColumn(
                name: "CopyType",
                schema: "dbo",
                table: "tbIncomingPayment");

            migrationBuilder.DropColumn(
                name: "LoanPartnerID",
                table: "tbSaleARDetail");

            migrationBuilder.DropColumn(
                name: "LoanPartnerID",
                table: "DraftARDetail");

            migrationBuilder.CreateTable(
                name: "SelfSyncEntity",
                columns: table => new
                {
                    RowId = table.Column<Guid>(nullable: false),
                    ChangeLog = table.Column<DateTimeOffset>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SyncDate = table.Column<DateTime>(nullable: false),
                    TxId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelfSyncEntity", x => x.RowId);
                });
        }
    }
}
