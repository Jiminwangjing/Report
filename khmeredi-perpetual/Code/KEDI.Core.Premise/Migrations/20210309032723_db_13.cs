using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Dedit",
                schema: "dbo",
                table: "tbJournalEntryDetail",
                newName: "Debit");

            migrationBuilder.AddColumn<int>(
                name: "JournalEntryID",
                schema: "dbo",
                table: "tbSeries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodeTM",
                schema: "dbo",
                table: "tbJournalEntryDetail",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbSeries_JournalEntryID",
                schema: "dbo",
                table: "tbSeries",
                column: "JournalEntryID");

            migrationBuilder.AddForeignKey(
                name: "FK_tbSeries_tbJournalEntry_JournalEntryID",
                schema: "dbo",
                table: "tbSeries",
                column: "JournalEntryID",
                principalSchema: "dbo",
                principalTable: "tbJournalEntry",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbSeries_tbJournalEntry_JournalEntryID",
                schema: "dbo",
                table: "tbSeries");

            migrationBuilder.DropIndex(
                name: "IX_tbSeries_JournalEntryID",
                schema: "dbo",
                table: "tbSeries");

            migrationBuilder.DropColumn(
                name: "JournalEntryID",
                schema: "dbo",
                table: "tbSeries");

            migrationBuilder.DropColumn(
                name: "CodeTM",
                schema: "dbo",
                table: "tbJournalEntryDetail");

            migrationBuilder.RenameColumn(
                name: "Debit",
                schema: "dbo",
                table: "tbJournalEntryDetail",
                newName: "Dedit");
        }
    }
}
