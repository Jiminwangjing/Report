using System.Collections.Generic;
using System.Linq;

namespace CKBS.Models.ServicesClass.FinancailReportPrint
{
    public class PirntCashFlowForTreasuryViewModel
    {
        public List<IGrouping<string, CashFlowForTreasuryViewModel>> CashFlowForTreasuryViewModels { get; set; }
        public SummaryCashFlow Summary { get; set; }
        public HeaderCashFlow HeaderCashflow { get; set; }

    }
    public class HeaderCashFlow
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
    public class SummaryCashFlow
    {
        public string DebitTotal { get; set; }  
        public string CreditTotal { get; set; }  
        public string TotalSummary { get; set; }  
        public string BalanceTotal { get; set; }
    }
}