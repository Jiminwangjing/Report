using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _158 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LineID",
                schema: "dbo",
                table: "tbInventoryAudit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "ARReserveEditableDetailHistories",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaleCopyType",
                table: "ARReserveEditableDetailHistories",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LineID",
                schema: "dbo",
                table: "tbInventoryAudit");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "ARReserveEditableDetailHistories");

            migrationBuilder.DropColumn(
                name: "SaleCopyType",
                table: "ARReserveEditableDetailHistories");
        }
    }
}
