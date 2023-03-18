using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportDashboard
{
    [Table("db_SaleSummary", Schema = "dbo")]
    public class DashboardSaleSummary
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public double Total { get; set; }
        public string Currency { get; set; }
    }
}
