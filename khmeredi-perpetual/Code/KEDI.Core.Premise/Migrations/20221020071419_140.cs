using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _140 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BaseOnID",
                schema: "dbo",
                table: "tbTransfer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PostingDate",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "PostingDate",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DelayHours",
                schema: "dbo",
                table: "tbGeneralSetting",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TransferRequests",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
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
                    LocalSetRate = table.Column<double>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    SeriseID = table.Column<int>(nullable: false),
                    SeriseDID = table.Column<int>(nullable: false),
                    DocTypeID = table.Column<int>(nullable: false),
                    TranRequStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferRequests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TransferRequests_tbBranch_BranchID",
                        column: x => x.BranchID,
                        principalSchema: "dbo",
                        principalTable: "tbBranch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferRequests_tbUserAccount_UserID",
                        column: x => x.UserID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferRequests_tbUserAccount_UserRequestID",
                        column: x => x.UserRequestID,
                        principalSchema: "dbo",
                        principalTable: "tbUserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferRequests_tbWarhouse_WarehouseFromID",
                        column: x => x.WarehouseFromID,
                        principalSchema: "dbo",
                        principalTable: "tbWarhouse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferRequests_tbWarhouse_WarehouseToID",
                        column: x => x.WarehouseToID,
                        principalSchema: "dbo",
                        principalTable: "tbWarhouse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransferRequestDetails",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TransferRequestID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    LineID = table.Column<int>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    FWarehouseID = table.Column<int>(nullable: false),
                    TWarehouseID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    OPenQty = table.Column<double>(nullable: false),
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
                    table.PrimaryKey("PK_TransferRequestDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TransferRequestDetails_tbCurrency_CurrencyID",
                        column: x => x.CurrencyID,
                        principalSchema: "dbo",
                        principalTable: "tbCurrency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferRequestDetails_tbItemMasterData_ItemID",
                        column: x => x.ItemID,
                        principalSchema: "dbo",
                        principalTable: "tbItemMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferRequestDetails_TransferRequests_TransferRequestID",
                        column: x => x.TransferRequestID,
                        principalTable: "TransferRequests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferRequestDetails_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequestDetails_CurrencyID",
                table: "TransferRequestDetails",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequestDetails_ItemID",
                table: "TransferRequestDetails",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequestDetails_TransferRequestID",
                table: "TransferRequestDetails",
                column: "TransferRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequestDetails_UomID",
                table: "TransferRequestDetails",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_BranchID",
                table: "TransferRequests",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_UserID",
                table: "TransferRequests",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_UserRequestID",
                table: "TransferRequests",
                column: "UserRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_WarehouseFromID",
                table: "TransferRequests",
                column: "WarehouseFromID");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_WarehouseToID",
                table: "TransferRequests",
                column: "WarehouseToID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransferRequestDetails");

            migrationBuilder.DropTable(
                name: "TransferRequests");

            migrationBuilder.DropColumn(
                name: "BaseOnID",
                schema: "dbo",
                table: "tbTransfer");

            migrationBuilder.DropColumn(
                name: "PostingDate",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "PostingDate",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "DelayHours",
                schema: "dbo",
                table: "tbGeneralSetting");
        }
    }
}
