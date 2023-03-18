using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _159 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SlideShow",
                schema: "dbo",
                table: "tbGeneralSetting",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TimeOut",
                schema: "dbo",
                table: "tbGeneralSetting",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SlideShow",
                schema: "dbo",
                table: "tbGeneralSetting");

            migrationBuilder.DropColumn(
                name: "TimeOut",
                schema: "dbo",
                table: "tbGeneralSetting");
        }
    }
}
