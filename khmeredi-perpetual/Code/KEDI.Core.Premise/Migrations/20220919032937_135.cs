using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _135 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlateNumber",
                schema: "dbo",
                table: "tbWarehouseDetail",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VatNumber",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<string>(
                name: "PlateNumber",
                table: "StockOut",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlateNumber",
                schema: "dbo",
                table: "tbWarehouseDetail");

            migrationBuilder.DropColumn(
                name: "PlateNumber",
                table: "StockOut");

            migrationBuilder.AlterColumn<double>(
                name: "VatNumber",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
