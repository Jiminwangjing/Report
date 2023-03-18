using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Purchase
{
    [Table("DraftAPDetail")]
    public class DraftAPDetail
    {
        [Key]
        public int DraftDetailID { get; set; }
        public int DraftID { get; set; }
        public int ItemID { get; set; }
        public int UomID { get; set; }
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
        public double TotalSys { get; set; }
        public double DiscountRate { get; set; }
        public double DiscountValue { get; set; }
        public string TypeDis { get; set; }//Percent ,cash
        public double Qty { get; set; }
        public double PurchasPrice { get; set; }
        public double Total { get; set; }//total=qty* PurchasePrice
        public int OrderID { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExpireDate { get; set; }
        public double AlertStock { get; set; }
        public bool Delete { get; set; }
        public double OpenQty { get; set; }
        public string Check { get; set; }
        public string Remark { get; set; }
    }
}
