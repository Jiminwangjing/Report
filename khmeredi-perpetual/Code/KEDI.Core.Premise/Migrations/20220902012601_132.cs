using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _132 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DraftDeliveries",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    CusID = table.Column<int>(nullable: false),
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
                    SubTotal = table.Column<double>(nullable: false),
                    SubTotalSys = table.Column<double>(nullable: false),
                    DisRate = table.Column<double>(nullable: false),
                    DisValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    VatRate = table.Column<double>(nullable: false),
                    VatValue = table.Column<double>(nullable: false),
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
                    SaleEmID = table.Column<int>(nullable: false),
                    BaseOnID = table.Column<int>(nullable: false),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftDeliveries", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DraftReserveInvoices",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
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
                    SaleEmID = table.Column<int>(nullable: false),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftReserveInvoices", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DraftServiceContracts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
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
                    SaleEmID = table.Column<int>(nullable: false),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftServiceContracts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MultiIncommings",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IncomingID = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    GLAccID = table.Column<int>(nullable: false),
                    CurrID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultiIncommings", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DraftDeliveryDetails",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DraftDeliveryID = table.Column<int>(nullable: false),
                    SQDID = table.Column<int>(nullable: false),
                    SODID = table.Column<int>(nullable: false),
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
                    Process = table.Column<string>(nullable: true),
                    CurrencyID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    ItemType = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftDeliveryDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DraftDeliveryDetails_DraftDeliveries_DraftDeliveryID",
                        column: x => x.DraftDeliveryID,
                        principalTable: "DraftDeliveries",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DraftReserveInvoiceDetails",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DraftReserveInvoiceID = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_DraftReserveInvoiceDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DraftReserveInvoiceDetails_DraftReserveInvoices_DraftReserveInvoiceID",
                        column: x => x.DraftReserveInvoiceID,
                        principalTable: "DraftReserveInvoices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DraftAttchmentFiles",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DraftServiceContractID = table.Column<int>(nullable: false),
                    TargetPath = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    AttachmentDate = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftAttchmentFiles", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DraftAttchmentFiles_DraftServiceContracts_DraftServiceContractID",
                        column: x => x.DraftServiceContractID,
                        principalTable: "DraftServiceContracts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DraftServiceContractDetails",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DraftServiceContractID = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_DraftServiceContractDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DraftServiceContractDetails_DraftServiceContracts_DraftServiceContractID",
                        column: x => x.DraftServiceContractID,
                        principalTable: "DraftServiceContracts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DraftAttchmentFiles_DraftServiceContractID",
                table: "DraftAttchmentFiles",
                column: "DraftServiceContractID");

            migrationBuilder.CreateIndex(
                name: "IX_DraftDeliveryDetails_DraftDeliveryID",
                table: "DraftDeliveryDetails",
                column: "DraftDeliveryID");

            migrationBuilder.CreateIndex(
                name: "IX_DraftReserveInvoiceDetails_DraftReserveInvoiceID",
                table: "DraftReserveInvoiceDetails",
                column: "DraftReserveInvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_DraftServiceContractDetails_DraftServiceContractID",
                table: "DraftServiceContractDetails",
                column: "DraftServiceContractID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DraftAttchmentFiles");

            migrationBuilder.DropTable(
                name: "DraftDeliveryDetails");

            migrationBuilder.DropTable(
                name: "DraftReserveInvoiceDetails");

            migrationBuilder.DropTable(
                name: "DraftServiceContractDetails");

            migrationBuilder.DropTable(
                name: "MultiIncommings");

            migrationBuilder.DropTable(
                name: "DraftDeliveries");

            migrationBuilder.DropTable(
                name: "DraftReserveInvoices");

            migrationBuilder.DropTable(
                name: "DraftServiceContracts");
        }
    }
}
