using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _78 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsKsms",
                schema: "dbo",
                table: "tbReceiptDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsKsmsMaster",
                schema: "dbo",
                table: "tbReceiptDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "KSServiceSetupId",
                schema: "dbo",
                table: "tbReceiptDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VehicleId",
                schema: "dbo",
                table: "tbReceiptDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VehicleID",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsKsms",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsKsmsMaster",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "KSServiceSetupId",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VehicleId",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VehicleID",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Scale",
                schema: "dbo",
                table: "tbItemMasterData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "VehicleID",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsKsms",
                table: "ReceiptDetailMemoKvms",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsKsmsMaster",
                table: "ReceiptDetailMemoKvms",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "KSServiceSetupId",
                table: "ReceiptDetailMemoKvms",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VehicleId",
                table: "ReceiptDetailMemoKvms",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GeneralServiceSetups",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralServiceSetups", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "KSServiceHistories",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    KSServiceMasterID = table.Column<int>(nullable: false),
                    KSServiceID = table.Column<int>(nullable: false),
                    ReceiptID = table.Column<int>(nullable: false),
                    ReceiptDID = table.Column<int>(nullable: false),
                    Qty = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KSServiceHistories", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "KSServiceMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Number = table.Column<string>(nullable: true),
                    DocTypeID = table.Column<int>(nullable: false),
                    SeriesID = table.Column<int>(nullable: false),
                    SeriesDID = table.Column<int>(nullable: false),
                    CusId = table.Column<int>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18, 8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KSServiceMaster", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "KSServices",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReceiptID = table.Column<int>(nullable: false),
                    ReceiptDID = table.Column<int>(nullable: false),
                    KSServiceSetupId = table.Column<int>(nullable: false),
                    VehicleID = table.Column<int>(nullable: false),
                    Qty = table.Column<double>(nullable: false),
                    MaxCount = table.Column<double>(nullable: false),
                    UsedCount = table.Column<double>(nullable: false),
                    CusId = table.Column<int>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KSServices", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceSetups",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PriceListID = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    SetupCode = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    UomID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceSetups", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceSetupDetials",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServiceSetupID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    Qty = table.Column<decimal>(nullable: false),
                    GUomID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    Cost = table.Column<decimal>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false),
                    Factor = table.Column<decimal>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceSetupDetials", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ServiceSetupDetials_ServiceSetups_ServiceSetupID",
                        column: x => x.ServiceSetupID,
                        principalTable: "ServiceSetups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSetupDetials_ServiceSetupID",
                table: "ServiceSetupDetials",
                column: "ServiceSetupID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralServiceSetups");

            migrationBuilder.DropTable(
                name: "KSServiceHistories");

            migrationBuilder.DropTable(
                name: "KSServiceMaster");

            migrationBuilder.DropTable(
                name: "KSServices");

            migrationBuilder.DropTable(
                name: "ServiceSetupDetials");

            migrationBuilder.DropTable(
                name: "ServiceSetups");

            migrationBuilder.DropColumn(
                name: "IsKsms",
                schema: "dbo",
                table: "tbReceiptDetail");

            migrationBuilder.DropColumn(
                name: "IsKsmsMaster",
                schema: "dbo",
                table: "tbReceiptDetail");

            migrationBuilder.DropColumn(
                name: "KSServiceSetupId",
                schema: "dbo",
                table: "tbReceiptDetail");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                schema: "dbo",
                table: "tbReceiptDetail");

            migrationBuilder.DropColumn(
                name: "VehicleID",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "IsKsms",
                schema: "dbo",
                table: "tbOrderDetail");

            migrationBuilder.DropColumn(
                name: "IsKsmsMaster",
                schema: "dbo",
                table: "tbOrderDetail");

            migrationBuilder.DropColumn(
                name: "KSServiceSetupId",
                schema: "dbo",
                table: "tbOrderDetail");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                schema: "dbo",
                table: "tbOrderDetail");

            migrationBuilder.DropColumn(
                name: "VehicleID",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "Scale",
                schema: "dbo",
                table: "tbItemMasterData");

            migrationBuilder.DropColumn(
                name: "VehicleID",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "IsKsms",
                table: "ReceiptDetailMemoKvms");

            migrationBuilder.DropColumn(
                name: "IsKsmsMaster",
                table: "ReceiptDetailMemoKvms");

            migrationBuilder.DropColumn(
                name: "KSServiceSetupId",
                table: "ReceiptDetailMemoKvms");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "ReceiptDetailMemoKvms");
        }
    }
}
