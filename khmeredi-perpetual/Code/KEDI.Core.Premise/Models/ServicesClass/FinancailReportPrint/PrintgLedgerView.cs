using CKBS.Models.Services.ReportSale.dev;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ServicesClass.FinancailReportPrint
{
    public class PrintgLedgerView
    {
        public string ReceiptNo { get; set; }
        public string Pos { get; set; }
        public string CusName { get; set; }
        public string DoDate { get; set; }
        public GeneralLedgerHeader Header { get; set; }
        public GeneralLedgerFooter Footer { get; set; }
        public List<GeneralLedgerDetailItem> DetailItems { get; set; }
    }
    public class GeneralLedgerDetailItem
    {
        public string SeriesName { get; set; }
        public string RecieptNumber { get; set; }
        public string OfsetAcc { get; set; }
        public string Pos { get; set; }
        public string CusName { get; set; }
        public string AccName { get; set; }
        public string DebitCredit  { get; set; }
        public string Cubalance { get; set; }

    }
    public class GeneralLedgerHeader
    {
        public string CusName { get; set; }
        public string FromDate { get; set; }
        public string Todate { get; set; }

    }
    public class GeneralLedgerFooter
    {
        public string Pos { get; set; }
        public string CusName { get; set; }
    }
}

