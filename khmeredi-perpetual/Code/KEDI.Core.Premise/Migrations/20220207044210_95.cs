using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _95 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemQty",
                table: "PointItem");

            migrationBuilder.AlterColumn<double>(
                name: "PointQty",
                table: "PointRedemption",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "PointRedemption",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WarehouseID",
                table: "PointRedemption",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Qty",
                table: "PointItem",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "PointRedemptionMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PostingDate = table.Column<DateTime>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    UserID = table.Column<int>(nullable: false),
                    CustomerID = table.Column<int>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    PLCurrencyID = table.Column<int>(nullable: false),
                    PLRate = table.Column<double>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false),
                    SeriesID = table.Column<int>(nullable: false),
                    SeriesDID = table.Column<int>(nullable: false),
                    DocTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointRedemptionMaster", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PointRedemptionHistory",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    PointQty = table.Column<double>(nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Redeemed = table.Column<bool>(nullable: false),
                    CustomerID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    PointRedemptionMasterID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointRedemptionHistory", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PointRedemptionHistory_PointRedemptionMaster_PointRedemptionMasterID",
                        column: x => x.PointRedemptionMasterID,
                        principalTable: "PointRedemptionMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PointItemHistory",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LineID = table.Column<string>(nullable: true),
                    ItemID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    Qty = table.Column<double>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    PointRedemptionHistoryID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointItemHistory", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PointItemHistory_PointRedemptionHistory_PointRedemptionHistoryID",
                        column: x => x.PointRedemptionHistoryID,
                        principalTable: "PointRedemptionHistory",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PointItemHistory_PointRedemptionHistoryID",
                table: "PointItemHistory",
                column: "PointRedemptionHistoryID");

            migrationBuilder.CreateIndex(
                name: "IX_PointRedemptionHistory_PointRedemptionMasterID",
                table: "PointRedemptionHistory",
                column: "PointRedemptionMasterID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointItemHistory");

            migrationBuilder.DropTable(
                name: "PointRedemptionHistory");

            migrationBuilder.DropTable(
                name: "PointRedemptionMaster");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "PointRedemption");

            migrationBuilder.DropColumn(
                name: "WarehouseID",
                table: "PointRedemption");

            migrationBuilder.DropColumn(
                name: "Qty",
                table: "PointItem");

            migrationBuilder.AlterColumn<int>(
                name: "PointQty",
                table: "PointRedemption",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<int>(
                name: "ItemQty",
                table: "PointItem",
                nullable: false,
                defaultValue: 0);
        }
    }
}
