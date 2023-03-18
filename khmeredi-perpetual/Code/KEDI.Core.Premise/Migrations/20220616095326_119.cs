using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _119 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReceivedBy",
                table: "DraftAR",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequestedBy",
                table: "DraftAR",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShippedBy",
                table: "DraftAR",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceivedBy",
                table: "DraftAR");

            migrationBuilder.DropColumn(
                name: "RequestedBy",
                table: "DraftAR");

            migrationBuilder.DropColumn(
                name: "ShippedBy",
                table: "DraftAR");
        }
    }
}
