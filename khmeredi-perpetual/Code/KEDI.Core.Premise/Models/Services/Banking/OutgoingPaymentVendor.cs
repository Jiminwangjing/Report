using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Banking
{
    [Table("tbOutgoingPaymentVendor", Schema = "dbo")]
    public class OutgoingPaymentVendor
    {
        [Key]
        public int OutgoingPaymentVendorID { get; set; }
        public int VendorID { get; set; }
        public int SeriesDetailID { get; set; }
        public int DocumentID { get; set; }
        public int CompanyID { get; set; }
        public string Number { get; set; }
        public string ItemInvoice { get; set; }
        public int BranchID { get; set; }
        public  int WarehouseID { get; set; }
        public int CurrencyID { get; set; }
        [Column(TypeName = "Date")]
        public DateTime PostingDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }
        public double OverdueDays { get; set; }
        public double Total { get; set; }
        public double BalanceDue { get; set; }
        public double TotalPayment { get; set; }
        public double Applied_Amount { get; set; }
        public string CurrencyName { get; set; }
        public string SysName { get; set; }
        public string Status { get; set; }
        public double CashDiscount { get; set; }
        public double TotalDiscount { get; set; }
        public double ExchangeRate { get; set; }
        public int SysCurrency { get; set; }
        public int LocalCurID { get; set; }
        public double LocalSetRate { get; set; }
        public TypePurchase TypePurchase { get; set; }
    }
}
