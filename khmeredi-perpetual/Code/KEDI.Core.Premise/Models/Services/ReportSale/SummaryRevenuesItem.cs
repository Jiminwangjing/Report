using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportSale
{
    [Table("rp_SummaryRevenuseItem",Schema ="dbo")]
    public class SummaryRevenuesItem
    {
        [Key]
        public int ItemID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Qty { get; set; }
        public double Cost { get; set; }
        public double Price { get; set; }
        public string Uom { get; set; }
        public double TotalCost { get; set; }
        public double TotalPrice { get; set; }
        public string Currency { get; set; }
        public double  Profit { get; set; }
        [NotMapped]
        public string DateFrom { get; set; }
        [NotMapped]
        public string DateTo { get; set; }
        [NotMapped]
        public string Branch { get; set; }

    }
}
