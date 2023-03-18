using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _111 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GLID",
                schema: "dbo",
                table: "tbGoodReceitpDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GLID",
                schema: "dbo",
                table: "tbGoodIssuesDetail",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GLID",
                schema: "dbo",
                table: "tbGoodReceitpDetail");

            migrationBuilder.DropColumn(
                name: "GLID",
                schema: "dbo",
                table: "tbGoodIssuesDetail");
        }
    }
}
