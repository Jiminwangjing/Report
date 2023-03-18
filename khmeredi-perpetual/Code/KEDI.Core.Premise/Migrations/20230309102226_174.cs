using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _174 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StopDate",
                schema: "dbo",
                table: "tbPromotionDetail",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                schema: "dbo",
                table: "tbPromotionDetail",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "StopDate",
                schema: "dbo",
                table: "tbPromotion",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                schema: "dbo",
                table: "tbPromotion",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                schema: "dbo",
                table: "tbPromotion",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StopTime",
                schema: "dbo",
                table: "tbPromotion",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartTime",
                schema: "dbo",
                table: "tbPromotion");

            migrationBuilder.DropColumn(
                name: "StopTime",
                schema: "dbo",
                table: "tbPromotion");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StopDate",
                schema: "dbo",
                table: "tbPromotionDetail",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                schema: "dbo",
                table: "tbPromotionDetail",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StopDate",
                schema: "dbo",
                table: "tbPromotion",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                schema: "dbo",
                table: "tbPromotion",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
