using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_028 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountID",
                schema: "dbo",
                table: "tbPaymentMeans",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                schema: "dbo",
                table: "tbPaymentMeans",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Default",
                schema: "dbo",
                table: "tbPaymentMeans",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                schema: "dbo",
                table: "tbJournalEntry",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountID",
                schema: "dbo",
                table: "tbPaymentMeans");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                schema: "dbo",
                table: "tbPaymentMeans");

            migrationBuilder.DropColumn(
                name: "Default",
                schema: "dbo",
                table: "tbPaymentMeans");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                schema: "dbo",
                table: "tbJournalEntry");
        }
    }
}
