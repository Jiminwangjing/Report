using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportSale
{
    [Table("rp_DetailTopSale",Schema ="dbo")]
    public class DetailTopSaleQty
    {
        [Key]
        public int ID { get; set; }
        public string DateOut { get; set; }
        public string TimeOut { get; set; }
        public double Qty { get; set; }
        public string Uom { get; set; }
    }
}
