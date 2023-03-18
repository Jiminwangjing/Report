using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _157 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CustomerTips",
                schema: "dbo",
                table: "tbGeneralSetting",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CustomerTips",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReceiptID = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    AltCurrencyID = table.Column<int>(nullable: false),
                    AltCurrency = table.Column<string>(nullable: true),
                    AltRate = table.Column<decimal>(nullable: false),
                    SCRate = table.Column<decimal>(type: "decimal(36,18)", nullable: false),
                    LCRate = table.Column<decimal>(nullable: false),
                    BaseCurrency = table.Column<string>(nullable: true),
                    BaseCurrencyID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerTips", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerTips");

            migrationBuilder.DropColumn(
                name: "CustomerTips",
                schema: "dbo",
                table: "tbGeneralSetting");
        }
    }
}
