using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _151 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "ReceiptDetailMemoKvms",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "ReceiptDetailMemoKvms",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "ReceiptDetailMemoKvms",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "ReceiptDetailMemoKvms",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "VoidReasons",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Reason = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoidReasons", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VoidReasons");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "ReceiptDetailMemoKvms");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "ReceiptDetailMemoKvms");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "ReceiptDetailMemoKvms");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "ReceiptDetailMemoKvms");
        }
    }
}
