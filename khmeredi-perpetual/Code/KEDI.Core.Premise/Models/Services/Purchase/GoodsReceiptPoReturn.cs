using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.HumanResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Purchase
{
    [Table("tbGoodReceiptReturn",Schema ="dbo")]
    public class GoodsReceiptPoReturn
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GoodsReturnID { get; set; }
        public int VendorID { get; set; }
        public int BranchID { get; set; }
        public int LocalCurrencyID { get; set; }
        public int SysCurrencyID { get; set; }
        public int WarehouseID { get; set; }
        public int UserID { get; set; }
        public string Reff_No { get; set; }
        public string InvoiceNo { get; set; }
      
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PostingDate { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DocumentDate { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DueDate  { get; set; }
        public double Sub_Total { get; set; }
        public double Sub_Total_sys { get; set; }
        public decimal SubTotalAfterDis { get; set; }
        public decimal SubTotalAfterDisSys { get; set; }
        public double DiscountRate { get; set; }
        public double DiscountValues { get; set; }
        public string TypeDis { get; set; }
        public double TaxRate { get; set; }
        public double TaxValuse { get; set; }
        public double Down_Payment { get; set; }
        public double Down_PaymentSys { get; set; }
        public double Applied_Amount { get; set; }
        public double Applied_AmountSys { get; set; }
        public double Return_Amount { get; set; }
        public double ExchangeRate { get; set; }
        public double Balance_Due { get; set; }
        public double Additional_Expense { get; set; }
        public string Additional_Node { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }//close,open
        public double Balance_Due_Sys { get; set; }
        public List<GoodsReceiptPoReturnDetail> GoodsReceiptPoReturnDetail { get; set; }
        //ForeignKey
        [ForeignKey("VendorID")]
        public BusinessPartner BusinessPartner { get; set; }
        [ForeignKey("WarehouseID")]
        public Warehouse Warehouse { get; set; }
        [ForeignKey("UserID")]
        public UserAccount UserAccount { get; set; } 
        [ForeignKey("BranchID")]
        public Branch Branch { get; set; }
    }
}
