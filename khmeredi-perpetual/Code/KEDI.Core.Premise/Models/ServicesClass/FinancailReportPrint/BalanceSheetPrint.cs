using CKBS.Models.Services.ChartOfAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass.FinancailReportPrint
{
    public class BalanceSheetPrint
    {
        public string Html { get; set; }
    }

    public class PrintBalance
    {
        public string Date { get; set; }
        public BalanceSheetReportViewModel Model { get; set; }
        public GLAccount Asset { get; set; }
        public GLAccount Liability { get; set; }
        public GLAccount CapReseve { get; set; }
        public string CompanyName { get; set; }
        public string CurrencyName { get; set; }

    }

    public class PrintProfitAddLoss 
    {
        public string DateFrom { get; set; } 
        public string DateTo { get; set; }
        public ProfitAndLossReportViewModel Model { get; set; }
        public GLAccount Turnover { get; set; }
        public GLAccount CostOfSales { get; set; }
        public GLAccount OperatingCosts { get; set; }
        public GLAccount NonOperatingIncomeExpenditure { get; set; }
        public GLAccount TaxationExtraordinaryItems { get; set; }
        public string CompanyName { get; set; }
        public string CurrencyName { get; set; }
        public string UserName{get;set;}

    }
    public class PrintTrialBalance
    {
        public string CompanyName { get; set; }
        public string CurrencyName { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string CType { get; set; }
        public string VType { get; set; }
        public string FromID { get; set; }
        public string ToID { get; set; }
        public List<TrialBalanceViewModel> Model { get; set; }
    }
}
