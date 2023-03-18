using CKBS.Models.Services.HumanResources;
using System.Collections.Generic;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.Services.ChartOfAccounts;
using System;

namespace CKBS.Models.ServicesClass
{
    public class TrialBalanceViewModel
    {
        public int ID { get; set; }
        public int GLAccID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // Vender,Customer
        public int PriceListID { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string CurrencyName { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public bool Delete { get; set; } = false;
        public AccountBalance AccountBalance { get; set; }
        public List<AutoMobile> AutoMobile { get; set; }
        public PriceLists PriceList { get; set; }
        public DateTime PostingDate { get; set; }

        /// <summary>
        ///  Display Quarter
        /// </summary>
        public decimal CQ1 { get; set; }
        public decimal DQ1 { get; set; }
        public decimal BQ1 { get; set; }
        public decimal CQ2 { get; set; }
        public decimal DQ2 { get; set; }
        public decimal BQ2 { get; set; }
        public decimal CQ3 { get; set; }
        public decimal DQ3 { get; set; }
        public decimal BQ3 { get; set; }
        public decimal CQ4 { get; set; }
        public decimal DQ4 { get; set; }
        public decimal BQ4 { get; set; }

        /// <summary>
        /// Display Monthly
        /// </summary>
        public decimal CM1 { get; set; }
        public decimal DM1 { get; set; }
        public decimal BM1 { get; set; }
        public decimal BM2 { get; set; }
        public decimal DM2 { get; set; }
        public decimal CM2 { get; set; }
        public decimal CM3 { get; set; }
        public decimal DM3 { get; set; }
        public decimal BM3 { get; set; }
        public decimal CM4 { get; set; }
        public decimal DM4 { get; set; }
        public decimal BM4 { get; set; }
        public decimal CM5 { get; set; }
        public decimal DM5 { get; set; }
        public decimal BM5 { get; set; }
        public decimal CM6 { get; set; }
        public decimal DM6 { get; set; }
        public decimal BM6 { get; set; }
        public decimal CM7 { get; set; }
        public decimal DM7 { get; set; }
        public decimal BM7 { get; set; }
        public decimal CM8 { get; set; }
        public decimal DM8 { get; set; }
        public decimal BM8 { get; set; }
        public decimal BM9 { get; set; }
        public decimal DM9 { get; set; }
        public decimal CM9 { get; set; }
        public decimal CM10 { get; set; }
        public decimal DM10 { get; set; }
        public decimal BM10 { get; set; }
        public decimal CM11 { get; set; }
        public decimal DM11 { get; set; }
        public decimal BM11 { get; set; }
        public decimal CM12 { get; set; }
        public decimal DM12 { get; set; }
        public decimal BM12 { get; set; }
    }
    public class DisplayTBViewModel
    {
        public List<TrialBalanceViewModel> TrialBalanceViewModels { get; set; }
        public ProfitAndLossReportViewModelForTB ProfitAndLossReportViewModel { get; set; }
        public GLAccountBalanceSheetreportViewModelForTB Asset { get; set; }
        public GLAccountBalanceSheetreportViewModelForTB Liability { get; set; }
        public GLAccountBalanceSheetreportViewModelForTB CapitalandReserve { get; set; }
        public GLAccountBalanceSheetreportViewModelForTB Turnover { get; set; }
        public GLAccountBalanceSheetreportViewModelForTB CostOfSale { get; set; }
        public GLAccountBalanceSheetreportViewModelForTB OperatingCost { get; set; }
        public GLAccountBalanceSheetreportViewModelForTB NonOperatingIncomeExpenditure { get; set; }
        public GLAccountBalanceSheetreportViewModelForTB TaxationExtraordinaryItem { get; set; }
        public GLAccountBalanceSheetreportViewModelForTB Asset2 { get; set; }
        public GLAccountBalanceSheetreportViewModelForTB Liability2 { get; set; }
        public GLAccountBalanceSheetreportViewModelForTB CapitalandReserve2 { get; set; }
        public GLAccountBalanceSheetreportViewModelForTB Turnover2 { get; set; }
        public GLAccountBalanceSheetreportViewModelForTB CostOfSale2 { get; set; }
        public GLAccountBalanceSheetreportViewModelForTB OperatingCost2 { get; set; }
        public GLAccountBalanceSheetreportViewModelForTB NonOperatingIncomeExpenditure2 { get; set; }
        public GLAccountBalanceSheetreportViewModelForTB TaxationExtraordinaryItem2 { get; set; }
        public bool ShowZeroAcc { get; set; }
        public bool ShowGLAcc { get; set; }
        public bool AnnualReport { get; set; }
        public bool QuarterlyReport { get; set; }
        public bool MonthlyReport { get; set; }
        public bool PeriodicReport { get; set; }
        public bool Count { get; set; }
    }
}