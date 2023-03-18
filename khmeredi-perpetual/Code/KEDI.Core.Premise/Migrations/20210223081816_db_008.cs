using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_008 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SetGlAccount",
                schema: "dbo",
                table: "tbItemMasterData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ItemAccounting",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WarehouseID = table.Column<int>(nullable: true),
                    ItemID = table.Column<int>(nullable: true),
                    ItemGroupID = table.Column<int>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    ExpenseAccount = table.Column<string>(nullable: true),
                    RevenueAccount = table.Column<string>(nullable: true),
                    InventoryAccount = table.Column<string>(nullable: true),
                    CostofGoodsSoldAccount = table.Column<string>(nullable: true),
                    AllocationAccount = table.Column<string>(nullable: true),
                    VarianceAccount = table.Column<string>(nullable: true),
                    PriceDifferenceAccount = table.Column<string>(nullable: true),
                    NegativeInventoryAdjustmentAcct = table.Column<string>(nullable: true),
                    InventoryOffsetDecreaseAccount = table.Column<string>(nullable: true),
                    InventoryOffsetIncreaseAccount = table.Column<string>(nullable: true),
                    SalesReturnsAccount = table.Column<string>(nullable: true),
                    RevenueAccountEU = table.Column<string>(nullable: true),
                    ExpenseAccountEU = table.Column<string>(nullable: true),
                    RevenueAccountForeign = table.Column<string>(nullable: true),
                    ExpenseAccountForeign = table.Column<string>(nullable: true),
                    ExchangeRateDifferencesAccount = table.Column<string>(nullable: true),
                    GoodsClearingAccount = table.Column<string>(nullable: true),
                    GLDecreaseAccount = table.Column<string>(nullable: true),
                    GLIncreaseAccount = table.Column<string>(nullable: true),
                    WIPInventoryAccount = table.Column<string>(nullable: true),
                    WIPInventoryVarianceAccount = table.Column<string>(nullable: true),
                    WIPOffsetPLAccount = table.Column<string>(nullable: true),
                    InventoryOffsetPLAccount = table.Column<string>(nullable: true),
                    ExpenseClearingAccount = table.Column<string>(nullable: true),
                    StockInTransitAccount = table.Column<string>(nullable: true),
                    ShippedGoodsAccount = table.Column<string>(nullable: true),
                    SalesCreditAccount = table.Column<string>(nullable: true),
                    PurchaseCreditAccount = table.Column<string>(nullable: true),
                    SalesCreditAccountForeign = table.Column<string>(nullable: true),
                    PurchaseCreditAccountForeign = table.Column<string>(nullable: true),
                    SalesCreditAccountEU = table.Column<string>(nullable: true),
                    PurchaseCreditAccountEU = table.Column<string>(nullable: true),
                    InStock = table.Column<double>(nullable: false),
                    Committed = table.Column<double>(nullable: false),
                    Ordered = table.Column<double>(nullable: false),
                    Available = table.Column<double>(nullable: false),
                    MinimunInventory = table.Column<double>(nullable: false),
                    MaximunInventory = table.Column<double>(nullable: false),
                    SetGlAccount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAccounting", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ItemAccounting_ItemGroup1_ItemGroupID",
                        column: x => x.ItemGroupID,
                        principalSchema: "dbo",
                        principalTable: "ItemGroup1",
                        principalColumn: "ItemG1ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemAccounting_tbItemMasterData_ItemID",
                        column: x => x.ItemID,
                        principalSchema: "dbo",
                        principalTable: "tbItemMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemAccounting_tbWarhouse_WarehouseID",
                        column: x => x.WarehouseID,
                        principalSchema: "dbo",
                        principalTable: "tbWarhouse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemAccounting_ItemGroupID",
                schema: "dbo",
                table: "ItemAccounting",
                column: "ItemGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAccounting_ItemID",
                schema: "dbo",
                table: "ItemAccounting",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAccounting_WarehouseID",
                schema: "dbo",
                table: "ItemAccounting",
                column: "WarehouseID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemAccounting",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "SetGlAccount",
                schema: "dbo",
                table: "tbItemMasterData");
        }
    }
}
