using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS.KVMS
{
    [Table("tbAgingPaymentCustomer", Schema = "dbo")]
    public class AgingPaymentCustomer
    {
        [Key]
        public int AgingPaymentCustomerID { get; set; }
        public int CustomerID { get; set; }
        public int BranchID { get; set; }
        public int WarehouseID { get; set; }
        public int CurrencyID { get; set; }
        public string DocumentNo { get; set; }
        public string DocumentType { get; set; }
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }
        [Column(TypeName = "Date")]
        public DateTime PostingDate { get; set; }
        public double OverdueDays { get; set; }
        public double Total { get; set; }
        public double TotalPayment { get; set; }
        public double Applied_Amount { get; set; }
        public double BalanceDue { get; set; }
        public string CurrencyName { get; set; }
        public string SysName { get; set; }
        public double DiscountRate { get; set; }
        public double DiscountValue { get; set; }
        public double Cash { get; set; }
        public double ExchangeRate { get; set; }
        public int SysCurrency { get; set; }
        public int LocalCurID { get; set; }
        public double LocalSetRate { get; set; }
        public StatusReceipt Status { get; set; }
    }
}
