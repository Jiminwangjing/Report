using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_0062 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountFreight",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Freight",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    RevenAcctID = table.Column<int>(nullable: false),
                    ExpenAcctID = table.Column<int>(nullable: false),
                    OutTaxID = table.Column<int>(nullable: false),
                    InTaxID = table.Column<int>(nullable: false),
                    AmountReven = table.Column<decimal>(nullable: false),
                    AmountExpen = table.Column<decimal>(nullable: false),
                    Default = table.Column<bool>(nullable: false),
                    OutTaxRate = table.Column<decimal>(nullable: false),
                    InTaxRate = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Freight", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FreightReceipt",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FreightID = table.Column<int>(nullable: false),
                    ReceiptID = table.Column<int>(nullable: false),
                    AmountReven = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreightReceipt", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TaxGroup",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GLID = table.Column<int>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxGroup", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TaxDefinition",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TaxGroupID = table.Column<int>(nullable: false),
                    EffectiveFrom = table.Column<DateTime>(nullable: false),
                    Rate = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxDefinition", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TaxDefinition_TaxGroup_TaxGroupID",
                        column: x => x.TaxGroupID,
                        principalSchema: "dbo",
                        principalTable: "TaxGroup",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaxDefinition_TaxGroupID",
                schema: "dbo",
                table: "TaxDefinition",
                column: "TaxGroupID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Freight");

            migrationBuilder.DropTable(
                name: "FreightReceipt");

            migrationBuilder.DropTable(
                name: "TaxDefinition",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TaxGroup",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "AmountFreight",
                schema: "dbo",
                table: "tbReceipt");
        }
    }
}
