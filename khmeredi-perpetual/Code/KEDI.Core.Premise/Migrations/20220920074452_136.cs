using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _136 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachmentFileOfContractTemplate_ContractTemplate_ContractTemplateID",
                table: "AttachmentFileOfContractTemplate");

            migrationBuilder.DropForeignKey(
                name: "FK_Converage_ContractTemplate_ContractTemplateID",
                table: "Converage");

            migrationBuilder.DropForeignKey(
                name: "FK_Remark_ContractTemplate_ContractTemplateID",
                table: "Remark");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Remark_ContractTemplateID",
                table: "Remark");

            migrationBuilder.DropIndex(
                name: "IX_Converage_ContractTemplateID",
                table: "Converage");

            migrationBuilder.RenameColumn(
                name: "ContractTemplateID",
                table: "Remark",
                newName: "ContractID");

            migrationBuilder.RenameColumn(
                name: "ContractTemplateID",
                table: "Converage",
                newName: "ContractID");

            migrationBuilder.RenameColumn(
                name: "StarttimeWed",
                table: "ContractTemplate",
                newName: "ContractName");

            migrationBuilder.AddColumn<string>(
                name: "PlateNumber",
                table: "TmpSerialNumberSelectedDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ConverageID",
                table: "ContractTemplate",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "ContractTemplate",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RemarkID",
                table: "ContractTemplate",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ContractTemplateID",
                table: "AttachmentFileOfContractTemplate",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "ContractID",
                table: "AttachmentFileOfContractTemplate",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DraftReserves",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    VendorID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    PurCurrencyID = table.Column<int>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    DocumentTypeID = table.Column<int>(nullable: false),
                    SeriesID = table.Column<int>(nullable: false),
                    SeriesDetailID = table.Column<int>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    ReffNo = table.Column<string>(nullable: true),
                    InvoiceNo = table.Column<string>(nullable: true),
                    PurRate = table.Column<double>(nullable: false),
                    BalanceDueSys = table.Column<double>(nullable: false),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DueDate = table.Column<DateTime>(type: "Date", nullable: false),
                    SubTotal = table.Column<double>(nullable: false),
                    SubTotalSys = table.Column<double>(nullable: false),
                    SubTotalAfterDis = table.Column<decimal>(nullable: false),
                    SubTotalAfterDisSys = table.Column<decimal>(nullable: false),
                    FrieghtAmount = table.Column<decimal>(nullable: false),
                    FrieghtAmountSys = table.Column<decimal>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    TaxRate = table.Column<double>(nullable: false),
                    TaxValue = table.Column<double>(nullable: false),
                    DownPayment = table.Column<double>(nullable: false),
                    DownPaymentSys = table.Column<double>(nullable: false),
                    AppliedAmount = table.Column<double>(nullable: false),
                    AppliedAmountSys = table.Column<double>(nullable: false),
                    ReturnAmount = table.Column<double>(nullable: false),
                    BalanceDue = table.Column<double>(nullable: false),
                    AdditionalExpense = table.Column<double>(nullable: false),
                    AdditionalNote = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    LocalSetRate = table.Column<double>(nullable: false),
                    LocalCurID = table.Column<int>(nullable: false),
                    BaseOnID = table.Column<int>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    CopyToNote = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftReserves", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DraftReserves_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DraftReserves_tbUserAccount_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DraftReserves_tbBusinessPartner_VendorID",
                        column: x => x.VendorID,
                        principalSchema: "dbo",
                        principalTable: "tbBusinessPartner",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DraftReserves_tbWarhouse_WarehouseID",
                        column: x => x.WarehouseID,
                        principalSchema: "dbo",
                        principalTable: "tbWarhouse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DraftReserveDetail",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DraftReserveID = table.Column<int>(nullable: false),
                    LineID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    TaxGroupID = table.Column<int>(nullable: false),
                    TaxRate = table.Column<decimal>(nullable: false),
                    TaxValue = table.Column<decimal>(nullable: false),
                    TaxOfFinDisValue = table.Column<decimal>(nullable: false),
                    FinTotalValue = table.Column<decimal>(nullable: false),
                    FinDisRate = table.Column<decimal>(nullable: false),
                    FinDisValue = table.Column<decimal>(nullable: false),
                    TotalWTax = table.Column<double>(nullable: false),
                    TotalWTaxSys = table.Column<double>(nullable: false),
                    TotalSys = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    OpenQty = table.Column<double>(nullable: false),
                    PurchasPrice = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    OrderID = table.Column<int>(nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: false),
                    AlertStock = table.Column<double>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    Check = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftReserveDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DraftReserveDetail_DraftReserves_DraftReserveID",
                        column: x => x.DraftReserveID,
                        principalTable: "DraftReserves",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DraftReserveDetail_tbItemMasterData_ItemID",
                        column: x => x.ItemID,
                        principalSchema: "dbo",
                        principalTable: "tbItemMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DraftReserveDetail_tbCurrency_LocalCurrencyID",
                        column: x => x.LocalCurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DraftReserveDetail_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractTemplate_ConverageID",
                table: "ContractTemplate",
                column: "ConverageID");

            migrationBuilder.CreateIndex(
                name: "IX_ContractTemplate_RemarkID",
                table: "ContractTemplate",
                column: "RemarkID");

            migrationBuilder.CreateIndex(
                name: "IX_DraftReserveDetail_DraftReserveID",
                table: "DraftReserveDetail",
                column: "DraftReserveID");

            migrationBuilder.CreateIndex(
                name: "IX_DraftReserveDetail_ItemID",
                table: "DraftReserveDetail",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_DraftReserveDetail_LocalCurrencyID",
                table: "DraftReserveDetail",
                column: "LocalCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_DraftReserveDetail_UomID",
                table: "DraftReserveDetail",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_DraftReserves_BranchID",
                table: "DraftReserves",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_DraftReserves_UserID",
                table: "DraftReserves",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_DraftReserves_VendorID",
                table: "DraftReserves",
                column: "VendorID");

            migrationBuilder.CreateIndex(
                name: "IX_DraftReserves_WarehouseID",
                table: "DraftReserves",
                column: "WarehouseID");

            migrationBuilder.AddForeignKey(
                name: "FK_AttachmentFileOfContractTemplate_ContractTemplate_ContractTemplateID",
                table: "AttachmentFileOfContractTemplate",
                column: "ContractTemplateID",
                principalTable: "ContractTemplate",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractTemplate_Converage_ConverageID",
                table: "ContractTemplate",
                column: "ConverageID",
                principalTable: "Converage",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractTemplate_Remark_RemarkID",
                table: "ContractTemplate",
                column: "RemarkID",
                principalTable: "Remark",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachmentFileOfContractTemplate_ContractTemplate_ContractTemplateID",
                table: "AttachmentFileOfContractTemplate");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractTemplate_Converage_ConverageID",
                table: "ContractTemplate");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractTemplate_Remark_RemarkID",
                table: "ContractTemplate");

            migrationBuilder.DropTable(
                name: "DraftReserveDetail");

            migrationBuilder.DropTable(
                name: "DraftReserves");

            migrationBuilder.DropIndex(
                name: "IX_ContractTemplate_ConverageID",
                table: "ContractTemplate");

            migrationBuilder.DropIndex(
                name: "IX_ContractTemplate_RemarkID",
                table: "ContractTemplate");

            migrationBuilder.DropColumn(
                name: "PlateNumber",
                table: "TmpSerialNumberSelectedDetail");

            migrationBuilder.DropColumn(
                name: "ConverageID",
                table: "ContractTemplate");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "ContractTemplate");

            migrationBuilder.DropColumn(
                name: "RemarkID",
                table: "ContractTemplate");

            migrationBuilder.DropColumn(
                name: "ContractID",
                table: "AttachmentFileOfContractTemplate");

            migrationBuilder.RenameColumn(
                name: "ContractID",
                table: "Remark",
                newName: "ContractTemplateID");

            migrationBuilder.RenameColumn(
                name: "ContractID",
                table: "Converage",
                newName: "ContractTemplateID");

            migrationBuilder.RenameColumn(
                name: "ContractName",
                table: "ContractTemplate",
                newName: "StarttimeWed");

            migrationBuilder.AlterColumn<int>(
                name: "ContractTemplateID",
                table: "AttachmentFileOfContractTemplate",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContractType = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Duration = table.Column<decimal>(nullable: false),
                    Expired = table.Column<bool>(nullable: false),
                    Friday = table.Column<bool>(nullable: false),
                    FridayEndTime = table.Column<string>(nullable: true),
                    FridayStartTime = table.Column<string>(nullable: true),
                    Includeholiday = table.Column<bool>(nullable: false),
                    Labor = table.Column<bool>(nullable: false),
                    Monday = table.Column<bool>(nullable: false),
                    MondayEndTime = table.Column<string>(nullable: true),
                    MondayStartTime = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Parts = table.Column<bool>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    ReminderType = table.Column<int>(nullable: false),
                    ReminderValue = table.Column<decimal>(nullable: false),
                    Renewal = table.Column<bool>(nullable: false),
                    ResolutionTimeType = table.Column<int>(nullable: false),
                    ResolutionTimeValue = table.Column<decimal>(nullable: false),
                    ResponseTimeType = table.Column<int>(nullable: false),
                    ResponseTimeValue = table.Column<decimal>(nullable: false),
                    Saturday = table.Column<bool>(nullable: false),
                    SaturdayEndTime = table.Column<string>(nullable: true),
                    SaturdayStartTime = table.Column<string>(nullable: true),
                    Sunday = table.Column<bool>(nullable: false),
                    SundayEndTime = table.Column<string>(nullable: true),
                    SundayStartTime = table.Column<string>(nullable: true),
                    Thursday = table.Column<bool>(nullable: false),
                    ThursdayEndTime = table.Column<string>(nullable: true),
                    ThursdayStartTime = table.Column<string>(nullable: true),
                    Travel = table.Column<bool>(nullable: false),
                    Tuesday = table.Column<bool>(nullable: false),
                    TuesdayEndTime = table.Column<string>(nullable: true),
                    TuesdayStartTime = table.Column<string>(nullable: true),
                    Wednesday = table.Column<bool>(nullable: false),
                    WednesdayEndTime = table.Column<string>(nullable: true),
                    WednesdayStartTime = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Remark_ContractTemplateID",
                table: "Remark",
                column: "ContractTemplateID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Converage_ContractTemplateID",
                table: "Converage",
                column: "ContractTemplateID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AttachmentFileOfContractTemplate_ContractTemplate_ContractTemplateID",
                table: "AttachmentFileOfContractTemplate",
                column: "ContractTemplateID",
                principalTable: "ContractTemplate",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Converage_ContractTemplate_ContractTemplateID",
                table: "Converage",
                column: "ContractTemplateID",
                principalTable: "ContractTemplate",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Remark_ContractTemplate_ContractTemplateID",
                table: "Remark",
                column: "ContractTemplateID",
                principalTable: "ContractTemplate",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
