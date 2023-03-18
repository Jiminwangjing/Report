using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _137 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlateNumber",
                table: "tbServiceData",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "MultiPaymentMean",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "MultiPaymentMean",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "MultiPaymentMean",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "MultiPaymentMean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlateNumber",
                table: "tbServiceData");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "MultiPaymentMean");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "MultiPaymentMean");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "MultiPaymentMean");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "MultiPaymentMean");
        }
    }
}
