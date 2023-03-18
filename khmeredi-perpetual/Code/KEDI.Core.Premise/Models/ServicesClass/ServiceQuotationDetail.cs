using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass
{
    [Table("ServiceQuotationDetail",Schema ="dbo")]
    public class ServiceQuotationDetail
    {
        [Key]
        public int ID { get; set; }
        public int QuotationDetailID { get; set; }
        public int PurchaseQuotationID { get; set; }
        public int LineID { get; set; }
        public int ItemID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Qty { get; set; }
        public double PurchasePrice { get; set; }
        public double Total { get; set; }
        public string UomName { get; set; }
        public double Total_Sys { get; set; }
        public double ExchangeRate { get; set; }
        public int SysCurrencyID { get; set; }
        public int LocalCurrencyID { get; set; }
        public string SysCurrency { get; set; }
        public string LocalCurrency { get; set; }
       
        public int UomID { get; set; }
        public int GroupUomID { get; set; }
        
        public string Barcode { get; set; }




    }
}
