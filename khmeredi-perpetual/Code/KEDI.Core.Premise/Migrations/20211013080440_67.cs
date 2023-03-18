using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _67 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbLogin",
                schema: "dbo");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                schema: "dbo",
                table: "tbUserAccount",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                schema: "dbo",
                table: "tbUserAccount",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "dbo",
                table: "tbUserAccount",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                schema: "dbo",
                table: "tbUserAccount",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                schema: "dbo",
                table: "tbMemberCard",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "BirthDate",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<decimal>(
                name: "CumulativePoint",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "GroupID",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "OutstandPoint",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RedeemedPoint",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "PointCard",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LineID = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    PointQty = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    RefNo = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointCard", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PointRedemption",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    PointQty = table.Column<int>(nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Redeemed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointRedemption", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SystemLicense",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SystemId = table.Column<string>(nullable: false),
                    MaximumUsers = table.Column<int>(nullable: false),
                    Expiration = table.Column<long>(nullable: false),
                    Unlimited = table.Column<bool>(nullable: false),
                    Version = table.Column<string>(nullable: true),
                    PublicKey = table.Column<string>(nullable: false),
                    EntryKey = table.Column<string>(nullable: true),
                    Signature = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemLicense", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PointItem",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PointRedemptID = table.Column<int>(nullable: false),
                    LineID = table.Column<string>(nullable: true),
                    ItemID = table.Column<int>(nullable: false),
                    ItemQty = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    PointRedemptionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointItem", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PointItem_PointRedemption_PointRedemptionID",
                        column: x => x.PointRedemptionID,
                        principalTable: "PointRedemption",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PointItem_PointRedemptionID",
                table: "PointItem",
                column: "PointRedemptionID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointCard");

            migrationBuilder.DropTable(
                name: "PointItem");

            migrationBuilder.DropTable(
                name: "SystemLicense");

            migrationBuilder.DropTable(
                name: "PointRedemption");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "dbo",
                table: "tbUserAccount");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                schema: "dbo",
                table: "tbUserAccount");

            migrationBuilder.DropColumn(
                name: "AppliedAmount",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                schema: "dbo",
                table: "tbMemberCard");

            migrationBuilder.DropColumn(
                name: "PrintCountBill",
                schema: "dbo",
                table: "tbGeneralSetting");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "CumulativePoint",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "GroupID",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "OutstandPoint",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "RedeemedPoint",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                schema: "dbo",
                table: "tbUserAccount",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                schema: "dbo",
                table: "tbUserAccount",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateTable(
                name: "tbLogin",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Datelogin = table.Column<DateTime>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    Timelogin = table.Column<string>(nullable: true),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbLogin", x => x.ID);
                });
        }
    }
}
