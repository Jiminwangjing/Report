using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _118 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FreightReceiptType",
                table: "FreightReceipt",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "FreightReceipt",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "FreightReceiptType",
                table: "Freight",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Freight",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FreightReceiptType",
                table: "FreightReceipt");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "FreightReceipt");

            migrationBuilder.DropColumn(
                name: "FreightReceiptType",
                table: "Freight");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Freight");
        }
    }
}
