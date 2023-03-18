using System.Collections.Generic;
using System.Linq;

namespace CKBS.Models.ServicesClass
{
    public class ProfitAndLossReportViewModel
    {
        public List<GLAccountBalanceSheetreportViewModel> Turnover { get; set; }
        public List<GLAccountBalanceSheetreportViewModel> CostOfSales { get; set; }
        public List<GLAccountBalanceSheetreportViewModel> OperatingCosts { get; set; }
        public List<GLAccountBalanceSheetreportViewModel> NonOperatingIncomeExpenditure { get; set; }
        public List<GLAccountBalanceSheetreportViewModel> TaxationExtraordinaryItems { get; set; }
        public bool AnnualReport { get; set; }
        public bool QuarterlyReport { get; set; }
        public bool MonthlyReport { get; set; }
        public bool PeriodicReport { get; set; }
        public bool ShowZeroAccount { get; set; }
        public List<IGrouping<int, GLAccountBalanceSheetreportViewModel>> ETurnovers { get; set; }
        public List<IGrouping<int, GLAccountBalanceSheetreportViewModel>> ECostofSales { get; set; }
        public List<IGrouping<int, GLAccountBalanceSheetreportViewModel>> EOperatingCosts { get; set; }
        public List<IGrouping<int, GLAccountBalanceSheetreportViewModel>> ENonOperatingIncomeExpenditure { get; set; }
        public List<IGrouping<int, GLAccountBalanceSheetreportViewModel>> ETaxationExtraordinaryItems { get; set; }
    }
}
