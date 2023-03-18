using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _144 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AltCurrencyID",
                table: "MultipayMeansSetting",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ARReDetEDTID",
                table: "DraftReserveInvoiceDetails",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AltCurrencyID",
                table: "MultipayMeansSetting");

            migrationBuilder.DropColumn(
                name: "ARReDetEDTID",
                table: "DraftReserveInvoiceDetails");
        }
    }
}
