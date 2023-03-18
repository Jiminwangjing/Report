using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_010 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemPageSize",
                schema: "dbo",
                table: "tbGeneralSetting",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ItemViewType",
                schema: "dbo",
                table: "tbGeneralSetting",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemPageSize",
                schema: "dbo",
                table: "tbGeneralSetting");

            migrationBuilder.DropColumn(
                name: "ItemViewType",
                schema: "dbo",
                table: "tbGeneralSetting");
        }
    }
}
