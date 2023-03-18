using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ReportClass
{
    [Table("ReportPurchaseRequst",Schema ="dbo")]
    public class ReportPurchaseRequset
    {
        [Key]
        public int ID { get; set; }
        public string InvoiceNo { get; set; } 
        public string Warehouse { get; set; }
        public string UserName { get; set; }
        public string SystemCurrency { get; set; }
        public double Balance_due { get; set; }
        public double ExchangeRate { get; set; }
        public string Status { get; set; }
        public string BranchName { get; set; } 
    }
}
