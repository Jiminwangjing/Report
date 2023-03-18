using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_007 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentCode",
                table: "tbGLAccount");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "tbGLAccount",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "tbGLAccount",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "tbGLAccount");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "tbGLAccount");

            migrationBuilder.AddColumn<string>(
                name: "ParentCode",
                table: "tbGLAccount",
                nullable: true);
        }
    }
}
