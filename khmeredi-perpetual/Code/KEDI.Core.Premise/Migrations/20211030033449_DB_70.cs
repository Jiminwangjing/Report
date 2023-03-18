using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class DB_70 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpirationStockItems",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ItemId = table.Column<int>(nullable: false),
                    WarehouseId = table.Column<int>(nullable: false),
                    WareDId = table.Column<int>(nullable: false),
                    IsRead = table.Column<bool>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    ItemName = table.Column<string>(nullable: true),
                    ItemCode = table.Column<string>(nullable: true),
                    ItemBarcode = table.Column<string>(nullable: true),
                    BatchNo = table.Column<string>(nullable: true),
                    BatchAttribute1 = table.Column<string>(nullable: true),
                    BatchAttribute2 = table.Column<string>(nullable: true),
                    ExpirationDate = table.Column<DateTime>(nullable: false),
                    AddmissionDate = table.Column<DateTime>(nullable: true),
                    MfrDate = table.Column<DateTime>(nullable: true),
                    Instock = table.Column<decimal>(nullable: false),
                    UomName = table.Column<string>(nullable: true),
                    WarehouseName = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ImageItem = table.Column<string>(nullable: true),
                    CompanyID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpirationStockItems", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpirationStockItems");
        }
    }
}
