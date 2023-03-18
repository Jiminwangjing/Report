    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass
{
    [Table("ServiceMapItemMasterPurchaseRequest", Schema = "dbo")]
    public class ServiceMapItemPurchaseRequest
    {
        [Key]
        public int ID { get; set; }
        public int RequestDetailID { get; set; }
        public int LineID { get; set; }
        public int ItemID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Qty { get; set; }
        public double UnitPrice { get; set; }
        public double Total { get; set; }
        public string SysCurrency { get; set; }
        public string UomName { get; set; }
        public int SysCurrencyID { get; set; }
        public int UomID { get; set; }
        public int GroupUomID { get; set; }
        public string Barcode { get; set; }
        public double OpenQty { get; set; }
        public double ExchangeRate { get; set; }
        public int VendorID { get; set; }
        public string RequiredDate { get; set; }
        public string VendorName { get; set; }
        public string Remark { get; set; }
       
    }
}
