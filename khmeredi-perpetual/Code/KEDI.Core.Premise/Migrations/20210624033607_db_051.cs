using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_051 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmount_Sys",
                table: "tbSaleQuote",
                newName: "TotalAmountSys");

            migrationBuilder.RenameColumn(
                name: "SubTotal_Sys",
                table: "tbSaleQuote",
                newName: "SubTotalSys");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmountSys",
                table: "tbSaleQuote",
                newName: "TotalAmount_Sys");

            migrationBuilder.RenameColumn(
                name: "SubTotalSys",
                table: "tbSaleQuote",
                newName: "SubTotal_Sys");
        }
    }
}
