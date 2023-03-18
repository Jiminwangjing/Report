using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _75 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbPointCard",
                schema: "dbo");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "BirthDate",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<decimal>(
                name: "CumulativePoint",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "GroupID",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "OutstandPoint",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RedeemedPoint",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "PointCard",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LineID = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    PointQty = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    RefNo = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointCard", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PointRedemption",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    PointQty = table.Column<int>(nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Redeemed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointRedemption", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Redeem",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CustomerID = table.Column<int>(nullable: false),
                    SeriesID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    DateIn = table.Column<DateTime>(type: "Date", nullable: false),
                    DateOut = table.Column<DateTime>(type: "Date", nullable: false),
                    Number = table.Column<string>(nullable: true),
                    RedeemPoint = table.Column<decimal>(nullable: false),
                    OutStanding = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Redeem", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PointItem",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PointRedemptID = table.Column<int>(nullable: false),
                    LineID = table.Column<string>(nullable: true),
                    ItemID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    ItemQty = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    PointRedemptionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointItem", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PointItem_PointRedemption_PointRedemptionID",
                        column: x => x.PointRedemptionID,
                        principalTable: "PointRedemption",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RedeemRetail",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    ItemName = table.Column<string>(nullable: true),
                    Uom = table.Column<string>(nullable: true),
                    UomID = table.Column<int>(nullable: false),
                    Qty = table.Column<decimal>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    RedeemID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedeemRetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RedeemRetail_Redeem_RedeemID",
                        column: x => x.RedeemID,
                        principalTable: "Redeem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PointItem_PointRedemptionID",
                table: "PointItem",
                column: "PointRedemptionID");

            migrationBuilder.CreateIndex(
                name: "IX_RedeemRetail_RedeemID",
                table: "RedeemRetail",
                column: "RedeemID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointCard");

            migrationBuilder.DropTable(
                name: "PointItem");

            migrationBuilder.DropTable(
                name: "RedeemRetail");

            migrationBuilder.DropTable(
                name: "PointRedemption");

            migrationBuilder.DropTable(
                name: "Redeem");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "CumulativePoint",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "GroupID",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "OutstandPoint",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "RedeemedPoint",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.CreateTable(
                name: "tbPointCard",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Approve = table.Column<string>(nullable: true),
                    DataApprove = table.Column<DateTime>(nullable: false),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Point = table.Column<int>(nullable: false),
                    PointID = table.Column<int>(nullable: false),
                    Ref_No = table.Column<string>(type: "nvarchar(25)", nullable: true),
                    Remain = table.Column<double>(nullable: false)
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbPointCard_PointID",
                schema: "dbo",
                table: "tbPointCard",
                column: "PointID");
        }
    }
}
