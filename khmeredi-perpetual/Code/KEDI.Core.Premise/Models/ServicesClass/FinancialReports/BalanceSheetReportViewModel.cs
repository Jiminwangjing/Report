using CKBS.Models.Services.ChartOfAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass
{
    public class BalanceSheetReportViewModel
    {
        public List<GLAccountBalanceSheetreportViewModel> Assets { get; set; }
        public List<GLAccountBalanceSheetreportViewModel> Liabilities { get; set; }
        public List<GLAccountBalanceSheetreportViewModel> CapitalandReserves { get; set; }
        public bool AnnualReport { get; set; }
        public bool QuarterlyReport { get; set; }
        public bool MonthlyReport { get; set; }
        public bool PeriodicReport { get; set; }
        public bool ShowZeroAccount { get; set; }
        public decimal FormatNumber { get; set; }
        public List<IGrouping<int, GLAccountBalanceSheetreportViewModel>> EAssets { get; set; }
        public List<IGrouping<int, GLAccountBalanceSheetreportViewModel>> ELiabilities { get; set; }
        public List<IGrouping<int, GLAccountBalanceSheetreportViewModel>> ECapitalandReserves { get; set; }
    }
}
