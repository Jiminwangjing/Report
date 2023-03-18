using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _166 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "SelfSyncEntity");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "SelfSyncEntity");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "SelfSyncEntity");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "ClientApi",
                newName: "UserId");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbWarhouse",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbVoidOrderDetail",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbUserPrivillege",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbUserAccount",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbUnitofMeasure",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbTable",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbSeriesDetail",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbSeries",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbReceiptInformation",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbReceiptDetail",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbPropertyDetails",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbProperty",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbPromotionDetail",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbPromotion",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbPrinterName",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbPriceListDetail",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbPriceList",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbPeriodIndicator",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbPaymentMeans",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                schema: "dbo",
                table: "tbPaymentMeans",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbOpenShift",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbMemberCard",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbItemMasterData",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbItemComment",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbGroupUoM",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbGroupTable",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbGroupDefindUoM",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbExchangeRate",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbEmployee",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbDocumentType",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbCurrency",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbCompany",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbCloseShift",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbChildProperty",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbBranch",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "TaxGroup",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "TaxDefinition",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "ItemGroup3",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "ItemGroup2",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "ItemGroup1",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "Funtion",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                schema: "dbo",
                table: "CardType",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "VoidItemDetail",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "VoidItem",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "TransactionChipMong",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "TransactionAeon",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "SelfSyncEntity",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "SelfSyncEntity",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TxId",
                table: "SelfSyncEntity",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "SaleCombo",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "RemarkDiscounts",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "RedeemRetail",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "Redeem",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "ReceiptDetailMemoKvms",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "PromoCodeDiscount",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "PromoCodeDetail",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "PointRedemptionHistory",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "PointRedemption",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "PointItemHistory",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "PointItem",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "PointCard",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "PBuyXGetXDis",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "MultiPaymentMean",
                type: "decimal(29,11)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(36,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SCRate",
                table: "MultiPaymentMean",
                type: "decimal(29,11)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(36,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OpenAmount",
                table: "MultiPaymentMean",
                type: "decimal(29,11)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(36,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LCRate",
                table: "MultiPaymentMean",
                type: "decimal(29,11)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(36,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "MultiPaymentMean",
                type: "decimal(29,11)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(36,18)");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "MultiPaymentMean",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "FreightReceipt",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "RowId",
                table: "FreightReceipt",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "Freight",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "ExchangeCanRingDetails",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "ComboDetails",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "CanRingMaster",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "CanRingDetail",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "CanRing",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "BuyXQtyGetXDis",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "BuyXGetXDetail",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "BuyXGetX",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ChangeLog",
                table: "AuthorizationTemplate",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateTable(
                name: "ClientSyncHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AppId = table.Column<string>(nullable: true),
                    TenantId = table.Column<string>(nullable: true),
                    RowId = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TxId = table.Column<string>(nullable: true),
                    ChangeLog = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSyncHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServerSyncHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AppId = table.Column<string>(nullable: true),
                    TenantId = table.Column<string>(nullable: true),
                    RowId = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TxId = table.Column<string>(nullable: true),
                    ChangeLog = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerSyncHistory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientSyncHistory");

            migrationBuilder.DropTable(
                name: "ServerSyncHistory");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbWarhouse");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbVoidOrderDetail");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbUserPrivillege");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbUserAccount");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbUnitofMeasure");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbTable");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbSeriesDetail");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbSeries");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbReceiptInformation");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbReceiptDetail");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbPropertyDetails");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbProperty");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbPromotionDetail");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbPromotion");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbPrinterName");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbPriceListDetail");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbPriceList");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbPeriodIndicator");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbPaymentMeans");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                schema: "dbo",
                table: "tbPaymentMeans");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbOpenShift");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbMemberCard");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbItemMasterData");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbItemComment");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbGroupUoM");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbGroupTable");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbGroupDefindUoM");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbExchangeRate");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbEmployee");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbDocumentType");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbCurrency");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbCompany");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbCloseShift");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbChildProperty");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "tbBranch");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "TaxGroup");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "TaxDefinition");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "ItemGroup3");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "ItemGroup2");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "ItemGroup1");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "Funtion");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                schema: "dbo",
                table: "CardType");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "VoidItemDetail");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "TransactionChipMong");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "TransactionAeon");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "SelfSyncEntity");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "SelfSyncEntity");

            migrationBuilder.DropColumn(
                name: "TxId",
                table: "SelfSyncEntity");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "SaleCombo");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "RemarkDiscounts");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "RedeemRetail");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "Redeem");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "ReceiptDetailMemoKvms");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "PromoCodeDiscount");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "PromoCodeDetail");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "PointRedemptionHistory");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "PointRedemption");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "PointItemHistory");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "PointItem");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "PointCard");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "PBuyXGetXDis");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "MultiPaymentMean");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "FreightReceipt");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "FreightReceipt");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "Freight");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "ExchangeCanRingDetails");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "ComboDetails");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "CanRingMaster");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "CanRingDetail");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "CanRing");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "BuyXQtyGetXDis");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "BuyXGetXDetail");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "BuyXGetX");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "AuthorizationTemplate");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ClientApi",
                newName: "CreatorId");

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "SelfSyncEntity",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "SelfSyncEntity",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "SelfSyncEntity",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "MultiPaymentMean",
                type: "decimal(36,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(29,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SCRate",
                table: "MultiPaymentMean",
                type: "decimal(36,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(29,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OpenAmount",
                table: "MultiPaymentMean",
                type: "decimal(36,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(29,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LCRate",
                table: "MultiPaymentMean",
                type: "decimal(36,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(29,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "MultiPaymentMean",
                type: "decimal(36,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(29,18)");
        }
    }
}
