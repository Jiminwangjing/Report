using System.Collections.Generic;

namespace CKBS.Models.ServicesClass
{
    public class ProfitAndLossReportViewModelForTB
    {
        public List<GLAccountBalanceSheetreportViewModelForTB> Assets { get; set; }
        public List<GLAccountBalanceSheetreportViewModelForTB> Liabilities { get; set; }
        public List<GLAccountBalanceSheetreportViewModelForTB> CapitalandReserves { get; set; }
        public List<GLAccountBalanceSheetreportViewModelForTB> Turnover { get; set; }
        public List<GLAccountBalanceSheetreportViewModelForTB> CostOfSales { get; set; }
        public List<GLAccountBalanceSheetreportViewModelForTB> OperatingCosts { get; set; }
        public List<GLAccountBalanceSheetreportViewModelForTB> NonOperatingIncomeExpenditure { get; set; }
        public List<GLAccountBalanceSheetreportViewModelForTB> TaxationExtraordinaryItems { get; set; }
        public bool AnnualReport { get; set; }
        public bool QuarterlyReport { get; set; }
        public bool MonthlyReport { get; set; }
        public bool PeriodicReport { get; set; }
        public bool ShowZeroAccount { get; set; }

    }
}
