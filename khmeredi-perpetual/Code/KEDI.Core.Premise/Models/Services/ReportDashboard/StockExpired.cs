using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CKBS.Models.Services.ReportDashboard
{
    [Table("db_StockExpire",Schema ="dbo")]
    public class StockExpired
    {
        [Key]
        public int ID { get; set; }
        public string KhmerName { get; set; }
        public string ExpireDate { get; set; }
     }
}
