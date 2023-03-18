using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Sale
{
    [Table("ARDownPaymentDetail")]
    public class ARDownPaymentDetail
    {
        [Key]
        public int ID { get; set; }
        public int ARDID { get; set; }
        public int SQDID { get; set; }
        public int ItemID { get; set; }
        public int TaxGroupID { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxValue { get; set; }
        public decimal TaxOfFinDisValue { get; set; } //Tax Of Final Discount Value
        public decimal TaxDownPaymentValue { get; set; }
        public decimal FinTotalValue { get; set; }
        public string ItemCode { get; set; }
        public string ItemNameKH { get; set; }
        public string ItemNameEN { get; set; }
        public decimal Qty { get; set; }
        public decimal OpenQty { get; set; }
        public decimal PrintQty { get; set; }
        public int GUomID { get; set; }
        public int UomID { get; set; }
        public string UomName { get; set; }
        public decimal Factor { get; set; }
        public decimal Cost { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DisRate { get; set; }
        public decimal DisValue { get; set; }
        public decimal FinDisRate { get; set; } // Final Discount Rate
        public decimal FinDisValue { get; set; } // Final Discount Value 
        public double Total { get; set; }
        public double TotalSys { get; set; }
        public decimal TotalWTax { get; set; } //Total With Tax
        public decimal TotalWTaxSys { get; set; }//Total With Tax system
        public string Process { get; set; }
        public int CurrencyID { get; set; }
        public DateTime ExpireDate { get; set; }
        public string ItemType { get; set; }
        public string Remarks { get; set; }
        public bool Delete { get; set; }
    }
}
