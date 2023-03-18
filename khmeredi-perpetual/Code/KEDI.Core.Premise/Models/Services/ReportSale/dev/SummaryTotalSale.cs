using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportSale.dev
{
    [Table("rp_SummaryTotalSale", Schema = "dbo")]
    public class SummaryTotalSale
    {
        [Key]
        public int ID { get; set; }
        public int CountReceipt { get; set; }
        public double SoldAmount { get; set; }
        public double DiscountItem { get; set; }
        public double DiscountTotal { get; set; }
        public double TaxValue { get; set; }
        public double GrandTotal { get; set; }
        public double GrandTotalSys { get; set; }
    }
}
