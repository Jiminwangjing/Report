
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Inventory.Category;
using CKBS.Models.Services.Inventory.PriceList;
using KEDI.Core.Premise.Models.Sync;
using KEDI.Core.Premise.Resources;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.Inventory
{
    [Table("tbItemMasterData", Schema = "dbo")]
    public class ItemMasterData : ISyncEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required(ErrorMessage = "Please input code !")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Please input product name !")]
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double StockIn { get; set; }
        [NotMapped]
        public double StockPending { get; set; }
        public double StockCommit { get; set; }
        public double StockOnHand { get; set; }
        public int TaxGroupSaleID { get; set; }
        public int TaxGroupPurID { get; set; }
        public double Cost { get; set; }
        public double UnitPrice { get; set; }
        public int BaseUomID { get; set; }
        public int PriceListID { get; set; }
        public int GroupUomID { get; set; }
        public int? PurchaseUomID { get; set; }
        public int? SaleUomID { get; set; }
        public int? InventoryUoMID { get; set; }
        public int WarehouseID { get; set; }
        [Required]
        public string Type { get; set; }//Item,Service,Labor

        public SetGlAccount SetGlAccount { get; set; } = SetGlAccount.ItemGroup;
        public int ItemGroup1ID { get; set; }
        public int? ItemGroup2ID { get; set; }
        public int? ItemGroup3ID { get; set; }
        public bool Inventory { get; set; }
        public bool Sale { get; set; }
        public bool Purchase { get; set; }
        public string Barcode { get; set; }

        public int? PrintToID { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Process { get; set; }//FIFO,Average,Standard/SEBA(SerialBatch)/FEFO
        public string ManageExpire { get; set; } = "None";
        public ManageItemBy ManItemBy { get; set; }
        public ManagementMethod ManMethod { get; set; }
        public bool Delete { get; set; }
        public bool Scale { get; set; }
        public int CompanyID { get; set; }
        public int ContractID { get; set; }
        public bool IsLimitOrder { get; set; }
        public double MinOrderQty { get; set; }
        public double MaxOrderQty { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }
        [NotMapped]
        public string UomName { get; set; }

        //Foreign Key
        [ForeignKey("PriceListID")]
        public PriceLists PriceList { get; set; }

        [ForeignKey("GroupUomID")]
        public GroupUOM GroupUOM { get; set; }

        [ForeignKey("ItemGroup1ID")]
        public ItemGroup1 ItemGroup1 { get; set; }

        [ForeignKey("ItemGroup2ID")]
        public ItemGroup2 ItemGroup2 { get; set; }

        [ForeignKey("ItemGroup3ID")]
        public ItemGroup3 ItemGroup3 { get; set; }
        [ForeignKey("SaleUomID")]
        public UnitofMeasure UnitofMeasureSale { get; set; }
        [ForeignKey("PurchaseUomID")]
        public UnitofMeasure UnitofMeasurePur { get; set; }
        [ForeignKey("InventoryUoMID")]
        public UnitofMeasure UnitofMeasureInv { get; set; }
        [ForeignKey("PrintToID")]
        public PrinterName PrinterName { get; set; }


        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public bool AIAJ { get; set; }//Advand Inventory Ajustment
        public DateTime SyncDate { get; set; }
    }
}
