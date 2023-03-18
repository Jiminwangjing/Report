using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _172 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchID",
                schema: "dbo",
                table: "tbJournalEntry",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Enable",
                schema: "dbo",
                table: "Funtion",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "MultiBrands",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BranchID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultiBrands", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MultiBrands");

            migrationBuilder.DropColumn(
                name: "BranchID",
                schema: "dbo",
                table: "tbJournalEntry");

            migrationBuilder.DropColumn(
                name: "Enable",
                schema: "dbo",
                table: "Funtion");
        }
    }
}
