using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Banking;
using CKBS.Models.ServicesClass.Property;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Inventory.Transaction
{
    [Table("tbGoodReceitpDetail",Schema ="dbo")]
    public class GoodReceiptDetail
    {
        [Key]
        public int GoodReceitpDetailID  { get; set; }
        public int GoodsReceiptID { get; set; }
        public int CurrencyID { get; set; }
        public int LineID { get; set; }
        public int ItemID { get; set; }
        public int UomID { get; set; }
        public int GLID { get; set; }
        public int WarehouseID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Quantity { get; set; }
        public double Cost { get; set; }
        public string Currency { get; set; }
        public string UomName { get; set; }
        public string BarCode { get; set; }
        public string Check { get; set; }
        public string ManageExpire { get; set; }
        [Column(TypeName = "Date")]
        public DateTime ExpireDate { get; set; }
        [ForeignKey("ItemID")]
        public ItemMasterData ItemMasterData { get; set; }
        [ForeignKey("UomID")]
        public UnitofMeasure UnitofMeasure { get; set; }
        [ForeignKey("CurrencyID")]
        public Currency Currencys { get; set; }
        [NotMapped]
        public Dictionary<string, PropertydetailsViewModel> AddictionProps { get; set; }
    }
}
