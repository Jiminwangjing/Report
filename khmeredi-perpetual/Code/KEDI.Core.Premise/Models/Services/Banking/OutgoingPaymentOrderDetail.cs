using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CKBS.Models.Services.Banking;
namespace KEDI.Core.Premise.Models.Services.Banking
{
      [Table("tbOutgoingpaymnetOrderDetail", Schema = "dbo")]
    public class OutgoingPaymentOrderDetail
    {
       [Key]
        public int ID { get; set; }
        public int OutgoingPaymentOrderID { get; set; }
        public string NumberInvioce { get; set; }
        public string ItemInvoice { get; set; }
        public string DocNo { get; set; }
        public bool CheckPay { get; set; }
        public int DocTypeID { get; set; }
        [Column(TypeName ="Date")]
        public DateTime Date { get; set; }
        public double OverdueDays { get; set; }
        public string CurrencyName{ get; set; }
        public double Total { get; set; }
        public double BalanceDue { get; set; }
        public double CashDiscount { get; set; }
        public double TotalDiscount { get; set; }
        public double Totalpayment { get; set; }
        public double Applied_Amount { get; set; }
        public int CurrencyID { get; set; }
        public double ExchangeRate { get; set; }
        public int BasedOnID { get; set; }
        public bool Delete { get; set; }
        [ForeignKey("CurrencyID")]
        public  Currency Currency { get; set; }
        public int LocalCurID { get; set; }
        public double LocalSetRate { get; set; }

    }
}