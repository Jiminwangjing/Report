using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _154 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbWarhouse",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbVoidOrderDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbVoidOrderDetail",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbVoidOrderDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbVoidOrderDetail",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbVoidOrderDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbUserPrivillege",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbUserAccount",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbUnitofMeasure",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbTable",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbSeriesDetail",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbSeries",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbReceiptInformation",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbReceiptDetail",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbPropertyDetails",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbProperty",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbPromotion",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbPrinterName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbPriceListDetail",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbPriceList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbPeriodIndicator",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbPaymentMeans",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbOpenShift",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbMemberCard",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbItemMasterData",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbItemComment",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbGroupUoM",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbGroupTable",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbGroupDefindUoM",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbExchangeRate",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbEmployee",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbDocumentType",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbCurrency",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbCompany",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbCloseShift",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbChildProperty",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "tbBranch",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "TaxGroup",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "TaxDefinition",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "ItemGroup3",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "ItemGroup2",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "ItemGroup1",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "Funtion",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                schema: "dbo",
                table: "CardType",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "VoidItemDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "VoidItemDetail",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "VoidItemDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "VoidItemDetail",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "VoidItemDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "VoidItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "VoidItem",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "VoidItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "VoidItem",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "VoidItem",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "SaleCombo",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "RemarkDiscounts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "RemarkDiscounts",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "RemarkDiscounts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "RemarkDiscounts",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "RemarkDiscounts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "RefreshToken",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "RefreshToken",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "RefreshToken",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "RefreshToken",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "RefreshToken",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "RedeemRetail",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "Redeem",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "ReceiptDetailMemoKvms",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "PromoCodeDiscount",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "PromoCodeDetail",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "PointRedemptionHistory",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "PointRedemption",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "PointItemHistory",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "PointItem",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "PointCard",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "PBuyXGetXDis",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "MultiPaymentMean",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "Freight",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "ExchangeCanRingDetails",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "ComboDetails",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "CanRingMaster",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "CanRingDetail",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "CanRing",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "BuyXQtyGetXDis",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "BuyXGetXDetail",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "BuyXGetX",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "AuthorizationTemplate",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "SelfSyncEntity",
                columns: table => new
                {
                    RowId = table.Column<Guid>(nullable: false),
                    Spk = table.Column<int>(nullable: false),
                    Cpk = table.Column<int>(nullable: false),
                    Synced = table.Column<bool>(nullable: false),
                    SyncDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelfSyncEntity", x => x.RowId);
                });

            migrationBuilder.CreateTable(
                name: "TenantTransaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<string>(nullable: true),
                    TransactId = table.Column<string>(nullable: true),
                    TransactType = table.Column<string>(nullable: true),
                    RowId = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Synced = table.Column<bool>(nullable: false),
                    SyncDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantTransaction", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SelfSyncEntity");

            migrationBuilder.DropTable(
                name: "TenantTransaction");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbWarhouse");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbVoidOrderDetail");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbVoidOrderDetail");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbVoidOrderDetail");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbVoidOrderDetail");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbVoidOrderDetail");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbUserPrivillege");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbUserAccount");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbUnitofMeasure");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbTable");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbSeriesDetail");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbSeries");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbReceiptInformation");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbReceiptDetail");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbPropertyDetails");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbProperty");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbPromotion");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbPrinterName");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbPriceListDetail");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbPriceList");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbPeriodIndicator");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbPaymentMeans");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbOpenShift");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbMemberCard");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbItemMasterData");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbItemComment");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbGroupUoM");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbGroupTable");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbGroupDefindUoM");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbExchangeRate");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbEmployee");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbDocumentType");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbCurrency");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbCompany");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbCloseShift");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbChildProperty");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "tbBranch");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "TaxGroup");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "TaxDefinition");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "ItemGroup3");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "ItemGroup2");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "ItemGroup1");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "Funtion");

            migrationBuilder.DropColumn(
                name: "RowId",
                schema: "dbo",
                table: "CardType");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "VoidItemDetail");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "VoidItemDetail");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "VoidItemDetail");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "VoidItemDetail");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "VoidItemDetail");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "SaleCombo");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "RemarkDiscounts");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "RemarkDiscounts");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "RemarkDiscounts");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "RemarkDiscounts");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "RemarkDiscounts");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "RedeemRetail");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "Redeem");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "ReceiptDetailMemoKvms");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "PromoCodeDiscount");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "PromoCodeDetail");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "PointRedemptionHistory");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "PointRedemption");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "PointItemHistory");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "PointItem");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "PointCard");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "PBuyXGetXDis");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "MultiPaymentMean");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "Freight");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "ExchangeCanRingDetails");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "ComboDetails");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "CanRingMaster");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "CanRingDetail");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "CanRing");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "BuyXQtyGetXDis");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "BuyXGetXDetail");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "BuyXGetX");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "AuthorizationTemplate");
        }
    }
}
