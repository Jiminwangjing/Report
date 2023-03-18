using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _79 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbVoidOrderDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxValue",
                schema: "dbo",
                table: "tbVoidOrderDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TaxOption",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbReceiptDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxValue",
                schema: "dbo",
                table: "tbReceiptDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TaxOption",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxValue",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TaxOption",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaxOption",
                schema: "dbo",
                table: "tbGeneralSetting",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaxGroupID",
                table: "VoidItemDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxValue",
                table: "VoidItemDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TaxOption",
                table: "VoidItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaxOption",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaxGroupID",
                table: "ReceiptDetailMemoKvms",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxValue",
                table: "ReceiptDetailMemoKvms",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbVoidOrderDetail");

            migrationBuilder.DropColumn(
                name: "TaxValue",
                schema: "dbo",
                table: "tbVoidOrderDetail");

            migrationBuilder.DropColumn(
                name: "TaxOption",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbReceiptDetail");

            migrationBuilder.DropColumn(
                name: "TaxValue",
                schema: "dbo",
                table: "tbReceiptDetail");

            migrationBuilder.DropColumn(
                name: "TaxOption",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "TaxGroupID",
                schema: "dbo",
                table: "tbOrderDetail");

            migrationBuilder.DropColumn(
                name: "TaxValue",
                schema: "dbo",
                table: "tbOrderDetail");

            migrationBuilder.DropColumn(
                name: "TaxOption",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "TaxOption",
                schema: "dbo",
                table: "tbGeneralSetting");

            migrationBuilder.DropColumn(
                name: "TaxGroupID",
                table: "VoidItemDetail");

            migrationBuilder.DropColumn(
                name: "TaxValue",
                table: "VoidItemDetail");

            migrationBuilder.DropColumn(
                name: "TaxOption",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "TaxOption",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "TaxGroupID",
                table: "ReceiptDetailMemoKvms");

            migrationBuilder.DropColumn(
                name: "TaxValue",
                table: "ReceiptDetailMemoKvms");
        }
    }
}
