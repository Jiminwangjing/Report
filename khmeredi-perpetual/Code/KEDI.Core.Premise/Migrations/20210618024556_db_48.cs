using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_48 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmount_Sys",
                table: "tbSaleDelivery",
                newName: "TotalAmountSys");

            migrationBuilder.RenameColumn(
                name: "SubTotal_Sys",
                table: "tbSaleDelivery",
                newName: "SubTotalSys");

            migrationBuilder.RenameColumn(
                name: "TotalAmount_Sys",
                table: "tbSaleAR",
                newName: "TotalAmountSys");

            migrationBuilder.RenameColumn(
                name: "SubTotal_Sys",
                table: "tbSaleAR",
                newName: "SubTotalSys");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmountSys",
                table: "tbSaleDelivery",
                newName: "TotalAmount_Sys");

            migrationBuilder.RenameColumn(
                name: "SubTotalSys",
                table: "tbSaleDelivery",
                newName: "SubTotal_Sys");

            migrationBuilder.RenameColumn(
                name: "TotalAmountSys",
                table: "tbSaleAR",
                newName: "TotalAmount_Sys");

            migrationBuilder.RenameColumn(
                name: "SubTotalSys",
                table: "tbSaleAR",
                newName: "SubTotal_Sys");
        }
    }
}
