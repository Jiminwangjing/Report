using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportInventory
{
    [Table("rp_SummaryTansferStock",Schema ="dbo")]
    public class SummaryTransferStock
    {
        [Key]
        public int TranID { get; set; }
        public string Number { get; set; }
        public string PostingDate { get; set; }
        public string DocumentDate { get; set; }
        public string  Time { get; set; }
    }
}
