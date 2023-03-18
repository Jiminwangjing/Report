using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _82 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubTotalsys",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "SubTotalSys");

            migrationBuilder.RenameColumn(
                name: "AdditionalNode",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "AdditionalNote");

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                schema: "dbo",
                table: "tbPurchaseOrderDetail",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FrieghtAmount",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FrieghtAmountSys",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                schema: "dbo",
                table: "tbPurchaseAPDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                schema: "dbo",
                table: "tbGoodsReciptPODatail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "tbPurchaseCreditMemoDetail",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remark",
                schema: "dbo",
                table: "tbPurchaseOrderDetail");

            migrationBuilder.DropColumn(
                name: "FrieghtAmount",
                schema: "dbo",
                table: "tbPurchaseCreditMemo");

            migrationBuilder.DropColumn(
                name: "FrieghtAmountSys",
                schema: "dbo",
                table: "tbPurchaseCreditMemo");

            migrationBuilder.DropColumn(
                name: "Remark",
                schema: "dbo",
                table: "tbPurchaseAPDetail");

            migrationBuilder.DropColumn(
                name: "Remark",
                schema: "dbo",
                table: "tbGoodsReciptPODatail");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "tbPurchaseCreditMemoDetail");

            migrationBuilder.RenameColumn(
                name: "SubTotalSys",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "SubTotalsys");

            migrationBuilder.RenameColumn(
                name: "AdditionalNote",
                schema: "dbo",
                table: "tbPurchaseCreditMemo",
                newName: "AdditionalNode");
        }
    }
}
