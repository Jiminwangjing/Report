using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportPurchase.dev
{
    [Table("rp_SummaryTotalPurchase",Schema ="dbo")]
    public class SummaryPurchaseTotal
    {
        [Key]
        public int ID { get; set; }
        public double CountReceipt { get; set; }
        public double DiscountItem { get; set; }
        public double DiscountTotal { get; set; }
        public double BalanceDue { get; set; }
        public double AppliedAmount { get; set; }
        public double BalanceDueSSC { get; set; }
    } 
}
