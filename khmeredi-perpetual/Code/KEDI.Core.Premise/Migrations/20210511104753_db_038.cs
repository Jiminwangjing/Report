using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_038 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCost",
                schema: "dbo",
                table: "rp_SummaryTotalSale");

            migrationBuilder.DropColumn(
                name: "TotalProfit",
                schema: "dbo",
                table: "rp_SummaryTotalSale");

            migrationBuilder.AddColumn<int>(
                name: "MainParentId",
                table: "tbGLAccount",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MainParentId",
                table: "tbGLAccount");

            migrationBuilder.AddColumn<double>(
                name: "TotalCost",
                schema: "dbo",
                table: "rp_SummaryTotalSale",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalProfit",
                schema: "dbo",
                table: "rp_SummaryTotalSale",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
