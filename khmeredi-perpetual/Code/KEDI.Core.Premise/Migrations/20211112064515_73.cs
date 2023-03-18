using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _73 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SessionToken",
                table: "SystemLicense",
                newName: "TenantHost");

            migrationBuilder.RenameColumn(
                name: "ServerOrigin",
                table: "SystemLicense",
                newName: "ServerHost");

            migrationBuilder.RenameColumn(
                name: "ClientOrigin",
                table: "SystemLicense",
                newName: "ApiKey");

            migrationBuilder.AlterColumn<string>(
                name: "TenantID",
                table: "SystemLicense",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TenantHost",
                table: "SystemLicense",
                newName: "SessionToken");

            migrationBuilder.RenameColumn(
                name: "ServerHost",
                table: "SystemLicense",
                newName: "ServerOrigin");

            migrationBuilder.RenameColumn(
                name: "ApiKey",
                table: "SystemLicense",
                newName: "ClientOrigin");

            migrationBuilder.AlterColumn<string>(
                name: "TenantID",
                table: "SystemLicense",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
