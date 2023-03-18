using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _83 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PromoCodeDiscRate",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PromoCodeDiscValue",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "PromoCodeID",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "PromoCodeDiscRate",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PromoCodeDiscValue",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "PromoCodeID",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PromoCodeDetail",
                columns: table => new
                {
                    PromoCodeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ID = table.Column<int>(nullable: false),
                    PromoCode = table.Column<string>(nullable: true),
                    UseCount = table.Column<int>(nullable: false),
                    MaxUse = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodeDetail", x => x.PromoCodeID);
                });

            migrationBuilder.CreateTable(
                name: "PromoCodeDiscount",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    DateF = table.Column<DateTime>(type: "Date", nullable: false),
                    TimeF = table.Column<string>(nullable: true),
                    DateT = table.Column<DateTime>(type: "Date", nullable: false),
                    TimeT = table.Column<string>(nullable: true),
                    PromoType = table.Column<int>(nullable: false),
                    PromoValue = table.Column<decimal>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    PromoCount = table.Column<int>(nullable: false),
                    UseCountCode = table.Column<int>(nullable: false),
                    StringCount = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Used = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodeDiscount", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromoCodeDetail");

            migrationBuilder.DropTable(
                name: "PromoCodeDiscount");

            migrationBuilder.DropColumn(
                name: "PromoCodeDiscRate",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "PromoCodeDiscValue",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "PromoCodeID",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "PromoCodeDiscRate",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "PromoCodeDiscValue",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "PromoCodeID",
                schema: "dbo",
                table: "tbOrder");
        }
    }
}
