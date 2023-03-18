using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportInventory
{
    [Table("rp_SummaryDetailTransferStock",Schema ="dbo")]
    public class SummaryDetaitTransferStock
    {
        [Key]
        public int ID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Qty { get; set; }
        public double Cost { get; set; }
        public string Uom { get; set; }
        public string Barcode { get; set; }
        public string ExpireDate { get; set; }
        public string Currency { get; set; }
    }
}
