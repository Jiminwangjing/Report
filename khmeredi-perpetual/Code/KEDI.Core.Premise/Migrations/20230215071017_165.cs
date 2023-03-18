using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _165 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateT",
                table: "BuyXGetX",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "Date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateF",
                table: "BuyXGetX",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "Date");

            migrationBuilder.CreateTable(
                name: "DataSyncSetting",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantCode = table.Column<string>(nullable: true),
                    TenantName = table.Column<string>(nullable: true),
                    BaseUrl = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    ApiKey = table.Column<string>(nullable: true),
                    SecretKey = table.Column<string>(nullable: true),
                    StartTime = table.Column<TimeSpan>(nullable: false),
                    EndTime = table.Column<TimeSpan>(nullable: false),
                    TickInterval = table.Column<double>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    Revoked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSyncSetting", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbPromotionDetail",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PriceListID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    Discount = table.Column<float>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    PromotionID = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    StopDate = table.Column<DateTime>(nullable: false),
                    RowId = table.Column<Guid>(nullable: false),
                    Spk = table.Column<int>(nullable: false),
                    Cpk = table.Column<int>(nullable: false),
                    Synced = table.Column<bool>(nullable: false),
                    SyncDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPromotionDetail", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataSyncSetting");

            migrationBuilder.DropTable(
                name: "tbPromotionDetail",
                schema: "dbo");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateT",
                table: "BuyXGetX",
                type: "Date",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateF",
                table: "BuyXGetX",
                type: "Date",
                nullable: false,
                oldClrType: typeof(DateTime));
        }
    }
}
