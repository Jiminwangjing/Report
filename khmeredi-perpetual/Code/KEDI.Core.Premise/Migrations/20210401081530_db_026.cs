using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_026 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SeriesDID",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesID",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeriesDID",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "SeriesID",
                schema: "dbo",
                table: "tbOrder");
        }
    }
}
