using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _167 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Cost",
                schema: "dbo",
                table: "tbPromotionDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyID",
                schema: "dbo",
                table: "tbPromotionDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "UnitPrice",
                schema: "dbo",
                table: "tbPromotionDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "UomID",
                schema: "dbo",
                table: "tbPromotionDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriceListID",
                schema: "dbo",
                table: "tbPromotion",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cost",
                schema: "dbo",
                table: "tbPromotionDetail");

            migrationBuilder.DropColumn(
                name: "CurrencyID",
                schema: "dbo",
                table: "tbPromotionDetail");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                schema: "dbo",
                table: "tbPromotionDetail");

            migrationBuilder.DropColumn(
                name: "UomID",
                schema: "dbo",
                table: "tbPromotionDetail");

            migrationBuilder.DropColumn(
                name: "PriceListID",
                schema: "dbo",
                table: "tbPromotion");
        }
    }
}
