using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Sale
{
    public class ARReserveInvoiceEditableDetail
    {
        [Key]
        public int ID { get; set; }
        public int ARReserveInvoiceEditableID { get; set; }
        public int SQDID { get; set; }
        public int SODID { get; set; }
        public int SDDID { get; set; }
        public int ItemID { get; set; }
        public int TaxGroupID { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxValue { get; set; }
        public decimal TaxOfFinDisValue { get; set; }
        public decimal FinTotalValue { get; set; }
        public string ItemCode { get; set; }
        public string ItemNameKH { get; set; }
        public string ItemNameEN { get; set; }
        public double Qty { get; set; }
        public double OpenQty { get; set; }
        public double PrintQty { get; set; }
        public double DeliveryQty { get; set; }
        public int GUomID { get; set; }
        public int UomID { get; set; }
        public string UomName { get; set; }
        public double Factor { get; set; }
        public double Cost { get; set; }
        public double UnitPrice { get; set; }
        public double DisRate { get; set; }
        public double DisValue { get; set; }
        public decimal FinDisRate { get; set; } // Final Discount Rate
        public decimal FinDisValue { get; set; } // Final Discount Value
        public string TypeDis { get; set; }
        public double VatRate { get; set; }
        public double VatValue { get; set; }
        public double Total { get; set; }
        public double TotalSys { get; set; }
        public double TotalWTax { get; set; } //Total With Tax
        public double TotalWTaxSys { get; set; }//Total With Tax system
        public int CurrencyID { get; set; }
        public DateTime ExpireDate { get; set; }
        public string ItemType { get; set; }
        public string Remarks { get; set; }

        public bool Delete { get; set; }
        public string LineID { get; set; }
        public SaleCopyType SaleCopyType { get; set; }
        public AREDetailStatus Status { get; set; } = AREDetailStatus.New;
    }
    public class DraftReserveInvoiceEditableDetail
    {
        [Key]
        public int DraffDID { get; set; }
        public int DraftReserveInvoiceEditableID { get; set; }
        public int SQDID { get; set; }
        public int SODID { get; set; }
        public int SDDID { get; set; }
        public int ItemID { get; set; }
        public int TaxGroupID { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxValue { get; set; }
        public decimal TaxOfFinDisValue { get; set; }
        public decimal FinTotalValue { get; set; }
        public string ItemCode { get; set; }
        public string ItemNameKH { get; set; }
        public string ItemNameEN { get; set; }
        public double Qty { get; set; }
        public double OpenQty { get; set; }
        public double PrintQty { get; set; }
        public int GUomID { get; set; }
        public int UomID { get; set; }
        public string UomName { get; set; }
        public double Factor { get; set; }
        public double Cost { get; set; }
        public double UnitPrice { get; set; }
        public double DisRate { get; set; }
        public double DisValue { get; set; }
        public decimal FinDisRate { get; set; } // Final Discount Rate
        public decimal FinDisValue { get; set; } // Final Discount Value
        public string TypeDis { get; set; }
        public double VatRate { get; set; }
        public double VatValue { get; set; }
        public double Total { get; set; }
        public double TotalSys { get; set; }
        public double TotalWTax { get; set; } //Total With Tax
        public double TotalWTaxSys { get; set; }//Total With Tax system
        public int CurrencyID { get; set; }
        public DateTime ExpireDate { get; set; }
        public string ItemType { get; set; }
        public string Remarks { get; set; }
        public bool Delete { get; set; }
        public string LineID { get; set; }
        public SaleCopyType SaleCopyType { get; set; }
    }
}
