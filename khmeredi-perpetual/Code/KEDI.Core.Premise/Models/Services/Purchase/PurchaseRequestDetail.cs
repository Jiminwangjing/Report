using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.Purchase
{
    [Table("tbPurchaseRequestDetail", Schema = "dbo")]
    public class PurchaseRequestDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int PurchaseRequestID { get; set; }
        public int ItemID { get; set; }
        public int UomID { get; set; }
        public int VendorID { get; set; }
        public int LocalCurrencyID { get; set; }
        public int TaxGroupID { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxValue { get; set; }
        public decimal TaxOfFinDisValue { get; set; }
        public decimal FinTotalValue { get; set; }
        public decimal FinDisRate { get; set; } // Final Discount Rate
        public decimal FinDisValue { get; set; } // Final Discount Value
        public double TotalWTax { get; set; } //Total With Tax
        public double TotalWTaxSys { get; set; }//Total With Tax system
        public double DiscountRate { get; set; }
        public double DiscountValue { get; set; }
        public double Qty { get; set; }
        public double OpenQty { get; set; }
        public double PurchasPrice { get; set; }
        public double Total { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExpireDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequiredDate { get; set; }
        public double AlertStock { get; set; }
        public double TotalSys { get; set; }
        public bool Delete { get; set; }
        public double OldQty { get; set; }
        public int QuotationID { get; set; }
        public string Check { get; set; }
        public string Remark { get; set; }
    }
}
