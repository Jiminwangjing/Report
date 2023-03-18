using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportSaleAdmin
{
  
    public class SummarySaleAdmin
    {

        public int ID { get; set; }
        public double CountInvoice { get; set; }
        public double SoldAmount { get; set; }
        public double AppliedAmount { get; set; }
        public double DisCountItem { get; set; }
        public double DisCountTotal { get; set; }
        public double TotalVatRate { get; set; }
        public double Total { get; set; }
        public double TotalSys { get; set; }
    }
}
