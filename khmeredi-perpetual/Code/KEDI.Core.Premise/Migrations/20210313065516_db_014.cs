using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_014 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeTM",
                schema: "dbo",
                table: "tbJournalEntryDetail");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodeTM",
                schema: "dbo",
                table: "tbJournalEntryDetail",
                nullable: true);
        }
    }
}
