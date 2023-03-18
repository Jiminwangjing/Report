using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _148 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ARReserveDID",
                table: "tbSaleDeliveryDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ARReserveInvoiceEditableDetails",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ARReserveDID",
                table: "tbSaleDeliveryDetail");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ARReserveInvoiceEditableDetails");
        }
    }
}
