using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _146 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ExpireDate",
                table: "TmpSerialNumberSelectedDetail",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<string>(
                name: "AdmissionDate",
                table: "TmpSerialNumberSelectedDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "TmpSerialNumberSelectedDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "TmpSerialNumberSelectedDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MfrDate",
                table: "TmpSerialNumberSelectedDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MfrWarDateEnd",
                table: "TmpSerialNumberSelectedDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MfrWarDateStart",
                table: "TmpSerialNumberSelectedDetail",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DeliveryQty",
                table: "ARReserveInvoiceEditableDetails",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdmissionDate",
                table: "TmpSerialNumberSelectedDetail");

            migrationBuilder.DropColumn(
                name: "Details",
                table: "TmpSerialNumberSelectedDetail");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "TmpSerialNumberSelectedDetail");

            migrationBuilder.DropColumn(
                name: "MfrDate",
                table: "TmpSerialNumberSelectedDetail");

            migrationBuilder.DropColumn(
                name: "MfrWarDateEnd",
                table: "TmpSerialNumberSelectedDetail");

            migrationBuilder.DropColumn(
                name: "MfrWarDateStart",
                table: "TmpSerialNumberSelectedDetail");

            migrationBuilder.DropColumn(
                name: "DeliveryQty",
                table: "ARReserveInvoiceEditableDetails");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpireDate",
                table: "TmpSerialNumberSelectedDetail",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
