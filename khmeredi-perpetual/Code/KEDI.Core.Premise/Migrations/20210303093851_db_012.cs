using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_012 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TypeID",
                schema: "dbo",
                table: "tbJournalEntryDetail",
                newName: "ItemID");

            migrationBuilder.AddColumn<int>(
                name: "BPAcctID",
                schema: "dbo",
                table: "tbJournalEntryDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "JournalEntryID",
                schema: "dbo",
                table: "tbJournalEntryDetail",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbJournalEntryDetail_JournalEntryID",
                schema: "dbo",
                table: "tbJournalEntryDetail",
                column: "JournalEntryID");

            migrationBuilder.AddForeignKey(
                name: "FK_tbJournalEntryDetail_tbJournalEntry_JournalEntryID",
                schema: "dbo",
                table: "tbJournalEntryDetail",
                column: "JournalEntryID",
                principalSchema: "dbo",
                principalTable: "tbJournalEntry",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbJournalEntryDetail_tbJournalEntry_JournalEntryID",
                schema: "dbo",
                table: "tbJournalEntryDetail");

            migrationBuilder.DropIndex(
                name: "IX_tbJournalEntryDetail_JournalEntryID",
                schema: "dbo",
                table: "tbJournalEntryDetail");

            migrationBuilder.DropColumn(
                name: "BPAcctID",
                schema: "dbo",
                table: "tbJournalEntryDetail");

            migrationBuilder.DropColumn(
                name: "JournalEntryID",
                schema: "dbo",
                table: "tbJournalEntryDetail");

            migrationBuilder.RenameColumn(
                name: "ItemID",
                schema: "dbo",
                table: "tbJournalEntryDetail",
                newName: "TypeID");
        }
    }
}
