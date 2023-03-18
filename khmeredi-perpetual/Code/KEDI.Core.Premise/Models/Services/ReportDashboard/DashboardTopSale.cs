using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportDashboard
{
    [Table("db_DashboardTopSale",Schema ="dbo")]
    public class DashboardTopSale
    {
        [Key]
        public int ID { get; set; }

        public string KhmerName { get; set; }
        public double InStock { get; set; }
        public string Uom { get; set; }
    }
}
