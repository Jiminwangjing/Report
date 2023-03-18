using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Sale
{
    [Table("tbSaleCreditMemoDetail")]
    public class SaleCreditMemoDetail
    {
        [Key]
        public int SCMODID { get; set; }
        public int SCMOID { get; set; }
        public int SARDID { get; set; }
        public int ARReDetEDTID { get; set; }
        public int ItemID { get; set; }
        public string ItemCode { get; set; }
        public int TaxGroupID { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxValue { get; set; }
        public decimal TaxOfFinDisValue { get; set; }
        public decimal TaxDownPaymentValue { get; set; }
        public decimal FinTotalValue { get; set; }
        public string ItemNameKH { get; set; }
        public string ItemNameEN { get; set; }
        public double Qty { get; set; }
        public double OpenQty { get; set; }
        public double PrintQty { get; set; }
        public int GUomID { get; set; }
        public int UomID { get; set; }
        public string UomName { get; set; }
        public double Factor { get; set; }
        public double UnitPrice { get; set; }
        public double Cost { get; set; }
        public double DisRate { get; set; }
        public string TypeDis { get; set; }
        public double DisValue { get; set; }
        public decimal FinDisRate { get; set; } // Final Discount Rate
        public decimal FinDisValue { get; set; } // Final Discount Value
        public double VatRate { get; set; }
        public double VatValue { get; set; }
        public double Total { get; set; }
        public double TotalSys { get; set; }
        public double TotalWTax { get; set; } //Total With Tax
        public double TotalWTaxSys { get; set; }//Total With Tax system
        public string Process { get; set; }
        public int CurrencyID { get; set; }
        public DateTime ExpireDate { get; set; }
        public string ItemType { get; set; }
        public string Remarks { get; set; }
        public bool Delete { get; set; }
        public string LineID { get; set; }
        public SaleCopyType SaleCopyType { get; set; }
    }
}
