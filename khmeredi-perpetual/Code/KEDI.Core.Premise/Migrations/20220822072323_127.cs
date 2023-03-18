using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _127 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbARAdjustableDetail_tbARAdjustable_SARID",
                table: "tbARAdjustableDetail");

            migrationBuilder.DropTable(
                name: "SaleAREditDetailReport");

            migrationBuilder.DropTable(
                name: "SaleAREditReport");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbARAdjustableDetail",
                table: "tbARAdjustableDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbARAdjustable",
                table: "tbARAdjustable");

            migrationBuilder.RenameTable(
                name: "tbARAdjustableDetail",
                newName: "SaleAREditeDetails");

            migrationBuilder.RenameTable(
                name: "tbARAdjustable",
                newName: "SaleAREdites");

            migrationBuilder.RenameIndex(
                name: "IX_tbARAdjustableDetail_SARID",
                table: "SaleAREditeDetails",
                newName: "IX_SaleAREditeDetails_SARID");

            migrationBuilder.AddColumn<int>(
                name: "JEID",
                table: "tbAccountBalance",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaleAREditeHistoryID",
                table: "SaleAREditeDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "SaleAREditeDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SaleAREditeDetails",
                table: "SaleAREditeDetails",
                column: "SARDID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SaleAREdites",
                table: "SaleAREdites",
                column: "SARID");

            migrationBuilder.CreateTable(
                name: "SaleAREditeDetailHistory",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SAREID = table.Column<int>(nullable: false),
                    SARDID = table.Column<int>(nullable: false),
                    SARID = table.Column<int>(nullable: false),
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
                    Delete = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleAREditeDetailHistory", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SaleAREditeHistory",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SARID = table.Column<int>(nullable: false),
                    CusID = table.Column<int>(nullable: false),
                    RequestedBy = table.Column<int>(nullable: false),
                    ShippedBy = table.Column<int>(nullable: false),
                    ReceivedBy = table.Column<int>(nullable: false),
                    BaseOnID = table.Column<int>(nullable: false),
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
                    LocalSetRate = table.Column<double>(nullable: false),
                    SaleEmID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleAREditeHistory", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleAREditeDetails_SaleAREditeHistoryID",
                table: "SaleAREditeDetails",
                column: "SaleAREditeHistoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleAREditeDetails_SaleAREdites_SARID",
                table: "SaleAREditeDetails",
                column: "SARID",
                principalTable: "SaleAREdites",
                principalColumn: "SARID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleAREditeDetails_SaleAREditeHistory_SaleAREditeHistoryID",
                table: "SaleAREditeDetails",
                column: "SaleAREditeHistoryID",
                principalTable: "SaleAREditeHistory",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleAREditeDetails_SaleAREdites_SARID",
                table: "SaleAREditeDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleAREditeDetails_SaleAREditeHistory_SaleAREditeHistoryID",
                table: "SaleAREditeDetails");

            migrationBuilder.DropTable(
                name: "SaleAREditeDetailHistory");

            migrationBuilder.DropTable(
                name: "SaleAREditeHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SaleAREdites",
                table: "SaleAREdites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SaleAREditeDetails",
                table: "SaleAREditeDetails");

            migrationBuilder.DropIndex(
                name: "IX_SaleAREditeDetails_SaleAREditeHistoryID",
                table: "SaleAREditeDetails");

            migrationBuilder.DropColumn(
                name: "JEID",
                table: "tbAccountBalance");

            migrationBuilder.DropColumn(
                name: "SaleAREditeHistoryID",
                table: "SaleAREditeDetails");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "SaleAREditeDetails");

            migrationBuilder.RenameTable(
                name: "SaleAREdites",
                newName: "tbARAdjustable");

            migrationBuilder.RenameTable(
                name: "SaleAREditeDetails",
                newName: "tbARAdjustableDetail");

            migrationBuilder.RenameIndex(
                name: "IX_SaleAREditeDetails_SARID",
                table: "tbARAdjustableDetail",
                newName: "IX_tbARAdjustableDetail_SARID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbARAdjustable",
                table: "tbARAdjustable",
                column: "SARID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbARAdjustableDetail",
                table: "tbARAdjustableDetail",
                column: "SARDID");

            migrationBuilder.CreateTable(
                name: "SaleAREditReport",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AppliedAmount = table.Column<double>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    CusID = table.Column<int>(nullable: false),
                    DisValue = table.Column<double>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    InvoiceNumber = table.Column<string>(nullable: true),
                    LocalCurID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    SaleCurrencyID = table.Column<int>(nullable: false),
                    SubTotal = table.Column<double>(nullable: false),
                    TimeIn = table.Column<string>(nullable: false),
                    TotalAmount = table.Column<double>(nullable: false),
                    TotalAmountSys = table.Column<double>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    VatRate = table.Column<double>(nullable: false),
                    VatValue = table.Column<double>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleAREditReport", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SaleAREditDetailReport",
                columns: table => new
                {
                    SARDID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Cost = table.Column<double>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    DisRate = table.Column<double>(nullable: false),
                    DisValue = table.Column<double>(nullable: false),
                    EditeQty = table.Column<double>(nullable: false),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    Factor = table.Column<double>(nullable: false),
                    FinDisRate = table.Column<decimal>(nullable: false),
                    FinDisValue = table.Column<decimal>(nullable: false),
                    FinTotalValue = table.Column<decimal>(nullable: false),
                    GUomID = table.Column<int>(nullable: false),
                    ID = table.Column<int>(nullable: false),
                    ItemCode = table.Column<string>(nullable: true),
                    ItemID = table.Column<int>(nullable: false),
                    ItemNameEN = table.Column<string>(nullable: true),
                    ItemNameKH = table.Column<string>(nullable: true),
                    ItemType = table.Column<string>(nullable: true),
                    OpenQty = table.Column<double>(nullable: false),
                    PrintQty = table.Column<double>(nullable: false),
                    Qty = table.Column<double>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    TaxGroupID = table.Column<int>(nullable: false),
                    TaxOfFinDisValue = table.Column<decimal>(nullable: false),
                    TaxRate = table.Column<decimal>(nullable: false),
                    TaxValue = table.Column<decimal>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    TotalSys = table.Column<double>(nullable: false),
                    TotalWTax = table.Column<double>(nullable: false),
                    TotalWTaxSys = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    UnitPrice = table.Column<double>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    UomName = table.Column<string>(nullable: true),
                    VatRate = table.Column<double>(nullable: false),
                    VatValue = table.Column<double>(nullable: false)
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

            migrationBuilder.AddForeignKey(
                name: "FK_tbARAdjustableDetail_tbARAdjustable_SARID",
                table: "tbARAdjustableDetail",
                column: "SARID",
                principalTable: "tbARAdjustable",
                principalColumn: "SARID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
