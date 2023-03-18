using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ServicesClass.FinancialReports
{
    public class GlSaleARViews
    {
        public string FromDate { get; set; }
        public string Todate { get; set; }
        public string PostingDate { get; set; }
        public string DocumentDate { get; set; }
        public string SeriesName { get; set; }
        public string DocType { get; set; }
        public string Receiptno { get; set; }
        public string TransNo { get; set; }
        public string Cumolativebalance { get; set; }
        public string DebitCredit { get; set; }
        public string AccountName { get; set; }
        public string Creator { get; set; }
    }
}
