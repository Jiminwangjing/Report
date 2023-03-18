using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _170 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "tbGLAccount",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "tbGLAccount",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "MultiPaymentMean",
                type: "decimal(29,11)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(36,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SCRate",
                table: "MultiPaymentMean",
                type: "decimal(29,11)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(36,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OpenAmount",
                table: "MultiPaymentMean",
                type: "decimal(29,11)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(36,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LCRate",
                table: "MultiPaymentMean",
                type: "decimal(29,11)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(36,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "MultiPaymentMean",
                type: "decimal(29,11)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(36,18)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowId",
                table: "tbGLAccount");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ChangeLog",
                table: "tbGLAccount",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "MultiPaymentMean",
                type: "decimal(36,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(29,11)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SCRate",
                table: "MultiPaymentMean",
                type: "decimal(36,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(29,11)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OpenAmount",
                table: "MultiPaymentMean",
                type: "decimal(36,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(29,11)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LCRate",
                table: "MultiPaymentMean",
                type: "decimal(36,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(29,11)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "MultiPaymentMean",
                type: "decimal(36,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(29,11)");
        }
    }
}
