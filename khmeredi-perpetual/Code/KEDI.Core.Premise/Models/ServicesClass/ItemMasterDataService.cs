using CKBS.Models.Services.Inventory.PriceList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass
{
    [Table("ServiceMasterData",Schema ="dbo")]
    public class ItemMasterDataService
    {
        [Key]
        public int ID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public string Cost { get; set; }
        public string UnitPrice { get; set; }
        public string Currency { get; set; }
        public string UoM { get; set; }
        public string Barcode { get; set; }
        public string   Process { get; set; }
        public int  ItemID { get; set; }
        public int Group1 { get; set; }
        public int Group2 { get; set; }
        public int Group3 { get; set; }
        public int PricListID { get; set; }
        public int UomID { get; set; }
        public string SysCurrency { get; set; }
      
    }
   
}
