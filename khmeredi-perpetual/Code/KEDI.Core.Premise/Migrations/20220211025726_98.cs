using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _98 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemarkDiscountID",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "RemarkDiscountID",
                table: "ReceiptMemo");

            migrationBuilder.AddColumn<decimal>(
                name: "CardMemberDiscountRate",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CardMemberDiscountValue",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                schema: "dbo",
                table: "tbUserAccount",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<decimal>(
                name: "CardMemberDiscountRate",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CardMemberDiscountValue",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CardMemberDiscountRate",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CardMemberDiscountValue",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CardMemberDiscountRate",
                table: "VoidItem",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CardMemberDiscountValue",
                table: "VoidItem",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CardMemberDiscountRate",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CardMemberDiscountValue",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RemarkDiscount",
                table: "ReceiptMemo",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CardMemberDiscountRate",
                table: "PendingVoidItem",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CardMemberDiscountValue",
                table: "PendingVoidItem",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardMemberDiscountRate",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "CardMemberDiscountValue",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "CardMemberDiscountRate",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "CardMemberDiscountValue",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "CardMemberDiscountRate",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "CardMemberDiscountValue",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "CardMemberDiscountRate",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "CardMemberDiscountValue",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "CardMemberDiscountRate",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "CardMemberDiscountValue",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "RemarkDiscount",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "CardMemberDiscountRate",
                table: "PendingVoidItem");

            migrationBuilder.DropColumn(
                name: "CardMemberDiscountValue",
                table: "PendingVoidItem");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                schema: "dbo",
                table: "tbUserAccount",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemarkDiscountID",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RemarkDiscountID",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: 0);
        }
    }
}
