using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_034 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbSettingAlert",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AlertManagementID = table.Column<int>(nullable: false),
                    BeforeAppDate = table.Column<int>(nullable: false),
                    TypeBeforeAppDate = table.Column<int>(nullable: false),
                    Frequently = table.Column<int>(nullable: false),
                    TypeFrequently = table.Column<int>(nullable: false),
                    DeleteAlert = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbSettingAlert", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbAgingPayment",
                schema: "dbo",
                columns: table => new
                {
                    AgingPaymentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<int>(nullable: false),
                    CustomerID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    Ref_No = table.Column<string>(nullable: true),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    TotalAmountDue = table.Column<double>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbAgingPayment", x => x.AgingPaymentID);
                    table.ForeignKey(
                        name: "FK_tbAgingPayment_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbAgingPayment_tbBusinessPartner_CustomerID",
                        column: x => x.CustomerID,
                        principalSchema: "dbo",
                        principalTable: "tbBusinessPartner",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbAgingPayment_tbUserAccount_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbAgingPaymentCustomer",
                schema: "dbo",
                columns: table => new
                {
                    AgingPaymentCustomerID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CustomerID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    DocumentNo = table.Column<string>(nullable: true),
                    DocumentType = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(type: "Date", nullable: false),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    OverdueDays = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    TotalPayment = table.Column<double>(nullable: false),
                    Applied_Amount = table.Column<double>(nullable: false),
                    BalanceDue = table.Column<double>(nullable: false),
                    CurrencyName = table.Column<string>(nullable: true),
                    SysName = table.Column<string>(nullable: true),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    Cash = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    SysCurrency = table.Column<int>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbAgingPaymentCustomer", x => x.AgingPaymentCustomerID);
                });

            migrationBuilder.CreateTable(
                name: "tbAlertManagement",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<int>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    StatusAlert = table.Column<int>(nullable: false),
                    DeleteAlert = table.Column<int>(nullable: false),
                    ReadAlert = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbAlertManagement", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbAppointment",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CustomerID = table.Column<int>(nullable: false),
                    VehicleID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    PostingDate = table.Column<DateTime>(nullable: false),
                    ClosingDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Notification = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbAppointment", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbKvmsInfo",
                schema: "dbo",
                columns: table => new
                {
                    KvmsInfoID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QNo = table.Column<string>(nullable: true),
                    CusID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PriceListID = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    AutoMID = table.Column<int>(nullable: false),
                    Plate = table.Column<string>(nullable: true),
                    Frame = table.Column<string>(nullable: true),
                    Engine = table.Column<string>(nullable: true),
                    TypeName = table.Column<string>(nullable: true),
                    BrandName = table.Column<string>(nullable: true),
                    ModelName = table.Column<string>(nullable: true),
                    ColorName = table.Column<string>(nullable: true),
                    Year = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbKvmsInfo", x => x.KvmsInfoID);
                });

            migrationBuilder.CreateTable(
                name: "tbQuoteAutoM",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QNo = table.Column<string>(nullable: true),
                    CusID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PriceListID = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    AutoMID = table.Column<int>(nullable: false),
                    Plate = table.Column<string>(nullable: true),
                    Frame = table.Column<string>(nullable: true),
                    Engine = table.Column<string>(nullable: true),
                    TypeName = table.Column<string>(nullable: true),
                    BrandName = table.Column<string>(nullable: true),
                    ModelName = table.Column<string>(nullable: true),
                    ColorName = table.Column<string>(nullable: true),
                    Year = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbQuoteAutoM", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbReceiptMemoKvms",
                schema: "dbo",
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
                    Sub_Total = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: false),
                    TaxRate = table.Column<double>(nullable: false),
                    TaxValue = table.Column<double>(nullable: false),
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
                    Return = table.Column<bool>(nullable: false),
                    PLCurrencyID = table.Column<int>(nullable: false),
                    PLRate = table.Column<double>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false),
                    AppliedAmount = table.Column<double>(nullable: false),
                    BalanceDue = table.Column<double>(nullable: false),
                    Status = table.Column<int>(nullable: false)
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
                name: "tbTypeOfAlertM",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    TypeOfAlertUsed = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbTypeOfAlertM", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbSettingAlertUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SetttingAlertID = table.Column<int>(nullable: false),
                    UserAccountID = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    StatusAlertUser = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbSettingAlertUser", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbSettingAlertUser_tbSettingAlert_SetttingAlertID",
                        column: x => x.SetttingAlertID,
                        principalTable: "tbSettingAlert",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbAgingPaymentDetail",
                schema: "dbo",
                columns: table => new
                {
                    AgingPaymentDetailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AgingPaymentID = table.Column<int>(nullable: false),
                    DocumentNo = table.Column<string>(nullable: true),
                    DocumentType = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(type: "Date", nullable: false),
                    OverdueDays = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    BalanceDue = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    Totalpayment = table.Column<double>(nullable: false),
                    Applied_Amount = table.Column<double>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    Cash = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbAgingPaymentDetail", x => x.AgingPaymentDetailID);
                    table.ForeignKey(
                        name: "FK_tbAgingPaymentDetail_tbAgingPayment_AgingPaymentID",
                        column: x => x.AgingPaymentID,
                        principalSchema: "dbo",
                        principalTable: "tbAgingPayment",
                        principalColumn: "AgingPaymentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbAgingPaymentDetail_tbCurrency_CurrencyID",
                        column: x => x.CurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbAppointmentService",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AppointmentID = table.Column<int>(nullable: false),
                    ServiceName = table.Column<string>(nullable: true),
                    ServiceDate = table.Column<DateTime>(nullable: false),
                    ServiceUom = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    TimelyService = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbAppointmentService", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbAppointmentService_tbAppointment_AppointmentID",
                        column: x => x.AppointmentID,
                        principalSchema: "dbo",
                        principalTable: "tbAppointment",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbReceiptKvms",
                schema: "dbo",
                columns: table => new
                {
                    ReceiptKvmsID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    KvmsInfoID = table.Column<int>(nullable: false),
                    OrderID = table.Column<int>(nullable: false),
                    OrderNo = table.Column<string>(nullable: false),
                    TableID = table.Column<int>(nullable: false),
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
                    Sub_Total = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: false),
                    TaxRate = table.Column<double>(nullable: false),
                    TaxValue = table.Column<double>(nullable: false),
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
                    Return = table.Column<bool>(nullable: false),
                    PLCurrencyID = table.Column<int>(nullable: false),
                    PLRate = table.Column<double>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false),
                    AppliedAmount = table.Column<double>(nullable: false),
                    BalanceDue = table.Column<double>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbReceiptKvms", x => x.ReceiptKvmsID);
                    table.ForeignKey(
                        name: "FK_tbReceiptKvms_tbKvmsInfo_KvmsInfoID",
                        column: x => x.KvmsInfoID,
                        principalSchema: "dbo",
                        principalTable: "tbKvmsInfo",
                        principalColumn: "KvmsInfoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbReceiptKvms_tbCurrency_LocalCurrencyID",
                        column: x => x.LocalCurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbReceiptKvms_tbTable_TableID",
                        column: x => x.TableID,
                        principalSchema: "dbo",
                        principalTable: "tbTable",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbReceiptKvms_tbUserAccount_UserOrderID",
                        column: x => x.UserOrderID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbOrderQAutoM",
                schema: "dbo",
                columns: table => new
                {
                    OrderID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuoteAutoMID = table.Column<int>(nullable: false),
                    OrderNo = table.Column<string>(nullable: false),
                    TableID = table.Column<int>(nullable: false),
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
                    Sub_Total = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: false),
                    TaxRate = table.Column<double>(nullable: false),
                    TaxValue = table.Column<double>(nullable: false),
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
                    LocalSetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbOrderQAutoM", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_tbOrderQAutoM_tbCurrency_LocalCurrencyID",
                        column: x => x.LocalCurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbOrderQAutoM_tbQuoteAutoM_QuoteAutoMID",
                        column: x => x.QuoteAutoMID,
                        principalSchema: "dbo",
                        principalTable: "tbQuoteAutoM",
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
                    ReceiptMemoKvmsID = table.Column<int>(nullable: false),
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
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    Total = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    ItemStatus = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true),
                    ItemType = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ParentLevel = table.Column<string>(nullable: true)
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

            migrationBuilder.CreateTable(
                name: "tbReceiptDetailKvms",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReceiptKvmsID = table.Column<int>(nullable: false),
                    OrderDetailID = table.Column<int>(nullable: false),
                    OrderID = table.Column<int>(nullable: true),
                    Line_ID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    PrintQty = table.Column<double>(nullable: false),
                    OpenQty = table.Column<double>(nullable: false),
                    UnitPrice = table.Column<double>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    Total = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    ItemStatus = table.Column<string>(nullable: true),
                    ItemPrintTo = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    ItemType = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ParentLevel = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbReceiptDetailKvms", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbReceiptDetailKvms_tbReceiptKvms_ReceiptKvmsID",
                        column: x => x.ReceiptKvmsID,
                        principalSchema: "dbo",
                        principalTable: "tbReceiptKvms",
                        principalColumn: "ReceiptKvmsID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbReceiptDetailKvms_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbOrderDetailQAutoMs",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderID = table.Column<int>(nullable: true),
                    Line_ID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Cost = table.Column<double>(nullable: false),
                    UnitPrice = table.Column<double>(nullable: false),
                    Qty = table.Column<double>(nullable: false),
                    PrintQty = table.Column<double>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    Uom = table.Column<string>(nullable: true),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    Total = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    ItemStatus = table.Column<string>(nullable: true),
                    ItemPrintTo = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    ItemType = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ParentLevel = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbOrderDetailQAutoMs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbOrderDetailQAutoMs_tbOrderQAutoM_OrderID",
                        column: x => x.OrderID,
                        principalSchema: "dbo",
                        principalTable: "tbOrderQAutoM",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbOrderDetailQAutoMs_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbSettingAlertUser_SetttingAlertID",
                table: "tbSettingAlertUser",
                column: "SetttingAlertID");

            migrationBuilder.CreateIndex(
                name: "IX_tbAgingPayment_BranchID",
                schema: "dbo",
                table: "tbAgingPayment",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_tbAgingPayment_CustomerID",
                schema: "dbo",
                table: "tbAgingPayment",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_tbAgingPayment_UserID",
                schema: "dbo",
                table: "tbAgingPayment",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbAgingPaymentDetail_AgingPaymentID",
                schema: "dbo",
                table: "tbAgingPaymentDetail",
                column: "AgingPaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_tbAgingPaymentDetail_CurrencyID",
                schema: "dbo",
                table: "tbAgingPaymentDetail",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbAppointmentService_AppointmentID",
                schema: "dbo",
                table: "tbAppointmentService",
                column: "AppointmentID");

            migrationBuilder.CreateIndex(
                name: "IX_tbOrderDetailQAutoMs_OrderID",
                schema: "dbo",
                table: "tbOrderDetailQAutoMs",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_tbOrderDetailQAutoMs_UomID",
                schema: "dbo",
                table: "tbOrderDetailQAutoMs",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbOrderQAutoM_LocalCurrencyID",
                schema: "dbo",
                table: "tbOrderQAutoM",
                column: "LocalCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbOrderQAutoM_QuoteAutoMID",
                schema: "dbo",
                table: "tbOrderQAutoM",
                column: "QuoteAutoMID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbReceiptDetailKvms_ReceiptKvmsID",
                schema: "dbo",
                table: "tbReceiptDetailKvms",
                column: "ReceiptKvmsID");

            migrationBuilder.CreateIndex(
                name: "IX_tbReceiptDetailKvms_UomID",
                schema: "dbo",
                table: "tbReceiptDetailKvms",
                column: "UomID");

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
                name: "IX_tbReceiptKvms_KvmsInfoID",
                schema: "dbo",
                table: "tbReceiptKvms",
                column: "KvmsInfoID");

            migrationBuilder.CreateIndex(
                name: "IX_tbReceiptKvms_LocalCurrencyID",
                schema: "dbo",
                table: "tbReceiptKvms",
                column: "LocalCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbReceiptKvms_TableID",
                schema: "dbo",
                table: "tbReceiptKvms",
                column: "TableID");

            migrationBuilder.CreateIndex(
                name: "IX_tbReceiptKvms_UserOrderID",
                schema: "dbo",
                table: "tbReceiptKvms",
                column: "UserOrderID");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbSettingAlertUser");

            migrationBuilder.DropTable(
                name: "tbAgingPaymentCustomer",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbAgingPaymentDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbAlertManagement",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbAppointmentService",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbOrderDetailQAutoMs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbReceiptDetailKvms",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbReceiptDetailMemoKvms",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbTypeOfAlertM",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbSettingAlert");

            migrationBuilder.DropTable(
                name: "tbAgingPayment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbAppointment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbOrderQAutoM",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbReceiptKvms",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbReceiptMemoKvms",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbQuoteAutoM",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbKvmsInfo",
                schema: "dbo");
        }
    }
}
