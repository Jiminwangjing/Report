using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _112 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EMType",
                schema: "dbo",
                table: "tbEmployee");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Activity");

            migrationBuilder.AddColumn<int>(
                name: "EMTypeID",
                schema: "dbo",
                table: "tbEmployee",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TypeID",
                table: "Activity",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EMTypeID",
                schema: "dbo",
                table: "tbEmployee");

            migrationBuilder.DropColumn(
                name: "TypeID",
                table: "Activity");

            migrationBuilder.AddColumn<string>(
                name: "EMType",
                schema: "dbo",
                table: "tbEmployee",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Activity",
                nullable: true);
        }
    }
}
