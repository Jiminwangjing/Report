using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.Tables;
using CKBS.Models.Services.Banking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS.KVMS
{
    public enum StatusReceipt { Aging, Paid }
    [Table("tbReceiptKvms", Schema = "dbo")]
    public class ReceiptKvms
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReceiptKvmsID { get; set; }
        public int KvmsInfoID { get; set; }
        public int OrderID { get; set; }
        [Required]
        public string OrderNo { get; set; }
        [Required]
        public int TableID { get; set; }
        [Required]
        public string ReceiptNo { get; set; }
        [Required]
        public string QueueNo { get; set; }
        [Required]
        [Column(TypeName = "Date")]
        public DateTime DateIn { get; set; }
        [Required]
        [Column(TypeName = "Date")]
        public DateTime DateOut { get; set; }
        [Required]
        public string TimeIn { get; set; }
        [Required]
        public string TimeOut { get; set; }

        public int WaiterID { get; set; }
        [Required]
        public int UserOrderID { get; set; }
        [Required]
        public int UserDiscountID { get; set; }
        [Required]
        public int CustomerID { get; set; }
        [Required]
        public int CustomerCount { get; set; }
        [Required]
        public int PriceListID { get; set; }
        [Required]
        public int LocalCurrencyID { get; set; }
        [Required]
        public int SysCurrencyID { get; set; }
        [Required]
        public double ExchangeRate { get; set; }
        [Required]
        public int WarehouseID { get; set; }
        [Required]
        public int BranchID { get; set; }
        [Required]
        public int CompanyID { get; set; }
        [Required]
        public double Sub_Total { get; set; }
        [Required]
        public double DiscountRate { get; set; }
        [Required]
        public double DiscountValue { get; set; }
        [Required]
        public string TypeDis { get; set; } = "Percent";
        [Required]
        public double TaxRate { get; set; }
        [Required]
        public double TaxValue { get; set; }
        [Required]
        public double GrandTotal { get; set; }
        [Required]
        public double GrandTotal_Sys { get; set; }
        public double Tip { get; set; } = 0;
        [Required]
        public double Received { get; set; }
        [Required]
        public double Change { get; set; }
        public string CurrencyDisplay { get; set; }
        public double DisplayRate { get; set; }
        public double GrandTotal_Display { get; set; }
        public double Change_Display { get; set; }
        [Required]
        public int PaymentMeansID { get; set; }
        [Required]
        public char CheckBill { get; set; }
        public bool Cancel { get; set; } = false;
        public bool Delete { get; set; } = false;
        public bool Return { get; set; } = false;
        public int PLCurrencyID { get; set; }
        public double PLRate { get; set; }
        public double LocalSetRate { get; set; }
        //Series
        public int SeriesID { get; set; }
        public int SeriesDID { get; set; }
        //Aging
        public double AppliedAmount { get; set; }
        public double BalanceDue { get; set; }
        public StatusReceipt Status { get; set; }

        [ForeignKey("LocalCurrencyID")]
        public Currency Currency { get; set; }
        [ForeignKey("UserOrderID")]
        public UserAccount UserAccount { get; set; }
        [ForeignKey("TableID")]
        public Table Table { get; set; }

        [ForeignKey("KvmsInfoID")]
        public KvmsInfo KvmsInfo { get; set; }
    }
}
