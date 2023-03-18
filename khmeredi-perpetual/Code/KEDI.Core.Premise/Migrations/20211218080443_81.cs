using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _81 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsKsms",
                schema: "dbo",
                table: "tbVoidOrderDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsKsmsMaster",
                schema: "dbo",
                table: "tbVoidOrderDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsScale",
                schema: "dbo",
                table: "tbVoidOrderDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "KSServiceSetupId",
                schema: "dbo",
                table: "tbVoidOrderDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VehicleId",
                schema: "dbo",
                table: "tbVoidOrderDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsScale",
                schema: "dbo",
                table: "tbReceiptDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsScale",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsScale",
                schema: "dbo",
                table: "ServiceItemSales",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsKsms",
                table: "VoidItemDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsKsmsMaster",
                table: "VoidItemDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsScale",
                table: "VoidItemDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVoided",
                table: "VoidItemDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "KSServiceSetupId",
                table: "VoidItemDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VehicleId",
                table: "VoidItemDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsScale",
                table: "ReceiptDetailMemoKvms",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsKsms",
                schema: "dbo",
                table: "tbVoidOrderDetail");

            migrationBuilder.DropColumn(
                name: "IsKsmsMaster",
                schema: "dbo",
                table: "tbVoidOrderDetail");

            migrationBuilder.DropColumn(
                name: "IsScale",
                schema: "dbo",
                table: "tbVoidOrderDetail");

            migrationBuilder.DropColumn(
                name: "KSServiceSetupId",
                schema: "dbo",
                table: "tbVoidOrderDetail");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                schema: "dbo",
                table: "tbVoidOrderDetail");

            migrationBuilder.DropColumn(
                name: "IsScale",
                schema: "dbo",
                table: "tbReceiptDetail");

            migrationBuilder.DropColumn(
                name: "IsScale",
                schema: "dbo",
                table: "tbOrderDetail");

            migrationBuilder.DropColumn(
                name: "IsScale",
                schema: "dbo",
                table: "ServiceItemSales");

            migrationBuilder.DropColumn(
                name: "IsKsms",
                table: "VoidItemDetail");

            migrationBuilder.DropColumn(
                name: "IsKsmsMaster",
                table: "VoidItemDetail");

            migrationBuilder.DropColumn(
                name: "IsScale",
                table: "VoidItemDetail");

            migrationBuilder.DropColumn(
                name: "IsVoided",
                table: "VoidItemDetail");

            migrationBuilder.DropColumn(
                name: "KSServiceSetupId",
                table: "VoidItemDetail");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "VoidItemDetail");

            migrationBuilder.DropColumn(
                name: "IsScale",
                table: "ReceiptDetailMemoKvms");
        }
    }
}
