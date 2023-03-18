using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _138 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShipTo",
                table: "tbSaleAR",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipTo",
                table: "SaleAREdites",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipTo",
                table: "SaleAREditeHistory",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipTo",
                table: "DraftAR",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShipTo",
                table: "tbSaleAR");

            migrationBuilder.DropColumn(
                name: "ShipTo",
                table: "SaleAREdites");

            migrationBuilder.DropColumn(
                name: "ShipTo",
                table: "SaleAREditeHistory");

            migrationBuilder.DropColumn(
                name: "ShipTo",
                table: "DraftAR");
        }
    }
}
