using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_039 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                schema: "dbo",
                table: "tbTransfer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriseDID",
                schema: "dbo",
                table: "tbTransfer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriseID",
                schema: "dbo",
                table: "tbTransfer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                schema: "dbo",
                table: "tbGoodsReceitp",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriseDID",
                schema: "dbo",
                table: "tbGoodsReceitp",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriseID",
                schema: "dbo",
                table: "tbGoodsReceitp",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                schema: "dbo",
                table: "tbGoodIssues",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriseDID",
                schema: "dbo",
                table: "tbGoodIssues",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriseID",
                schema: "dbo",
                table: "tbGoodIssues",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyID",
                schema: "dbo",
                table: "tbTransfer");

            migrationBuilder.DropColumn(
                name: "SeriseDID",
                schema: "dbo",
                table: "tbTransfer");

            migrationBuilder.DropColumn(
                name: "SeriseID",
                schema: "dbo",
                table: "tbTransfer");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                schema: "dbo",
                table: "tbGoodsReceitp");

            migrationBuilder.DropColumn(
                name: "SeriseDID",
                schema: "dbo",
                table: "tbGoodsReceitp");

            migrationBuilder.DropColumn(
                name: "SeriseID",
                schema: "dbo",
                table: "tbGoodsReceitp");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                schema: "dbo",
                table: "tbGoodIssues");

            migrationBuilder.DropColumn(
                name: "SeriseDID",
                schema: "dbo",
                table: "tbGoodIssues");

            migrationBuilder.DropColumn(
                name: "SeriseID",
                schema: "dbo",
                table: "tbGoodIssues");
        }
    }
}
