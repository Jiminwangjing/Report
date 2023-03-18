using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ReportClass
{   
    [Table("ReportPurchaseQuotation",Schema ="dbo")]
    public class ReportPurchaseQuotation 
    {   
        [Key]
        public int ID { get; set; }
        public string InvoiceNo { get; set; }
        public string BusinessName { get; set; }
        public string Warehouse { get; set; }
        public string UserName { get; set; }
        public string LocalCurrency { get; set; }
        public string SystemCurrency { get; set; }
        public double Balance_due { get; set; }
        public double Balance_due_sys { get; set; }
        public double ExchangeRate { get; set; }
        public string Status { get; set; }

    }
}
