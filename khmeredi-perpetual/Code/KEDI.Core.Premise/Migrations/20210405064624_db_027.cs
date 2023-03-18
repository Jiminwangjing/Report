using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_027 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                schema: "dbo",
                table: "tbInventoryAudit",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DocumentTypeID",
                schema: "dbo",
                table: "tbInventoryAudit",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesDetailID",
                schema: "dbo",
                table: "tbInventoryAudit",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesID",
                schema: "dbo",
                table: "tbInventoryAudit",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyID",
                schema: "dbo",
                table: "tbInventoryAudit");

            migrationBuilder.DropColumn(
                name: "DocumentTypeID",
                schema: "dbo",
                table: "tbInventoryAudit");

            migrationBuilder.DropColumn(
                name: "SeriesDetailID",
                schema: "dbo",
                table: "tbInventoryAudit");

            migrationBuilder.DropColumn(
                name: "SeriesID",
                schema: "dbo",
                table: "tbInventoryAudit");
        }
    }
}
