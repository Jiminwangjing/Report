using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _143 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SAREDTDID",
                schema: "dbo",
                table: "tbSaleOrderDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SAREDTDID",
                table: "tbSaleQuoteDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SAREDTDID",
                table: "tbSaleDeliveryDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ARReDetEDTID",
                table: "tbSaleCreditMemoDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SAREDTDID",
                table: "DraftReserveInvoiceDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ARReserveInvoiceEditables",
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
                    SaleEmID = table.Column<int>(nullable: false),
                    Delivery = table.Column<bool>(nullable: false),
                    Paded = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARReserveInvoiceEditables", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DraftReserveInvoiceEditables",
                columns: table => new
                {
                    DraffID = table.Column<int>(nullable: false)
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
                    table.PrimaryKey("PK_DraftReserveInvoiceEditables", x => x.DraffID);
                });

            migrationBuilder.CreateTable(
                name: "ARReserveInvoiceEditableDetails",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ARReserveInvoiceEditableID = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_ARReserveInvoiceEditableDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ARReserveInvoiceEditableDetails_ARReserveInvoiceEditables_ARReserveInvoiceEditableID",
                        column: x => x.ARReserveInvoiceEditableID,
                        principalTable: "ARReserveInvoiceEditables",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DraftReserveInvoiceEditableDetails",
                columns: table => new
                {
                    DraffDID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DraftReserveInvoiceEditableID = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_DraftReserveInvoiceEditableDetails", x => x.DraffDID);
                    table.ForeignKey(
                        name: "FK_DraftReserveInvoiceEditableDetails_DraftReserveInvoiceEditables_DraftReserveInvoiceEditableID",
                        column: x => x.DraftReserveInvoiceEditableID,
                        principalTable: "DraftReserveInvoiceEditables",
                        principalColumn: "DraffID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ARReserveInvoiceEditableDetails_ARReserveInvoiceEditableID",
                table: "ARReserveInvoiceEditableDetails",
                column: "ARReserveInvoiceEditableID");

            migrationBuilder.CreateIndex(
                name: "IX_DraftReserveInvoiceEditableDetails_DraftReserveInvoiceEditableID",
                table: "DraftReserveInvoiceEditableDetails",
                column: "DraftReserveInvoiceEditableID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ARReserveInvoiceEditableDetails");

            migrationBuilder.DropTable(
                name: "DraftReserveInvoiceEditableDetails");

            migrationBuilder.DropTable(
                name: "ARReserveInvoiceEditables");

            migrationBuilder.DropTable(
                name: "DraftReserveInvoiceEditables");

            migrationBuilder.DropColumn(
                name: "SAREDTDID",
                schema: "dbo",
                table: "tbSaleOrderDetail");

            migrationBuilder.DropColumn(
                name: "SAREDTDID",
                table: "tbSaleQuoteDetail");

            migrationBuilder.DropColumn(
                name: "SAREDTDID",
                table: "tbSaleDeliveryDetail");

            migrationBuilder.DropColumn(
                name: "ARReDetEDTID",
                table: "tbSaleCreditMemoDetail");

            migrationBuilder.DropColumn(
                name: "SAREDTDID",
                table: "DraftReserveInvoiceDetails");
        }
    }
}
