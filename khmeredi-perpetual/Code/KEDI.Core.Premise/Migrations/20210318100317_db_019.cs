using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DocuNumberingID",
                schema: "dbo",
                table: "tbSeries",
                newName: "DocuTypeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DocuTypeID",
                schema: "dbo",
                table: "tbSeries",
                newName: "DocuNumberingID");
        }
    }
}
