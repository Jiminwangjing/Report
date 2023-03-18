using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _113 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmpName",
                table: "Activity");

            migrationBuilder.DropColumn(
                name: "Employee",
                table: "Activity");

            migrationBuilder.DropColumn(
                name: "SubName",
                table: "Activity");

            migrationBuilder.AddColumn<int>(
                name: "EmpNameID",
                table: "Activity",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubNameID",
                table: "Activity",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmpNameID",
                table: "Activity");

            migrationBuilder.DropColumn(
                name: "SubNameID",
                table: "Activity");

            migrationBuilder.AddColumn<string>(
                name: "EmpName",
                table: "Activity",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Employee",
                table: "Activity",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubName",
                table: "Activity",
                nullable: true);
        }
    }
}
