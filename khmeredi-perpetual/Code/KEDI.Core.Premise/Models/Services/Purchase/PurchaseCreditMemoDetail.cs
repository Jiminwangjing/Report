using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Purchase
{
    [Table("tbPurchaseCreditMemoDetail")]
    public class PurchaseCreditMemoDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PurchaseMemoDetailID { get; set; }
        public int PurchaseCreditMemoID { get; set; }
        public int ItemID { get; set; }
        [NotMapped]
        public string LineIDUN { get; set; }
        [NotMapped]
        public int BaseOnID { get; set; }
        public int LineID { get; set; }
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
        public double Qty { get; set; }
        public double OpenQty { get; set; }
        public double Total { get; set; }//total=qty*purchase
        public double TotalSys { get; set; }
        public double DiscountRate { get; set; }
        public double DiscountValue { get; set; }
        public string TypeDis { get; set; }//Percent ,cash
        public double PurchasPrice { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExpireDate { get; set; }
        public double AlertStock { get; set; }
        public string Check { get; set; }
        public string Remark { get; set; }
        public PurCopyType PurCopyType { get; set; }
        [ForeignKey("ItemID")]
        public virtual ItemMasterData ItemMasterData { get; set; }
        [ForeignKey("UomID")]
        public virtual UnitofMeasure UnitofMeasure { get; set; }
        [ForeignKey("LocalCurrencyID")]
        public Currency Currency { get; set; }
    }
}
