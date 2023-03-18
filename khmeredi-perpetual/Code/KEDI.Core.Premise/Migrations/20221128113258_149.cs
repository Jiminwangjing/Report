using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _149 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbOpenShift",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbOpenShift",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbOpenShift",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbOpenShift",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbCloseShift",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbCloseShift",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbCloseShift",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbCloseShift",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbOpenShift");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbOpenShift");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbOpenShift");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbOpenShift");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbCloseShift");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbCloseShift");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbCloseShift");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbCloseShift");
        }
    }
}
