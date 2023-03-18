using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "CopyPurchaseOrder_from_Quotation",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WarehouseID = table.Column<int>(nullable: false),
                    Warehouse = table.Column<string>(nullable: true),
                    VendorID = table.Column<int>(nullable: false),
                    Vendor = table.Column<string>(nullable: true),
                    Reff_No = table.Column<string>(nullable: true),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    SystemCurrencyID = table.Column<int>(nullable: false),
                    SystemCurrency = table.Column<string>(nullable: true),
                    LocalCurrency = table.Column<string>(nullable: true),
                    UomID = table.Column<int>(nullable: false),
                    UomName = table.Column<string>(nullable: true),
                    UserID = table.Column<int>(nullable: false),
                    User = table.Column<string>(nullable: true),
                    Invoice = table.Column<string>(nullable: true),
                    PostingDate = table.Column<string>(nullable: true),
                    DocumentDate = table.Column<string>(nullable: true),
                    RequiredDate = table.Column<string>(nullable: true),
                    ValidUntil = table.Column<string>(nullable: true),
                    Sub_Total = table.Column<double>(nullable: false),
                    Sub_Total_Sys = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValues = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    TaxRate = table.Column<double>(nullable: false),
                    TaxValues = table.Column<double>(nullable: false),                    
                    AppliedAmount = table.Column<double>(nullable: false),
                    Balance_Due = table.Column<double>(nullable: false),
                    Balance_Due_Sys = table.Column<double>(nullable: false),
                    ExchangRate = table.Column<double>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    ItemID = table.Column<int>(nullable: false),
                    LineID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    OpenQty = table.Column<double>(nullable: false),
                    PurchasePrice = table.Column<double>(nullable: false),
                    Discount_Rate = table.Column<double>(nullable: false),
                    Discount_Values = table.Column<double>(nullable: false),
                    Type_Dis = table.Column<string>(nullable: true),
                    Total = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    ManageExpire = table.Column<string>(nullable: true),
                    ExpireDate = table.Column<string>(nullable: true),
                    AlertStock = table.Column<double>(nullable: false),
                    Barcode = table.Column<string>(nullable: true),
                    GroupUomID = table.Column<int>(nullable: false),
                    DetailQuotationID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CopyPurchaseOrder_from_Quotation", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServicePriceListCopyItem",
                columns: table => new
                {
                    ItemID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    UoM = table.Column<string>(nullable: true),
                    Barcode = table.Column<string>(nullable: true),
                    Process = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicePriceListCopyItem", x => x.ItemID);
                });

            migrationBuilder.CreateTable(
                name: "SummarySaleAdmin",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CountInvoice = table.Column<double>(nullable: false),
                    SoldAmount = table.Column<double>(nullable: false),
                    AppliedAmount = table.Column<double>(nullable: false),
                    DisCountItem = table.Column<double>(nullable: false),
                    DisCountTotal = table.Column<double>(nullable: false),
                    TotalVatRate = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    TotalSys = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummarySaleAdmin", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbGLAccount",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    ExternalCode = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    AccountType = table.Column<int>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false),
                    IsTitle = table.Column<bool>(nullable: false),
                    IsConfidential = table.Column<bool>(nullable: false),
                    IsIndexed = table.Column<bool>(nullable: false),
                    IsCashAccount = table.Column<bool>(nullable: false),
                    IsControlAccount = table.Column<bool>(nullable: false),
                    BlockManualPosting = table.Column<bool>(nullable: false),
                    CashFlowRelavant = table.Column<bool>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    CurrencyName = table.Column<string>(nullable: true),
                    ChangeLog = table.Column<DateTime>(nullable: false),
                    ParentCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbGLAccount", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbSaleAR",
                columns: table => new
                {
                    SARID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CusID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    SaleCurrencyID = table.Column<int>(nullable: false),
                    RefNo = table.Column<string>(nullable: true),
                    InvoiceNo = table.Column<string>(nullable: true),
                    ExchangeRate = table.Column<double>(nullable: false),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DueDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    IncludeVat = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    SubTotal = table.Column<double>(nullable: false),
                    SubTotal_Sys = table.Column<double>(nullable: false),
                    DisRate = table.Column<double>(nullable: false),
                    DisValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    VatRate = table.Column<double>(nullable: false),
                    VatValue = table.Column<double>(nullable: false),
                    AppliedAmount = table.Column<double>(nullable: false),
                    FeeNote = table.Column<string>(nullable: true),
                    FeeAmount = table.Column<double>(nullable: false),
                    TotalAmount = table.Column<double>(nullable: false),
                    TotalAmount_Sys = table.Column<double>(nullable: false),
                    CopyType = table.Column<int>(nullable: false),
                    CopyKey = table.Column<string>(nullable: true),
                    BasedCopyKeys = table.Column<string>(nullable: true),
                    ChangeLog = table.Column<DateTime>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbSaleAR", x => x.SARID);
                });

            migrationBuilder.CreateTable(
                name: "tbSaleCreditMemo",
                columns: table => new
                {
                    SCMOID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CusID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    SaleCurrencyID = table.Column<int>(nullable: false),
                    RefNo = table.Column<string>(nullable: true),
                    InvoiceNo = table.Column<string>(nullable: true),
                    ExchangeRate = table.Column<double>(nullable: false),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DueDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    IncludeVat = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    SubTotal = table.Column<double>(nullable: false),
                    SubTotal_Sys = table.Column<double>(nullable: false),
                    DisRate = table.Column<double>(nullable: false),
                    DisValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    VatRate = table.Column<double>(nullable: false),
                    VatValue = table.Column<double>(nullable: false),
                    AppliedAmount = table.Column<double>(nullable: false),
                    FeeNote = table.Column<string>(nullable: true),
                    FeeAmount = table.Column<double>(nullable: false),
                    TotalAmount = table.Column<double>(nullable: false),
                    TotalAmount_Sys = table.Column<double>(nullable: false),
                    CopyType = table.Column<int>(nullable: false),
                    CopyKey = table.Column<string>(nullable: true),
                    BasedCopyKeys = table.Column<string>(nullable: true),
                    ChangeLog = table.Column<DateTime>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbSaleCreditMemo", x => x.SCMOID);
                });

            migrationBuilder.CreateTable(
                name: "tbSaleDelivery",
                columns: table => new
                {
                    SDID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CusID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    SaleCurrencyID = table.Column<int>(nullable: false),
                    RefNo = table.Column<string>(nullable: true),
                    InvoiceNo = table.Column<string>(nullable: true),
                    ExchangeRate = table.Column<double>(nullable: false),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DueDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    IncludeVat = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    SubTotal = table.Column<double>(nullable: false),
                    SubTotal_Sys = table.Column<double>(nullable: false),
                    DisRate = table.Column<double>(nullable: false),
                    DisValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    VatRate = table.Column<double>(nullable: false),
                    VatValue = table.Column<double>(nullable: false),
                    FeeNote = table.Column<string>(nullable: true),
                    FeeAmount = table.Column<double>(nullable: false),
                    TotalAmount = table.Column<double>(nullable: false),
                    TotalAmount_Sys = table.Column<double>(nullable: false),
                    CopyType = table.Column<int>(nullable: false),
                    CopyKey = table.Column<string>(nullable: true),
                    BasedCopyKeys = table.Column<string>(nullable: true),
                    ChangeLog = table.Column<DateTime>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbSaleDelivery", x => x.SDID);
                });

            migrationBuilder.CreateTable(
                name: "tbSaleQuote",
                columns: table => new
                {
                    SQID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CusID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    SaleCurrencyID = table.Column<int>(nullable: false),
                    RefNo = table.Column<string>(nullable: true),
                    InvoiceNo = table.Column<string>(nullable: true),
                    ExchangeRate = table.Column<double>(nullable: false),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    ValidUntilDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    IncludeVat = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    SubTotal = table.Column<double>(nullable: false),
                    SubTotal_Sys = table.Column<double>(nullable: false),
                    DisRate = table.Column<double>(nullable: false),
                    DisValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    VatRate = table.Column<double>(nullable: false),
                    VatValue = table.Column<double>(nullable: false),
                    TotalAmount = table.Column<double>(nullable: false),
                    TotalAmount_Sys = table.Column<double>(nullable: false),
                    ChangeLog = table.Column<DateTime>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbSaleQuote", x => x.SQID);
                });

            migrationBuilder.CreateTable(
                name: "Background",
                schema: "dbo",
                columns: table => new
                {
                    BackID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Background", x => x.BackID);
                });

            migrationBuilder.CreateTable(
                name: "CardType",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Discount = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Colors",
                schema: "dbo",
                columns: table => new
                {
                    ColorID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.ColorID);
                });

            migrationBuilder.CreateTable(
                name: "CopyPurchaseAP_To_PurchaseMemo",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WarehouseID = table.Column<int>(nullable: false),
                    Warehouse = table.Column<string>(nullable: true),
                    VendorID = table.Column<int>(nullable: false),
                    Vendor = table.Column<string>(nullable: true),
                    Reff_No = table.Column<string>(nullable: true),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    SystemCurrencyID = table.Column<int>(nullable: false),
                    SystemCurrency = table.Column<string>(nullable: true),
                    LocalCurrency = table.Column<string>(nullable: true),
                    UomID = table.Column<int>(nullable: false),
                    UomName = table.Column<string>(nullable: true),
                    UserID = table.Column<int>(nullable: false),
                    User = table.Column<string>(nullable: true),
                    Invoice = table.Column<string>(nullable: true),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Sub_Total = table.Column<double>(nullable: false),
                    Sub_Total_Sys = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValues = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    TaxRate = table.Column<double>(nullable: false),
                    TaxValues = table.Column<double>(nullable: false),
                    DownPayment = table.Column<double>(nullable: false),
                    AppliedAmount = table.Column<double>(nullable: false),
                    Balance_Due = table.Column<double>(nullable: false),
                    Balance_Due_Sys = table.Column<double>(nullable: false),
                    AdditionalExpense = table.Column<double>(nullable: false),
                    ReturnAmount = table.Column<double>(nullable: false),
                    AdditionalNode = table.Column<string>(nullable: true),
                    ExchangRate = table.Column<double>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    ItemID = table.Column<int>(nullable: false),
                    LineID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    OpenQty = table.Column<double>(nullable: false),
                    PurchasePrice = table.Column<double>(nullable: false),
                    Discount_Rate = table.Column<double>(nullable: false),
                    Discount_Values = table.Column<double>(nullable: false),
                    Type_Dis = table.Column<string>(nullable: true),
                    Total = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    ManageExpire = table.Column<string>(nullable: true),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: false),
                    AlertStock = table.Column<double>(nullable: false),
                    Barcode = table.Column<string>(nullable: true),
                    GroupUomID = table.Column<int>(nullable: false),
                    OrderID = table.Column<int>(nullable: false),
                    APID = table.Column<int>(nullable: false),
                    SetRate = table.Column<double>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CopyPurchaseAP_To_PurchaseMemo", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "db_DashboardTopSale",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    KhmerName = table.Column<string>(nullable: true),
                    InStock = table.Column<double>(nullable: false),
                    Uom = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_db_DashboardTopSale", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "db_SaleSummary",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Total = table.Column<double>(nullable: false),
                    Currency = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_db_SaleSummary", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "db_StockExpire",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    KhmerName = table.Column<string>(nullable: true),
                    ExpireDate = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_db_StockExpire", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Funtion",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funtion", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PrintPurchaseAP",
                schema: "dbo",
                columns: table => new
                {
                    PurchasID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Invoice = table.Column<string>(nullable: true),
                    ExchangeRate = table.Column<double>(nullable: false),
                    Balance_Due_Sys = table.Column<double>(nullable: false),
                    Balance_Due_Local = table.Column<double>(nullable: false),
                    PostingDate = table.Column<string>(nullable: true),
                    DocumentDate = table.Column<string>(nullable: true),
                    DueDate = table.Column<string>(nullable: true),
                    Sub_Total = table.Column<double>(nullable: false),
                    Sub_Total_Sys = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    TaxValue = table.Column<double>(nullable: false),
                    TaxRate = table.Column<double>(nullable: false),
                    Applied_Amount = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    VendorName = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    LocalCurrency = table.Column<string>(nullable: true),
                    SysCurrency = table.Column<string>(nullable: true),
                    VendorNo = table.Column<string>(nullable: true),
                    BaseOn = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    DiscountValue_Detail = table.Column<double>(nullable: false),
                    DiscountRate_Detail = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    UomName = table.Column<string>(nullable: true),
                    Logo = table.Column<string>(nullable: true),
                    Sub_Total_Detail = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintPurchaseAP", x => x.PurchasID);
                });

            migrationBuilder.CreateTable(
                name: "ReportPurchasAP",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InvoiceNo = table.Column<string>(nullable: true),
                    BusinessName = table.Column<string>(nullable: true),
                    Warehouse = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    LocalCurrency = table.Column<string>(nullable: true),
                    SystemCurrency = table.Column<string>(nullable: true),
                    Balance_due = table.Column<double>(nullable: false),
                    Balance_due_sys = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Total = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportPurchasAP", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ReportPurchasCreditMemo",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InvoiceNo = table.Column<string>(nullable: true),
                    BusinessName = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    LocalCurrency = table.Column<string>(nullable: true),
                    SystemCurrency = table.Column<string>(nullable: true),
                    Balance_due = table.Column<double>(nullable: false),
                    Balance_due_sys = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Total = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportPurchasCreditMemo", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ReportPurchaseOrder",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InvoiceNo = table.Column<string>(nullable: true),
                    BusinessName = table.Column<string>(nullable: true),
                    Warehouse = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    LocalCurrency = table.Column<string>(nullable: true),
                    SystemCurrency = table.Column<string>(nullable: true),
                    Balance_due = table.Column<double>(nullable: false),
                    Balance_due_sys = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Total = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportPurchaseOrder", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ReportPurchaseQuotation",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InvoiceNo = table.Column<string>(nullable: true),
                    BusinessName = table.Column<string>(nullable: true),
                    Warehouse = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    LocalCurrency = table.Column<string>(nullable: true),
                    SystemCurrency = table.Column<string>(nullable: true),
                    Balance_due = table.Column<double>(nullable: false),
                    Balance_due_sys = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportPurchaseQuotation", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ReportPurchaseRequst",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InvoiceNo = table.Column<string>(nullable: true),
                    Warehouse = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    SystemCurrency = table.Column<string>(nullable: true),
                    Balance_due = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    BranchName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportPurchaseRequst", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "rp_Cashout",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    DisItemValue = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    Currency_Sys = table.Column<string>(nullable: true),
                    TotalSoldAmount = table.Column<double>(nullable: false),
                    TotalDiscountItem = table.Column<double>(nullable: false),
                    TotalDiscountTotal = table.Column<double>(nullable: false),
                    TotalVat = table.Column<double>(nullable: false),
                    GrandTotal = table.Column<double>(nullable: false),
                    GrandTotal_Sys = table.Column<double>(nullable: false),
                    TotalCashIn_Sys = table.Column<double>(nullable: false),
                    TotalCashOut_Sys = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    GrandTotal_Display = table.Column<double>(nullable: false),
                    ExchangeRate_Display = table.Column<double>(nullable: false),
                    CurrencyDisplay = table.Column<string>(nullable: true),
                    ItemGroup1 = table.Column<string>(nullable: true),
                    ItemGroup2 = table.Column<string>(nullable: true),
                    ItemGroup3 = table.Column<string>(nullable: true),
                    DateIn = table.Column<string>(nullable: true),
                    DateOut = table.Column<string>(nullable: true),
                    TimeIn = table.Column<string>(nullable: true),
                    TimeOut = table.Column<string>(nullable: true),
                    EmpName = table.Column<string>(nullable: true),
                    Uom = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rp_Cashout", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "rp_CashoutPaymentMeans",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReceiptNo = table.Column<string>(nullable: true),
                    DisItemValue = table.Column<double>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    PaymentMeans = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true),
                    Currency_Sys = table.Column<string>(nullable: true),
                    TotalSoldAmount = table.Column<double>(nullable: false),
                    TotalDiscountItem = table.Column<double>(nullable: false),
                    TotalDiscountTotal = table.Column<double>(nullable: false),
                    TotalVat = table.Column<double>(nullable: false),
                    GrandTotal = table.Column<double>(nullable: false),
                    GrandTotal_Sys = table.Column<double>(nullable: false),
                    TotalCashIn_Sys = table.Column<double>(nullable: false),
                    TotalCashOut_Sys = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    GrandTotal_Display = table.Column<double>(nullable: false),
                    ExchangeRate_Display = table.Column<double>(nullable: false),
                    CurrencyDisplay = table.Column<string>(nullable: true),
                    DateIn = table.Column<string>(nullable: true),
                    DateOut = table.Column<string>(nullable: true),
                    TimeIn = table.Column<string>(nullable: true),
                    TimeOut = table.Column<string>(nullable: true),
                    EmpName = table.Column<string>(nullable: true),
                    ReceiptTime = table.Column<string>(nullable: true),
                    ReceiptDate = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rp_CashoutPaymentMeans", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "rp_CloseShift",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Branch = table.Column<string>(nullable: true),
                    User = table.Column<string>(nullable: true),
                    DateOut = table.Column<string>(nullable: true),
                    DateIn = table.Column<string>(nullable: true),
                    TimeIn = table.Column<string>(nullable: true),
                    TimeOut = table.Column<string>(nullable: true),
                    CashInAmount = table.Column<double>(nullable: false),
                    CashOutAmount = table.Column<double>(nullable: false),
                    SaleAmount_Local = table.Column<double>(nullable: false),
                    SaleAmount_Sys = table.Column<double>(nullable: false),
                    SysCurrency = table.Column<string>(nullable: true),
                    LocalCurrency = table.Column<string>(nullable: true),
                    UserID = table.Column<int>(nullable: false),
                    Tran_From = table.Column<double>(nullable: false),
                    Tran_To = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rp_CloseShift", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "rp_DetailPurchaseAP",
                schema: "dbo",
                columns: table => new
                {
                    PurchaseID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    Uom = table.Column<string>(nullable: true),
                    DisItem = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    UnitPrice = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rp_DetailPurchaseAP", x => x.PurchaseID);
                });

            migrationBuilder.CreateTable(
                name: "rp_DetailSale",
                schema: "dbo",
                columns: table => new
                {
                    OrderID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    Uom = table.Column<string>(nullable: true),
                    DisItem = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    UnitPrice = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rp_DetailSale", x => x.OrderID);
                });

            migrationBuilder.CreateTable(
                name: "rp_DetailTopSale",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateOut = table.Column<string>(nullable: true),
                    TimeOut = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    Uom = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rp_DetailTopSale", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "rp_StockInWarehouse_Detail",
                schema: "dbo",
                columns: table => new
                {
                    ItemID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Cost = table.Column<string>(nullable: true),
                    InStock = table.Column<double>(nullable: false),
                    Committed = table.Column<double>(nullable: false),
                    Ordered = table.Column<double>(nullable: false),
                    Uom = table.Column<string>(nullable: true),
                    UomID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<string>(nullable: true),
                    Warehouse = table.Column<string>(nullable: true),
                    ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rp_StockInWarehouse_Detail", x => x.ItemID);
                });

            migrationBuilder.CreateTable(
                name: "rp_StokInWarehouse",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    InStock = table.Column<double>(nullable: false),
                    Committed = table.Column<double>(nullable: false),
                    Ordered = table.Column<double>(nullable: false),
                    Uom = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    Barcode = table.Column<string>(nullable: true),
                    ExpireDate = table.Column<string>(nullable: true),
                    ItemID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rp_StokInWarehouse", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "rp_SummaryDetailOutgoingPayment",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DocumentNo = table.Column<string>(nullable: true),
                    DocumentType = table.Column<string>(nullable: true),
                    Date = table.Column<string>(nullable: true),
                    OverdueDay = table.Column<double>(nullable: false),
                    BalanceDue = table.Column<double>(nullable: false),
                    Discount = table.Column<double>(nullable: false),
                    TotalPay = table.Column<double>(nullable: false),
                    Cash = table.Column<double>(nullable: false),
                    LocalCurrency = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rp_SummaryDetailOutgoingPayment", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "rp_SummaryDetailTransferStock",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    Uom = table.Column<string>(nullable: true),
                    Barcode = table.Column<string>(nullable: true),
                    ExpireDate = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rp_SummaryDetailTransferStock", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "rp_SummaryOutgoingPayment",
                schema: "dbo",
                columns: table => new
                {
                    OutID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Number = table.Column<string>(nullable: true),
                    PostingDate = table.Column<string>(nullable: true),
                    DocumentDate = table.Column<string>(nullable: true),
                    TotalAmountDue = table.Column<double>(nullable: false),
                    SysCurrency = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rp_SummaryOutgoingPayment", x => x.OutID);
                });

            migrationBuilder.CreateTable(
                name: "rp_SummaryPurchaseAP",
                schema: "dbo",
                columns: table => new
                {
                    PurchaseID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Invoice = table.Column<string>(nullable: true),
                    PostingDate = table.Column<string>(nullable: true),
                    DocumentDate = table.Column<string>(nullable: true),
                    DueDate = table.Column<string>(nullable: true),
                    Sub_Total = table.Column<double>(nullable: false),
                    SubTotal_Sys = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    TaxRate = table.Column<double>(nullable: false),
                    Balance_Due = table.Column<double>(nullable: false),
                    Balance_Deu_Sys = table.Column<double>(nullable: false),
                    Applied_Amount = table.Column<double>(nullable: false),
                    Warehouse = table.Column<string>(nullable: true),
                    Branch = table.Column<string>(nullable: true),
                    VendorName = table.Column<string>(nullable: true),
                    User = table.Column<string>(nullable: true),
                    LocalCurrency = table.Column<string>(nullable: true),
                    SysCurrency = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rp_SummaryPurchaseAP", x => x.PurchaseID);
                });

            migrationBuilder.CreateTable(
                name: "rp_SummaryRevenuseItem",
                schema: "dbo",
                columns: table => new
                {
                    ItemID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    Uom = table.Column<string>(nullable: true),
                    TotalCost = table.Column<double>(nullable: false),
                    TotalPrice = table.Column<double>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    Profit = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rp_SummaryRevenuseItem", x => x.ItemID);
                });

            migrationBuilder.CreateTable(
                name: "rp_summarysale",
                schema: "dbo",
                columns: table => new
                {
                    OrderID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Branch = table.Column<string>(nullable: true),
                    User = table.Column<string>(nullable: true),
                    Receipt = table.Column<string>(nullable: true),
                    DateIn = table.Column<string>(nullable: true),
                    TimeIn = table.Column<string>(nullable: true),
                    DateOut = table.Column<string>(nullable: true),
                    TimeOut = table.Column<string>(nullable: true),
                    GrandTotal = table.Column<double>(nullable: false),
                    GrandTotal_Sys = table.Column<double>(nullable: false),
                    SysCurrency = table.Column<string>(nullable: true),
                    LocalCurrency = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rp_summarysale", x => x.OrderID);
                });

            migrationBuilder.CreateTable(
                name: "rp_SummaryTansferStock",
                schema: "dbo",
                columns: table => new
                {
                    TranID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Number = table.Column<string>(nullable: true),
                    PostingDate = table.Column<string>(nullable: true),
                    DocumentDate = table.Column<string>(nullable: true),
                    Time = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rp_SummaryTansferStock", x => x.TranID);
                });

            migrationBuilder.CreateTable(
                name: "rp_SummaryTotalPurchase",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CountReceipt = table.Column<double>(nullable: false),
                    DiscountItem = table.Column<double>(nullable: false),
                    DiscountTotal = table.Column<double>(nullable: false),
                    BalanceDue = table.Column<double>(nullable: false),
                    AppliedAmount = table.Column<double>(nullable: false),
                    BalanceDueSSC = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rp_SummaryTotalPurchase", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "rp_SummaryTotalSale",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CountReceipt = table.Column<int>(nullable: false),
                    SoldAmount = table.Column<double>(nullable: false),
                    DiscountItem = table.Column<double>(nullable: false),
                    DiscountTotal = table.Column<double>(nullable: false),
                    TaxValue = table.Column<double>(nullable: false),
                    GrandTotal = table.Column<double>(nullable: false),
                    GrandTotalSys = table.Column<double>(nullable: false),
                    TotalCost = table.Column<double>(nullable: false),
                    TotalProfit = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rp_SummaryTotalSale", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "rp_TopSaleQuantity",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    Uom = table.Column<string>(nullable: true),
                    DateOut = table.Column<string>(nullable: true),
                    TimeOut = table.Column<string>(nullable: true),
                    Branch = table.Column<string>(nullable: true),
                    ItemID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rp_TopSaleQuantity", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCheckPayment",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Invoice = table.Column<string>(nullable: true),
                    Check = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCheckPayment", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceDiscountItemDetail",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Uom = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    Discount = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceDiscountItemDetail", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceInventoryAudit",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UomID = table.Column<int>(nullable: false),
                    Tarns_Type = table.Column<string>(nullable: true),
                    Process = table.Column<string>(nullable: true),
                    SystemDate = table.Column<DateTime>(type: "Date", nullable: false),
                    TimeIn = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    CumulativeQty = table.Column<double>(nullable: false),
                    CumulativeValue = table.Column<double>(nullable: false),
                    Trans_value = table.Column<double>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    EnglistName = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Warehouse = table.Column<string>(nullable: true),
                    Branch = table.Column<string>(nullable: true),
                    Employee = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true),
                    Uom = table.Column<string>(nullable: true),
                    WarehouseID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceInventoryAudit", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceItemSales",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ItemID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Group1 = table.Column<int>(nullable: false),
                    Group2 = table.Column<int>(nullable: false),
                    Group3 = table.Column<int>(nullable: false),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    PrintQty = table.Column<double>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    UnitPrice = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<float>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    VAT = table.Column<double>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    UomID = table.Column<int>(nullable: false),
                    UoM = table.Column<string>(nullable: true),
                    Barcode = table.Column<string>(nullable: true),
                    Process = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    PricListID = table.Column<int>(nullable: false),
                    GroupUomID = table.Column<int>(nullable: false),
                    PrintTo = table.Column<string>(nullable: true),
                    ItemType = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceItemSales", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceMapItemMasterDataPurchasAP",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PurchaseDetailAPID = table.Column<int>(nullable: false),
                    LineID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    PurchasPrice = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    Total = table.Column<double>(nullable: false),
                    SysCurrency = table.Column<string>(nullable: true),
                    UomName = table.Column<string>(nullable: true),
                    ManageExpire = table.Column<string>(nullable: true),
                    ExpireDate = table.Column<string>(nullable: true),
                    AlertStock = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    LocalCurrency = table.Column<string>(nullable: true),
                    UomID = table.Column<int>(nullable: false),
                    GroupUomID = table.Column<int>(nullable: false),
                    Barcode = table.Column<string>(nullable: true),
                    OpenQty = table.Column<double>(nullable: false),
                    SetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceMapItemMasterDataPurchasAP", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceMapItemMasterDataPurchaseCreditMemo",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PurchaseMemoDetailID = table.Column<int>(nullable: false),
                    LineID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    PurchasPrice = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    Total = table.Column<double>(nullable: false),
                    SysCurrency = table.Column<string>(nullable: true),
                    UomName = table.Column<string>(nullable: true),
                    ManageExpire = table.Column<string>(nullable: true),
                    ExpireDate = table.Column<string>(nullable: true),
                    AlertStock = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    LocalCurrency = table.Column<string>(nullable: true),
                    UomID = table.Column<int>(nullable: false),
                    GroupUomID = table.Column<int>(nullable: false),
                    Barcode = table.Column<string>(nullable: true),
                    SetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceMapItemMasterDataPurchaseCreditMemo", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceMapItemMasterDataPurchaseOrder",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PurchaseOrderDetailID = table.Column<int>(nullable: false),
                    LineID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    PurchasPrice = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    Total = table.Column<double>(nullable: false),
                    SysCurrency = table.Column<string>(nullable: true),
                    UomName = table.Column<string>(nullable: true),
                    ManageExpire = table.Column<string>(nullable: true),
                    ExpireDate = table.Column<string>(nullable: true),
                    AlertStock = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    LocalCurrency = table.Column<string>(nullable: true),
                    UomID = table.Column<int>(nullable: false),
                    GroupUomID = table.Column<int>(nullable: false),
                    Barcode = table.Column<string>(nullable: true),
                    OpenQty = table.Column<double>(nullable: false),
                    Choosed = table.Column<bool>(nullable: false),
                    SetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceMapItemMasterDataPurchaseOrder", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceMapItemMasterDataQuotation",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuotationDetailID = table.Column<int>(nullable: false),
                    LineID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    RequiredDate = table.Column<string>(nullable: true),
                    QuotedDate = table.Column<string>(nullable: true),
                    RequiredQty = table.Column<double>(nullable: false),
                    Qty = table.Column<double>(nullable: false),
                    PurchasPrice = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    Total = table.Column<double>(nullable: false),
                    SysCurrency = table.Column<string>(nullable: true),
                    UomName = table.Column<string>(nullable: true),
                    ManageExpire = table.Column<string>(nullable: true),
                    ExpireDate = table.Column<string>(nullable: true),
                    AlertStock = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    LocalCurrency = table.Column<string>(nullable: true),
                    UomID = table.Column<int>(nullable: false),
                    GroupUomID = table.Column<int>(nullable: false),
                    Barcode = table.Column<string>(nullable: true),
                    OpenQty = table.Column<double>(nullable: false),
                    Choosed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceMapItemMasterDataQuotation", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceMapItemMasterPurchaseRequest",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RequestDetailID = table.Column<int>(nullable: false),
                    LineID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    UnitPrice = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    SysCurrency = table.Column<string>(nullable: true),
                    UomName = table.Column<string>(nullable: true),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    GroupUomID = table.Column<int>(nullable: false),
                    Barcode = table.Column<string>(nullable: true),
                    OpenQty = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    VendorID = table.Column<int>(nullable: false),
                    RequiredDate = table.Column<string>(nullable: true),
                    VendorName = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceMapItemMasterPurchaseRequest", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceMasterData",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Cost = table.Column<string>(nullable: true),
                    UnitPrice = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true),
                    UoM = table.Column<string>(nullable: true),
                    Barcode = table.Column<string>(nullable: true),
                    Process = table.Column<string>(nullable: true),
                    ItemID = table.Column<int>(nullable: false),
                    Group1 = table.Column<int>(nullable: false),
                    Group2 = table.Column<int>(nullable: false),
                    Group3 = table.Column<int>(nullable: false),
                    PricListID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    SysCurrency = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceMasterData", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServicePointDetail",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ItemID = table.Column<int>(nullable: false),
                    Qty = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Barcode = table.Column<string>(nullable: true),
                    UnitPrice = table.Column<double>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    CurrencyName = table.Column<string>(nullable: true),
                    UomID = table.Column<int>(nullable: false),
                    UomName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicePointDetail", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServicePricelistSetPrice",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Uom = table.Column<string>(nullable: true),
                    Cost = table.Column<double>(nullable: false),
                    Makup = table.Column<double>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    Discount = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    Process = table.Column<string>(nullable: true),
                    SysCurrency = table.Column<string>(nullable: true),
                    Barcode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicePricelistSetPrice", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceQuotationDetail",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuotationDetailID = table.Column<int>(nullable: false),
                    PurchaseQuotationID = table.Column<int>(nullable: false),
                    LineID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    PurchasePrice = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    UomName = table.Column<string>(nullable: true),
                    Total_Sys = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    SysCurrency = table.Column<string>(nullable: true),
                    LocalCurrency = table.Column<string>(nullable: true),
                    UomID = table.Column<int>(nullable: false),
                    GroupUomID = table.Column<int>(nullable: false),
                    Barcode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceQuotationDetail", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbCloseShift",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateIn = table.Column<DateTime>(type: "Date", nullable: false),
                    TimeIn = table.Column<string>(nullable: true),
                    DateOut = table.Column<DateTime>(type: "Date", nullable: false),
                    TimeOut = table.Column<string>(nullable: true),
                    BranchID = table.Column<int>(nullable: true),
                    UserID = table.Column<int>(nullable: false),
                    CashInAmount_Sys = table.Column<double>(nullable: false),
                    SaleAmount_Sys = table.Column<double>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false),
                    CashOutAmount_Sys = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    Trans_From = table.Column<double>(nullable: false),
                    Trans_To = table.Column<double>(nullable: false),
                    Close = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbCloseShift", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbCurrency",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Symbol = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbCurrency", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbDisplayCurrency",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PriceListID = table.Column<int>(nullable: false),
                    AltCurrencyID = table.Column<int>(nullable: false),
                    DisplayRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbDisplayCurrency", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbEmployee",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 50, nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    Birthdate = table.Column<DateTime>(type: "date", nullable: false),
                    Hiredate = table.Column<DateTime>(type: "date", nullable: false),
                    Address = table.Column<string>(maxLength: 220, nullable: true),
                    Phone = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Photo = table.Column<string>(nullable: true),
                    Stopwork = table.Column<bool>(nullable: false),
                    Position = table.Column<string>(nullable: true),
                    IsUser = table.Column<bool>(nullable: false),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbEmployee", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbGeneralSetting",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BranchID = table.Column<int>(nullable: false),
                    Receiptsize = table.Column<string>(nullable: true),
                    ReceiptTemplate = table.Column<string>(nullable: true),
                    DaulScreen = table.Column<bool>(nullable: false),
                    PrintReceiptOrder = table.Column<bool>(nullable: false),
                    PrintReceiptTender = table.Column<bool>(nullable: false),
                    PrintCountReceipt = table.Column<int>(nullable: false),
                    QueueCount = table.Column<int>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    RateIn = table.Column<double>(nullable: false),
                    RateOut = table.Column<double>(nullable: false),
                    Printer = table.Column<string>(nullable: true),
                    PaymentMeansID = table.Column<int>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    CustomerID = table.Column<int>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    VatAble = table.Column<bool>(nullable: false),
                    VatNum = table.Column<string>(nullable: true),
                    Wifi = table.Column<string>(nullable: true),
                    MacAddress = table.Column<string>(nullable: true),
                    AutoQueue = table.Column<bool>(nullable: false),
                    PrintLabel = table.Column<bool>(nullable: false),
                    CloseShift = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbGeneralSetting", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbGroupUoM",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbGroupUoM", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbIncomingPaymentCustomer",
                schema: "dbo",
                columns: table => new
                {
                    IncomingPaymentCustomerID = table.Column<int>(nullable: false)
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
                    BalanceDue = table.Column<double>(nullable: false),
                    TotalPayment = table.Column<double>(nullable: false),
                    Applied_Amount = table.Column<double>(nullable: false),
                    CurrencyName = table.Column<string>(nullable: true),
                    SysName = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    CashDiscount = table.Column<double>(nullable: false),
                    TotalDiscount = table.Column<double>(nullable: false),
                    Cash = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    SysCurrency = table.Column<int>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbIncomingPaymentCustomer", x => x.IncomingPaymentCustomerID);
                });

            migrationBuilder.CreateTable(
                name: "tbItemComment",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbItemComment", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbLogin",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<int>(nullable: false),
                    Timelogin = table.Column<string>(nullable: true),
                    Datelogin = table.Column<DateTime>(nullable: false),
                    Status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbLogin", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbOpenShift",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateIn = table.Column<DateTime>(type: "Date", nullable: false),
                    TimeIn = table.Column<string>(nullable: true),
                    BranchID = table.Column<int>(nullable: true),
                    UserID = table.Column<int>(nullable: true),
                    CashAmount_Sys = table.Column<double>(nullable: false),
                    Trans_From = table.Column<double>(nullable: false),
                    Open = table.Column<bool>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbOpenShift", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbOrder_Queue",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BranchID = table.Column<int>(nullable: false),
                    OrderNo = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbOrder_Queue", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbOrder_Receipt",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BranchID = table.Column<int>(nullable: false),
                    ReceiptID = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbOrder_Receipt", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbOutgoingPaymentVendor",
                schema: "dbo",
                columns: table => new
                {
                    OutgoingPaymentVendorID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VendorID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    DocumentNo = table.Column<string>(nullable: true),
                    DocumentType = table.Column<string>(nullable: true),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Date = table.Column<DateTime>(type: "Date", nullable: false),
                    OverdueDays = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    BalanceDue = table.Column<double>(nullable: false),
                    TotalPayment = table.Column<double>(nullable: false),
                    Applied_Amount = table.Column<double>(nullable: false),
                    CurrencyName = table.Column<string>(nullable: true),
                    SysName = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    CashDiscount = table.Column<double>(nullable: false),
                    TotalDiscount = table.Column<double>(nullable: false),
                    Cash = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    SysCurrency = table.Column<int>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbOutgoingPaymentVendor", x => x.OutgoingPaymentVendorID);
                });

            migrationBuilder.CreateTable(
                name: "tbPaymentMeans",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPaymentMeans", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbPoint",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Quantity = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    Amount = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPoint", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbPrinterName",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    MachineName = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPrinterName", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbPromotion",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(type: "Date", nullable: false),
                    StopDate = table.Column<DateTime>(type: "Date", nullable: false),
                    StartTime = table.Column<TimeSpan>(nullable: false),
                    StopTime = table.Column<TimeSpan>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPromotion", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbSaleOrder",
                schema: "dbo",
                columns: table => new
                {
                    SOID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CusID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    SaleCurrencyID = table.Column<int>(nullable: false),
                    RefNo = table.Column<string>(nullable: true),
                    InvoiceNo = table.Column<string>(nullable: true),
                    ExchangeRate = table.Column<double>(nullable: false),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    IncludeVat = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    SubTotal = table.Column<double>(nullable: false),
                    SubTotal_Sys = table.Column<double>(nullable: false),
                    DisRate = table.Column<double>(nullable: false),
                    DisValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    VatRate = table.Column<double>(nullable: false),
                    VatValue = table.Column<double>(nullable: false),
                    FeeNote = table.Column<string>(nullable: true),
                    FeeAmount = table.Column<double>(nullable: false),
                    TotalAmount = table.Column<double>(nullable: false),
                    TotalAmount_Sys = table.Column<double>(nullable: false),
                    CopyType = table.Column<int>(nullable: false),
                    CopyKey = table.Column<string>(nullable: true),
                    BasedCopyKeys = table.Column<string>(nullable: true),
                    ChangeLog = table.Column<DateTime>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbSaleOrder", x => x.SOID);
                });

            migrationBuilder.CreateTable(
                name: "tbSystemType",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbSystemType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbTax",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Rate = table.Column<double>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    Effective = table.Column<DateTime>(nullable: false),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbTax", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbUnitofMeasure",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    AltUomName = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbUnitofMeasure", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tmpOrderDetail",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ItemID = table.Column<int>(nullable: false),
                    Qty = table.Column<double>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    UnitPrice = table.Column<double>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    Process = table.Column<string>(nullable: true),
                    WarehouseID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    InvoiceNo = table.Column<string>(nullable: true),
                    ExchageRate = table.Column<double>(nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tmpOrderDetail", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tpGoodReciptStock",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ItemID = table.Column<int>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Qty = table.Column<double>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    LocalcurrencyID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tpGoodReciptStock", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tpItemCopyToPriceList",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ToPriceListID = table.Column<int>(nullable: false),
                    FromPriceListID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tpItemCopyToPriceList", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tpItemCopyToWH",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ToWHID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tpItemCopyToWH", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tpPriceList",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    currencyID = table.Column<int>(nullable: false),
                    PirceListID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tpPriceList", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbSaleARDetail",
                columns: table => new
                {
                    SARDID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SARID = table.Column<int>(nullable: false),
                    SQDID = table.Column<int>(nullable: false),
                    SODID = table.Column<int>(nullable: false),
                    SDDID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
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
                    TypeDis = table.Column<string>(nullable: true),
                    VatRate = table.Column<double>(nullable: false),
                    VatValue = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    ItemType = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbSaleARDetail", x => x.SARDID);
                    table.ForeignKey(
                        name: "FK_tbSaleARDetail_tbSaleAR_SARID",
                        column: x => x.SARID,
                        principalTable: "tbSaleAR",
                        principalColumn: "SARID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbSaleCreditMemoDetail",
                columns: table => new
                {
                    SCMODID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SCMOID = table.Column<int>(nullable: false),
                    SARDID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
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
                    UnitPrice = table.Column<double>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    DisRate = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    DisValue = table.Column<double>(nullable: false),
                    VatRate = table.Column<double>(nullable: false),
                    VatValue = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    Process = table.Column<string>(nullable: true),
                    CurrencyID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    ItemType = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbSaleCreditMemoDetail", x => x.SCMODID);
                    table.ForeignKey(
                        name: "FK_tbSaleCreditMemoDetail_tbSaleCreditMemo_SCMOID",
                        column: x => x.SCMOID,
                        principalTable: "tbSaleCreditMemo",
                        principalColumn: "SCMOID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbSaleDeliveryDetail",
                columns: table => new
                {
                    SDDID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SDID = table.Column<int>(nullable: false),
                    SQDID = table.Column<int>(nullable: false),
                    SODID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
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
                    TypeDis = table.Column<string>(nullable: true),
                    VatRate = table.Column<double>(nullable: false),
                    VatValue = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    Process = table.Column<string>(nullable: true),
                    CurrencyID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    ItemType = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbSaleDeliveryDetail", x => x.SDDID);
                    table.ForeignKey(
                        name: "FK_tbSaleDeliveryDetail_tbSaleDelivery_SDID",
                        column: x => x.SDID,
                        principalTable: "tbSaleDelivery",
                        principalColumn: "SDID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbSaleQuoteDetail",
                columns: table => new
                {
                    SQDID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SQID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    ItemCode = table.Column<string>(nullable: true),
                    ItemNameKH = table.Column<string>(nullable: true),
                    ItemNameEN = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    OpenQty = table.Column<double>(nullable: false),
                    GUomID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    UomName = table.Column<string>(nullable: true),
                    Factor = table.Column<double>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    UnitPrice = table.Column<double>(nullable: false),
                    DisRate = table.Column<double>(nullable: false),
                    DisValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    VatRate = table.Column<double>(nullable: false),
                    VatValue = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    Process = table.Column<string>(nullable: true),
                    CurrencyID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    ItemType = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbSaleQuoteDetail", x => x.SQDID);
                    table.ForeignKey(
                        name: "FK_tbSaleQuoteDetail_tbSaleQuote_SQID",
                        column: x => x.SQID,
                        principalTable: "tbSaleQuote",
                        principalColumn: "SQID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbMemberCard",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CardTypeID = table.Column<int>(nullable: false),
                    Ref_No = table.Column<string>(type: "nvarchar(25)", nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    Approve = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    DateApprove = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbMemberCard", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbMemberCard_CardType_CardTypeID",
                        column: x => x.CardTypeID,
                        principalSchema: "dbo",
                        principalTable: "CardType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemGroup1",
                schema: "dbo",
                columns: table => new
                {
                    ItemG1ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Images = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false),
                    Visible = table.Column<bool>(nullable: false),
                    ColorID = table.Column<int>(nullable: true),
                    BackID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemGroup1", x => x.ItemG1ID);
                    table.ForeignKey(
                        name: "FK_ItemGroup1_Background_BackID",
                        column: x => x.BackID,
                        principalSchema: "dbo",
                        principalTable: "Background",
                        principalColumn: "BackID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemGroup1_Colors_ColorID",
                        column: x => x.ColorID,
                        principalSchema: "dbo",
                        principalTable: "Colors",
                        principalColumn: "ColorID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbUserPrivillege",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<int>(nullable: false),
                    FunctionID = table.Column<int>(nullable: false),
                    Used = table.Column<bool>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbUserPrivillege", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbUserPrivillege_Funtion_FunctionID",
                        column: x => x.FunctionID,
                        principalSchema: "dbo",
                        principalTable: "Funtion",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbExchangeRate",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CurrencyID = table.Column<int>(nullable: false),
                    Rate = table.Column<double>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    RateOut = table.Column<double>(nullable: false),
                    SetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbExchangeRate", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbExchangeRate_tbCurrency_CurrencyID",
                        column: x => x.CurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbOrder",
                schema: "dbo",
                columns: table => new
                {
                    OrderID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
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
                    table.PrimaryKey("PK_tbOrder", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_tbOrder_tbCurrency_PLCurrencyID",
                        column: x => x.PLCurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbPriceList",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPriceList", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbPriceList_tbCurrency_CurrencyID",
                        column: x => x.CurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbPromotionPrice",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Amount = table.Column<double>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    Discount = table.Column<float>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPromotionPrice", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbPromotionPrice_tbCurrency_CurrencyID",
                        column: x => x.CurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbVoidOrder",
                schema: "dbo",
                columns: table => new
                {
                    OrderID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
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
                    table.PrimaryKey("PK_tbVoidOrder", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_tbVoidOrder_tbCurrency_PLCurrencyID",
                        column: x => x.PLCurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbPointCard",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PointID = table.Column<int>(nullable: false),
                    Ref_No = table.Column<string>(type: "nvarchar(25)", nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Remain = table.Column<double>(nullable: false),
                    Point = table.Column<int>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    Approve = table.Column<string>(nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DataApprove = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPointCard", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbPointCard_tbPoint_PointID",
                        column: x => x.PointID,
                        principalSchema: "dbo",
                        principalTable: "tbPoint",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbCounter",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    PrinterID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbCounter", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbCounter_tbPrinterName_PrinterID",
                        column: x => x.PrinterID,
                        principalSchema: "dbo",
                        principalTable: "tbPrinterName",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbSaleOrderDetail",
                schema: "dbo",
                columns: table => new
                {
                    SODID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SOID = table.Column<int>(nullable: false),
                    SQDID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
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
                    TypeDis = table.Column<string>(nullable: true),
                    VatRate = table.Column<double>(nullable: false),
                    VatValue = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    Process = table.Column<string>(nullable: true),
                    CurrencyID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    ItemType = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbSaleOrderDetail", x => x.SODID);
                    table.ForeignKey(
                        name: "FK_tbSaleOrderDetail_tbSaleOrder_SOID",
                        column: x => x.SOID,
                        principalSchema: "dbo",
                        principalTable: "tbSaleOrder",
                        principalColumn: "SOID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbGroupDefindUoM",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UoMID = table.Column<int>(nullable: false),
                    GroupUoMID = table.Column<int>(nullable: false),
                    AltQty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BaseQty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Factor = table.Column<double>(type: "float", nullable: false),
                    BaseUOM = table.Column<int>(nullable: false),
                    AltUOM = table.Column<int>(nullable: false),
                    Defined = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbGroupDefindUoM", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbGroupDefindUoM_tbGroupUoM_GroupUoMID",
                        column: x => x.GroupUoMID,
                        principalSchema: "dbo",
                        principalTable: "tbGroupUoM",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGroupDefindUoM_tbUnitofMeasure_UoMID",
                        column: x => x.UoMID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbOrderDetail_Addon",
                schema: "dbo",
                columns: table => new
                {
                    AddOnID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderDetailID = table.Column<int>(nullable: false),
                    OrderID = table.Column<int>(nullable: true),
                    Line_ID = table.Column<int>(nullable: false),
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
                    Total = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    ItemStatus = table.Column<string>(nullable: true),
                    ItemPrintTo = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    ItemType = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbOrderDetail_Addon", x => x.AddOnID);
                    table.ForeignKey(
                        name: "FK_tbOrderDetail_Addon_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tpItemCopyToPriceListDetail",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ItemID = table.Column<int>(nullable: false),
                    ItemCopyToPriceListID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tpItemCopyToPriceListDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tpItemCopyToPriceListDetail_tpItemCopyToPriceList_ItemCopyToPriceListID",
                        column: x => x.ItemCopyToPriceListID,
                        principalSchema: "dbo",
                        principalTable: "tpItemCopyToPriceList",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tpItemCopyToWHDetail",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ItemID = table.Column<int>(nullable: false),
                    ItemCopyToWHID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tpItemCopyToWHDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tpItemCopyToWHDetail_tpItemCopyToWH_ItemCopyToWHID",
                        column: x => x.ItemCopyToWHID,
                        principalSchema: "dbo",
                        principalTable: "tpItemCopyToWH",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemGroup2",
                schema: "dbo",
                columns: table => new
                {
                    ItemG2ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Images = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false),
                    ItemG1ID = table.Column<int>(nullable: false),
                    ColorID = table.Column<int>(nullable: true),
                    BackID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemGroup2", x => x.ItemG2ID);
                    table.ForeignKey(
                        name: "FK_ItemGroup2_Background_BackID",
                        column: x => x.BackID,
                        principalSchema: "dbo",
                        principalTable: "Background",
                        principalColumn: "BackID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemGroup2_Colors_ColorID",
                        column: x => x.ColorID,
                        principalSchema: "dbo",
                        principalTable: "Colors",
                        principalColumn: "ColorID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemGroup2_ItemGroup1_ItemG1ID",
                        column: x => x.ItemG1ID,
                        principalSchema: "dbo",
                        principalTable: "ItemGroup1",
                        principalColumn: "ItemG1ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbOrderDetail",
                schema: "dbo",
                columns: table => new
                {
                    OrderDetailID = table.Column<int>(nullable: false)
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
                    table.PrimaryKey("PK_tbOrderDetail", x => x.OrderDetailID);
                    table.ForeignKey(
                        name: "FK_tbOrderDetail_tbOrder_OrderID",
                        column: x => x.OrderID,
                        principalSchema: "dbo",
                        principalTable: "tbOrder",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbOrderDetail_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbBusinessPartner",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    PriceListID = table.Column<int>(nullable: false),
                    Phone = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbBusinessPartner", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbBusinessPartner_tbPriceList_PriceListID",
                        column: x => x.PriceListID,
                        principalSchema: "dbo",
                        principalTable: "tbPriceList",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbCompany",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    Location = table.Column<string>(maxLength: 50, nullable: false),
                    Address = table.Column<string>(maxLength: 220, nullable: true),
                    Process = table.Column<string>(maxLength: 25, nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    SystemCurrencyID = table.Column<int>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    Logo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbCompany", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbCompany_tbCurrency_LocalCurrencyID",
                        column: x => x.LocalCurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbCompany_tbPriceList_PriceListID",
                        column: x => x.PriceListID,
                        principalSchema: "dbo",
                        principalTable: "tbPriceList",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbCompany_tbCurrency_SystemCurrencyID",
                        column: x => x.SystemCurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbPriceListDetail",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PriceListID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: true),
                    CurrencyID = table.Column<int>(nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                    Discount = table.Column<float>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    PromotionID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    SystemDate = table.Column<DateTime>(nullable: false),
                    TimeIn = table.Column<DateTime>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    UnitPrice = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPriceListDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbPriceListDetail_tbPriceList_PriceListID",
                        column: x => x.PriceListID,
                        principalSchema: "dbo",
                        principalTable: "tbPriceList",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbVoidOrderDetail",
                schema: "dbo",
                columns: table => new
                {
                    OrderDetailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderID = table.Column<int>(nullable: true),
                    Line_ID = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_tbVoidOrderDetail", x => x.OrderDetailID);
                    table.ForeignKey(
                        name: "FK_tbVoidOrderDetail_tbVoidOrder_OrderID",
                        column: x => x.OrderID,
                        principalSchema: "dbo",
                        principalTable: "tbVoidOrder",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbVoidOrderDetail_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemGroup3",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    Images = table.Column<string>(nullable: true),
                    ItemG1ID = table.Column<int>(nullable: false),
                    ItemG2ID = table.Column<int>(nullable: false),
                    ColorID = table.Column<int>(nullable: false),
                    BackID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemGroup3", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ItemGroup3_Background_BackID",
                        column: x => x.BackID,
                        principalSchema: "dbo",
                        principalTable: "Background",
                        principalColumn: "BackID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemGroup3_Colors_ColorID",
                        column: x => x.ColorID,
                        principalSchema: "dbo",
                        principalTable: "Colors",
                        principalColumn: "ColorID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemGroup3_ItemGroup1_ItemG1ID",
                        column: x => x.ItemG1ID,
                        principalSchema: "dbo",
                        principalTable: "ItemGroup1",
                        principalColumn: "ItemG1ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemGroup3_ItemGroup2_ItemG2ID",
                        column: x => x.ItemG2ID,
                        principalSchema: "dbo",
                        principalTable: "ItemGroup2",
                        principalColumn: "ItemG2ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbBranch",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbBranch", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbBranch_tbCompany_CompanyID",
                        column: x => x.CompanyID,
                        principalSchema: "dbo",
                        principalTable: "tbCompany",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbItemMasterData",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    KhmerName = table.Column<string>(nullable: false),
                    EnglishName = table.Column<string>(nullable: false),
                    StockIn = table.Column<double>(nullable: false),
                    StockCommit = table.Column<double>(nullable: false),
                    StockOnHand = table.Column<double>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    UnitPrice = table.Column<double>(nullable: false),
                    BaseUomID = table.Column<int>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    GroupUomID = table.Column<int>(nullable: false),
                    PurchaseUomID = table.Column<int>(nullable: true),
                    SaleUomID = table.Column<int>(nullable: true),
                    InventoryUoMID = table.Column<int>(nullable: true),
                    WarehouseID = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    ItemGroup1ID = table.Column<int>(nullable: false),
                    ItemGroup2ID = table.Column<int>(nullable: true),
                    ItemGroup3ID = table.Column<int>(nullable: true),
                    Inventory = table.Column<bool>(nullable: false),
                    Sale = table.Column<bool>(nullable: false),
                    Purchase = table.Column<bool>(nullable: false),
                    Barcode = table.Column<string>(nullable: true),
                    PrintToID = table.Column<int>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Process = table.Column<string>(nullable: true),
                    ManageExpire = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbItemMasterData", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbItemMasterData_tbGroupUoM_GroupUomID",
                        column: x => x.GroupUomID,
                        principalSchema: "dbo",
                        principalTable: "tbGroupUoM",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbItemMasterData_tbUnitofMeasure_InventoryUoMID",
                        column: x => x.InventoryUoMID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbItemMasterData_ItemGroup1_ItemGroup1ID",
                        column: x => x.ItemGroup1ID,
                        principalSchema: "dbo",
                        principalTable: "ItemGroup1",
                        principalColumn: "ItemG1ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbItemMasterData_ItemGroup2_ItemGroup2ID",
                        column: x => x.ItemGroup2ID,
                        principalSchema: "dbo",
                        principalTable: "ItemGroup2",
                        principalColumn: "ItemG2ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbItemMasterData_ItemGroup3_ItemGroup3ID",
                        column: x => x.ItemGroup3ID,
                        principalSchema: "dbo",
                        principalTable: "ItemGroup3",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbItemMasterData_tbPriceList_PriceListID",
                        column: x => x.PriceListID,
                        principalSchema: "dbo",
                        principalTable: "tbPriceList",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbItemMasterData_tbPrinterName_PrintToID",
                        column: x => x.PrintToID,
                        principalSchema: "dbo",
                        principalTable: "tbPrinterName",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbItemMasterData_tbUnitofMeasure_PurchaseUomID",
                        column: x => x.PurchaseUomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbItemMasterData_tbUnitofMeasure_SaleUomID",
                        column: x => x.SaleUomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbGroupTable",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    Types = table.Column<string>(nullable: false),
                    Image = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbGroupTable", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbGroupTable_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbReceiptInformation",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BranchID = table.Column<int>(nullable: true),
                    Title = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    Tel1 = table.Column<string>(nullable: false),
                    Tel2 = table.Column<string>(nullable: false),
                    KhmerDescription = table.Column<string>(nullable: false),
                    EnglishDescription = table.Column<string>(nullable: false),
                    PowerBy = table.Column<string>(nullable: true),
                    TeamCondition = table.Column<string>(nullable: true),
                    Logo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbReceiptInformation", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbReceiptInformation_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbUserAccount",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmployeeID = table.Column<int>(nullable: true),
                    CompanyID = table.Column<int>(nullable: true),
                    BranchID = table.Column<int>(nullable: true),
                    Username = table.Column<string>(maxLength: 50, nullable: true),
                    Password = table.Column<string>(nullable: true),
                    ComfirmPassword = table.Column<string>(nullable: true),
                    UserPos = table.Column<bool>(nullable: false),
                    Language = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    UserID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbUserAccount", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbUserAccount_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbUserAccount_tbCompany_CompanyID",
                        column: x => x.CompanyID,
                        principalSchema: "dbo",
                        principalTable: "tbCompany",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbUserAccount_tbEmployee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalSchema: "dbo",
                        principalTable: "tbEmployee",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbUserAccount_tbUserPrivillege_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserPrivillege",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbWarhouse",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BranchID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 50, nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    StockIn = table.Column<double>(nullable: false),
                    Location = table.Column<string>(maxLength: 50, nullable: false),
                    Address = table.Column<string>(maxLength: 50, nullable: false),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbWarhouse", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbWarhouse_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbPointDetail",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PointID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: true),
                    Qty = table.Column<int>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: true),
                    UomID = table.Column<int>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPointDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbPointDetail_tbCurrency_CurrencyID",
                        column: x => x.CurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPointDetail_tbItemMasterData_ItemID",
                        column: x => x.ItemID,
                        principalSchema: "dbo",
                        principalTable: "tbItemMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPointDetail_tbPoint_PointID",
                        column: x => x.PointID,
                        principalSchema: "dbo",
                        principalTable: "tbPoint",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPointDetail_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbTable",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    GroupTableID = table.Column<int>(nullable: false),
                    Image = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: false),
                    Time = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbTable", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbTable_tbGroupTable_GroupTableID",
                        column: x => x.GroupTableID,
                        principalSchema: "dbo",
                        principalTable: "tbGroupTable",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbBOMaterial",
                schema: "dbo",
                columns: table => new
                {
                    BID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ItemID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    SysCID = table.Column<int>(nullable: false),
                    TotalCost = table.Column<double>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbBOMaterial", x => x.BID);
                    table.ForeignKey(
                        name: "FK_tbBOMaterial_tbItemMasterData_ItemID",
                        column: x => x.ItemID,
                        principalSchema: "dbo",
                        principalTable: "tbItemMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbBOMaterial_tbCurrency_SysCID",
                        column: x => x.SysCID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbBOMaterial_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbBOMaterial_tbUserAccount_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbIncomingPayment",
                schema: "dbo",
                columns: table => new
                {
                    IncomingPaymentID = table.Column<int>(nullable: false)
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
                    table.PrimaryKey("PK_tbIncomingPayment", x => x.IncomingPaymentID);
                    table.ForeignKey(
                        name: "FK_tbIncomingPayment_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbIncomingPayment_tbBusinessPartner_CustomerID",
                        column: x => x.CustomerID,
                        principalSchema: "dbo",
                        principalTable: "tbBusinessPartner",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbIncomingPayment_tbUserAccount_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbOutgoingpayment",
                schema: "dbo",
                columns: table => new
                {
                    OutgoingPaymentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<int>(nullable: false),
                    VendorID = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_tbOutgoingpayment", x => x.OutgoingPaymentID);
                    table.ForeignKey(
                        name: "FK_tbOutgoingpayment_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbOutgoingpayment_tbUserAccount_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbOutgoingpayment_tbBusinessPartner_VendorID",
                        column: x => x.VendorID,
                        principalSchema: "dbo",
                        principalTable: "tbBusinessPartner",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbGoodIssues",
                schema: "dbo",
                columns: table => new
                {
                    GoodIssuesID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BranchID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Ref_No = table.Column<string>(nullable: true),
                    Number_No = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    SysCurID = table.Column<int>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbGoodIssues", x => x.GoodIssuesID);
                    table.ForeignKey(
                        name: "FK_tbGoodIssues_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGoodIssues_tbUserAccount_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGoodIssues_tbWarhouse_WarehouseID",
                        column: x => x.WarehouseID,
                        principalSchema: "dbo",
                        principalTable: "tbWarhouse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbGoodReceiptReturn",
                schema: "dbo",
                columns: table => new
                {
                    GoodsReturnID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VendorID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    Reff_No = table.Column<string>(nullable: true),
                    InvoiceNo = table.Column<string>(nullable: true),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DueDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Sub_Total = table.Column<double>(nullable: false),
                    Sub_Total_sys = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValues = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    TaxRate = table.Column<double>(nullable: false),
                    TaxValuse = table.Column<double>(nullable: false),
                    Down_Payment = table.Column<double>(nullable: false),
                    Applied_Amount = table.Column<double>(nullable: false),
                    Return_Amount = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    Balance_Due = table.Column<double>(nullable: false),
                    Additional_Expense = table.Column<double>(nullable: false),
                    Additional_Node = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Balance_Due_Sys = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbGoodReceiptReturn", x => x.GoodsReturnID);
                    table.ForeignKey(
                        name: "FK_tbGoodReceiptReturn_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGoodReceiptReturn_tbUserAccount_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGoodReceiptReturn_tbBusinessPartner_VendorID",
                        column: x => x.VendorID,
                        principalSchema: "dbo",
                        principalTable: "tbBusinessPartner",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGoodReceiptReturn_tbWarhouse_WarehouseID",
                        column: x => x.WarehouseID,
                        principalSchema: "dbo",
                        principalTable: "tbWarhouse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbGoodsReceitp",
                schema: "dbo",
                columns: table => new
                {
                    GoodsReceiptID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BranchID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Ref_No = table.Column<string>(nullable: true),
                    Number_No = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    SysCurID = table.Column<int>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbGoodsReceitp", x => x.GoodsReceiptID);
                    table.ForeignKey(
                        name: "FK_tbGoodsReceitp_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGoodsReceitp_tbUserAccount_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGoodsReceitp_tbWarhouse_WarehouseID",
                        column: x => x.WarehouseID,
                        principalSchema: "dbo",
                        principalTable: "tbWarhouse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbGoodsReciptPO",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VendorID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    PurCurrencyID = table.Column<int>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    PurRate = table.Column<double>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    Reff_No = table.Column<string>(nullable: true),
                    InvoiceNo = table.Column<string>(nullable: true),
                    Balance_Due_Sys = table.Column<double>(nullable: false),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DueDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Sub_Total = table.Column<double>(nullable: false),
                    Sub_Total_sys = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    TaxRate = table.Column<double>(nullable: false),
                    TaxValuse = table.Column<double>(nullable: false),
                    Down_Payment = table.Column<double>(nullable: false),
                    Applied_Amount = table.Column<double>(nullable: false),
                    Return_Amount = table.Column<double>(nullable: false),
                    Balance_Due = table.Column<double>(nullable: false),
                    Additional_Expense = table.Column<double>(nullable: false),
                    Additional_Note = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbGoodsReciptPO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbGoodsReciptPO_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGoodsReciptPO_tbUserAccount_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGoodsReciptPO_tbBusinessPartner_VendorID",
                        column: x => x.VendorID,
                        principalSchema: "dbo",
                        principalTable: "tbBusinessPartner",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGoodsReciptPO_tbWarhouse_WarehouseID",
                        column: x => x.WarehouseID,
                        principalSchema: "dbo",
                        principalTable: "tbWarhouse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbInventoryAudit",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WarehouseID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    InvoiceNo = table.Column<string>(nullable: true),
                    Trans_Type = table.Column<string>(nullable: true),
                    Process = table.Column<string>(nullable: true),
                    SystemDate = table.Column<DateTime>(type: "Date", nullable: false),
                    TimeIn = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    CumulativeQty = table.Column<double>(nullable: false),
                    CumulativeValue = table.Column<double>(nullable: false),
                    Trans_Valuse = table.Column<double>(nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbInventoryAudit", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbInventoryAudit_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbInventoryAudit_tbCurrency_CurrencyID",
                        column: x => x.CurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbInventoryAudit_tbItemMasterData_ItemID",
                        column: x => x.ItemID,
                        principalSchema: "dbo",
                        principalTable: "tbItemMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbInventoryAudit_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbInventoryAudit_tbUserAccount_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbInventoryAudit_tbWarhouse_WarehouseID",
                        column: x => x.WarehouseID,
                        principalSchema: "dbo",
                        principalTable: "tbWarhouse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbPurchase_AP",
                schema: "dbo",
                columns: table => new
                {
                    PurchaseAPID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VendorID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    PurCurrencyID = table.Column<int>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    Reff_No = table.Column<string>(nullable: true),
                    InvoiceNo = table.Column<string>(nullable: true),
                    PurRate = table.Column<double>(nullable: false),
                    Balance_Due_Sys = table.Column<double>(nullable: false),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DueDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Sub_Total = table.Column<double>(nullable: false),
                    Sub_Total_sys = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    TaxRate = table.Column<double>(nullable: false),
                    TaxValuse = table.Column<double>(nullable: false),
                    Down_Payment = table.Column<double>(nullable: false),
                    Applied_Amount = table.Column<double>(nullable: false),
                    Return_Amount = table.Column<double>(nullable: false),
                    Balance_Due = table.Column<double>(nullable: false),
                    Additional_Expense = table.Column<double>(nullable: false),
                    Additional_Note = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    LocalSetRate = table.Column<double>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPurchase_AP", x => x.PurchaseAPID);
                    table.ForeignKey(
                        name: "FK_tbPurchase_AP_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchase_AP_tbUserAccount_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchase_AP_tbBusinessPartner_VendorID",
                        column: x => x.VendorID,
                        principalSchema: "dbo",
                        principalTable: "tbBusinessPartner",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchase_AP_tbWarhouse_WarehouseID",
                        column: x => x.WarehouseID,
                        principalSchema: "dbo",
                        principalTable: "tbWarhouse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbPurchaseCreditMemo",
                schema: "dbo",
                columns: table => new
                {
                    PurchaseMemoID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VendorID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    MemoCurID = table.Column<int>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    Reff_No = table.Column<string>(nullable: true),
                    InvoiceNo = table.Column<string>(nullable: true),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DueDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Sub_Total = table.Column<double>(nullable: false),
                    Sub_Total_sys = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValues = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    TaxRate = table.Column<double>(nullable: false),
                    TaxValuse = table.Column<double>(nullable: false),
                    Down_Payment = table.Column<double>(nullable: false),
                    Applied_Amount = table.Column<double>(nullable: false),
                    Return_Amount = table.Column<double>(nullable: false),
                    MemoRate = table.Column<double>(nullable: false),
                    Balance_Due = table.Column<double>(nullable: false),
                    Additional_Expense = table.Column<double>(nullable: false),
                    Additional_Node = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Balance_Due_Sys = table.Column<double>(nullable: false),
                    BaseOn = table.Column<string>(nullable: true),
                    LocalSetRate = table.Column<double>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPurchaseCreditMemo", x => x.PurchaseMemoID);
                    table.ForeignKey(
                        name: "FK_tbPurchaseCreditMemo_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseCreditMemo_tbUserAccount_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseCreditMemo_tbBusinessPartner_VendorID",
                        column: x => x.VendorID,
                        principalSchema: "dbo",
                        principalTable: "tbBusinessPartner",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseCreditMemo_tbWarhouse_WarehouseID",
                        column: x => x.WarehouseID,
                        principalSchema: "dbo",
                        principalTable: "tbWarhouse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbPurchaseOrder",
                schema: "dbo",
                columns: table => new
                {
                    PurchaseOrderID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VendorID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    PurCurrencyID = table.Column<int>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    Reff_No = table.Column<string>(nullable: true),
                    InvoiceNo = table.Column<string>(nullable: true),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Sub_Total = table.Column<double>(nullable: false),
                    Sub_Total_Sys = table.Column<double>(nullable: false),
                    DiscountValues = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    TaxRate = table.Column<double>(nullable: false),
                    TaxValues = table.Column<double>(nullable: false),
                    Balance_Due = table.Column<double>(nullable: false),
                    PurRate = table.Column<double>(nullable: false),
                    Balance_Due_Sys = table.Column<double>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    Down_Payment = table.Column<double>(nullable: false),
                    Applied_Amount = table.Column<double>(nullable: false),
                    Return_Amount = table.Column<double>(nullable: false),
                    Additional_Expense = table.Column<double>(nullable: false),
                    Additional_Note = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    LocalSetRate = table.Column<double>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPurchaseOrder", x => x.PurchaseOrderID);
                    table.ForeignKey(
                        name: "FK_tbPurchaseOrder_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseOrder_tbUserAccount_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseOrder_tbBusinessPartner_VendorID",
                        column: x => x.VendorID,
                        principalSchema: "dbo",
                        principalTable: "tbBusinessPartner",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseOrder_tbWarhouse_WarehouseID",
                        column: x => x.WarehouseID,
                        principalSchema: "dbo",
                        principalTable: "tbWarhouse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbPurchaseQuotation",
                schema: "dbo",
                columns: table => new
                {
                    PurchaseQuotationID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VendorID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    Reff_No = table.Column<string>(nullable: true),
                    InvoiceNo = table.Column<string>(nullable: true),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    RequiredDate = table.Column<DateTime>(type: "Date", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "Date", nullable: false),
                    Applied_Amount = table.Column<double>(nullable: false),
                    Balance_Due = table.Column<double>(nullable: false),
                    Sub_Total = table.Column<double>(nullable: false),
                    Sub_Total_Sys = table.Column<double>(nullable: false),
                    DiscountValues = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    TaxRate = table.Column<double>(nullable: false),
                    TaxValues = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    Balance_Due_Sys = table.Column<double>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPurchaseQuotation", x => x.PurchaseQuotationID);
                    table.ForeignKey(
                        name: "FK_tbPurchaseQuotation_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseQuotation_tbUserAccount_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseQuotation_tbBusinessPartner_VendorID",
                        column: x => x.VendorID,
                        principalSchema: "dbo",
                        principalTable: "tbBusinessPartner",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseQuotation_tbWarhouse_WarehouseID",
                        column: x => x.WarehouseID,
                        principalSchema: "dbo",
                        principalTable: "tbWarhouse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbPurchaseRequest",
                schema: "dbo",
                columns: table => new
                {
                    PurchaseRequestID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BranchID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    SystemCurrencyID = table.Column<int>(nullable: false),
                    WarehoueseID = table.Column<int>(nullable: false),
                    InvoiceNo = table.Column<string>(nullable: true),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    RequiredDate = table.Column<DateTime>(type: "Date", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "Date", nullable: false),
                    Balance_Due = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPurchaseRequest", x => x.PurchaseRequestID);
                    table.ForeignKey(
                        name: "FK_tbPurchaseRequest_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseRequest_tbCurrency_SystemCurrencyID",
                        column: x => x.SystemCurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseRequest_tbUserAccount_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseRequest_tbWarhouse_WarehoueseID",
                        column: x => x.WarehoueseID,
                        principalSchema: "dbo",
                        principalTable: "tbWarhouse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbRevenueItem",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WarehouseID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    InvoiceNo = table.Column<string>(nullable: true),
                    Trans_Type = table.Column<string>(nullable: true),
                    Process = table.Column<string>(nullable: true),
                    SystemDate = table.Column<DateTime>(type: "Date", nullable: false),
                    TimeIn = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    CumulativeQty = table.Column<double>(nullable: false),
                    CumulativeValue = table.Column<double>(nullable: false),
                    Trans_Valuse = table.Column<double>(nullable: false),
                    OpenQty = table.Column<double>(nullable: false),
                    ReceiptID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbRevenueItem", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbRevenueItem_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbRevenueItem_tbCurrency_CurrencyID",
                        column: x => x.CurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbRevenueItem_tbItemMasterData_ItemID",
                        column: x => x.ItemID,
                        principalSchema: "dbo",
                        principalTable: "tbItemMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbRevenueItem_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbRevenueItem_tbUserAccount_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbRevenueItem_tbWarhouse_WarehouseID",
                        column: x => x.WarehouseID,
                        principalSchema: "dbo",
                        principalTable: "tbWarhouse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbStockMoving",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WarehouseID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    SyetemDate = table.Column<DateTime>(type: "Date", nullable: false),
                    TimeIn = table.Column<string>(nullable: true),
                    InStock = table.Column<double>(nullable: false),
                    Committed = table.Column<double>(nullable: false),
                    Ordered = table.Column<double>(nullable: false),
                    Available = table.Column<double>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    WarehoseDetailLineID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbStockMoving", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbStockMoving_tbWarhouse_WarehouseID",
                        column: x => x.WarehouseID,
                        principalSchema: "dbo",
                        principalTable: "tbWarhouse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbTransfer",
                schema: "dbo",
                columns: table => new
                {
                    TarmsferID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WarehouseFromID = table.Column<int>(nullable: false),
                    WarehouseToID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    BranchToID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    UserRequestID = table.Column<int>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    Time = table.Column<string>(nullable: true),
                    SysCurID = table.Column<int>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbTransfer", x => x.TarmsferID);
                    table.ForeignKey(
                        name: "FK_tbTransfer_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbTransfer_tbUserAccount_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbTransfer_tbUserAccount_UserRequestID",
                        column: x => x.UserRequestID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbTransfer_tbWarhouse_WarehouseFromID",
                        column: x => x.WarehouseFromID,
                        principalSchema: "dbo",
                        principalTable: "tbWarhouse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbTransfer_tbWarhouse_WarehouseToID",
                        column: x => x.WarehouseToID,
                        principalSchema: "dbo",
                        principalTable: "tbWarhouse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbWarehouseDetail",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WarehouseID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    SyetemDate = table.Column<DateTime>(type: "Date", nullable: false),
                    TimeIn = table.Column<DateTime>(nullable: false),
                    InStock = table.Column<double>(nullable: false),
                    Committed = table.Column<double>(nullable: false),
                    Ordered = table.Column<double>(nullable: false),
                    Available = table.Column<double>(nullable: false),
                    Factor = table.Column<double>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    Cost = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbWarehouseDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbWarehouseDetail_tbWarhouse_WarehouseID",
                        column: x => x.WarehouseID,
                        principalSchema: "dbo",
                        principalTable: "tbWarhouse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbWarehouseSummary",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WarehouseID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    SyetemDate = table.Column<DateTime>(type: "Date", nullable: false),
                    TimeIn = table.Column<DateTime>(nullable: false),
                    InStock = table.Column<double>(nullable: false),
                    Committed = table.Column<double>(nullable: false),
                    Ordered = table.Column<double>(nullable: false),
                    Available = table.Column<double>(nullable: false),
                    Factor = table.Column<double>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    Cost = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbWarehouseSummary", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbWarehouseSummary_tbWarhouse_WarehouseID",
                        column: x => x.WarehouseID,
                        principalSchema: "dbo",
                        principalTable: "tbWarhouse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbReceipt",
                schema: "dbo",
                columns: table => new
                {
                    ReceiptID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
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
                    LocalSetRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbReceipt", x => x.ReceiptID);
                    table.ForeignKey(
                        name: "FK_tbReceipt_tbCurrency_PLCurrencyID",
                        column: x => x.PLCurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbReceipt_tbTable_TableID",
                        column: x => x.TableID,
                        principalSchema: "dbo",
                        principalTable: "tbTable",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbReceipt_tbUserAccount_UserOrderID",
                        column: x => x.UserOrderID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbBOMDetail",
                schema: "dbo",
                columns: table => new
                {
                    BDID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    Qty = table.Column<double>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    GUomID = table.Column<int>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    Detele = table.Column<bool>(nullable: false),
                    NegativeStock = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbBOMDetail", x => x.BDID);
                    table.ForeignKey(
                        name: "FK_tbBOMDetail_tbBOMaterial_BID",
                        column: x => x.BID,
                        principalSchema: "dbo",
                        principalTable: "tbBOMaterial",
                        principalColumn: "BID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbBOMDetail_tbGroupUoM_GUomID",
                        column: x => x.GUomID,
                        principalSchema: "dbo",
                        principalTable: "tbGroupUoM",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbBOMDetail_tbItemMasterData_ItemID",
                        column: x => x.ItemID,
                        principalSchema: "dbo",
                        principalTable: "tbItemMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbBOMDetail_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbIncomingPaymentDetail",
                schema: "dbo",
                columns: table => new
                {
                    IncomingPaymentDetailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IncomingPaymentID = table.Column<int>(nullable: false),
                    DocumentNo = table.Column<string>(nullable: true),
                    DocumentType = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(type: "Date", nullable: false),
                    OverdueDays = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    BalanceDue = table.Column<double>(nullable: false),
                    CashDiscount = table.Column<double>(nullable: false),
                    TotalDiscount = table.Column<double>(nullable: false),
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
                    table.PrimaryKey("PK_tbIncomingPaymentDetail", x => x.IncomingPaymentDetailID);
                    table.ForeignKey(
                        name: "FK_tbIncomingPaymentDetail_tbCurrency_CurrencyID",
                        column: x => x.CurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbIncomingPaymentDetail_tbIncomingPayment_IncomingPaymentID",
                        column: x => x.IncomingPaymentID,
                        principalSchema: "dbo",
                        principalTable: "tbIncomingPayment",
                        principalColumn: "IncomingPaymentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbOutgoingpaymnetDetail",
                schema: "dbo",
                columns: table => new
                {
                    OutgoingPaymentDetailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OutgoingPaymentID = table.Column<int>(nullable: false),
                    DocumentNo = table.Column<string>(nullable: true),
                    DocumentType = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(type: "Date", nullable: false),
                    OverdueDays = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    BalanceDue = table.Column<double>(nullable: false),
                    CashDiscount = table.Column<double>(nullable: false),
                    TotalDiscount = table.Column<double>(nullable: false),
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
                    table.PrimaryKey("PK_tbOutgoingpaymnetDetail", x => x.OutgoingPaymentDetailID);
                    table.ForeignKey(
                        name: "FK_tbOutgoingpaymnetDetail_tbCurrency_CurrencyID",
                        column: x => x.CurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbOutgoingpaymnetDetail_tbOutgoingpayment_OutgoingPaymentID",
                        column: x => x.OutgoingPaymentID,
                        principalSchema: "dbo",
                        principalTable: "tbOutgoingpayment",
                        principalColumn: "OutgoingPaymentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbGoodIssuesDetail",
                schema: "dbo",
                columns: table => new
                {
                    GoodIssuesDetailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GoodIssuesID = table.Column<int>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    LineID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Quantity = table.Column<double>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    UomName = table.Column<string>(nullable: true),
                    BarCode = table.Column<string>(nullable: true),
                    Check = table.Column<string>(nullable: true),
                    ManageExpire = table.Column<string>(nullable: true),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbGoodIssuesDetail", x => x.GoodIssuesDetailID);
                    table.ForeignKey(
                        name: "FK_tbGoodIssuesDetail_tbCurrency_CurrencyID",
                        column: x => x.CurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGoodIssuesDetail_tbGoodIssues_GoodIssuesID",
                        column: x => x.GoodIssuesID,
                        principalSchema: "dbo",
                        principalTable: "tbGoodIssues",
                        principalColumn: "GoodIssuesID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGoodIssuesDetail_tbItemMasterData_ItemID",
                        column: x => x.ItemID,
                        principalSchema: "dbo",
                        principalTable: "tbItemMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGoodIssuesDetail_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbGoodsReceiptReturnDetail",
                schema: "dbo",
                columns: table => new
                {
                    GoodsReturnDetailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GoodsReceiptPoReturnID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    LineID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    Qty = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    PurchasPrice = table.Column<double>(nullable: false),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    AlertStock = table.Column<double>(nullable: false),
                    check = table.Column<string>(nullable: true),
                    APID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbGoodsReceiptReturnDetail", x => x.GoodsReturnDetailID);
                    table.ForeignKey(
                        name: "FK_tbGoodsReceiptReturnDetail_tbGoodReceiptReturn_GoodsReceiptPoReturnID",
                        column: x => x.GoodsReceiptPoReturnID,
                        principalSchema: "dbo",
                        principalTable: "tbGoodReceiptReturn",
                        principalColumn: "GoodsReturnID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGoodsReceiptReturnDetail_tbItemMasterData_ItemID",
                        column: x => x.ItemID,
                        principalSchema: "dbo",
                        principalTable: "tbItemMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGoodsReceiptReturnDetail_tbCurrency_LocalCurrencyID",
                        column: x => x.LocalCurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGoodsReceiptReturnDetail_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbGoodReceitpDetail",
                schema: "dbo",
                columns: table => new
                {
                    GoodReceitpDetailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GoodsReceiptID = table.Column<int>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    LineID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Quantity = table.Column<double>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    UomName = table.Column<string>(nullable: true),
                    BarCode = table.Column<string>(nullable: true),
                    Check = table.Column<string>(nullable: true),
                    ManageExpire = table.Column<string>(nullable: true),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbGoodReceitpDetail", x => x.GoodReceitpDetailID);
                    table.ForeignKey(
                        name: "FK_tbGoodReceitpDetail_tbCurrency_CurrencyID",
                        column: x => x.CurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGoodReceitpDetail_tbGoodsReceitp_GoodsReceiptID",
                        column: x => x.GoodsReceiptID,
                        principalSchema: "dbo",
                        principalTable: "tbGoodsReceitp",
                        principalColumn: "GoodsReceiptID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGoodReceitpDetail_tbItemMasterData_ItemID",
                        column: x => x.ItemID,
                        principalSchema: "dbo",
                        principalTable: "tbItemMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGoodReceitpDetail_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbGoodsReciptPODatail",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GoodsReciptPOID = table.Column<int>(nullable: false),
                    LineID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    PurchasPrice = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    OrderID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: false),
                    AlertStock = table.Column<double>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    OpenQty = table.Column<double>(nullable: false),
                    Check = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbGoodsReciptPODatail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbGoodsReciptPODatail_tbGoodsReciptPO_GoodsReciptPOID",
                        column: x => x.GoodsReciptPOID,
                        principalSchema: "dbo",
                        principalTable: "tbGoodsReciptPO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGoodsReciptPODatail_tbItemMasterData_ItemID",
                        column: x => x.ItemID,
                        principalSchema: "dbo",
                        principalTable: "tbItemMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbGoodsReciptPODatail_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbPurchaseAPDetail",
                schema: "dbo",
                columns: table => new
                {
                    PurchaseDetailAPID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Purchase_APID = table.Column<int>(nullable: true),
                    LineID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    PurchasPrice = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    OrderID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: false),
                    AlertStock = table.Column<double>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    OpenQty = table.Column<double>(nullable: false),
                    Check = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPurchaseAPDetail", x => x.PurchaseDetailAPID);
                    table.ForeignKey(
                        name: "FK_tbPurchaseAPDetail_tbItemMasterData_ItemID",
                        column: x => x.ItemID,
                        principalSchema: "dbo",
                        principalTable: "tbItemMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseAPDetail_tbCurrency_LocalCurrencyID",
                        column: x => x.LocalCurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseAPDetail_tbPurchase_AP_Purchase_APID",
                        column: x => x.Purchase_APID,
                        principalSchema: "dbo",
                        principalTable: "tbPurchase_AP",
                        principalColumn: "PurchaseAPID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseAPDetail_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbPurchaseCreditMemoDetail",
                columns: table => new
                {
                    PurchaseMemoDetailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PurchaseCreditMemoID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    LineID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    Qty = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    PurchasPrice = table.Column<double>(nullable: false),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    AlertStock = table.Column<double>(nullable: false),
                    check = table.Column<string>(nullable: true),
                    APID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPurchaseCreditMemoDetail", x => x.PurchaseMemoDetailID);
                    table.ForeignKey(
                        name: "FK_tbPurchaseCreditMemoDetail_tbItemMasterData_ItemID",
                        column: x => x.ItemID,
                        principalSchema: "dbo",
                        principalTable: "tbItemMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseCreditMemoDetail_tbCurrency_LocalCurrencyID",
                        column: x => x.LocalCurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseCreditMemoDetail_tbPurchaseCreditMemo_PurchaseCreditMemoID",
                        column: x => x.PurchaseCreditMemoID,
                        principalSchema: "dbo",
                        principalTable: "tbPurchaseCreditMemo",
                        principalColumn: "PurchaseMemoID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseCreditMemoDetail_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbPurchaseOrderDetail",
                schema: "dbo",
                columns: table => new
                {
                    PurchaseOrderDetailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PurchaseOrderID = table.Column<int>(nullable: false),
                    LineID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    PurchasPrice = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    AlertStock = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    OpenQty = table.Column<double>(nullable: false),
                    OldQty = table.Column<double>(nullable: false),
                    QuotationID = table.Column<int>(nullable: false),
                    Check = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPurchaseOrderDetail", x => x.PurchaseOrderDetailID);
                    table.ForeignKey(
                        name: "FK_tbPurchaseOrderDetail_tbItemMasterData_ItemID",
                        column: x => x.ItemID,
                        principalSchema: "dbo",
                        principalTable: "tbItemMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseOrderDetail_tbCurrency_LocalCurrencyID",
                        column: x => x.LocalCurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseOrderDetail_tbPurchaseOrder_PurchaseOrderID",
                        column: x => x.PurchaseOrderID,
                        principalSchema: "dbo",
                        principalTable: "tbPurchaseOrder",
                        principalColumn: "PurchaseOrderID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseOrderDetail_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbPurchaseQuotationDetail",
                schema: "dbo",
                columns: table => new
                {
                    PurchaseQuotaionDetailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PurchaseQuotationID = table.Column<int>(nullable: false),
                    LineID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    PurchasPrice = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: false),
                    AlertStock = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    OpenQty = table.Column<double>(nullable: false),
                    OldQty = table.Column<double>(nullable: false),
                    RequiredDate = table.Column<DateTime>(type: "Date", nullable: false),
                    QuotedDate = table.Column<DateTime>(type: "Date", nullable: false),
                    RequiredQty = table.Column<double>(nullable: false),
                    requestID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPurchaseQuotationDetail", x => x.PurchaseQuotaionDetailID);
                    table.ForeignKey(
                        name: "FK_tbPurchaseQuotationDetail_tbItemMasterData_ItemID",
                        column: x => x.ItemID,
                        principalSchema: "dbo",
                        principalTable: "tbItemMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseQuotationDetail_tbCurrency_LocalCurrencyID",
                        column: x => x.LocalCurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseQuotationDetail_tbPurchaseQuotation_PurchaseQuotationID",
                        column: x => x.PurchaseQuotationID,
                        principalSchema: "dbo",
                        principalTable: "tbPurchaseQuotation",
                        principalColumn: "PurchaseQuotationID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseQuotationDetail_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbPurchaseRequiredDetail",
                schema: "dbo",
                columns: table => new
                {
                    RequiredDetailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PurchaseRequestID = table.Column<int>(nullable: false),
                    LineID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    VendorID = table.Column<int>(nullable: false),
                    SystemCurrencyID = table.Column<int>(nullable: false),
                    OpenQty = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    RequiredDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Qty = table.Column<double>(nullable: false),
                    UnitPrice = table.Column<double>(nullable: false),
                    ExchangRate = table.Column<double>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPurchaseRequiredDetail", x => x.RequiredDetailID);
                    table.ForeignKey(
                        name: "FK_tbPurchaseRequiredDetail_tbItemMasterData_ItemID",
                        column: x => x.ItemID,
                        principalSchema: "dbo",
                        principalTable: "tbItemMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseRequiredDetail_tbPurchaseRequest_PurchaseRequestID",
                        column: x => x.PurchaseRequestID,
                        principalSchema: "dbo",
                        principalTable: "tbPurchaseRequest",
                        principalColumn: "PurchaseRequestID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseRequiredDetail_tbCurrency_SystemCurrencyID",
                        column: x => x.SystemCurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbPurchaseRequiredDetail_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbTarnsferDetail",
                schema: "dbo",
                columns: table => new
                {
                    TarnsferDetailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TransferID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    LineID = table.Column<int>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    UomName = table.Column<string>(nullable: true),
                    Barcode = table.Column<string>(nullable: true),
                    Check = table.Column<string>(nullable: true),
                    ManageExpire = table.Column<string>(nullable: true),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbTarnsferDetail", x => x.TarnsferDetailID);
                    table.ForeignKey(
                        name: "FK_tbTarnsferDetail_tbCurrency_CurrencyID",
                        column: x => x.CurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbTarnsferDetail_tbItemMasterData_ItemID",
                        column: x => x.ItemID,
                        principalSchema: "dbo",
                        principalTable: "tbItemMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbTarnsferDetail_tbTransfer_TransferID",
                        column: x => x.TransferID,
                        principalSchema: "dbo",
                        principalTable: "tbTransfer",
                        principalColumn: "TarmsferID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbTarnsferDetail_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbReceiptDetail",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReceiptID = table.Column<int>(nullable: false),
                    OrderDetailID = table.Column<int>(nullable: false),
                    OrderID = table.Column<int>(nullable: true),
                    Line_ID = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_tbReceiptDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbReceiptDetail_tbReceipt_ReceiptID",
                        column: x => x.ReceiptID,
                        principalSchema: "dbo",
                        principalTable: "tbReceipt",
                        principalColumn: "ReceiptID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbReceiptDetail_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseCreditMemoDetail_ItemID",
                table: "tbPurchaseCreditMemoDetail",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseCreditMemoDetail_LocalCurrencyID",
                table: "tbPurchaseCreditMemoDetail",
                column: "LocalCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseCreditMemoDetail_PurchaseCreditMemoID",
                table: "tbPurchaseCreditMemoDetail",
                column: "PurchaseCreditMemoID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseCreditMemoDetail_UomID",
                table: "tbPurchaseCreditMemoDetail",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbSaleARDetail_SARID",
                table: "tbSaleARDetail",
                column: "SARID");

            migrationBuilder.CreateIndex(
                name: "IX_tbSaleCreditMemoDetail_SCMOID",
                table: "tbSaleCreditMemoDetail",
                column: "SCMOID");

            migrationBuilder.CreateIndex(
                name: "IX_tbSaleDeliveryDetail_SDID",
                table: "tbSaleDeliveryDetail",
                column: "SDID");

            migrationBuilder.CreateIndex(
                name: "IX_tbSaleQuoteDetail_SQID",
                table: "tbSaleQuoteDetail",
                column: "SQID");

            migrationBuilder.CreateIndex(
                name: "IX_ItemGroup1_BackID",
                schema: "dbo",
                table: "ItemGroup1",
                column: "BackID");

            migrationBuilder.CreateIndex(
                name: "IX_ItemGroup1_ColorID",
                schema: "dbo",
                table: "ItemGroup1",
                column: "ColorID");

            migrationBuilder.CreateIndex(
                name: "IX_ItemGroup2_BackID",
                schema: "dbo",
                table: "ItemGroup2",
                column: "BackID");

            migrationBuilder.CreateIndex(
                name: "IX_ItemGroup2_ColorID",
                schema: "dbo",
                table: "ItemGroup2",
                column: "ColorID");

            migrationBuilder.CreateIndex(
                name: "IX_ItemGroup2_ItemG1ID",
                schema: "dbo",
                table: "ItemGroup2",
                column: "ItemG1ID");

            migrationBuilder.CreateIndex(
                name: "IX_ItemGroup3_BackID",
                schema: "dbo",
                table: "ItemGroup3",
                column: "BackID");

            migrationBuilder.CreateIndex(
                name: "IX_ItemGroup3_ColorID",
                schema: "dbo",
                table: "ItemGroup3",
                column: "ColorID");

            migrationBuilder.CreateIndex(
                name: "IX_ItemGroup3_ItemG1ID",
                schema: "dbo",
                table: "ItemGroup3",
                column: "ItemG1ID");

            migrationBuilder.CreateIndex(
                name: "IX_ItemGroup3_ItemG2ID",
                schema: "dbo",
                table: "ItemGroup3",
                column: "ItemG2ID");

            migrationBuilder.CreateIndex(
                name: "IX_tbBOMaterial_ItemID",
                schema: "dbo",
                table: "tbBOMaterial",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_tbBOMaterial_SysCID",
                schema: "dbo",
                table: "tbBOMaterial",
                column: "SysCID");

            migrationBuilder.CreateIndex(
                name: "IX_tbBOMaterial_UomID",
                schema: "dbo",
                table: "tbBOMaterial",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbBOMaterial_UserID",
                schema: "dbo",
                table: "tbBOMaterial",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbBOMDetail_BID",
                schema: "dbo",
                table: "tbBOMDetail",
                column: "BID");

            migrationBuilder.CreateIndex(
                name: "IX_tbBOMDetail_GUomID",
                schema: "dbo",
                table: "tbBOMDetail",
                column: "GUomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbBOMDetail_ItemID",
                schema: "dbo",
                table: "tbBOMDetail",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_tbBOMDetail_UomID",
                schema: "dbo",
                table: "tbBOMDetail",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbBranch_CompanyID",
                schema: "dbo",
                table: "tbBranch",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbBusinessPartner_Code",
                schema: "dbo",
                table: "tbBusinessPartner",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbBusinessPartner_PriceListID",
                schema: "dbo",
                table: "tbBusinessPartner",
                column: "PriceListID");

            migrationBuilder.CreateIndex(
                name: "IX_tbCompany_LocalCurrencyID",
                schema: "dbo",
                table: "tbCompany",
                column: "LocalCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbCompany_PriceListID",
                schema: "dbo",
                table: "tbCompany",
                column: "PriceListID");

            migrationBuilder.CreateIndex(
                name: "IX_tbCompany_SystemCurrencyID",
                schema: "dbo",
                table: "tbCompany",
                column: "SystemCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbCounter_PrinterID",
                schema: "dbo",
                table: "tbCounter",
                column: "PrinterID");

            migrationBuilder.CreateIndex(
                name: "IX_tbEmployee_Code",
                schema: "dbo",
                table: "tbEmployee",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbExchangeRate_CurrencyID",
                schema: "dbo",
                table: "tbExchangeRate",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodIssues_BranchID",
                schema: "dbo",
                table: "tbGoodIssues",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodIssues_UserID",
                schema: "dbo",
                table: "tbGoodIssues",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodIssues_WarehouseID",
                schema: "dbo",
                table: "tbGoodIssues",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodIssuesDetail_CurrencyID",
                schema: "dbo",
                table: "tbGoodIssuesDetail",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodIssuesDetail_GoodIssuesID",
                schema: "dbo",
                table: "tbGoodIssuesDetail",
                column: "GoodIssuesID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodIssuesDetail_ItemID",
                schema: "dbo",
                table: "tbGoodIssuesDetail",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodIssuesDetail_UomID",
                schema: "dbo",
                table: "tbGoodIssuesDetail",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodReceiptReturn_BranchID",
                schema: "dbo",
                table: "tbGoodReceiptReturn",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodReceiptReturn_UserID",
                schema: "dbo",
                table: "tbGoodReceiptReturn",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodReceiptReturn_VendorID",
                schema: "dbo",
                table: "tbGoodReceiptReturn",
                column: "VendorID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodReceiptReturn_WarehouseID",
                schema: "dbo",
                table: "tbGoodReceiptReturn",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodReceitpDetail_CurrencyID",
                schema: "dbo",
                table: "tbGoodReceitpDetail",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodReceitpDetail_GoodsReceiptID",
                schema: "dbo",
                table: "tbGoodReceitpDetail",
                column: "GoodsReceiptID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodReceitpDetail_ItemID",
                schema: "dbo",
                table: "tbGoodReceitpDetail",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodReceitpDetail_UomID",
                schema: "dbo",
                table: "tbGoodReceitpDetail",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodsReceiptReturnDetail_GoodsReceiptPoReturnID",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail",
                column: "GoodsReceiptPoReturnID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodsReceiptReturnDetail_ItemID",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodsReceiptReturnDetail_LocalCurrencyID",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail",
                column: "LocalCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodsReceiptReturnDetail_UomID",
                schema: "dbo",
                table: "tbGoodsReceiptReturnDetail",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodsReceitp_BranchID",
                schema: "dbo",
                table: "tbGoodsReceitp",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodsReceitp_UserID",
                schema: "dbo",
                table: "tbGoodsReceitp",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodsReceitp_WarehouseID",
                schema: "dbo",
                table: "tbGoodsReceitp",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodsReciptPO_BranchID",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodsReciptPO_UserID",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodsReciptPO_VendorID",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                column: "VendorID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodsReciptPO_WarehouseID",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodsReciptPODatail_GoodsReciptPOID",
                schema: "dbo",
                table: "tbGoodsReciptPODatail",
                column: "GoodsReciptPOID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodsReciptPODatail_ItemID",
                schema: "dbo",
                table: "tbGoodsReciptPODatail",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGoodsReciptPODatail_UomID",
                schema: "dbo",
                table: "tbGoodsReciptPODatail",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGroupDefindUoM_GroupUoMID",
                schema: "dbo",
                table: "tbGroupDefindUoM",
                column: "GroupUoMID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGroupDefindUoM_UoMID",
                schema: "dbo",
                table: "tbGroupDefindUoM",
                column: "UoMID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGroupTable_BranchID",
                schema: "dbo",
                table: "tbGroupTable",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_tbGroupUoM_Code",
                schema: "dbo",
                table: "tbGroupUoM",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbIncomingPayment_BranchID",
                schema: "dbo",
                table: "tbIncomingPayment",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_tbIncomingPayment_CustomerID",
                schema: "dbo",
                table: "tbIncomingPayment",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_tbIncomingPayment_UserID",
                schema: "dbo",
                table: "tbIncomingPayment",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbIncomingPaymentDetail_CurrencyID",
                schema: "dbo",
                table: "tbIncomingPaymentDetail",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbIncomingPaymentDetail_IncomingPaymentID",
                schema: "dbo",
                table: "tbIncomingPaymentDetail",
                column: "IncomingPaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_tbInventoryAudit_BranchID",
                schema: "dbo",
                table: "tbInventoryAudit",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_tbInventoryAudit_CurrencyID",
                schema: "dbo",
                table: "tbInventoryAudit",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbInventoryAudit_ItemID",
                schema: "dbo",
                table: "tbInventoryAudit",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_tbInventoryAudit_UomID",
                schema: "dbo",
                table: "tbInventoryAudit",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbInventoryAudit_UserID",
                schema: "dbo",
                table: "tbInventoryAudit",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbInventoryAudit_WarehouseID",
                schema: "dbo",
                table: "tbInventoryAudit",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_tbItemMasterData_GroupUomID",
                schema: "dbo",
                table: "tbItemMasterData",
                column: "GroupUomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbItemMasterData_InventoryUoMID",
                schema: "dbo",
                table: "tbItemMasterData",
                column: "InventoryUoMID");

            migrationBuilder.CreateIndex(
                name: "IX_tbItemMasterData_ItemGroup1ID",
                schema: "dbo",
                table: "tbItemMasterData",
                column: "ItemGroup1ID");

            migrationBuilder.CreateIndex(
                name: "IX_tbItemMasterData_ItemGroup2ID",
                schema: "dbo",
                table: "tbItemMasterData",
                column: "ItemGroup2ID");

            migrationBuilder.CreateIndex(
                name: "IX_tbItemMasterData_ItemGroup3ID",
                schema: "dbo",
                table: "tbItemMasterData",
                column: "ItemGroup3ID");

            migrationBuilder.CreateIndex(
                name: "IX_tbItemMasterData_PriceListID",
                schema: "dbo",
                table: "tbItemMasterData",
                column: "PriceListID");

            migrationBuilder.CreateIndex(
                name: "IX_tbItemMasterData_PrintToID",
                schema: "dbo",
                table: "tbItemMasterData",
                column: "PrintToID");

            migrationBuilder.CreateIndex(
                name: "IX_tbItemMasterData_PurchaseUomID",
                schema: "dbo",
                table: "tbItemMasterData",
                column: "PurchaseUomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbItemMasterData_SaleUomID",
                schema: "dbo",
                table: "tbItemMasterData",
                column: "SaleUomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbMemberCard_CardTypeID",
                schema: "dbo",
                table: "tbMemberCard",
                column: "CardTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_tbOrder_PLCurrencyID",
                schema: "dbo",
                table: "tbOrder",
                column: "PLCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbOrderDetail_OrderID",
                schema: "dbo",
                table: "tbOrderDetail",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_tbOrderDetail_UomID",
                schema: "dbo",
                table: "tbOrderDetail",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbOrderDetail_Addon_UomID",
                schema: "dbo",
                table: "tbOrderDetail_Addon",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbOutgoingpayment_BranchID",
                schema: "dbo",
                table: "tbOutgoingpayment",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_tbOutgoingpayment_UserID",
                schema: "dbo",
                table: "tbOutgoingpayment",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbOutgoingpayment_VendorID",
                schema: "dbo",
                table: "tbOutgoingpayment",
                column: "VendorID");

            migrationBuilder.CreateIndex(
                name: "IX_tbOutgoingpaymnetDetail_CurrencyID",
                schema: "dbo",
                table: "tbOutgoingpaymnetDetail",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbOutgoingpaymnetDetail_OutgoingPaymentID",
                schema: "dbo",
                table: "tbOutgoingpaymnetDetail",
                column: "OutgoingPaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPointCard_PointID",
                schema: "dbo",
                table: "tbPointCard",
                column: "PointID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPointDetail_CurrencyID",
                schema: "dbo",
                table: "tbPointDetail",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPointDetail_ItemID",
                schema: "dbo",
                table: "tbPointDetail",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPointDetail_PointID",
                schema: "dbo",
                table: "tbPointDetail",
                column: "PointID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPointDetail_UomID",
                schema: "dbo",
                table: "tbPointDetail",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPriceList_CurrencyID",
                schema: "dbo",
                table: "tbPriceList",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPriceListDetail_PriceListID",
                schema: "dbo",
                table: "tbPriceListDetail",
                column: "PriceListID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPromotionPrice_CurrencyID",
                schema: "dbo",
                table: "tbPromotionPrice",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchase_AP_BranchID",
                schema: "dbo",
                table: "tbPurchase_AP",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchase_AP_UserID",
                schema: "dbo",
                table: "tbPurchase_AP",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchase_AP_VendorID",
                schema: "dbo",
                table: "tbPurchase_AP",
                column: "VendorID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchase_AP_WarehouseID",
                schema: "dbo",
                table: "tbPurchase_AP",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseAPDetail_ItemID",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseAPDetail_LocalCurrencyID",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                column: "LocalCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseAPDetail_Purchase_APID",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                column: "Purchase_APID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseAPDetail_UomID",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseCreditMemo_BranchID",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseCreditMemo_UserID",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseCreditMemo_VendorID",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                column: "VendorID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseCreditMemo_WarehouseID",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseOrder_BranchID",
                schema: "dbo",
                table: "tbPurchaseOrder",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseOrder_UserID",
                schema: "dbo",
                table: "tbPurchaseOrder",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseOrder_VendorID",
                schema: "dbo",
                table: "tbPurchaseOrder",
                column: "VendorID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseOrder_WarehouseID",
                schema: "dbo",
                table: "tbPurchaseOrder",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseOrderDetail_ItemID",
                schema: "dbo",
                table: "tbPurchaseOrderDetail",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseOrderDetail_LocalCurrencyID",
                schema: "dbo",
                table: "tbPurchaseOrderDetail",
                column: "LocalCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseOrderDetail_PurchaseOrderID",
                schema: "dbo",
                table: "tbPurchaseOrderDetail",
                column: "PurchaseOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseOrderDetail_UomID",
                schema: "dbo",
                table: "tbPurchaseOrderDetail",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseQuotation_BranchID",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseQuotation_UserID",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseQuotation_VendorID",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                column: "VendorID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseQuotation_WarehouseID",
                schema: "dbo",
                table: "tbPurchaseQuotation",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseQuotationDetail_ItemID",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseQuotationDetail_LocalCurrencyID",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                column: "LocalCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseQuotationDetail_PurchaseQuotationID",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                column: "PurchaseQuotationID");

            migrationBuilder.CreateIndex(
                name: "IX_tbPurchaseQuotationDetail_UomID",
                schema: "dbo",
                table: "tbPurchaseQuotationDetail",
                column: "UomID");

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
                name: "IX_tbPurchaseRequiredDetail_PurchaseRequestID",
                schema: "dbo",
                table: "tbPurchaseRequiredDetail",
                column: "PurchaseRequestID");

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

            migrationBuilder.CreateIndex(
                name: "IX_tbReceipt_PLCurrencyID",
                schema: "dbo",
                table: "tbReceipt",
                column: "PLCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbReceipt_TableID",
                schema: "dbo",
                table: "tbReceipt",
                column: "TableID");

            migrationBuilder.CreateIndex(
                name: "IX_tbReceipt_UserOrderID",
                schema: "dbo",
                table: "tbReceipt",
                column: "UserOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_tbReceiptDetail_ReceiptID",
                schema: "dbo",
                table: "tbReceiptDetail",
                column: "ReceiptID");

            migrationBuilder.CreateIndex(
                name: "IX_tbReceiptDetail_UomID",
                schema: "dbo",
                table: "tbReceiptDetail",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbReceiptInformation_BranchID",
                schema: "dbo",
                table: "tbReceiptInformation",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_tbRevenueItem_BranchID",
                schema: "dbo",
                table: "tbRevenueItem",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_tbRevenueItem_CurrencyID",
                schema: "dbo",
                table: "tbRevenueItem",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbRevenueItem_ItemID",
                schema: "dbo",
                table: "tbRevenueItem",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_tbRevenueItem_UomID",
                schema: "dbo",
                table: "tbRevenueItem",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbRevenueItem_UserID",
                schema: "dbo",
                table: "tbRevenueItem",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbRevenueItem_WarehouseID",
                schema: "dbo",
                table: "tbRevenueItem",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_tbSaleOrderDetail_SOID",
                schema: "dbo",
                table: "tbSaleOrderDetail",
                column: "SOID");

            migrationBuilder.CreateIndex(
                name: "IX_tbStockMoving_WarehouseID",
                schema: "dbo",
                table: "tbStockMoving",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_tbTable_GroupTableID",
                schema: "dbo",
                table: "tbTable",
                column: "GroupTableID");

            migrationBuilder.CreateIndex(
                name: "IX_tbTarnsferDetail_CurrencyID",
                schema: "dbo",
                table: "tbTarnsferDetail",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbTarnsferDetail_ItemID",
                schema: "dbo",
                table: "tbTarnsferDetail",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_tbTarnsferDetail_TransferID",
                schema: "dbo",
                table: "tbTarnsferDetail",
                column: "TransferID");

            migrationBuilder.CreateIndex(
                name: "IX_tbTarnsferDetail_UomID",
                schema: "dbo",
                table: "tbTarnsferDetail",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbTransfer_BranchID",
                schema: "dbo",
                table: "tbTransfer",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_tbTransfer_UserID",
                schema: "dbo",
                table: "tbTransfer",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbTransfer_UserRequestID",
                schema: "dbo",
                table: "tbTransfer",
                column: "UserRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_tbTransfer_WarehouseFromID",
                schema: "dbo",
                table: "tbTransfer",
                column: "WarehouseFromID");

            migrationBuilder.CreateIndex(
                name: "IX_tbTransfer_WarehouseToID",
                schema: "dbo",
                table: "tbTransfer",
                column: "WarehouseToID");

            migrationBuilder.CreateIndex(
                name: "IX_tbUnitofMeasure_Code",
                schema: "dbo",
                table: "tbUnitofMeasure",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbUserAccount_BranchID",
                schema: "dbo",
                table: "tbUserAccount",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_tbUserAccount_CompanyID",
                schema: "dbo",
                table: "tbUserAccount",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbUserAccount_EmployeeID",
                schema: "dbo",
                table: "tbUserAccount",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_tbUserAccount_UserID",
                schema: "dbo",
                table: "tbUserAccount",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbUserPrivillege_FunctionID",
                schema: "dbo",
                table: "tbUserPrivillege",
                column: "FunctionID");

            migrationBuilder.CreateIndex(
                name: "IX_tbVoidOrder_PLCurrencyID",
                schema: "dbo",
                table: "tbVoidOrder",
                column: "PLCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_tbVoidOrderDetail_OrderID",
                schema: "dbo",
                table: "tbVoidOrderDetail",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_tbVoidOrderDetail_UomID",
                schema: "dbo",
                table: "tbVoidOrderDetail",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbWarehouseDetail_WarehouseID",
                schema: "dbo",
                table: "tbWarehouseDetail",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_tbWarehouseSummary_WarehouseID",
                schema: "dbo",
                table: "tbWarehouseSummary",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_tbWarhouse_BranchID",
                schema: "dbo",
                table: "tbWarhouse",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_tbWarhouse_Code",
                schema: "dbo",
                table: "tbWarhouse",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tpItemCopyToPriceListDetail_ItemCopyToPriceListID",
                schema: "dbo",
                table: "tpItemCopyToPriceListDetail",
                column: "ItemCopyToPriceListID");

            migrationBuilder.CreateIndex(
                name: "IX_tpItemCopyToWHDetail_ItemCopyToWHID",
                schema: "dbo",
                table: "tpItemCopyToWHDetail",
                column: "ItemCopyToWHID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CopyPurchaseOrder_from_Quotation");

            migrationBuilder.DropTable(
                name: "ServicePriceListCopyItem");

            migrationBuilder.DropTable(
                name: "SummarySaleAdmin");

            migrationBuilder.DropTable(
                name: "tbGLAccount");

            migrationBuilder.DropTable(
                name: "tbPurchaseCreditMemoDetail");

            migrationBuilder.DropTable(
                name: "tbSaleARDetail");

            migrationBuilder.DropTable(
                name: "tbSaleCreditMemoDetail");

            migrationBuilder.DropTable(
                name: "tbSaleDeliveryDetail");

            migrationBuilder.DropTable(
                name: "tbSaleQuoteDetail");

            migrationBuilder.DropTable(
                name: "CopyPurchaseAP_To_PurchaseMemo",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "db_DashboardTopSale",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "db_SaleSummary",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "db_StockExpire",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PrintPurchaseAP",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ReportPurchasAP",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ReportPurchasCreditMemo",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ReportPurchaseOrder",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ReportPurchaseQuotation",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ReportPurchaseRequst",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "rp_Cashout",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "rp_CashoutPaymentMeans",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "rp_CloseShift",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "rp_DetailPurchaseAP",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "rp_DetailSale",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "rp_DetailTopSale",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "rp_StockInWarehouse_Detail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "rp_StokInWarehouse",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "rp_SummaryDetailOutgoingPayment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "rp_SummaryDetailTransferStock",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "rp_SummaryOutgoingPayment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "rp_SummaryPurchaseAP",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "rp_SummaryRevenuseItem",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "rp_summarysale",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "rp_SummaryTansferStock",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "rp_SummaryTotalPurchase",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "rp_SummaryTotalSale",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "rp_TopSaleQuantity",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ServiceCheckPayment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ServiceDiscountItemDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ServiceInventoryAudit",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ServiceItemSales",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ServiceMapItemMasterDataPurchasAP",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ServiceMapItemMasterDataPurchaseCreditMemo",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ServiceMapItemMasterDataPurchaseOrder",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ServiceMapItemMasterDataQuotation",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ServiceMapItemMasterPurchaseRequest",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ServiceMasterData",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ServicePointDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ServicePricelistSetPrice",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ServiceQuotationDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbBOMDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbCloseShift",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbCounter",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbDisplayCurrency",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbExchangeRate",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbGeneralSetting",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbGoodIssuesDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbGoodReceitpDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbGoodsReceiptReturnDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbGoodsReciptPODatail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbGroupDefindUoM",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbIncomingPaymentCustomer",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbIncomingPaymentDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbInventoryAudit",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbItemComment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbLogin",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbMemberCard",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbOpenShift",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbOrder_Queue",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbOrder_Receipt",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbOrderDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbOrderDetail_Addon",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbOutgoingPaymentVendor",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbOutgoingpaymnetDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbPaymentMeans",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbPointCard",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbPointDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbPriceListDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbPromotion",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbPromotionPrice",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbPurchaseAPDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbPurchaseOrderDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbPurchaseQuotationDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbPurchaseRequiredDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbReceiptDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbReceiptInformation",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbRevenueItem",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbSaleOrderDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbStockMoving",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbSystemType",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbTarnsferDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbTax",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbVoidOrderDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbWarehouseDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbWarehouseSummary",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tmpOrderDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tpGoodReciptStock",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tpItemCopyToPriceListDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tpItemCopyToWHDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tpPriceList",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbPurchaseCreditMemo",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbSaleAR");

            migrationBuilder.DropTable(
                name: "tbSaleCreditMemo");

            migrationBuilder.DropTable(
                name: "tbSaleDelivery");

            migrationBuilder.DropTable(
                name: "tbSaleQuote");

            migrationBuilder.DropTable(
                name: "tbBOMaterial",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbGoodIssues",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbGoodsReceitp",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbGoodReceiptReturn",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbGoodsReciptPO",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbIncomingPayment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CardType",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbOrder",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbOutgoingpayment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbPoint",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbPurchase_AP",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbPurchaseOrder",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbPurchaseQuotation",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbPurchaseRequest",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbReceipt",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbSaleOrder",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbTransfer",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbVoidOrder",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tpItemCopyToPriceList",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tpItemCopyToWH",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbItemMasterData",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbBusinessPartner",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbTable",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbUserAccount",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbWarhouse",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbGroupUoM",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbUnitofMeasure",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ItemGroup3",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbPrinterName",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbGroupTable",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbEmployee",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbUserPrivillege",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ItemGroup2",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbBranch",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Funtion",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ItemGroup1",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbCompany",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Background",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Colors",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbPriceList",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbCurrency",
                schema: "dbo");
        }
    }
}
