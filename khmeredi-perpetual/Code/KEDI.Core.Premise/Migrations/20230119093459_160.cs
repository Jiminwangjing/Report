using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _160 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "dbo",
                table: "tbTable",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RefNo",
                schema: "dbo",
                table: "tbReceipt",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                schema: "dbo",
                table: "tbReceipt",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CopyID",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefNo",
                schema: "dbo",
                table: "tbOrder",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                schema: "dbo",
                table: "tbTable");

            migrationBuilder.DropColumn(
                name: "RefNo",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "Remark",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "CopyID",
                schema: "dbo",
                table: "tbOrderDetail");

            migrationBuilder.DropColumn(
                name: "RefNo",
                schema: "dbo",
                table: "tbOrder");
        }
    }
}
