using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Purchase.Print
{
    [Table("PrintPurchaseAP",Schema ="dbo")]
    public class PrintPurchaseAP
    {
        [Key]
        // Master

        public string Brand { get; set; }
        public string CusCode { get; set; }
        public int PurchasID { get; set; }
        public string Invoice { get; set; }
        public double ExchangeRate { get; set; }
        public double Balance_Due_Sys { get; set; }
        public double Balance_Due_Local { get; set; }
        public string PostingDate { get; set; }
        public string DocumentDate { get; set; }
        public string DueDate { get; set; }
        public double Sub_Total { get; set; }
        public double Sub_Total_Sys { get; set; }
        public double DiscountValue { get; set; }
        public double DiscountRate { get; set; }
        public double TaxValue { get; set; }
        public double TaxRate { get; set; }
        public double Applied_Amount { get; set; }
        public string TypeDis { get; set; }
        public string VendorName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        public string LocalCurrency { get; set; }
        public string SysCurrency { get; set; }
        public string VendorNo { get; set; }
        public string BaseOn { get; set; }
        public string PreFix { get; set; }

        //Detail
        public string CompanyName { get; set; }
        public string Addresskh { get; set; }
        public string AddressEng { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string Remark { get; set; }
        public string SQN { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Qty { get; set; }
        public double Price { get; set; }
        public double DiscountValue_Detail { get; set; }
        public double DiscountRate_Detail { get; set; }
        public double Total { get; set; }
        public double Total_Sys { get; set; }
        public string UomName { get; set; }
        public string Logo { get; set; }
        public double Sub_Total_Detail { get; set; }

    }
}
