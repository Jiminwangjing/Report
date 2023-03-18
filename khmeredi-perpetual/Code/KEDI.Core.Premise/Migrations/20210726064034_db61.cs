using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db61 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUserOrder",
                schema: "dbo",
                table: "tbUserAccount",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOrderByQR",
                schema: "dbo",
                table: "tbGeneralSetting",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUserOrder",
                schema: "dbo",
                table: "tbUserAccount");

            migrationBuilder.DropColumn(
                name: "IsOrderByQR",
                schema: "dbo",
                table: "tbGeneralSetting");
        }
    }
}
