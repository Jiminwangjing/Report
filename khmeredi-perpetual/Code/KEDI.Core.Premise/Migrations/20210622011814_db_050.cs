using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_050 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocTypeID",
                schema: "dbo",
                table: "tbTransfer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DocTypeID",
                schema: "dbo",
                table: "tbGoodsReceitp",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GLID",
                schema: "dbo",
                table: "tbGoodsReceitp",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMeansID",
                schema: "dbo",
                table: "tbGoodsReceitp",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DocTypeID",
                schema: "dbo",
                table: "tbGoodIssues",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GLID",
                schema: "dbo",
                table: "tbGoodIssues",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocTypeID",
                schema: "dbo",
                table: "tbTransfer");

            migrationBuilder.DropColumn(
                name: "DocTypeID",
                schema: "dbo",
                table: "tbGoodsReceitp");

            migrationBuilder.DropColumn(
                name: "GLID",
                schema: "dbo",
                table: "tbGoodsReceitp");

            migrationBuilder.DropColumn(
                name: "PaymentMeansID",
                schema: "dbo",
                table: "tbGoodsReceitp");

            migrationBuilder.DropColumn(
                name: "DocTypeID",
                schema: "dbo",
                table: "tbGoodIssues");

            migrationBuilder.DropColumn(
                name: "GLID",
                schema: "dbo",
                table: "tbGoodIssues");
        }
    }
}
