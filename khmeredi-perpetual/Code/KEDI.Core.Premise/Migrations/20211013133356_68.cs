using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _68 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointCard");

            migrationBuilder.DropTable(
                name: "PointItem");

            migrationBuilder.DropTable(
                name: "PointRedemption");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                schema: "dbo",
                table: "tbMemberCard");

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

            migrationBuilder.AddColumn<string>(
                name: "PasswordStamp",
                schema: "dbo",
                table: "tbUserAccount",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordStamp",
                schema: "dbo",
                table: "tbUserAccount");

            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                schema: "dbo",
                table: "tbMemberCard",
                nullable: false,
                defaultValue: 0);

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
                    Active = table.Column<bool>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    LineID = table.Column<string>(nullable: true),
                    PointQty = table.Column<int>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    RefNo = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
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
                    Active = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    PointQty = table.Column<int>(nullable: false),
                    Redeemed = table.Column<bool>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointRedemption", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PointItem",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Deleted = table.Column<bool>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    ItemQty = table.Column<int>(nullable: false),
                    LineID = table.Column<string>(nullable: true),
                    PointRedemptID = table.Column<int>(nullable: false),
                    PointRedemptionID = table.Column<int>(nullable: true),
                    UomID = table.Column<int>(nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_PointItem_PointRedemptionID",
                table: "PointItem",
                column: "PointRedemptionID");
        }
    }
}
