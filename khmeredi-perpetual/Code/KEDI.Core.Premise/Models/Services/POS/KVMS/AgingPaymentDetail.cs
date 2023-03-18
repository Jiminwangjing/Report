using CKBS.Models.Services.Banking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS.KVMS
{
    [Table("tbAgingPaymentDetail", Schema = "dbo")]
    public class AgingPaymentDetail
    {
        [Key]
        public int AgingPaymentDetailID { get; set; }
        public int AgingPaymentID { get; set; }
        public string DocumentNo { get; set; }
        public string DocumentType { get; set; }
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }
        public double OverdueDays { get; set; }
        public double Total { get; set; }
        public double BalanceDue { get; set; }
        public double DiscountRate { get; set; }
        public double DiscountValue { get; set; }
        public double Totalpayment { get; set; }
        public double Applied_Amount { get; set; }
        public int CurrencyID { get; set; }
        public double Cash { get; set; }
        public double ExchangeRate { get; set; }
        public bool Delete { get; set; }
        [ForeignKey("CurrencyID")]
        public Currency Currency { get; set; }
        public int LocalCurID { get; set; }
        public double LocalSetRate { get; set; }
    }
}
