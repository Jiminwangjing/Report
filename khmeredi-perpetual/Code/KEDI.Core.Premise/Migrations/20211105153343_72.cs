using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _72 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SystemId",
                table: "SystemLicense");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "SystemLicense",
                newName: "ID");

            migrationBuilder.AddColumn<bool>(
                name: "IsVoided",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TenantID",
                schema: "dbo",
                table: "tbCompany",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PublicKey",
                table: "SystemLicense",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "Certificate",
                table: "SystemLicense",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientOrigin",
                table: "SystemLicense",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceKey",
                table: "SystemLicense",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SystemLicense",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrivateKey",
                table: "SystemLicense",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "SystemLicense",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SecretKey",
                table: "SystemLicense",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServerOrigin",
                table: "SystemLicense",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionToken",
                table: "SystemLicense",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantID",
                table: "SystemLicense",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VoidItem",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderID = table.Column<int>(nullable: false),
                    OrderNo = table.Column<string>(nullable: true),
                    TableID = table.Column<int>(nullable: false),
                    ReceiptNo = table.Column<string>(nullable: true),
                    QueueNo = table.Column<string>(nullable: true),
                    DateIn = table.Column<DateTime>(type: "Date", nullable: false),
                    DateOut = table.Column<DateTime>(type: "Date", nullable: false),
                    TimeIn = table.Column<string>(nullable: true),
                    TimeOut = table.Column<string>(nullable: true),
                    WaiterID = table.Column<int>(nullable: false),
                    UserOrderID = table.Column<int>(nullable: false),
                    UserDiscountID = table.Column<int>(nullable: false),
                    CustomerID = table.Column<int>(nullable: false),
                    CustomerCount = table.Column<int>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    LocalCurrencyID = table.Column<int>(nullable: false),
                    SysCurrencyID = table.Column<int>(nullable: false),
                    ExchangeRate = table.Column<double>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    Sub_Total = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    TaxRate = table.Column<double>(nullable: false),
                    TaxValue = table.Column<double>(nullable: false),
                    GrandTotal = table.Column<double>(nullable: false),
                    GrandTotal_Sys = table.Column<double>(nullable: false),
                    Tip = table.Column<double>(nullable: false),
                    Received = table.Column<double>(nullable: false),
                    Change = table.Column<double>(nullable: false),
                    CurrencyDisplay = table.Column<string>(nullable: true),
                    DisplayRate = table.Column<double>(nullable: false),
                    GrandTotal_Display = table.Column<double>(nullable: false),
                    Change_Display = table.Column<double>(nullable: false),
                    PaymentMeansID = table.Column<int>(nullable: false),
                    CheckBill = table.Column<string>(nullable: false),
                    Cancel = table.Column<bool>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    PLCurrencyID = table.Column<int>(nullable: false),
                    PLRate = table.Column<double>(nullable: false),
                    LocalSetRate = table.Column<double>(nullable: false),
                    Reason = table.Column<string>(nullable: true),
                    IsVoided = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoidItem", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "VoidItemDetail",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VoidItemID = table.Column<int>(nullable: false),
                    OrderDetailID = table.Column<int>(nullable: false),
                    OrderID = table.Column<int>(nullable: true),
                    Line_ID = table.Column<int>(nullable: false),
                    LineID = table.Column<string>(nullable: true),
                    ItemID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    PrintQty = table.Column<double>(nullable: false),
                    UnitPrice = table.Column<double>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    Total = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    ItemStatus = table.Column<string>(nullable: true),
                    ItemPrintTo = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    ItemType = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ParentLineID = table.Column<string>(nullable: true),
                    ParentLevel = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoidItemDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_VoidItemDetail_tbUnitofMeasure_UomID",
                        column: x => x.UomID,
                        principalSchema: "dbo",
                        principalTable: "tbUnitofMeasure",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoidItemDetail_VoidItem_VoidItemID",
                        column: x => x.VoidItemID,
                        principalTable: "VoidItem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VoidItemDetail_UomID",
                table: "VoidItemDetail",
                column: "UomID");

            migrationBuilder.CreateIndex(
                name: "IX_VoidItemDetail_VoidItemID",
                table: "VoidItemDetail",
                column: "VoidItemID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VoidItemDetail");

            migrationBuilder.DropTable(
                name: "VoidItem");

            migrationBuilder.DropColumn(
                name: "IsVoided",
                schema: "dbo",
                table: "tbOrderDetail");

            migrationBuilder.DropColumn(
                name: "TenantID",
                schema: "dbo",
                table: "tbCompany");

            migrationBuilder.DropColumn(
                name: "Certificate",
                table: "SystemLicense");

            migrationBuilder.DropColumn(
                name: "ClientOrigin",
                table: "SystemLicense");

            migrationBuilder.DropColumn(
                name: "DeviceKey",
                table: "SystemLicense");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "SystemLicense");

            migrationBuilder.DropColumn(
                name: "PrivateKey",
                table: "SystemLicense");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "SystemLicense");

            migrationBuilder.DropColumn(
                name: "SecretKey",
                table: "SystemLicense");

            migrationBuilder.DropColumn(
                name: "ServerOrigin",
                table: "SystemLicense");

            migrationBuilder.DropColumn(
                name: "SessionToken",
                table: "SystemLicense");

            migrationBuilder.DropColumn(
                name: "TenantID",
                table: "SystemLicense");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "SystemLicense",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "PublicKey",
                table: "SystemLicense",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SystemId",
                table: "SystemLicense",
                nullable: false,
                defaultValue: "");
        }
    }
}
