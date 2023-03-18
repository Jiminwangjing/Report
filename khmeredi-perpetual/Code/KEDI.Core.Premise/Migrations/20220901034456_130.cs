using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _130 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PLDisplayRate",
                schema: "dbo",
                table: "tbDisplayCurrency",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "DisplayRate",
                schema: "dbo",
                table: "tbDisplayCurrency",
                nullable: false,
                oldClrType: typeof(double));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "PLDisplayRate",
                schema: "dbo",
                table: "tbDisplayCurrency",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "DisplayRate",
                schema: "dbo",
                table: "tbDisplayCurrency",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
