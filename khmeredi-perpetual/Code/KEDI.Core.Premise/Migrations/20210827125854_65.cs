using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _65 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LineID",
                schema: "dbo",
                table: "tbVoidOrderDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentLineID",
                schema: "dbo",
                table: "tbVoidOrderDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                schema: "dbo",
                table: "tbReceiptDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentLineID",
                schema: "dbo",
                table: "tbReceiptDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PromoType",
                schema: "dbo",
                table: "tbReceiptDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                schema: "dbo",
                table: "tbOrderDetailAddon",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentLineID",
                schema: "dbo",
                table: "tbOrderDetailAddon",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BaseQty",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IsReadonly",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentLineID",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prefix",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PromoType",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "dbo",
                table: "tbItemComment",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                schema: "dbo",
                table: "tbItemComment",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "ReceiptDetailMemoKvms",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentLineID",
                table: "ReceiptDetailMemoKvms",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BuyXGetX",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    PriListID = table.Column<int>(nullable: false),
                    DateF = table.Column<DateTime>(type: "Date", nullable: false),
                    DateT = table.Column<DateTime>(type: "Date", nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyXGetX", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BuyXGetXDetail",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BuyXGetXID = table.Column<int>(nullable: false),
                    Procode = table.Column<string>(nullable: true),
                    BuyItemID = table.Column<int>(nullable: false),
                    ItemUomID = table.Column<int>(nullable: false),
                    BuyQty = table.Column<decimal>(nullable: false),
                    GetItemID = table.Column<int>(nullable: false),
                    GetUomID = table.Column<int>(nullable: false),
                    GetQty = table.Column<decimal>(nullable: false),
                    LineID = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyXGetXDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BuyXGetXDetail_BuyXGetX_BuyXGetXID",
                        column: x => x.BuyXGetXID,
                        principalTable: "BuyXGetX",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuyXGetXDetail_BuyXGetXID",
                table: "BuyXGetXDetail",
                column: "BuyXGetXID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuyXGetXDetail");

            migrationBuilder.DropTable(
                name: "BuyXGetX");

            migrationBuilder.DropColumn(
                name: "LineID",
                schema: "dbo",
                table: "tbVoidOrderDetail");

            migrationBuilder.DropColumn(
                name: "ParentLineID",
                schema: "dbo",
                table: "tbVoidOrderDetail");

            migrationBuilder.DropColumn(
                name: "LineID",
                schema: "dbo",
                table: "tbReceiptDetail");

            migrationBuilder.DropColumn(
                name: "ParentLineID",
                schema: "dbo",
                table: "tbReceiptDetail");

            migrationBuilder.DropColumn(
                name: "PromoType",
                schema: "dbo",
                table: "tbReceiptDetail");

            migrationBuilder.DropColumn(
                name: "LineID",
                schema: "dbo",
                table: "tbOrderDetailAddon");

            migrationBuilder.DropColumn(
                name: "ParentLineID",
                schema: "dbo",
                table: "tbOrderDetailAddon");

            migrationBuilder.DropColumn(
                name: "BaseQty",
                schema: "dbo",
                table: "tbOrderDetail");

            migrationBuilder.DropColumn(
                name: "IsReadonly",
                schema: "dbo",
                table: "tbOrderDetail");

            migrationBuilder.DropColumn(
                name: "LineID",
                schema: "dbo",
                table: "tbOrderDetail");

            migrationBuilder.DropColumn(
                name: "ParentLineID",
                schema: "dbo",
                table: "tbOrderDetail");

            migrationBuilder.DropColumn(
                name: "Prefix",
                schema: "dbo",
                table: "tbOrderDetail");

            migrationBuilder.DropColumn(
                name: "PromoType",
                schema: "dbo",
                table: "tbOrderDetail");

            migrationBuilder.DropColumn(
                name: "Deleted",
                schema: "dbo",
                table: "tbItemComment");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "ReceiptDetailMemoKvms");

            migrationBuilder.DropColumn(
                name: "ParentLineID",
                table: "ReceiptDetailMemoKvms");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "dbo",
                table: "tbItemComment",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
