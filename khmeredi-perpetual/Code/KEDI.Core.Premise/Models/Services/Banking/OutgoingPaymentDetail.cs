using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Banking
{
    [Table("tbOutgoingpaymnetDetail", Schema = "dbo")]
    public class OutgoingPaymentDetail
    {
        [Key]
        public int OutgoingPaymentDetailID { get; set; }
        public int OutgoingPaymentID { get; set; }
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
