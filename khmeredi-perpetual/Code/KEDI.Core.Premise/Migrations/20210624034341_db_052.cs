using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_052 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmount_Sys",
                schema: "dbo",
                table: "tbSaleOrder",
                newName: "TotalAmountSys");

            migrationBuilder.RenameColumn(
                name: "SubTotal_Sys",
                schema: "dbo",
                table: "tbSaleOrder",
                newName: "SubTotalSys");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmountSys",
                schema: "dbo",
                table: "tbSaleOrder",
                newName: "TotalAmount_Sys");

            migrationBuilder.RenameColumn(
                name: "SubTotalSys",
                schema: "dbo",
                table: "tbSaleOrder",
                newName: "SubTotal_Sys");
        }
    }
}
