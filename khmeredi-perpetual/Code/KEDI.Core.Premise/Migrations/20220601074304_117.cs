using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _117 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBatchNo",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSerialNumber",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "OutStandingPoint",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Point",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "TmpBatchNo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderDetailID = table.Column<int>(nullable: false),
                    LineID = table.Column<string>(nullable: true),
                    ItemCode = table.Column<string>(nullable: true),
                    ItemName = table.Column<string>(nullable: true),
                    WhsCode = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(nullable: false),
                    TotalNeeded = table.Column<decimal>(nullable: false),
                    TotalSelected = table.Column<decimal>(nullable: false),
                    TotalBatches = table.Column<decimal>(nullable: false),
                    Direction = table.Column<string>(nullable: true),
                    ItemID = table.Column<int>(nullable: false),
                    Cost = table.Column<decimal>(nullable: false),
                    BpId = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    SaleID = table.Column<int>(nullable: false),
                    BaseQty = table.Column<decimal>(nullable: false),
                    WareId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TmpBatchNo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TmpBatchNo_tbOrderDetail_OrderDetailID",
                        column: x => x.OrderDetailID,
                        principalSchema: "dbo",
                        principalTable: "tbOrderDetail",
                        principalColumn: "OrderDetailID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TmpSerialNumber",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderDetailID = table.Column<int>(nullable: false),
                    LineID = table.Column<string>(nullable: true),
                    ItemCode = table.Column<string>(nullable: true),
                    ItemName = table.Column<string>(nullable: true),
                    WhsCode = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(nullable: false),
                    TotalSelected = table.Column<decimal>(nullable: false),
                    OpenQty = table.Column<decimal>(nullable: false),
                    Direction = table.Column<string>(nullable: true),
                    ItemID = table.Column<int>(nullable: false),
                    Cost = table.Column<decimal>(nullable: false),
                    BpId = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    SaleID = table.Column<int>(nullable: false),
                    BaseQty = table.Column<decimal>(nullable: false),
                    WareId = table.Column<int>(nullable: false),
                    Barcode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TmpSerialNumber", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TmpSerialNumber_tbOrderDetail_OrderDetailID",
                        column: x => x.OrderDetailID,
                        principalSchema: "dbo",
                        principalTable: "tbOrderDetail",
                        principalColumn: "OrderDetailID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TmpBatchNoSelected",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BatchNoID = table.Column<int>(nullable: false),
                    TotalSelected = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TmpBatchNoSelected", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TmpBatchNoSelected_TmpBatchNo_BatchNoID",
                        column: x => x.BatchNoID,
                        principalTable: "TmpBatchNo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TmpSerialNumberSelected",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SerialNumberID = table.Column<int>(nullable: false),
                    TotalSelected = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TmpSerialNumberSelected", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TmpSerialNumberSelected_TmpSerialNumber_SerialNumberID",
                        column: x => x.SerialNumberID,
                        principalTable: "TmpSerialNumber",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TmpBatchNoSelectedDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LineID = table.Column<string>(nullable: true),
                    BatchNo = table.Column<string>(nullable: true),
                    AvailableQty = table.Column<decimal>(nullable: false),
                    SelectedQty = table.Column<decimal>(nullable: false),
                    UnitCost = table.Column<decimal>(nullable: false),
                    BPID = table.Column<int>(nullable: false),
                    OrigialQty = table.Column<decimal>(nullable: false),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    BatchNoSelectedID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TmpBatchNoSelectedDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TmpBatchNoSelectedDetail_TmpBatchNoSelected_BatchNoSelectedID",
                        column: x => x.BatchNoSelectedID,
                        principalTable: "TmpBatchNoSelected",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TmpSerialNumberSelectedDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LineID = table.Column<string>(nullable: true),
                    SerialNumber = table.Column<string>(nullable: true),
                    MfrSerialNo = table.Column<string>(nullable: true),
                    LotNumber = table.Column<string>(nullable: true),
                    UnitCost = table.Column<decimal>(nullable: false),
                    Qty = table.Column<decimal>(nullable: false),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    SerialNumberSelectedID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TmpSerialNumberSelectedDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TmpSerialNumberSelectedDetail_TmpSerialNumberSelected_SerialNumberSelectedID",
                        column: x => x.SerialNumberSelectedID,
                        principalTable: "TmpSerialNumberSelected",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TmpBatchNo_OrderDetailID",
                table: "TmpBatchNo",
                column: "OrderDetailID");

            migrationBuilder.CreateIndex(
                name: "IX_TmpBatchNoSelected_BatchNoID",
                table: "TmpBatchNoSelected",
                column: "BatchNoID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TmpBatchNoSelectedDetail_BatchNoSelectedID",
                table: "TmpBatchNoSelectedDetail",
                column: "BatchNoSelectedID");

            migrationBuilder.CreateIndex(
                name: "IX_TmpSerialNumber_OrderDetailID",
                table: "TmpSerialNumber",
                column: "OrderDetailID");

            migrationBuilder.CreateIndex(
                name: "IX_TmpSerialNumberSelected_SerialNumberID",
                table: "TmpSerialNumberSelected",
                column: "SerialNumberID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TmpSerialNumberSelectedDetail_SerialNumberSelectedID",
                table: "TmpSerialNumberSelectedDetail",
                column: "SerialNumberSelectedID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TmpBatchNoSelectedDetail");

            migrationBuilder.DropTable(
                name: "TmpSerialNumberSelectedDetail");

            migrationBuilder.DropTable(
                name: "TmpBatchNoSelected");

            migrationBuilder.DropTable(
                name: "TmpSerialNumberSelected");

            migrationBuilder.DropTable(
                name: "TmpBatchNo");

            migrationBuilder.DropTable(
                name: "TmpSerialNumber");

            migrationBuilder.DropColumn(
                name: "IsBatchNo",
                schema: "dbo",
                table: "tbOrderDetail");

            migrationBuilder.DropColumn(
                name: "IsSerialNumber",
                schema: "dbo",
                table: "tbOrderDetail");

            migrationBuilder.DropColumn(
                name: "OutStandingPoint",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "Point",
                schema: "dbo",
                table: "tbOrder");
        }
    }
}
