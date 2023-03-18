using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Banking;
using CKBS.Models.ServicesClass.Property;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.Inventory.Transaction
{
    [Table("tbGoodIssuesDetail", Schema = "dbo")]
    public class GoodIssuesDetail
    {
        [Key]
        public int GoodIssuesDetailID { get; set; }
        public int GoodIssuesID { get; set; }
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
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        [DisplayFormat(DataFormatString = "0:yyyy-MM-dd")]
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
