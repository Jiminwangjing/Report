using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportPurchase
{
    [Table("rp_SummaryDetailOutgoingPayment",Schema ="dbo")]
    public class SummaryDetailOutgoingPayment
    {
        [Key]
        public int ID { get; set; }
        public string DocumentNo { get; set; }
        public string DocumentType { get; set; }
        public string Date { get; set; }
        public double OverdueDay { get; set; }
        public double BalanceDue { get; set; }
        public double Discount { get; set; }
        public double TotalPay { get; set; }
        public double Cash { get; set; }
        public string LocalCurrency { get; set; }
    }
}
