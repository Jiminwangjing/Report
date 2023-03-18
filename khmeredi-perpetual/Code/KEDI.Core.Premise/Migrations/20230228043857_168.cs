using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _168 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "MultiPaymentMean",
                type: "decimal(36,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(29,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SCRate",
                table: "MultiPaymentMean",
                type: "decimal(36,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(29,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OpenAmount",
                table: "MultiPaymentMean",
                type: "decimal(36,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(29,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LCRate",
                table: "MultiPaymentMean",
                type: "decimal(36,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(29,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "MultiPaymentMean",
                type: "decimal(36,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(29,18)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "MultiPaymentMean",
                type: "decimal(29,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(36,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SCRate",
                table: "MultiPaymentMean",
                type: "decimal(29,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(36,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OpenAmount",
                table: "MultiPaymentMean",
                type: "decimal(29,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(36,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LCRate",
                table: "MultiPaymentMean",
                type: "decimal(29,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(36,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "MultiPaymentMean",
                type: "decimal(29,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(36,18)");
        }
    }
}
