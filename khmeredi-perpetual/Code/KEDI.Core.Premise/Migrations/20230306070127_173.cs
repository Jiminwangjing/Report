using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _173 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerConsignmentDetails",
                columns: table => new
                {
                    CustomerConsignmentDetailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CustomerConsignmentID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    Qty = table.Column<double>(nullable: false),
                    OpenQty = table.Column<double>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    GrpUomID = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerConsignmentDetails", x => x.CustomerConsignmentDetailID);
                });

            migrationBuilder.CreateTable(
                name: "CustomerConsignments",
                columns: table => new
                {
                    CustomerConsignmentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CustomerID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    InvoiceID = table.Column<int>(nullable: false),
                    LengthExpire = table.Column<int>(nullable: false),
                    InvocieDate = table.Column<DateTime>(type: "Date", nullable: false),
                    ValidDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerConsignments", x => x.CustomerConsignmentID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerConsignmentDetails");

            migrationBuilder.DropTable(
                name: "CustomerConsignments");
        }
    }
}
