using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _93 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaxGroupID",
                table: "VoidItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaxGroupID",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "TaxGroupID",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "TaxGroupID",
                table: "ReceiptMemo");
        }
    }
}
