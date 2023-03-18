using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.Category;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.Financials
{
    [Table("ItemAccounting", Schema = "dbo")]
    public class ItemAccounting
    {
        [Key]
        public int ID { get; set; }
        public int? WarehouseID { get; set; }
        public int? ItemID { get; set; }
        public int? ItemGroupID { get; set; }
        public double InStock { get; set; }
        public double Committed { get; set; }
        public double Ordered { get; set; }
        public double Available { get; set; }
        public double MinimunInventory { get; set; }
        public double MaximunInventory { get; set; }
        public decimal CumulativeValue { get; set; }
        public SetGlAccount SetGlAccount { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string ExpenseAccount { get; set; }
        public string RevenueAccount { get; set; }
        public string InventoryAccount { get; set; }
        public string CostofGoodsSoldAccount { get; set; }
        public string AllocationAccount { get; set; }
        public string VarianceAccount { get; set; }
        public string PriceDifferenceAccount { get; set; }
        public string NegativeInventoryAdjustmentAcct { get; set; }
        public string InventoryOffsetDecreaseAccount { get; set; }
        public string InventoryOffsetIncreaseAccount { get; set; }
        public string SalesReturnsAccount { get; set; }
        public string RevenueAccountEU { get; set; }
        public string ExpenseAccountEU { get; set; }
        public string RevenueAccountForeign { get; set; }
        public string ExpenseAccountForeign { get; set; }
        public string ExchangeRateDifferencesAccount { get; set; }
        public string GoodsClearingAccount { get; set; }
        public string GLDecreaseAccount { get; set; }
        public string GLIncreaseAccount { get; set; }
        public string WIPInventoryAccount { get; set; }
        public string WIPInventoryVarianceAccount { get; set; }
        public string WIPOffsetPLAccount { get; set; }
        public string InventoryOffsetPLAccount { get; set; }
        public string ExpenseClearingAccount { get; set; }
        public string StockInTransitAccount { get; set; }
        public string ShippedGoodsAccount { get; set; }
        public string SalesCreditAccount { get; set; }
        public string PurchaseCreditAccount { get; set; }
        public string SalesCreditAccountForeign { get; set; }
        public string PurchaseCreditAccountForeign { get; set; }
        public string SalesCreditAccountEU { get; set; }
        public string PurchaseCreditAccountEU { get; set; }

        [ForeignKey("WarehouseID")]
        public Warehouse Warehouse { get; set; }

        [ForeignKey("ItemID")]
        public ItemMasterData ItemMasterData { get; set; }
        [ForeignKey("ItemGroupID")]
        public ItemGroup1 ItemGroup1 { get; set; }
    }

    public enum SetGlAccount
    {
        None = 0,
        ItemGroup = 1,
        ItemLevel = 2
    }
}
