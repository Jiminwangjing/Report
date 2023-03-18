using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _171 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OpenShiftID",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpenShiftID",
                schema: "dbo",
                table: "tbCloseShift",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpenShiftID",
                table: "VoidItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpenShiftID",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpenShiftID",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "OpenShiftID",
                schema: "dbo",
                table: "tbCloseShift");

            migrationBuilder.DropColumn(
                name: "OpenShiftID",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "OpenShiftID",
                table: "ReceiptMemo");
        }
    }
}
