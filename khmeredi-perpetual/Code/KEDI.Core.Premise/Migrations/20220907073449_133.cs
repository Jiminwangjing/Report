using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _133 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbWarhouse",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbWarhouse",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbWarhouse",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbWarhouse",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbUserPrivillege",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbUserPrivillege",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbUserPrivillege",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbUserPrivillege",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbUserAccount",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbUserAccount",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbUserAccount",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbUserAccount",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbUnitofMeasure",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbUnitofMeasure",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbUnitofMeasure",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbUnitofMeasure",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbTable",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbTable",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbTable",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbTable",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbSeriesDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbSeriesDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbSeriesDetail",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbSeriesDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbSeries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbSeries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbSeries",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbSeries",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbReceiptInformation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbReceiptInformation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbReceiptInformation",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbReceiptInformation",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbReceiptDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbReceiptDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbReceiptDetail",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbReceiptDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbPropertyDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbPropertyDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbPropertyDetails",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbPropertyDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbProperty",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbProperty",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbProperty",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbProperty",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbPromotion",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbPromotion",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbPromotion",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbPromotion",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbPrinterName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbPrinterName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbPrinterName",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbPrinterName",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbPriceListDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbPriceListDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbPriceListDetail",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbPriceListDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbPriceList",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbPriceList",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbPriceList",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbPriceList",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbPeriodIndicator",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbPeriodIndicator",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbPeriodIndicator",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbPeriodIndicator",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbPaymentMeans",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbPaymentMeans",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbPaymentMeans",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbPaymentMeans",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbMemberCard",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbMemberCard",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbMemberCard",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbMemberCard",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbItemMasterData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbItemMasterData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbItemMasterData",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbItemMasterData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbItemComment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbItemComment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbItemComment",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbItemComment",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbGroupUoM",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbGroupUoM",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbGroupUoM",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbGroupUoM",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbGroupTable",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbGroupTable",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbGroupTable",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbGroupTable",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbGroupDefindUoM",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbGroupDefindUoM",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbGroupDefindUoM",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbGroupDefindUoM",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbExchangeRate",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbExchangeRate",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbExchangeRate",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbExchangeRate",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbEmployee",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbEmployee",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbEmployee",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbEmployee",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbDocumentType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbDocumentType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbDocumentType",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbDocumentType",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbCurrency",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbCurrency",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbCurrency",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbCurrency",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbCompany",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbCompany",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbCompany",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbCompany",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbChildProperty",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbChildProperty",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbChildProperty",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbChildProperty",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "tbBranch",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "tbBranch",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "tbBranch",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "tbBranch",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "TaxGroup",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "TaxGroup",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "TaxGroup",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "TaxGroup",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "TaxDefinition",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "TaxDefinition",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "TaxDefinition",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "TaxDefinition",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "ItemGroup3",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "ItemGroup3",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "ItemGroup3",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "ItemGroup3",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "ItemGroup2",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "ItemGroup2",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "ItemGroup2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "ItemGroup2",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "ItemGroup1",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "ItemGroup1",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "ItemGroup1",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "ItemGroup1",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "Funtion",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "Funtion",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "Funtion",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "Funtion",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                schema: "dbo",
                table: "CardType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                schema: "dbo",
                table: "CardType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                schema: "dbo",
                table: "CardType",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                schema: "dbo",
                table: "CardType",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "SaleCombo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "SaleCombo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "SaleCombo",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "SaleCombo",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "RedeemRetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "RedeemRetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "RedeemRetail",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "RedeemRetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "Redeem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "Redeem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "Redeem",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "Redeem",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "PromoCodeDiscount",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "PromoCodeDiscount",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "PromoCodeDiscount",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "PromoCodeDiscount",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "PromoCodeDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "PromoCodeDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "PromoCodeDetail",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "PromoCodeDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "PointRedemptionHistory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "PointRedemptionHistory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "PointRedemptionHistory",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "PointRedemptionHistory",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "PointRedemption",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "PointRedemption",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "PointRedemption",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "PointRedemption",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "PointItemHistory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "PointItemHistory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "PointItemHistory",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "PointItemHistory",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "PointItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "PointItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "PointItem",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "PointItem",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "PointCard",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "PointCard",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "PointCard",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "PointCard",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "PBuyXGetXDis",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "PBuyXGetXDis",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "PBuyXGetXDis",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "PBuyXGetXDis",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "Freight",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "Freight",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "Freight",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "Freight",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "ExchangeCanRingDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "ExchangeCanRingDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "ExchangeCanRingDetails",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "ExchangeCanRingDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "ComboDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "ComboDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "ComboDetails",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "ComboDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "CanRingMaster",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "CanRingMaster",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "CanRingMaster",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "CanRingMaster",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "CanRingDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "CanRingDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "CanRingDetail",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "CanRingDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "CanRing",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "CanRing",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "CanRing",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "CanRing",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "BuyXQtyGetXDis",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "BuyXQtyGetXDis",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "BuyXQtyGetXDis",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "BuyXQtyGetXDis",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "BuyXGetXDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "BuyXGetXDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "BuyXGetXDetail",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "BuyXGetXDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "BuyXGetX",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "BuyXGetX",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "BuyXGetX",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "BuyXGetX",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Cpk",
                table: "AuthorizationTemplate",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Spk",
                table: "AuthorizationTemplate",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "AuthorizationTemplate",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Synced",
                table: "AuthorizationTemplate",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ClientApi",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClientCode = table.Column<string>(nullable: true),
                    ClientName = table.Column<string>(nullable: true),
                    AppId = table.Column<string>(nullable: true),
                    IpAddress = table.Column<string>(nullable: true),
                    Signature = table.Column<string>(nullable: true),
                    PublicKey = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    CreatorId = table.Column<int>(nullable: false),
                    StrictIpAddress = table.Column<bool>(nullable: false),
                    Readonly = table.Column<bool>(nullable: false),
                    Revoked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientApi", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientApi");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbWarhouse");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbWarhouse");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbWarhouse");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbWarhouse");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbUserPrivillege");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbUserPrivillege");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbUserPrivillege");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbUserPrivillege");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbUserAccount");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbUserAccount");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbUserAccount");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbUserAccount");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbUnitofMeasure");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbUnitofMeasure");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbUnitofMeasure");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbUnitofMeasure");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbTable");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbTable");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbTable");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbTable");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbSeriesDetail");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbSeriesDetail");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbSeriesDetail");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbSeriesDetail");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbSeries");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbSeries");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbSeries");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbSeries");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbReceiptInformation");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbReceiptInformation");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbReceiptInformation");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbReceiptInformation");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbReceiptDetail");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbReceiptDetail");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbReceiptDetail");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbReceiptDetail");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbPropertyDetails");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbPropertyDetails");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbPropertyDetails");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbPropertyDetails");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbProperty");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbProperty");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbProperty");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbProperty");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbPromotion");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbPromotion");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbPromotion");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbPromotion");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbPrinterName");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbPrinterName");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbPrinterName");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbPrinterName");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbPriceListDetail");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbPriceListDetail");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbPriceListDetail");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbPriceListDetail");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbPriceList");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbPriceList");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbPriceList");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbPriceList");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbPeriodIndicator");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbPeriodIndicator");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbPeriodIndicator");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbPeriodIndicator");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbPaymentMeans");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbPaymentMeans");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbPaymentMeans");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbPaymentMeans");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbMemberCard");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbMemberCard");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbMemberCard");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbMemberCard");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbItemMasterData");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbItemMasterData");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbItemMasterData");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbItemMasterData");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbItemComment");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbItemComment");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbItemComment");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbItemComment");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbGroupUoM");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbGroupUoM");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbGroupUoM");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbGroupUoM");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbGroupTable");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbGroupTable");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbGroupTable");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbGroupTable");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbGroupDefindUoM");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbGroupDefindUoM");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbGroupDefindUoM");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbGroupDefindUoM");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbExchangeRate");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbExchangeRate");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbExchangeRate");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbExchangeRate");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbEmployee");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbEmployee");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbEmployee");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbEmployee");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbDocumentType");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbDocumentType");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbDocumentType");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbDocumentType");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbCurrency");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbCurrency");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbCurrency");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbCurrency");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbCompany");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbCompany");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbCompany");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbCompany");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbChildProperty");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbChildProperty");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbChildProperty");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbChildProperty");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "tbBranch");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "tbBranch");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "tbBranch");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "tbBranch");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "TaxGroup");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "TaxGroup");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "TaxGroup");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "TaxGroup");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "TaxDefinition");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "TaxDefinition");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "TaxDefinition");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "TaxDefinition");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "ItemGroup3");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "ItemGroup3");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "ItemGroup3");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "ItemGroup3");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "ItemGroup2");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "ItemGroup2");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "ItemGroup2");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "ItemGroup2");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "ItemGroup1");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "ItemGroup1");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "ItemGroup1");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "ItemGroup1");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "Funtion");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "Funtion");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "Funtion");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "Funtion");

            migrationBuilder.DropColumn(
                name: "Cpk",
                schema: "dbo",
                table: "CardType");

            migrationBuilder.DropColumn(
                name: "Spk",
                schema: "dbo",
                table: "CardType");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                schema: "dbo",
                table: "CardType");

            migrationBuilder.DropColumn(
                name: "Synced",
                schema: "dbo",
                table: "CardType");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "SaleCombo");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "SaleCombo");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "SaleCombo");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "SaleCombo");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "RedeemRetail");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "RedeemRetail");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "RedeemRetail");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "RedeemRetail");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "Redeem");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "Redeem");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "Redeem");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "Redeem");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "PromoCodeDiscount");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "PromoCodeDiscount");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "PromoCodeDiscount");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "PromoCodeDiscount");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "PromoCodeDetail");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "PromoCodeDetail");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "PromoCodeDetail");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "PromoCodeDetail");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "PointRedemptionHistory");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "PointRedemptionHistory");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "PointRedemptionHistory");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "PointRedemptionHistory");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "PointRedemption");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "PointRedemption");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "PointRedemption");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "PointRedemption");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "PointItemHistory");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "PointItemHistory");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "PointItemHistory");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "PointItemHistory");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "PointItem");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "PointItem");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "PointItem");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "PointItem");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "PointCard");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "PointCard");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "PointCard");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "PointCard");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "PBuyXGetXDis");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "PBuyXGetXDis");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "PBuyXGetXDis");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "PBuyXGetXDis");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "Freight");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "Freight");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "Freight");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "Freight");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "ExchangeCanRingDetails");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "ExchangeCanRingDetails");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "ExchangeCanRingDetails");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "ExchangeCanRingDetails");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "ComboDetails");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "ComboDetails");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "ComboDetails");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "ComboDetails");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "CanRingMaster");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "CanRingMaster");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "CanRingMaster");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "CanRingMaster");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "CanRingDetail");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "CanRingDetail");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "CanRingDetail");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "CanRingDetail");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "CanRing");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "CanRing");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "CanRing");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "CanRing");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "BuyXQtyGetXDis");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "BuyXQtyGetXDis");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "BuyXQtyGetXDis");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "BuyXQtyGetXDis");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "BuyXGetXDetail");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "BuyXGetXDetail");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "BuyXGetXDetail");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "BuyXGetXDetail");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "BuyXGetX");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "BuyXGetX");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "BuyXGetX");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "BuyXGetX");

            migrationBuilder.DropColumn(
                name: "Cpk",
                table: "AuthorizationTemplate");

            migrationBuilder.DropColumn(
                name: "Spk",
                table: "AuthorizationTemplate");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "AuthorizationTemplate");

            migrationBuilder.DropColumn(
                name: "Synced",
                table: "AuthorizationTemplate");
        }
    }
}
