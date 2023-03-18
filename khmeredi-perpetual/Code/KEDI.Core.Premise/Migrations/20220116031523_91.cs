using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _91 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "CreatorID",
                table: "SaleCombo",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorID",
                table: "SaleCombo");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
