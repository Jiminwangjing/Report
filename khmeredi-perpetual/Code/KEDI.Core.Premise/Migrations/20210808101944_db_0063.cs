using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_0063 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TelegramUserID",
                schema: "dbo",
                table: "tbUserAccount",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "tbAlertManagement",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "TypeOfAlert",
                schema: "dbo",
                table: "tbAlertManagement",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CumulativeValue",
                schema: "dbo",
                table: "ItemAccounting",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllWh",
                table: "tbSettingAlert",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AlertDetails",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AlertMasterID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    IsAllWh = table.Column<bool>(nullable: false),
                    IsAllUsers = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertDetails", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AlertMasters",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    TypeOfAlert = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    DeleteAlert = table.Column<int>(nullable: false),
                    Frequently = table.Column<long>(nullable: false),
                    TypeFrequently = table.Column<int>(nullable: false),
                    TypeBeforeAppDate = table.Column<int>(nullable: false),
                    BeforeAppDate = table.Column<long>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CashOutAlerts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BrandID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    EmpID = table.Column<int>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    TimeIn = table.Column<string>(nullable: true),
                    TimeOut = table.Column<string>(nullable: true),
                    DateIn = table.Column<DateTime>(nullable: false),
                    DateOut = table.Column<DateTime>(nullable: false),
                    CashInAmountSys = table.Column<decimal>(nullable: false),
                    SaleAmountSys = table.Column<decimal>(nullable: false),
                    GrandTotal = table.Column<decimal>(nullable: false),
                    IsRead = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashOutAlerts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DueDateAlerts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BPID = table.Column<int>(nullable: false),
                    InvoiceID = table.Column<int>(nullable: false),
                    SeriesDID = table.Column<int>(nullable: false),
                    InvoiceNumber = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    IsRead = table.Column<bool>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DueDateType = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    TimeLeft = table.Column<string>(nullable: true),
                    CompanyID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DueDateAlerts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "StockAlerts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ItemID = table.Column<int>(nullable: false),
                    ItemName = table.Column<string>(nullable: true),
                    InStock = table.Column<double>(nullable: false),
                    IsRead = table.Column<bool>(nullable: false),
                    MinInv = table.Column<double>(nullable: false),
                    MaxInv = table.Column<double>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    WarehouseName = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    StockType = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    CompanyID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAlerts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TelegramTokens",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramTokens", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AlertWarehouse",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AlertDetailID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    IsAlert = table.Column<bool>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    SetttingAlertID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertWarehouse", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AlertWarehouse_AlertDetails_AlertDetailID",
                        column: x => x.AlertDetailID,
                        principalTable: "AlertDetails",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlertWarehouse_tbSettingAlert_SetttingAlertID",
                        column: x => x.SetttingAlertID,
                        principalTable: "tbSettingAlert",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAlerts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AlertDetailID = table.Column<int>(nullable: false),
                    UserAccountID = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    TelegramUserID = table.Column<string>(nullable: true),
                    IsAlert = table.Column<bool>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAlerts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserAlerts_AlertDetails_AlertDetailID",
                        column: x => x.AlertDetailID,
                        principalTable: "AlertDetails",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertWarehouse_AlertDetailID",
                table: "AlertWarehouse",
                column: "AlertDetailID");

            migrationBuilder.CreateIndex(
                name: "IX_AlertWarehouse_SetttingAlertID",
                table: "AlertWarehouse",
                column: "SetttingAlertID");

            migrationBuilder.CreateIndex(
                name: "IX_UserAlerts_AlertDetailID",
                table: "UserAlerts",
                column: "AlertDetailID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertMasters");

            migrationBuilder.DropTable(
                name: "AlertWarehouse");

            migrationBuilder.DropTable(
                name: "CashOutAlerts");

            migrationBuilder.DropTable(
                name: "DueDateAlerts");

            migrationBuilder.DropTable(
                name: "StockAlerts");

            migrationBuilder.DropTable(
                name: "TelegramTokens");

            migrationBuilder.DropTable(
                name: "UserAlerts");

            migrationBuilder.DropTable(
                name: "AlertDetails");

            migrationBuilder.DropColumn(
                name: "TelegramUserID",
                schema: "dbo",
                table: "tbUserAccount");

            migrationBuilder.DropColumn(
                name: "TypeOfAlert",
                schema: "dbo",
                table: "tbAlertManagement");

            migrationBuilder.DropColumn(
                name: "CumulativeValue",
                schema: "dbo",
                table: "ItemAccounting");

            migrationBuilder.DropColumn(
                name: "IsAllWh",
                table: "tbSettingAlert");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "tbAlertManagement",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
