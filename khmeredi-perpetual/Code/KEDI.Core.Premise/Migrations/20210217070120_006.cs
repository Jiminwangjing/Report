using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _006 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbOrderDetail_Addon_tbUnitofMeasure_UomID",
                schema: "dbo",
                table: "tbOrderDetail_Addon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbOrderDetail_Addon",
                schema: "dbo",
                table: "tbOrderDetail_Addon");

            migrationBuilder.RenameTable(
                name: "tbOrderDetail_Addon",
                schema: "dbo",
                newName: "tbOrderDetailAddon",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_tbOrderDetail_Addon_UomID",
                schema: "dbo",
                table: "tbOrderDetailAddon",
                newName: "IX_tbOrderDetailAddon_UomID");

            migrationBuilder.AlterColumn<string>(
                name: "Line_ID",
                schema: "dbo",
                table: "tbOrderDetailAddon",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbOrderDetailAddon",
                schema: "dbo",
                table: "tbOrderDetailAddon",
                column: "AddOnID");

            migrationBuilder.AddForeignKey(
                name: "FK_tbOrderDetailAddon_tbUnitofMeasure_UomID",
                schema: "dbo",
                table: "tbOrderDetailAddon",
                column: "UomID",
                principalSchema: "dbo",
                principalTable: "tbUnitofMeasure",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbOrderDetailAddon_tbUnitofMeasure_UomID",
                schema: "dbo",
                table: "tbOrderDetailAddon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbOrderDetailAddon",
                schema: "dbo",
                table: "tbOrderDetailAddon");

            migrationBuilder.RenameTable(
                name: "tbOrderDetailAddon",
                schema: "dbo",
                newName: "tbOrderDetail_Addon",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_tbOrderDetailAddon_UomID",
                schema: "dbo",
                table: "tbOrderDetail_Addon",
                newName: "IX_tbOrderDetail_Addon_UomID");

            migrationBuilder.AlterColumn<int>(
                name: "Line_ID",
                schema: "dbo",
                table: "tbOrderDetail_Addon",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbOrderDetail_Addon",
                schema: "dbo",
                table: "tbOrderDetail_Addon",
                column: "AddOnID");

            migrationBuilder.AddForeignKey(
                name: "FK_tbOrderDetail_Addon_tbUnitofMeasure_UomID",
                schema: "dbo",
                table: "tbOrderDetail_Addon",
                column: "UomID",
                principalSchema: "dbo",
                principalTable: "tbUnitofMeasure",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
