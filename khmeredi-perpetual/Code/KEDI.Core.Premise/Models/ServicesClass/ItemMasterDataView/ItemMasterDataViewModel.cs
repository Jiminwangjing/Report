using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.Category;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.Services.Inventory.Property;
using CKBS.Models.ServicesClass.Property;
using System.Collections.Generic;

namespace CKBS.Models.ServicesClass.ItemMasterDataView
{
    public class ItemMasterDataViewModel
    {
        public OnlyItemMasterDataViewModel ItemMasterData { get; set; }
        public List<PropertyDetails> PropertyDetails { get; set; }
        public List<PropertydetailsViewModel> PropWithName { get; set; }
    }
    public class OnlyItemMasterDataViewModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public string UomName { get; set; }
        public string IG1Name { get; set; }
        public string Barcode { get; set; }
        public string Type { get; set; }//Item,Service,Labor
        public string Process { get; set; }//FIFO,Average,Standard
        public double UnitPrice { get; set; }
        public double StockIn { get; set; }
        public string Image { get; set; }
        public double Stock { get; set; }
        public double Cost { get; set; }
        public int PriceListID { get; set; }
        public int? SaleUomID { get; set; }
        public int? InventoryUoMID { get; set; }
        public int WarehouseID { get; set; }
        public SetGlAccount SetGlAccount { get; set; } = SetGlAccount.ItemGroup;
        public int ItemGroup1ID { get; set; }
        public int? ItemGroup2ID { get; set; }
        public int? ItemGroup3ID { get; set; }
        public int? PrintToID { get; set; }
        public string Description { get; set; }
        public PriceLists PriceList { get; set; }
        public ItemGroup1 ItemGroup1 { get; set; }
        public ItemGroup2 ItemGroup2 { get; set; }
        public ItemGroup3 ItemGroup3 { get; set; }
        public UnitofMeasure UnitofMeasureSale { get; set; }
        public UnitofMeasure UnitofMeasureInv { get; set; }
        public PrinterName PrinterName { get; set; }
        public string KhmerName1 { get; set; }
        public List<PropertyViewModel> Props { get; internal set; }
        public List<PropertydetailsViewModel> PropWithName { get; internal set; }
    }
}
