using KEDI.Core.Premise.Models.ServicesClass.FinancialReports;
using System.Collections.Generic;
using System.Linq;

namespace CKBS.Models.ServicesClass.FinancailReportPrint
{
    public class PirntSaleGrossProfitViewModel
    {
        public SummarySaleGrossProfit SummarySaleGrossProfit { get; set; }
        public HeaderSaleGrossProfit HeaderSaleGrossProfit { get; set; }
        public IEnumerable<IGrouping<int, SaleGrossProfitReport>> SaleGrossProfitReports { get; internal set; }
    }
    public class HeaderSaleGrossProfit
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public string UserName { get; set; }
    }
    public class SummarySaleGrossProfit
    {  
        public string TotalItem { get; set; }  
        public string TotalSaleGrossProfit { get; set; }
        public string TotalDiscount { get; set; }
        public string TotalSaleAfterDiscount { get; set; }
        public string TotalCost { get; set; }
        public string TotalPrice { get; set; }
        public string TotalSaleGrossProfitP { get; set; }
    }
}