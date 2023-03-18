using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Administrator.SystemInitialization
{
    public class PostingPeriodViewModel
    {
        public int ID { get; set; }
        public string PeriodCode { get; set; }
        public string PeriodName { get; set; }
        public string SubPeriod { get; set; }
        public string NoOfPeroid { get; set; }
        public int PeroidIndID { get; set; }
        public string PeroidStatus { get; set; }
        public string PostingDateFrom { get; set; }
        public string PostingDateTo { get; set; }
        public string DueDateFrom { get; set; }
        public string DueDateTo { get; set; }
        public string DocuDateFrom { get; set; }
        public string DocuDateTo { get; set; }
        public string StartOfFiscalYear { get; set; }
        public string FiscalYear { get; set; }
    }
}
