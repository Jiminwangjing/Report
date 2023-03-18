using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass
{
    [Table("ServiceMapItemMasterDataQuotation", Schema = "dbo")]
    public class ServiceMapItemMasterDataQuotation 
    {
        [Key]
        public int ID { get; set; }
        public int QuotationDetailID { get; set; }
        public int LineID { get; set; }
        public int ItemID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public string RequiredDate { get; set; }
        public string QuotedDate { get; set; }
        public double RequiredQty { get; set; }
        public double Qty { get; set; }
        public double PurchasPrice { get; set; }
        public double DiscountRate { get; set; }
        public double DiscountValue { get; set; }
        public string TypeDis { get; set; }//Percent ,cash
        public double Total { get; set; }
        public string SysCurrency { get; set; }
        public string UomName { get; set; }
        public string ManageExpire { get; set; }
        public string ExpireDate { get; set; }
        public double AlertStock { get; set; }
        public double Total_Sys { get; set; }
        public double ExchangeRate { get; set; }
        public int SysCurrencyID { get; set; }
        public int LocalCurrencyID { get; set; }
        public string LocalCurrency { get; set; }
        public int UomID { get; set; }
        public int GroupUomID { get; set; }
        public string Barcode { get; set; }
        public double OpenQty { get; set; }
        public bool Choosed { get; set; }

    }
}
