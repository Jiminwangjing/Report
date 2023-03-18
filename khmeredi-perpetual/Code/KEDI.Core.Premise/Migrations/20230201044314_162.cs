using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _162 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PostingDate",
                schema: "dbo",
                table: "tbInventoryAudit",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatorID",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmID",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EmName",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlphabetColor",
                table: "ColorSetting",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BackgroundColorName",
                table: "ColorSetting",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HoverColor",
                table: "ColorSetting",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SkinName",
                table: "ColorSetting",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "designName",
                table: "ColorSetting",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostingDate",
                schema: "dbo",
                table: "tbInventoryAudit");

            migrationBuilder.DropColumn(
                name: "CreatorID",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer");

            migrationBuilder.DropColumn(
                name: "EmID",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer");

            migrationBuilder.DropColumn(
                name: "EmName",
                schema: "dbo",
                table: "tbIncomingPaymentCustomer");

            migrationBuilder.DropColumn(
                name: "AlphabetColor",
                table: "ColorSetting");

            migrationBuilder.DropColumn(
                name: "BackgroundColorName",
                table: "ColorSetting");

            migrationBuilder.DropColumn(
                name: "HoverColor",
                table: "ColorSetting");

            migrationBuilder.DropColumn(
                name: "SkinName",
                table: "ColorSetting");

            migrationBuilder.DropColumn(
                name: "designName",
                table: "ColorSetting");
        }
    }
}
