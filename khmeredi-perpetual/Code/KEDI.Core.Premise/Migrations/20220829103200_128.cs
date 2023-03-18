using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _128 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tpItemCopyToPriceListDetail_tpItemCopyToPriceList_ItemCopyToPriceListID",
                schema: "dbo",
                table: "tpItemCopyToPriceListDetail");

            migrationBuilder.AlterColumn<int>(
                name: "ItemCopyToPriceListID",
                schema: "dbo",
                table: "tpItemCopyToPriceListDetail",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                schema: "dbo",
                table: "tpItemCopyToPriceListDetail",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tpItemCopyToPriceListDetail_tpItemCopyToPriceList_ItemCopyToPriceListID",
                schema: "dbo",
                table: "tpItemCopyToPriceListDetail",
                column: "ItemCopyToPriceListID",
                principalSchema: "dbo",
                principalTable: "tpItemCopyToPriceList",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tpItemCopyToPriceListDetail_tpItemCopyToPriceList_ItemCopyToPriceListID",
                schema: "dbo",
                table: "tpItemCopyToPriceListDetail");

            migrationBuilder.DropColumn(
                name: "Barcode",
                schema: "dbo",
                table: "tpItemCopyToPriceListDetail");

            migrationBuilder.AlterColumn<int>(
                name: "ItemCopyToPriceListID",
                schema: "dbo",
                table: "tpItemCopyToPriceListDetail",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_tpItemCopyToPriceListDetail_tpItemCopyToPriceList_ItemCopyToPriceListID",
                schema: "dbo",
                table: "tpItemCopyToPriceListDetail",
                column: "ItemCopyToPriceListID",
                principalSchema: "dbo",
                principalTable: "tpItemCopyToPriceList",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
