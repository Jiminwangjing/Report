using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_015 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumberingID",
                schema: "dbo",
                table: "tbSeriesDetail",
                newName: "SeriesID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SeriesID",
                schema: "dbo",
                table: "tbSeriesDetail",
                newName: "NumberingID");
        }
    }
}
