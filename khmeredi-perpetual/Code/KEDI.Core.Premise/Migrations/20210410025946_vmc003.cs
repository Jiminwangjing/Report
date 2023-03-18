using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class vmc003 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocTypeID",
                schema: "dbo",
                table: "tbOrderQAutoM",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesDID",
                schema: "dbo",
                table: "tbOrderQAutoM",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesID",
                schema: "dbo",
                table: "tbOrderQAutoM",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocTypeID",
                schema: "dbo",
                table: "tbOrderQAutoM");

            migrationBuilder.DropColumn(
                name: "SeriesDID",
                schema: "dbo",
                table: "tbOrderQAutoM");

            migrationBuilder.DropColumn(
                name: "SeriesID",
                schema: "dbo",
                table: "tbOrderQAutoM");
        }
    }
}
