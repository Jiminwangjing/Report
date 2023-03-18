using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _131 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PLDisplayRate",
                schema: "dbo",
                table: "tbDisplayCurrency",
                type: "decimal(36,18)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "DisplayRate",
                schema: "dbo",
                table: "tbDisplayCurrency",
                type: "decimal(36,18)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PLDisplayRate",
                schema: "dbo",
                table: "tbDisplayCurrency",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(36,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DisplayRate",
                schema: "dbo",
                table: "tbDisplayCurrency",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(36,18)");
        }
    }
}
