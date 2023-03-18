using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportSale
{
    [Table("rp_summarysale", Schema = "dbo")]
    public class SummarySale
    {
        [Key]
        public Int32 OrderID { get; set; }
        public string Branch { get; set; }
        public string User { get; set; }
        public string Receipt { get; set; }
        public string DateIn {get;set;}
        public string TimeIn { get; set; }
        public string DateOut { get; set; }
        public string TimeOut { get; set; }
        public double GrandTotal { get; set; }
        public double GrandTotal_Sys { get; set; }
        public string SysCurrency { get; set; }
        public string LocalCurrency { get; set; }
        [NotMapped]
        public string DateFrom { get; set; }
        [NotMapped]
        public string DateTo { get; set; }


    }
}
