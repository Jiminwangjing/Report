using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using CKBS.Models.Services.Banking;

namespace KEDI.Core.Premise.Models.Services.Banking
{
    public class IncomingPaymentOrderDetail
    {
        [Key]
        public int ID { get; set; }
        public int IncomingPaymentOrderID { get; set; }
        public string InvoiceNumber { get; set; }
        public string ItemInvoice { get; set; }
        public string DocNo { get; set; }
        public bool CheckPay { get; set; }
        public int DocTypeID { get; set; }
        public string DocumentNo { get; set; }
        public string DocumentType { get; set; }
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }
        public double OverdueDays { get; set; }
        public string CurrencyName { get; set; }
        public double Total { get; set; }
        public double BalanceDue { get; set; }
        public double Applied_Amount { get; set; }
        public double CashDiscount { get; set; }
        public double TotalDiscount { get; set; }
        public double Totalpayment { get; set; }
        public double OpenTotalpayment { get; set; }
        public int CurrencyID { get; set; }
        public double ExchangeRate { get; set; }
        public bool Delete { get; set; }
        [ForeignKey("CurrencyID")]
        public Currency Currency { get; set; }
        public int LocalCurID { get; set; }
        public double LocalSetRate { get; set; }
        public int IcoPayCusID { get; set; }
    }
}
