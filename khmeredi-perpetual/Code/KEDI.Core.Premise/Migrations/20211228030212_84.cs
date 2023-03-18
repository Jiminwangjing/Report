using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _84 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ComboSaleType",
                schema: "dbo",
                table: "tbVoidOrderDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsReadonly",
                schema: "dbo",
                table: "tbVoidOrderDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ComboSaleType",
                schema: "dbo",
                table: "tbReceiptDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsReadonly",
                schema: "dbo",
                table: "tbReceiptDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ComboSaleType",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ComboSaleType",
                table: "VoidItemDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsReadonly",
                table: "VoidItemDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ComboSaleType",
                table: "ReceiptDetailMemoKvms",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsReadonly",
                table: "ReceiptDetailMemoKvms",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "SaleCombo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Barcode = table.Column<string>(nullable: true),
                    ItemID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    PriListID = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleCombo", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ComboDetails",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SaleComboID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    Qty = table.Column<double>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    GUomID = table.Column<int>(nullable: false),
                    Detele = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ComboDetails_SaleCombo_SaleComboID",
                        column: x => x.SaleComboID,
                        principalTable: "SaleCombo",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComboDetails_SaleComboID",
                table: "ComboDetails",
                column: "SaleComboID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComboDetails");

            migrationBuilder.DropTable(
                name: "SaleCombo");

            migrationBuilder.DropColumn(
                name: "ComboSaleType",
                schema: "dbo",
                table: "tbVoidOrderDetail");

            migrationBuilder.DropColumn(
                name: "IsReadonly",
                schema: "dbo",
                table: "tbVoidOrderDetail");

            migrationBuilder.DropColumn(
                name: "ComboSaleType",
                schema: "dbo",
                table: "tbReceiptDetail");

            migrationBuilder.DropColumn(
                name: "IsReadonly",
                schema: "dbo",
                table: "tbReceiptDetail");

            migrationBuilder.DropColumn(
                name: "ComboSaleType",
                schema: "dbo",
                table: "tbOrderDetail");

            migrationBuilder.DropColumn(
                name: "ComboSaleType",
                table: "VoidItemDetail");

            migrationBuilder.DropColumn(
                name: "IsReadonly",
                table: "VoidItemDetail");

            migrationBuilder.DropColumn(
                name: "ComboSaleType",
                table: "ReceiptDetailMemoKvms");

            migrationBuilder.DropColumn(
                name: "IsReadonly",
                table: "ReceiptDetailMemoKvms");
        }
    }
}
