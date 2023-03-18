using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _102 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BalancePay",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BalanceReturn",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BalancePay",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BalanceReturn",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BalancePay",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "BalanceReturn",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "BalancePay",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "BalanceReturn",
                table: "ReceiptMemo");
        }
    }
}
