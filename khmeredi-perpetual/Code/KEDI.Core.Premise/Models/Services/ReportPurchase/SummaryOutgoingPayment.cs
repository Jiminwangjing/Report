using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportPurchase
{
    [Table("rp_SummaryOutgoingPayment",Schema ="dbo")]
    public class SummaryOutgoingPayment
    {
        [Key]
        public int OutID { get; set; }
        public string Number { get;set; }
        public string PostingDate { get; set; }
        public string DocumentDate { get; set; }
        public double TotalAmountDue { get; set; }
        public string SysCurrency { get; set; }
    }
}
