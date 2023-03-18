using System.Collections.Generic;
using System.Linq;
using System;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.ServicesClass;
using CKBS.Models.Services.ChartOfAccounts;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.ServicesClass.FinancialReports;
using KEDI.Core.Premise.Models.ServicesClass.FinancialReports;
using CKBS.Models.Services.POS;
using KEDI.Core.Premise.Models.Sale;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.POS.KVMS;
using CKBS.Models.Services.HumanResources;
using KEDI.Core.Premise.Repository;
using CKBS.Models.Services.Purchase;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using CKBS.Models.Services.Financials;
using CKBS.Models.Services.Account;

namespace CKBS.Models.Services.Responsitory
{
    public interface IFinancialReports
    {
        // Balance Sheet
        List<PostingPeriod> GetPostingPeriods();

        GLAccount GetGategories(string code, int sysCurID);
        List<GLAccountBalanceSheetreportViewModel> GetGLAccountsActiveOnly(string date, int comID, int sysCurID);
        BalanceSheetReportViewModel GetBalanceSheet(string date, int comID, int sysCurID, int TypeDisplayReport, bool showZeroAcc);

        // Profit and Loss
        GLAccount GetGategoriesPL(string code, int sysCurID);
        ProfitAndLossReportViewModel GetGLAccountsPL(string fromDate, string toDate, int comID, int sysCurID, int typeDisplay, bool showZeroAcc, List<MultiBrand> branch);
        List<GLAccountBalanceSheetreportViewModel> GetGLAccountsActiveOnlyPL(string fromDate, string toDate, int comID, int sysCurID, List<MultiBrand> branch);

        // Trial Balance
        List<BusinessPartner> GetBusPartnersAll(string type);
        dynamic GetBusPartners(string VType, string CType,
                int fromId, int toId, string fromDate,
                string toDate, int sysCurID, int comId,
                int DisplayType, bool showZeroAcc, bool showGla);

        // Cash Flow For Treasury //
        IEnumerable<CashFlowForTreasuryViewModel> GetCashFlowForTreasuryReport(string fromDate, string toDate, Company company);
        Task<IEnumerable<SaleGrossProfitReport>> GetSaleGrossProfitReportAsync(string fromDate, string toDate, int userId, string timeFrom, string timeTo, Company company);

        List<TransactionJournalReport> GetTransactionJournalReport(string valuejournal, string fromDate, string toDate, Company company);
        List<GlSaleARViews> GetGeneralLedgerReports(string dateFrom, string dateTo, Company company);
    }
    public class FinancialReportsRepository : IFinancialReports
    {
        private const string ASSETCODE = "100000000000000";
        private const string LIABILIITYCODE = "200000000000000";
        private const string CAPITLANDRESERVECODE = "300000000000000";
        private const string TURNOVERCODE = "400000000000000";
        private const string COSTOFSALESCODE = "500000000000000";
        private const string OPERATINGCOSTSCODE = "600000000000000";
        private const string NONOPERATINGINCOMEEXPENDITURECODE = "700000000000000";
        private const string TAXATIONEXTRAORDINARYITEMSCODE = "800000000000000";
        private readonly DataContext _context;
        private readonly UtilityModule _format;
        public FinancialReportsRepository(DataContext context, UtilityModule format)
        {
            _context = context;
            _format = format;
        }
        private decimal CovertToDoubles(string value)
        {
            decimal values;
            values = Convert.ToDecimal(value);
            return values;
        }
        public BalanceSheetReportViewModel GetBalanceSheet(string date, int comID, int sysCurID, int TypeDisplayReport, bool showZeroAcc)
        {
            List<GLAccountBalanceSheetreportViewModel> assets = new();
            List<GLAccountBalanceSheetreportViewModel> Liabilities = new();
            List<GLAccountBalanceSheetreportViewModel> CapitalandReserves = new();
            List<AccountBalance> accountBalancesAsset = new();
            List<AccountBalance> accountBalancesLiability = new();
            List<AccountBalance> accountBalancesCapitalAndReserve = new();
            bool AnnualReport = false;
            bool QuarterlyReport = false;
            bool MonthlyReport = false;
            bool PeriodicReport = false;
            var asset = _context.GLAccounts.FirstOrDefault(i => i.Code == ASSETCODE);
            var liability = _context.GLAccounts.FirstOrDefault(i => i.Code == LIABILIITYCODE);
            var capitalAndReserve = _context.GLAccounts.FirstOrDefault(i => i.Code == CAPITLANDRESERVECODE);
            AddFirstGLAcc(assets, asset, sysCurID);
            AddFirstGLAcc(Liabilities, liability, sysCurID);
            AddFirstGLAcc(CapitalandReserves, capitalAndReserve, sysCurID);
            var curName = _context.Currency.Find(sysCurID);
            var displayCurr = _context.DisplayCurrencies.FirstOrDefault(c => c.AltCurrencyID == curName.ID) ?? new DisplayCurrency();
            var disformat = _context.Displays.FirstOrDefault(c => c.DisplayCurrencyID == displayCurr.AltCurrencyID) ?? new Display();
            _ = DateTime.TryParse(date, out DateTime _toDate);
            var gLAccounts = (from glacc in _context.GLAccounts.Where(i => i.CompanyID == comID)
                              let _accbals = _context.AccountBalances.Where(i => i.PostingDate <= _toDate && i.GLAID == glacc.ID).ToList()
                              let _accbal = _context.AccountBalances.Where(i => i.PostingDate <= _toDate && i.GLAID == glacc.ID).DefaultIfEmpty().Last()
                              let balance = _context.AccountBalances.Where(i => i.PostingDate <= _toDate && i.GLAID == glacc.ID).Sum(i => i.Debit - i.Credit)
                              select new GLAccountBalanceSheetreportViewModel
                              {
                                  ID = glacc.ID,
                                  MainParentID = glacc.MainParentId,
                                  Code = glacc.Code,
                                  ExternalCode = glacc.ExternalCode,
                                  Name = glacc.Name,
                                  Level = glacc.Level,
                                  AccountType = glacc.AccountType,

                                  Balance = CovertToDoubles(_format.ToCurrency(balance, disformat.Amounts)),
                                  FormatNumber = disformat.Amounts,
                                  IsTitle = glacc.IsTitle,
                                  IsActive = glacc.IsActive,
                                  IsConfidential = glacc.IsConfidential,
                                  IsIndexed = glacc.IsIndexed,
                                  IsCashAccount = glacc.IsCashAccount,
                                  IsControlAccount = glacc.IsControlAccount,
                                  BlockManualPosting = glacc.BlockManualPosting,
                                  CashFlowRelavant = glacc.CashFlowRelavant,
                                  CurrencyID = glacc.CurrencyID,
                                  CurrencyName = curName.Description,
                                  ChangeLog = glacc.ChangeLog,
                                  ParentId = glacc.ParentId,
                                  CompanyID = glacc.CompanyID,
                                  AccountBalances = _accbals,
                                  AccountBalance = _accbal,

                              }).ToList();
            var accs = gLAccounts.ToList();
            if (showZeroAcc)
            {
                accs = gLAccounts.ToList();
            }
            else
            {
                accs = gLAccounts.Where(i =>
                    (!i.IsActive && i.AccountBalances.Count == 0) || (i.IsActive && i.AccountBalances.Count > 0))
                    .ToList();
            }

            foreach (var i in accs)
            {
                if (i.MainParentID == asset.ID)
                {
                    if (i.AccountBalance != null)
                    {
                        accountBalancesAsset.Add(i.AccountBalance);
                    }
                    i.AccountBalances = accountBalancesAsset;
                    assets.Add(i);
                }

                if (i.MainParentID == liability.ID)
                {
                    if (i.AccountBalance != null)
                    {
                        i.Balance *= -1;
                        accountBalancesLiability.Add(i.AccountBalance);
                    }
                    i.AccountBalances = accountBalancesLiability;
                    Liabilities.Add(i);
                }
                if (i.MainParentID == capitalAndReserve.ID)
                {
                    if (i.AccountBalance != null)
                    {
                        i.Balance *= -1;
                        accountBalancesCapitalAndReserve.Add(i.AccountBalance);
                    }
                    i.AccountBalances = accountBalancesCapitalAndReserve;
                    CapitalandReserves.Add(i);
                }
            }


            /// TypeDisplayReport :::: 0: AnnualReport, 1: QuarterReport, 2: MonthlyReport, 3: PeriodicReport
            if (TypeDisplayReport == 0)
            {
                AnnualReport = true;
                AddSumRecursively(assets.ToList(), false, true);
                SumLevel(assets);

                AddSumRecursively(CapitalandReserves.ToList(), true, true);
                SumLevel(CapitalandReserves, true);

                AddSumRecursively(Liabilities.ToList(), true, true);
                SumLevel(Liabilities, true);
            }
            if (TypeDisplayReport == 1)
            {
                QuarterlyReport = true;
                QuarterDisplayReport(date, assets, false, true);
                QuarterDisplayReport(date, CapitalandReserves, true, true);
                QuarterDisplayReport(date, Liabilities, true, true);
            }
            if (TypeDisplayReport == 2)
            {
                MonthlyReport = true;
                MonthlyDisplayReport(date, assets);
                MonthlyDisplayReport(date, CapitalandReserves, true);
                MonthlyDisplayReport(date, Liabilities, true);
            }
            if (TypeDisplayReport == 3)
            {
                PeriodicReport = true;
            }
            var balanceSheet = new BalanceSheetReportViewModel
            {
                FormatNumber = disformat.Amounts,
                AnnualReport = AnnualReport,
                QuarterlyReport = QuarterlyReport,
                MonthlyReport = MonthlyReport,
                PeriodicReport = PeriodicReport,
                Assets = assets,
                CapitalandReserves = CapitalandReserves,
                Liabilities = Liabilities,
                ShowZeroAccount = showZeroAcc,
                EAssets = assets.GroupBy(i => i.Level).ToList(),
                ELiabilities = Liabilities.GroupBy(i => i.Level).ToList(),
                ECapitalandReserves = CapitalandReserves.GroupBy(i => i.Level).ToList(),
            };
            return balanceSheet;
        }
        public List<GLAccountBalanceSheetreportViewModel> GetGLAccountsActiveOnly(string toDate, int comID, int sysCurID)
        {
            var curName = _context.Currency.Find(sysCurID);
            var displayCurr = _context.DisplayCurrencies.FirstOrDefault(c => c.AltCurrencyID == curName.ID) ?? new DisplayCurrency();
            var disformat = _context.Displays.FirstOrDefault(c => c.DisplayCurrencyID == displayCurr.AltCurrencyID) ?? new Display();
            _ = DateTime.TryParse(toDate, out DateTime _toDate);
            var gLAccounts = (from glacc in _context.GLAccounts.Where(i => i.CompanyID == comID && i.IsActive)
                              let _accbal = _context.AccountBalances.LastOrDefault(i => i.PostingDate <= _toDate && i.GLAID == glacc.ID)
                              select new GLAccountBalanceSheetreportViewModel
                              {
                                  ID = glacc.ID,
                                  Code = glacc.Code,
                                  ExternalCode = glacc.ExternalCode,
                                  Name = glacc.Name,
                                  Level = glacc.Level,
                                  AccountType = glacc.AccountType,
                                  Balance = glacc.Balance,
                                  IsTitle = glacc.IsTitle,
                                  IsActive = glacc.IsActive,
                                  IsConfidential = glacc.IsConfidential,
                                  IsIndexed = glacc.IsIndexed,
                                  IsCashAccount = glacc.IsCashAccount,
                                  IsControlAccount = glacc.IsControlAccount,
                                  BlockManualPosting = glacc.BlockManualPosting,
                                  CashFlowRelavant = glacc.CashFlowRelavant,
                                  CurrencyID = glacc.CurrencyID,
                                  CurrencyName = curName.Description,
                                  ChangeLog = glacc.ChangeLog,
                                  ParentId = glacc.ParentId,
                                  CompanyID = glacc.CompanyID,
                                  AccountBalance = _accbal,
                                  FormatNumber = disformat.Amounts,
                              });
            return gLAccounts.Where(i => i.AccountBalance != null).ToList();
        }
        public ProfitAndLossReportViewModel GetGLAccountsPL(string fromDate, string toDate, int comID, int sysCurID, int typeDisplay, bool showZeroAcc, List<MultiBrand> branch)
        {
            List<GLAccountBalanceSheetreportViewModel> Turnovers = new();
            List<GLAccountBalanceSheetreportViewModel> CostOfSales = new();
            List<GLAccountBalanceSheetreportViewModel> OperatingCosts = new();
            List<GLAccountBalanceSheetreportViewModel> NonOperatingIncomeExpenditures = new();
            List<GLAccountBalanceSheetreportViewModel> TaxationExtraordinaryItems = new();

            List<AccountBalance> accountBalancesTurnovers = new();
            List<AccountBalance> accountBalancesCostofSales = new();
            List<AccountBalance> accountBalancesNonOperatingIncomeExpenditures = new();
            List<AccountBalance> accountBalancesOperatingCosts = new();
            List<AccountBalance> accountBalancesTaxationExtraordinaryItems = new();

            bool AnnualReport = false;
            bool QuarterlyReport = false;
            bool MonthlyReport = false;
            bool PeriodicReport = false;
            var turnover = _context.GLAccounts.FirstOrDefault(i => i.Code == TURNOVERCODE);
            var costOfSale = _context.GLAccounts.FirstOrDefault(i => i.Code == COSTOFSALESCODE);
            var operatingCost = _context.GLAccounts.FirstOrDefault(i => i.Code == OPERATINGCOSTSCODE);
            var nonOIE = _context.GLAccounts.FirstOrDefault(i => i.Code == NONOPERATINGINCOMEEXPENDITURECODE);
            var taxEI = _context.GLAccounts.FirstOrDefault(i => i.Code == TAXATIONEXTRAORDINARYITEMSCODE);



            var curName = _context.Currency.Find(sysCurID);
            var displayCurr = _context.DisplayCurrencies.FirstOrDefault(c => c.AltCurrencyID == curName.ID) ?? new DisplayCurrency();
            var disformat = _context.Displays.FirstOrDefault(c => c.DisplayCurrencyID == displayCurr.AltCurrencyID) ?? new Display();
             var JournalEntries = branch.Count!=0? _context.JournalEntries.Where(s=>branch.Any(x=> x.ID== s.BranchID)):_context.JournalEntries;
            _ = DateTime.TryParse(toDate, out DateTime _toDate);
            _ = DateTime.TryParse(fromDate, out DateTime _fromDate);
            var listacAB = (from accb in _context.AccountBalances.Where(i => i.PostingDate <= _toDate && i.PostingDate >= _fromDate)
                            join je in JournalEntries on accb.JEID equals je.ID
                            join user in _context.UserAccounts on je.Creator equals user.ID
                          //  join bch in _context.Branches.Where(s => s.ID == branch) on user.BranchID equals bch.ID
                            select new AccountBalance
                            {
                                ID = accb.ID,
                                PostingDate = accb.PostingDate,
                                Origin = accb.Origin,
                                OriginNo = accb.OriginNo,
                                OffsetAccount = accb.OffsetAccount,
                                Details = accb.Details,
                                CumulativeBalance = accb.CumulativeBalance,
                                Debit = accb.Debit,
                                Credit = accb.Credit,
                                LocalSetRate = accb.LocalSetRate,
                                GLAID = accb.GLAID,
                                BPAcctID = accb.BPAcctID,
                                Creator = accb.Creator,
                                JEID = accb.JEID,
                            }).ToList();
            var gLAccounts = (from glacc in _context.GLAccounts.Where(i => i.CompanyID == comID)
                              let _accbals = branch.Count == 0 ? _context.AccountBalances.Where(i => i.PostingDate <= _toDate && i.PostingDate >= _fromDate && i.GLAID == glacc.ID).ToList() : listacAB.Where(s => s.GLAID == glacc.ID).ToList()
                              let balance = branch.Count == 0 ? _context.AccountBalances.Where(i => i.PostingDate <= _toDate && i.PostingDate >= _fromDate && i.GLAID == glacc.ID).Sum(i => i.Debit - i.Credit) : listacAB.Where(s => s.GLAID == glacc.ID).Sum(i => i.Debit - i.Credit)
                              select new GLAccountBalanceSheetreportViewModel
                              {
                                  ID = glacc.ID,
                                  Code = glacc.Code,
                                  ExternalCode = glacc.ExternalCode,
                                  Name = glacc.Name,
                                  Level = glacc.Level,
                                  AccountType = glacc.AccountType,
                                  Balance = balance,
                                  IsTitle = glacc.IsTitle,
                                  IsActive = glacc.IsActive,
                                  IsConfidential = glacc.IsConfidential,
                                  IsIndexed = glacc.IsIndexed,
                                  IsCashAccount = glacc.IsCashAccount,
                                  IsControlAccount = glacc.IsControlAccount,
                                  BlockManualPosting = glacc.BlockManualPosting,
                                  CashFlowRelavant = glacc.CashFlowRelavant,
                                  CurrencyID = glacc.CurrencyID,
                                  CurrencyName = curName.Description,
                                  ChangeLog = glacc.ChangeLog,
                                  ParentId = glacc.ParentId,
                                  CompanyID = glacc.CompanyID,
                                  AccountBalances = _accbals,
                                  FormatNumber = disformat.Amounts,
                                  //AccountBalance = _accbal,
                                  MainParentID = glacc.MainParentId
                              }).ToList();

            var accs = gLAccounts.ToList();
            if (showZeroAcc)
            {
                accs = gLAccounts.ToList();
            }
            else
            {
                accs = gLAccounts
                    .Where(i =>
                        (!i.IsActive && i.AccountBalances.Count == 0) ||
                        (i.IsActive && i.AccountBalances.Count > 0))
                    .ToList();
            }

            foreach (var i in accs)
            {
                if (i.MainParentID == turnover.ID)
                {
                    if (i.AccountBalances.Count > 0)
                    {
                        var parent = accs.FirstOrDefault(p => i.ParentId == p.ParentId) ?? new GLAccountBalanceSheetreportViewModel();
                        i.AccountBalances.ForEach(acc =>
                        {
                            acc.ParentID = parent.ID;
                            accountBalancesTurnovers.Add(acc);
                        });
                    };
                    i.AccountBalances = accountBalancesTurnovers;
                    Turnovers.Add(i);

                }

                if (i.MainParentID == costOfSale.ID)
                {
                    if (i.AccountBalances.Count > 0)
                    {
                        var parent = accs.FirstOrDefault(p => i.ParentId == p.ParentId) ?? new GLAccountBalanceSheetreportViewModel();
                        i.AccountBalances.ForEach(acc =>
                        {
                            acc.ParentID = parent.ID;
                            accountBalancesCostofSales.Add(acc);
                        });
                    }
                    i.AccountBalances = accountBalancesCostofSales;
                    CostOfSales.Add(i);
                }
                if (i.MainParentID == operatingCost.ID)
                {
                    if (i.AccountBalances.Count > 0)
                    {
                        var parent = accs.FirstOrDefault(p => i.ParentId == p.ParentId) ?? new GLAccountBalanceSheetreportViewModel();
                        i.AccountBalances.ForEach(acc =>
                        {
                            acc.ParentID = parent.ID;
                            accountBalancesOperatingCosts.Add(acc);
                        });
                    }
                    i.AccountBalances = accountBalancesOperatingCosts;
                    OperatingCosts.Add(i);
                }
                if (i.MainParentID == nonOIE.ID)
                {
                    if (i.AccountBalances.Count > 0)
                    {
                        var parent = accs.FirstOrDefault(p => i.ParentId == p.ParentId) ?? new GLAccountBalanceSheetreportViewModel();
                        i.AccountBalances.ForEach(acc =>
                        {
                            acc.ParentID = parent.ID;
                            accountBalancesNonOperatingIncomeExpenditures.Add(acc);
                        });
                    }
                    i.AccountBalances = accountBalancesNonOperatingIncomeExpenditures;
                    NonOperatingIncomeExpenditures.Add(i);
                }
                if (i.MainParentID == taxEI.ID)
                {
                    if (i.AccountBalances.Count > 0)
                    {
                        var parent = accs.FirstOrDefault(p => i.ParentId == p.ParentId) ?? new GLAccountBalanceSheetreportViewModel();
                        i.AccountBalances.ForEach(acc =>
                        {
                            acc.ParentID = parent.ID;
                            accountBalancesTaxationExtraordinaryItems.Add(acc);
                        });
                    }
                    i.AccountBalances = accountBalancesTaxationExtraordinaryItems;
                    TaxationExtraordinaryItems.Add(i);
                }
            }

            AddFirstGLAcc(Turnovers, turnover, sysCurID, accountBalancesTurnovers, true);
            AddFirstGLAcc(CostOfSales, costOfSale, sysCurID, accountBalancesCostofSales, true);
            AddFirstGLAcc(OperatingCosts, operatingCost, sysCurID, accountBalancesOperatingCosts, true);
            AddFirstGLAcc(NonOperatingIncomeExpenditures, nonOIE, sysCurID, accountBalancesNonOperatingIncomeExpenditures, false, true);
            AddFirstGLAcc(TaxationExtraordinaryItems, taxEI, sysCurID, accountBalancesTaxationExtraordinaryItems, true);


            /// TypeDisplayReport :::: 0: AnnualReport, 1: QuarterReport, 2: MonthlyReport, 3: PeriodicReport
            if (typeDisplay == 0)
            {
                AnnualReport = true;
                AddSumRecursively(Turnovers.ToList(), true);
                SumLevelPL(Turnovers, true);

                AddSumRecursively(OperatingCosts.ToList(), true);
                SumLevelPL(OperatingCosts, true);

                AddSumRecursively(CostOfSales.ToList(), true);
                SumLevelPL(CostOfSales, true);

                AddSumRecursively(NonOperatingIncomeExpenditures.ToList(), false);
                SumLevelPL(NonOperatingIncomeExpenditures, false);

                AddSumRecursively(TaxationExtraordinaryItems.ToList(), true);
                SumLevelPL(TaxationExtraordinaryItems, true);
            }
            if (typeDisplay == 1)
            {
                QuarterlyReport = true;
                QuarterDisplayReportPL(toDate, Turnovers, true);
                QuarterDisplayReportPL(toDate, OperatingCosts, true);
                QuarterDisplayReportPL(toDate, CostOfSales, true);
                QuarterDisplayReportPL(toDate, NonOperatingIncomeExpenditures, true);
                QuarterDisplayReportPL(toDate, TaxationExtraordinaryItems, true);
            }
            if (typeDisplay == 2)
            {
                MonthlyReport = true;
                MonthlyDisplayReportPL(toDate, Turnovers, true);
                MonthlyDisplayReportPL(toDate, OperatingCosts, true);
                MonthlyDisplayReportPL(toDate, CostOfSales, true);
                MonthlyDisplayReportPL(toDate, NonOperatingIncomeExpenditures, true);
                MonthlyDisplayReportPL(toDate, TaxationExtraordinaryItems, true);
            }
            if (typeDisplay == 3)
            {
                PeriodicReport = true;
            }
            var profitAndLost = new ProfitAndLossReportViewModel
            {
                AnnualReport = AnnualReport,
                QuarterlyReport = QuarterlyReport,
                MonthlyReport = MonthlyReport,
                PeriodicReport = PeriodicReport,
                ShowZeroAccount = showZeroAcc,
                Turnover = Turnovers,
                CostOfSales = CostOfSales,
                OperatingCosts = OperatingCosts,
                NonOperatingIncomeExpenditure = NonOperatingIncomeExpenditures,
                TaxationExtraordinaryItems = TaxationExtraordinaryItems,
                ETurnovers = Turnovers.GroupBy(i => i.Level).ToList(),
                ECostofSales = CostOfSales.GroupBy(i => i.Level).ToList(),
                EOperatingCosts = OperatingCosts.GroupBy(i => i.Level).ToList(),
                ENonOperatingIncomeExpenditure = NonOperatingIncomeExpenditures.GroupBy(i => i.Level).ToList(),
                ETaxationExtraordinaryItems = TaxationExtraordinaryItems.GroupBy(i => i.Level).ToList(),

            };
            return profitAndLost;
        }
        public List<GLAccountBalanceSheetreportViewModel> GetGLAccountsActiveOnlyPL(string fromDate, string toDate, int comID, int sysCurID, List<MultiBrand> branch)
        {
            var curName = _context.Currency.Find(sysCurID);
            var displayCurr = _context.DisplayCurrencies.FirstOrDefault(c => c.AltCurrencyID == curName.ID) ?? new DisplayCurrency();
            var disformat = _context.Displays.FirstOrDefault(c => c.DisplayCurrencyID == displayCurr.AltCurrencyID) ?? new Display();
             var JournalEntries  = branch.Count!=0?_context.JournalEntries.Where(s=> branch.Any(x=> x.ID==s.BranchID)):_context.JournalEntries;
            _ = DateTime.TryParse(toDate, out DateTime _toDate);
            _ = DateTime.TryParse(fromDate, out DateTime _fromDate);
            var listacAB = (from accb in _context.AccountBalances.Where(i => i.PostingDate <= _toDate && i.PostingDate >= _fromDate)
                            join je in JournalEntries on accb.JEID equals je.ID
                            join user in _context.UserAccounts on je.Creator equals user.ID
                          //  join bch in _context.Branches.Where(s => s.ID == branch) on user.BranchID equals bch.ID
                            select new AccountBalance
                            {
                                ID = accb.ID,
                                PostingDate = accb.PostingDate,
                                Origin = accb.Origin,
                                OriginNo = accb.OriginNo,
                                OffsetAccount = accb.OffsetAccount,
                                Details = accb.Details,
                                CumulativeBalance = accb.CumulativeBalance,
                                Debit = accb.Debit,
                                Credit = accb.Credit,
                                LocalSetRate = accb.LocalSetRate,
                                GLAID = accb.GLAID,
                                BPAcctID = accb.BPAcctID,
                                Creator = accb.Creator,
                                JEID = accb.JEID,
                            }).ToList();
            var gLAccounts = (from glacc in _context.GLAccounts.Where(i => i.CompanyID == comID && i.IsActive)
                               let _accbal = branch.Count == 0 ? _context.AccountBalances.LastOrDefault(i => i.PostingDate <= _toDate && i.PostingDate >= _fromDate && i.GLAID == glacc.ID) : listacAB.LastOrDefault(s => s.GLAID == glacc.ID)
                              select new GLAccountBalanceSheetreportViewModel
                              {
                                  ID = glacc.ID,
                                  Code = glacc.Code,
                                  ExternalCode = glacc.ExternalCode,
                                  Name = glacc.Name,
                                  Level = glacc.Level,
                                  AccountType = glacc.AccountType,
                                  Balance = glacc.Balance,
                                  IsTitle = glacc.IsTitle,
                                  IsActive = glacc.IsActive,
                                  IsConfidential = glacc.IsConfidential,
                                  IsIndexed = glacc.IsIndexed,
                                  IsCashAccount = glacc.IsCashAccount,
                                  IsControlAccount = glacc.IsControlAccount,
                                  BlockManualPosting = glacc.BlockManualPosting,
                                  CashFlowRelavant = glacc.CashFlowRelavant,
                                  CurrencyID = glacc.CurrencyID,
                                  CurrencyName = curName.Description,
                                  ChangeLog = glacc.ChangeLog,
                                  ParentId = glacc.ParentId,
                                  CompanyID = glacc.CompanyID,
                                  AccountBalance = _accbal,
                                  FormatNumber = disformat.Amounts,
                              }).ToList();
            return gLAccounts.Where(i => i.AccountBalance != null).ToList();
        }
        //public GLAccount GetGategories(string code)
        //{
        //    var glAccs = _context.GLAccounts.FirstOrDefault(i => i.Code == code);
        //    return glAccs;
        //}
        public GLAccount GetGategories(string code, int sysCurID)
        {
            var curName = _context.Currency.Find(sysCurID);
            var displayCurr = _context.DisplayCurrencies.FirstOrDefault(c => c.AltCurrencyID == curName.ID) ?? new DisplayCurrency();
            var disformat = _context.Displays.FirstOrDefault(c => c.DisplayCurrencyID == displayCurr.AltCurrencyID) ?? new Display();
            // var glAccs = _context.GLAccounts.FirstOrDefault(i => i.Code == code) ?? new GLAccount();
            var glAccs = (from gla in _context.GLAccounts.Where(i => i.Code == code)
                          select new GLAccount
                          {
                              AccountType = gla.AccountType,
                              Balance = gla.Balance,
                              BlockManualPosting = gla.BlockManualPosting,
                              CashFlowRelavant = gla.CashFlowRelavant,
                              ChangeLog = gla.ChangeLog,
                              Code = gla.Code,
                              CompanyID = gla.CompanyID,
                              CurrencyID = gla.CurrencyID,
                              CurrencyName = gla.CurrencyName,
                              Edit = gla.Edit,
                              ExternalCode = gla.ExternalCode,
                              ID = gla.ID,
                              IsActive = gla.IsActive,
                              IsCashAccount = gla.IsCashAccount,
                              IsConfidential = gla.IsConfidential,
                              IsControlAccount = gla.IsControlAccount,
                              IsIndexed = gla.IsIndexed,
                              IsTitle = gla.IsTitle,
                              Level = gla.Level,
                              MainParentId = gla.MainParentId,
                              Name = gla.Name,

                              ParentId = gla.ParentId,
                              TotalBalance = gla.TotalBalance,
                              FormatNumber = disformat.Amounts,

                          }).FirstOrDefault();
            return glAccs;
        }
        public GLAccount GetGategoriesPL(string code, int sysCurID)
        {
            var curName = _context.Currency.Find(sysCurID);
            var displayCurr = _context.DisplayCurrencies.FirstOrDefault(c => c.AltCurrencyID == curName.ID) ?? new DisplayCurrency();
            var disformat = _context.Displays.FirstOrDefault(c => c.DisplayCurrencyID == displayCurr.AltCurrencyID) ?? new Display();
            var glAccs = (from gla in _context.GLAccounts.Where(i => i.Code == code)
                          select new GLAccount
                          {
                              AccountType = gla.AccountType,
                              Balance = gla.Balance,
                              BlockManualPosting = gla.BlockManualPosting,
                              CashFlowRelavant = gla.CashFlowRelavant,
                              ChangeLog = gla.ChangeLog,
                              Code = gla.Code,
                              CompanyID = gla.CompanyID,
                              CurrencyID = gla.CurrencyID,
                              CurrencyName = curName.Description,
                              Edit = gla.Edit,
                              ExternalCode = gla.ExternalCode,
                              ID = gla.ID,
                              IsActive = gla.IsActive,
                              IsCashAccount = gla.IsCashAccount,
                              IsConfidential = gla.IsConfidential,
                              IsControlAccount = gla.IsControlAccount,
                              IsIndexed = gla.IsIndexed,
                              IsTitle = gla.IsTitle,
                              Level = gla.Level,
                              MainParentId = gla.MainParentId,
                              Name = gla.Name,

                              ParentId = gla.ParentId,
                              TotalBalance = gla.TotalBalance,
                              FormatNumber = disformat.Amounts,

                          }).FirstOrDefault() ?? new GLAccount();
            return glAccs;
        }
        public List<PostingPeriod> GetPostingPeriods()
        {
            var postingPeriods = _context.PostingPeriods.ToList();
            return postingPeriods;
        }
        public List<BusinessPartner> GetBusPartnersAll(string type)
        {
            var busPartners = _context.BusinessPartners.Where(i => !i.Delete).ToList();
            var bp = busPartners;
            if (type == "All" || type == null) return bp;
            else bp = busPartners.Where(i => i.Type == type).ToList();
            return bp;

        }
        public dynamic GetBusPartners(string VType, string CType, int fromId, int toId, string fromDate, string toDate, int sysCurID, int comId, int DisplayType, bool showZeroAcc, bool showGla)
        {
            List<TrialBalanceViewModel> trialBalanceViewModels = new();
            _ = DateTime.TryParse(toDate, out DateTime _toDate);
            var _fromdate = fromDate ?? $"{_toDate.Year}-1-1";
            _ = DateTime.TryParse(_fromdate, out DateTime _fromDate);
            var annualReport = false;
            var quaterlyReport = false;
            var monthlyReport = false;
            var currencName = _context.Currency.Find(sysCurID);
            var displayCurr = _context.DisplayCurrencies.FirstOrDefault(c => c.AltCurrencyID == currencName.ID) ?? new DisplayCurrency();
            var disformat = _context.Displays.FirstOrDefault(c => c.DisplayCurrencyID == displayCurr.AltCurrencyID) ?? new Display();
            var busPart = from bpn in _context.BusinessPartners.Where(i => !i.Delete)
                          select new TrialBalanceViewModel
                          {
                              ID = bpn.ID,
                              GLAccID = bpn.GLAccID,
                              Code = bpn.Code,
                              Name = bpn.Name,
                              Type = bpn.Type, // Vender,Customer
                              PriceListID = bpn.PriceListID,
                              Phone = bpn.Phone,
                              Email = bpn.Email,
                              Address = bpn.Address,
                              Delete = bpn.Delete,
                              PriceList = bpn.PriceList,
                              CurrencyName = currencName.Description,
                              Debit = 0,
                              Credit = 0,
                              Balance = 0,
                          }
                     ;
            var bps = from jd in _context.JournalEntryDetails
                      .Where(i => i.Type == Financials.Type.BPCode)
                      join ab in _context.AccountBalances
                      .Where(i => i.PostingDate >= _fromDate && i.PostingDate <= _toDate)
                      on jd.ItemID equals ab.GLAID
                      join bp in _context.BusinessPartners
                      .Where(i => !i.Delete) on jd.BPAcctID equals bp.ID
                      select new TrialBalanceViewModel
                      {
                          ID = bp.ID,
                          GLAccID = bp.GLAccID,
                          Code = bp.Code,
                          Name = bp.Name,
                          Type = bp.Type, // Vender,Customer
                          PriceListID = bp.PriceListID,
                          Phone = bp.Phone,
                          Email = bp.Email,
                          Address = bp.Address,
                          Delete = bp.Delete,
                          PriceList = bp.PriceList,
                          PostingDate = ab.PostingDate,
                          CurrencyName = currencName.Description,
                          Debit = ab.Debit,
                          Credit = ab.Credit,
                      }
                    ;
            var busPartnersFilter = bps.ToList();
            if (VType == "all" && CType == "all" && fromId != 0 && toId != 0)
                busPartnersFilter = bps.Where(i => i.ID >= fromId && i.ID <= toId).ToList();

            if (VType != "all" && CType != "all" && fromId == 0 && toId == 0)
                busPartnersFilter = bps.Where(i => i.Type == VType || i.Type == CType).ToList();

            if (VType == "all" && CType == "all" && fromId == 0 && toId == 0)
                busPartnersFilter = bps.ToList();

            if (VType != "all" && CType != "all" && fromId != 0 && toId != 0)
                busPartnersFilter = bps.Where(i => i.Type == VType || i.Type == CType && i.ID >= fromId && i.ID <= toId).ToList();

            if (VType == "all" && CType != "all" && fromId != 0 && toId == 0)
                busPartnersFilter = bps.Where(i => i.Type == CType && i.ID >= fromId).ToList();

            if (VType != "all" && CType == "all" && fromId == 0 && toId != 0)
                busPartnersFilter = bps.Where(i => i.Type == VType && i.ID <= toId).ToList();


            var result = busPartnersFilter.GroupBy(i => i.ID).ToList();
            if (result.Count <= 0)
            {
                annualReport = !annualReport;
                quaterlyReport = !quaterlyReport;
                monthlyReport = !monthlyReport;
                showGla = true;
            }
            foreach (var i in result)
            {
                foreach (var j in busPart)
                {
                    if (i.Key == j.ID)
                    {
                        var k = i.FirstOrDefault();
                        j.Debit = i.Sum(i => i.Debit);
                        j.Credit = i.Sum(i => i.Credit);
                        j.Balance = i.Sum(i => i.Balance);
                        /// DisPlay Type :: 0: AnnualReport, 1: QuarterlyReport, 2: MonthlyReport, 3: PeriodicReport
                        if (DisplayType == 0)
                        {
                            annualReport = true;
                        }
                        if (DisplayType == 1)
                        {
                            quaterlyReport = true;
                            TBQuarterDisplayReport(toDate, i.ToList());
                        }
                        if (DisplayType == 2)
                        {
                            monthlyReport = true;
                            TBMonthlyDisplayReport(toDate, busPartnersFilter);
                        }
                        if (DisplayType == 3)
                        {
                            //TBMonthlyDisplayReport(toDate, busPartnersFilter);
                        }
                        /// Month ///
                        AddMonthTB(j, k);

                        /// Quarter ///
                        AddQuarterTB(j, k);
                        trialBalanceViewModels.Add(j);
                    }
                }

            }

            var accFromPNL = GetGLAccountsPLForTB(fromDate, toDate, comId, sysCurID, DisplayType, showZeroAcc);
            DisplayTBViewModel displayTBViewModel = new()
            {
                TrialBalanceViewModels = trialBalanceViewModels,
                ProfitAndLossReportViewModel = result.Count > 0 ? accFromPNL : new ProfitAndLossReportViewModelForTB(),
                AnnualReport = annualReport,
                MonthlyReport = monthlyReport,
                QuarterlyReport = quaterlyReport,
                Asset = accFromPNL.Assets.FirstOrDefault(i => i.Level == 1),
                Liability = accFromPNL.Liabilities.FirstOrDefault(i => i.Level == 1),
                CapitalandReserve = accFromPNL.CapitalandReserves.FirstOrDefault(i => i.Level == 1),
                CostOfSale = accFromPNL.CostOfSales.FirstOrDefault(i => i.Level == 1),
                NonOperatingIncomeExpenditure = accFromPNL.NonOperatingIncomeExpenditure.FirstOrDefault(i => i.Level == 1),
                OperatingCost = accFromPNL.OperatingCosts.FirstOrDefault(i => i.Level == 1),
                Turnover = accFromPNL.Turnover.FirstOrDefault(i => i.Level == 1),
                TaxationExtraordinaryItem = accFromPNL.TaxationExtraordinaryItems.FirstOrDefault(i => i.Level == 1),
                ShowGLAcc = showGla,
                Count = result.Count > 0,
                Asset2 = accFromPNL.Assets.FirstOrDefault(i => i.Level == 2) ?? new GLAccountBalanceSheetreportViewModelForTB(),
                Liability2 = accFromPNL.Liabilities.FirstOrDefault(i => i.Level == 2) ?? new GLAccountBalanceSheetreportViewModelForTB(),
                CapitalandReserve2 = accFromPNL.CapitalandReserves.FirstOrDefault(i => i.Level == 2) ?? new GLAccountBalanceSheetreportViewModelForTB(),
                CostOfSale2 = accFromPNL.CostOfSales.FirstOrDefault(i => i.Level == 2) ?? new GLAccountBalanceSheetreportViewModelForTB(),
                NonOperatingIncomeExpenditure2 = accFromPNL.NonOperatingIncomeExpenditure.FirstOrDefault(i => i.Level == 2) ?? new GLAccountBalanceSheetreportViewModelForTB(),
                OperatingCost2 = accFromPNL.OperatingCosts.FirstOrDefault(i => i.Level == 2) ?? new GLAccountBalanceSheetreportViewModelForTB(),
                Turnover2 = accFromPNL.Turnover.FirstOrDefault(i => i.Level == 2) ?? new GLAccountBalanceSheetreportViewModelForTB(),
                TaxationExtraordinaryItem2 = accFromPNL.TaxationExtraordinaryItems.FirstOrDefault(i => i.Level == 2) ?? new GLAccountBalanceSheetreportViewModelForTB(),
            };
            return displayTBViewModel;
        }
        private bool AddFirstGLAcc(List<GLAccountBalanceSheetreportViewModel> listGl, dynamic Gl, int sysCurID)
        {
            var curName = _context.Currency.Find(sysCurID);
            var displayCurr = _context.DisplayCurrencies.FirstOrDefault(c => c.AltCurrencyID == curName.ID) ?? new DisplayCurrency();
            var disformat = _context.Displays.FirstOrDefault(c => c.DisplayCurrencyID == displayCurr.AltCurrencyID) ?? new Display();
            listGl.Add(new GLAccountBalanceSheetreportViewModel
            {
                ID = Gl.ID,
                MainParentID = Gl.MainParentId,
                Code = Gl.Code,
                ExternalCode = Gl.ExternalCode,
                Name = Gl.Name,
                Level = Gl.Level,
                AccountType = Gl.AccountType,
                Balance = Gl.Balance,
                IsTitle = Gl.IsTitle,
                IsActive = Gl.IsActive,
                IsConfidential = Gl.IsConfidential,
                IsIndexed = Gl.IsIndexed,
                IsCashAccount = Gl.IsCashAccount,
                IsControlAccount = Gl.IsControlAccount,
                BlockManualPosting = Gl.BlockManualPosting,
                CashFlowRelavant = Gl.CashFlowRelavant,
                CurrencyID = Gl.CurrencyID,
                CurrencyName = curName.Description,
                ChangeLog = Gl.ChangeLog,
                ParentId = Gl.ParentId,
                CompanyID = Gl.CompanyID,

            });
            return true;
        }
        private bool AddFirstGLAcc(List<GLAccountBalanceSheetreportViewModel> listGl, dynamic Gl, int sysCurID, List<AccountBalance> accountBalances, bool op = false, bool only7 = false)
        {
            var curName = _context.Currency.Find(sysCurID);
            listGl.ForEach(i =>
            {
                if (op)
                    i.Balance *= -1;
                if (only7)
                {
                    if (i.AccountType == AccountType.Expenditure)
                    {
                        if (i.Balance > 0) i.Balance *= -1;
                        i.AccountBalances.ForEach(acc =>
                        {
                            if (acc.ParentID == i.ID)
                            {
                                acc.Debit *= -1;
                                acc.Credit *= -1;
                            }
                        });
                    }
                    if (i.AccountType == AccountType.Sale)
                    {
                        if (i.Balance < 0) i.Balance *= -1;
                    }
                }
            });
            listGl.Add(new GLAccountBalanceSheetreportViewModel
            {
                ID = Gl.ID,
                MainParentID = Gl.MainParentId,
                Code = Gl.Code,
                ExternalCode = Gl.ExternalCode,
                Name = Gl.Name,
                Level = Gl.Level,
                AccountType = Gl.AccountType,
                Balance = Gl.Balance,
                IsTitle = Gl.IsTitle,
                IsActive = Gl.IsActive,
                IsConfidential = Gl.IsConfidential,
                IsIndexed = Gl.IsIndexed,
                IsCashAccount = Gl.IsCashAccount,
                IsControlAccount = Gl.IsControlAccount,
                BlockManualPosting = Gl.BlockManualPosting,
                CashFlowRelavant = Gl.CashFlowRelavant,
                CurrencyID = Gl.CurrencyID,
                CurrencyName = curName.Description,
                ChangeLog = Gl.ChangeLog,
                ParentId = Gl.ParentId,
                CompanyID = Gl.CompanyID,
                AccountBalances = accountBalances
            });
            return true;
        }
        private decimal AddSumRecursively(List<GLAccountBalanceSheetreportViewModel> activeGLs, bool op = false, bool bs = false)
        {
            activeGLs.ForEach(gl =>
            {
                activeGLs.Where(_gl => _gl.ParentId == gl.ID).ToList().ForEach(_gl =>
                {
                    if (_gl.IsTitle)
                    {
                        _gl.Balance = AddSumRecursively(activeGLs.Where(ag => ag.ParentId == _gl.ID).ToList(), op, bs);
                        gl.Balance += _gl.Balance;
                    }
                });
            });
            //decimal sunDebit = 
            if (bs)
            {
                var sumActive = activeGLs.Where(ag => ag.IsActive).Count() < 0 ? 0
                : activeGLs.Where(ag => ag.IsActive).Sum(ag => ag.AccountBalance == null ? 0 : ag.AccountBalance.CumulativeBalance);
                if (op)
                    return sumActive * (-1);
                else
                    return sumActive;
            }
            else
            {
                var glActive = activeGLs.FirstOrDefault(ag => ag.IsActive);
                var sumActive = glActive == null ? 0
                : glActive.AccountBalances.Where(i => i.ParentID == glActive.ID).Sum(dc => dc.Debit - dc.Credit);
                if (op)
                    return sumActive * -1;
                else
                    return sumActive;
            }
        }
        private static void SumLevel(List<GLAccountBalanceSheetreportViewModel> items, bool op = false, DateTime? date = null, DateTime? toDate = null)
        {
            if (date != null && date != toDate)
            {
                var _items = items.Where(i => i.Level == 2).ToList();
                foreach (var i in _items)
                {
                    var sum = i.AccountBalances.Where(i => i.PostingDate >= date && i.PostingDate <= toDate).Sum(i => i.CumulativeBalance);
                    if (op)
                        i.Balance = sum * -1;
                    else
                        i.Balance = sum;
                }
            }
            else
            {
                var _items = items.Where(i => i.Level == 2).ToList();
                foreach (var i in _items)
                {
                    if (op)
                        i.Balance = i.AccountBalances == null ? 0
                            : i.AccountBalances.Sum(i => i == null ? 0 : i.CumulativeBalance) * -1;
                    else
                        i.Balance = i.AccountBalances == null ? 0
                            : i.AccountBalances.Sum(i => i == null ? 0 : i.CumulativeBalance);
                }
            }
        }
        private static void SumLevelPL(List<GLAccountBalanceSheetreportViewModel> items, bool op = false)
        {
            items.Where(i => i.Level == 2).ToList().ForEach(i =>
            {
                if (op)
                    i.Balance = i.AccountBalances == null ? 0 : (i.AccountBalances.Sum(i => i == null ? 0 : i.Debit) - i.AccountBalances.Sum(i => i == null ? 0 : i.Credit)) * (-1);
                else
                    i.Balance = i.AccountBalances == null ? 0 : (i.AccountBalances.Sum(i => i == null ? 0 : i.Debit) - i.AccountBalances.Sum(i => i == null ? 0 : i.Credit));
            });

        }
        private void QuarterDisplayReport(string date, List<GLAccountBalanceSheetreportViewModel> data, bool op = false, bool bs = false)
        {
            _ = DateTime.TryParse(date, out DateTime _toDate);
            for (var i = 1; i <= 12; i += 3)
            {
                List<GLAccountBalanceSheetreportViewModel> GLAcc = new();
                var __date = new DateTime(_toDate.Year, i, 1);
                var endQ = __date.Month + 2;
                var lastDayOfMonth = DateTime.DaysInMonth(_toDate.Year, endQ);
                var finDate = __date.GetQuarter();
                var todate = new DateTime(_toDate.Year, endQ, lastDayOfMonth);
                var title = data.Where(i => i.IsTitle).ToList();
                title.ForEach(i => GLAcc.Add(i));
                var _data = data
                    .Where(i => i.IsActive && i.AccountBalances.Any(j => j.PostingDate >= __date && j.PostingDate <= todate)).ToList();
                _data.ForEach(i => GLAcc.Add(i));

                if (finDate == 1)
                {
                    AddSumRecursively(GLAcc, op, bs);
                    SumLevel(GLAcc, op, __date, todate);
                    GLAcc.ForEach(i => i.Q1 = i.Balance);
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (finDate == 2)
                {
                    AddSumRecursively(GLAcc, op, bs);
                    SumLevel(GLAcc, op, __date, todate);
                    GLAcc.ForEach(i => i.Q2 = i.Balance);
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (finDate == 3)
                {
                    AddSumRecursively(GLAcc, op, bs);
                    SumLevel(GLAcc, op, __date, todate);
                    GLAcc.ForEach(i => i.Q3 = i.Balance);
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (finDate == 4)
                {
                    AddSumRecursively(GLAcc, op, bs);
                    SumLevel(GLAcc, op, __date, todate);
                    GLAcc.ForEach(i => i.Q4 = i.Balance);
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
            }
        }
        private void MonthlyDisplayReport(string date, List<GLAccountBalanceSheetreportViewModel> data, bool op = false)
        {
            _ = DateTime.TryParse(date, out DateTime _toDate);
            for (var i = 1; i <= _toDate.Month; i++)
            {
                List<GLAccountBalanceSheetreportViewModel> GLAcc = new();
                var __date = new DateTime(_toDate.Year, i, 1);
                var lastDayOfMonth = DateTime.DaysInMonth(_toDate.Year, i);
                var title = data.Where(i => i.IsTitle).ToList();
                title.ForEach(i => GLAcc.Add(i));
                var todate = new DateTime(_toDate.Year, i, lastDayOfMonth);
                var _data = data
                    .Where(i => i.IsActive && i.AccountBalances.Any(j => j.PostingDate >= __date && j.PostingDate <= todate)).ToList();
                _data.ForEach(i => GLAcc.Add(i));
                if (i == 1)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op, __date, todate);
                    GLAcc.ForEach(glacc => glacc.M1 = glacc.Balance);
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 2)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op, __date, todate);
                    GLAcc.ForEach(glacc => glacc.M2 = glacc.Balance);
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 3)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op, __date, todate);
                    GLAcc.ForEach(glacc => glacc.M3 = glacc.Balance);
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 4)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op, __date, todate);
                    GLAcc.ForEach(glacc => glacc.M4 = glacc.Balance);
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 5)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op, __date, todate);
                    GLAcc.ForEach(glacc => glacc.M5 = glacc.Balance);
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 6)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op, __date, todate);
                    GLAcc.ForEach(glacc => glacc.M6 = glacc.Balance);
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 7)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op, __date, todate);
                    GLAcc.ForEach(glacc => glacc.M7 = glacc.Balance);
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 8)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op, __date, todate);
                    GLAcc.ForEach(glacc => glacc.M8 = glacc.Balance);
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 9)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op, __date, todate);
                    GLAcc.ForEach(glacc => glacc.M9 = glacc.Balance);
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 10)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op, __date, todate);
                    GLAcc.ForEach(glacc => glacc.M10 = glacc.Balance);
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 11)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op, __date, todate);
                    GLAcc.ForEach(glacc => glacc.M11 = glacc.Balance);
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 12)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op, __date, todate);
                    GLAcc.ForEach(glacc => glacc.M12 = glacc.Balance);
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
            }
        }
        private void QuarterDisplayReportPL(string date, List<GLAccountBalanceSheetreportViewModel> data, bool op = false, bool bs = false)
        {
            _ = DateTime.TryParse(date, out DateTime _toDate);
            for (var i = 1; i <= 12; i += 3)
            {
                List<GLAccountBalanceSheetreportViewModel> GLAcc = new();
                var __date = new DateTime(_toDate.Year, i, 1);
                var endQ = __date.Month + 2;
                var lastDayOfMonth = DateTime.DaysInMonth(_toDate.Year, endQ);
                var finDate = __date.GetQuarter();
                var todate = new DateTime(_toDate.Year, endQ, lastDayOfMonth);
                var title = data.Where(i => i.IsTitle).ToList();
                title.ForEach(i => GLAcc.Add(i));
                var _data = data
                    .Where(i => i.IsActive && i.AccountBalances.Any(j => j.PostingDate >= __date && j.PostingDate <= todate)).ToList();
                _data.ForEach(i => GLAcc.Add(i));

                if (finDate == 1)
                {
                    AddSumRecursively(GLAcc, op, bs);
                    SumLevel(GLAcc, op);
                    GLAcc.ForEach(i =>
                    {
                        if (i.Level == 2)
                        {
                            var sum = i.AccountBalances.Where(i => i.PostingDate >= __date && i.PostingDate <= todate).Sum(i => i.Debit - i.Credit);
                            if (op)
                                i.Q1 = sum * (-1);
                            else
                                i.Q1 = sum;
                        }
                        else
                            i.Q1 = i.Balance;

                    });
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (finDate == 2)
                {
                    AddSumRecursively(GLAcc, op, bs);
                    SumLevel(GLAcc, op);
                    GLAcc.ForEach(i =>
                    {
                        if (i.Level == 2)
                        {
                            var sum = i.AccountBalances.Where(i => i.PostingDate >= __date && i.PostingDate <= todate).Sum(i => i.Debit - i.Credit);
                            if (op)
                                i.Q2 = sum * (-1);
                            else
                                i.Q2 = sum;
                        }
                        else
                            i.Q2 = i.Balance;
                    });
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (finDate == 3)
                {
                    AddSumRecursively(GLAcc, op, bs);
                    SumLevel(GLAcc, op);
                    GLAcc.ForEach(i =>
                    {
                        if (i.Level == 2)
                        {
                            var sum = i.AccountBalances.Where(i => i.PostingDate >= __date && i.PostingDate <= todate).Sum(i => i.Debit - i.Credit);
                            if (op)
                                i.Q3 = sum * (-1);
                            else
                                i.Q3 = sum;
                        }
                        else
                            i.Q3 = i.Balance;
                    });
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (finDate == 4)
                {
                    AddSumRecursively(GLAcc, op, bs);
                    SumLevel(GLAcc, op);
                    GLAcc.ForEach(i =>
                    {
                        if (i.Level == 2)
                        {
                            var sum = i.AccountBalances.Where(i => i.PostingDate >= __date && i.PostingDate <= todate).Sum(i => i.Debit - i.Credit);
                            if (op)
                                i.Q4 = sum * (-1);
                            else
                                i.Q4 = sum;
                        }
                        else
                            i.Q4 = i.Balance;
                    });
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
            }
        }
        private void MonthlyDisplayReportPL(string date, List<GLAccountBalanceSheetreportViewModel> data, bool op = false)
        {
            _ = DateTime.TryParse(date, out DateTime _toDate);
            for (var i = 1; i <= _toDate.Month; i++)
            {
                List<GLAccountBalanceSheetreportViewModel> GLAcc = new();
                var __date = new DateTime(_toDate.Year, i, 1);
                var lastDayOfMonth = DateTime.DaysInMonth(_toDate.Year, i);
                var title = data.Where(i => i.IsTitle).ToList();
                title.ForEach(i => GLAcc.Add(i));
                var todate = new DateTime(_toDate.Year, i, lastDayOfMonth);
                var _data = data
                    .Where(i => i.IsActive && i.AccountBalances.Any(j => j.PostingDate >= __date && j.PostingDate <= todate)).ToList();
                _data.ForEach(i => GLAcc.Add(i));
                if (i == 1)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var sum = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date &&
                             accbal.PostingDate <= todate).Sum(i => i.Debit - i.Credit);
                            if (op)
                                glacc.M1 = sum * (-1);
                            else
                                glacc.M1 = sum;
                        }
                        else
                        {
                            glacc.M1 = glacc.Balance;
                        }
                    });
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 2)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var sum = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date &&
                             accbal.PostingDate <= todate).Sum(i => i.Debit - i.Credit);
                            if (op)
                                glacc.M2 = sum * (-1);
                            else
                                glacc.M2 = sum;
                        }
                        else
                        {
                            glacc.M2 = glacc.Balance;
                        }
                    });
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 3)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var sum = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date &&
                            accbal.PostingDate <= todate).Sum(i => i.Debit - i.Credit);
                            if (op)
                                glacc.M3 = sum * (-1);
                            else
                                glacc.M3 = sum;
                        }
                        else
                        {
                            glacc.M3 = glacc.Balance;
                        }
                    });
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 4)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var sum = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date &&
                            accbal.PostingDate <= todate).Sum(i => i.Debit - i.Credit);
                            if (op)
                                glacc.M4 = sum * (-1);
                            else
                                glacc.M4 = sum;
                        }
                        else
                        {
                            glacc.M4 = glacc.Balance;
                        }
                    });
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 5)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var sum = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date &&
                            accbal.PostingDate <= todate).Sum(i => i.Debit - i.Credit);
                            if (op)
                                glacc.M5 = sum * (-1);
                            else
                                glacc.M5 = sum;
                        }
                        else
                        {
                            glacc.M5 = glacc.Balance;
                        }
                    });
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 6)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var sum = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date &&
                            accbal.PostingDate <= todate).Sum(i => i.Debit - i.Credit);
                            if (op)
                                glacc.M6 = sum * (-1);
                            else
                                glacc.M6 = sum;
                        }
                        else
                        {
                            glacc.M6 = glacc.Balance;
                        }
                    });
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 7)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var sum = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date &&
                            accbal.PostingDate <= todate).Sum(i => i.Debit - i.Credit);
                            if (op)
                                glacc.M7 = sum * (-1);
                            else
                                glacc.M7 = sum;
                        }
                        else
                        {
                            glacc.M7 = glacc.Balance;
                        }
                    });
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 8)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var sum = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date &&
                            accbal.PostingDate <= todate).Sum(i => i.Debit - i.Credit);
                            if (op)
                                glacc.M8 = sum * (-1);
                            else
                                glacc.M8 = sum;
                        }
                        else
                        {
                            glacc.M8 = glacc.Balance;
                        }
                    });
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 9)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var sum = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date &&
                            accbal.PostingDate <= todate).Sum(i => i.Debit - i.Credit);
                            if (op)
                                glacc.M9 = sum * (-1);
                            else
                                glacc.M9 = sum;
                        }
                        else
                        {
                            glacc.M9 = glacc.Balance;
                        }
                    });
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 10)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var sum = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date &&
                            accbal.PostingDate <= todate).Sum(i => i.Debit - i.Credit);
                            if (op)
                                glacc.M10 = sum * (-1);
                            else
                                glacc.M10 = sum;
                        }
                        else
                        {
                            glacc.M10 = glacc.Balance;
                        }
                    });
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 11)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var sum = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date &&
                            accbal.PostingDate <= todate).Sum(i => i.Debit - i.Credit);
                            if (op)
                                glacc.M11 = sum * (-1);
                            else
                                glacc.M11 = sum;
                        }
                        else
                        {
                            glacc.M11 = glacc.Balance;
                        }
                    });
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
                if (i == 12)
                {
                    AddSumRecursively(GLAcc, op);
                    SumLevel(GLAcc, op);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var sum = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date &&
                            accbal.PostingDate <= todate).Sum(i => i.Debit - i.Credit);
                            if (op)
                                glacc.M12 = sum * (-1);
                            else
                                glacc.M12 = sum;
                        }
                        else
                        {
                            glacc.M12 = glacc.Balance;
                        }
                    });
                    GLAcc = new List<GLAccountBalanceSheetreportViewModel>();
                }
            }
        }
        public IEnumerable<CashFlowForTreasuryViewModel> GetCashFlowForTreasuryReport(string fromDate, string toDate, Company company)
        {
            decimal balance = 0;
            _ = DateTime.TryParse(toDate, out DateTime _toDate);
            _ = DateTime.TryParse(fromDate, out DateTime _fromDate);
            var docTypes = _context.DocumentTypes.Where(i => i.Code == "RC" || i.Code == "PS" || i.Code == "SP" || i.Code == "RP").ToList();
            var currName = _context.Currency.Find(company.SystemCurrencyID);
            var displayCurr = _context.DisplayCurrencies.FirstOrDefault(c => c.AltCurrencyID == currName.ID) ?? new DisplayCurrency();
            var disformat = _context.Displays.FirstOrDefault(c => c.DisplayCurrencyID == displayCurr.AltCurrencyID) ?? new Display();
            var data = (from doc in docTypes
                        join ab in _context.AccountBalances
                        .Where(i => i.PostingDate >= _fromDate && i.PostingDate <= _toDate) on doc.ID equals ab.Origin
                        join gla in _context.GLAccounts
                        .Where(i => i.CashFlowRelavant && i.IsCashAccount && i.CompanyID == company.ID) on ab.GLAID equals gla.ID
                        join series in _context.Series on doc.ID equals series.DocuTypeID
                        select new CashFlowForTreasuryViewModel
                        {
                            ID = ab.ID,
                            Credit = (ab.Credit == 0) ? "" : _format.ToCurrency(ab.Credit, disformat.Amounts),
                            Debit = (ab.Debit == 0) ? "" : _format.ToCurrency(ab.Debit, disformat.Amounts),
                            ControlAccount = gla.Code,
                            Origin = doc.Code,
                            Remarks = ab.Details,
                            Referrence = ab.OriginNo,
                            GLAccBPCode = gla.Code,
                            Total = currName.Description + " " + _format.ToCurrency(ab.Debit - ab.Credit, disformat.Amounts),
                            DueDate = ab.PostingDate.ToString("MM/dd/yyyy"),

                        }).GroupBy(i => i.ID).Select(i => i.FirstOrDefault()).ToList();
            data.ForEach(i =>
            {
                balance += Convert.ToDecimal(i.Total.Split(" ")[1]);
                i.Balance = currName.Description + " " + _format.ToCurrency(balance, disformat.Amounts);

                i.CreditTotal = currName.Description + " " + _format.ToCurrency(data.Sum(s => Convert.ToDecimal(s.Credit == "" ? "0" : s.Credit)), disformat.Amounts);
                i.DebitTotal = currName.Description + " " + _format.ToCurrency(data.Sum(s => Convert.ToDecimal(s.Debit == "" ? "0" : s.Debit)), disformat.Amounts);
                i.TotalSummary = currName.Description + " " + _format.ToCurrency(Convert.ToDecimal(i.DebitTotal.Split(" ")[1]) - Convert.ToDecimal(i.CreditTotal.Split(" ")[1]), disformat.Amounts);
                i.BalanceTotal = currName.Description + " " + _format.ToCurrency(Convert.ToDecimal(i.DebitTotal.Split(" ")[1]) - Convert.ToDecimal(i.CreditTotal.Split(" ")[1]), disformat.Amounts);
            });
            return data;
        }
        // TB Area //
        private static void AddQuarterTB(TrialBalanceViewModel j, TrialBalanceViewModel k)
        {
            /// Quarter ///
            j.DQ1 = k.DQ1;
            j.DQ2 = k.DQ2;
            j.DQ3 = k.DQ3;
            j.DQ4 = k.DQ4;
            j.CQ1 = k.CQ1;
            j.CQ2 = k.CQ2;
            j.CQ3 = k.CQ3;
            j.CQ4 = k.CQ4;
            j.BQ1 = k.BQ1;
            j.BQ2 = k.BQ2;
            j.BQ3 = k.BQ3;
            j.BQ4 = k.BQ4;
        }
        private static void AddMonthTB(TrialBalanceViewModel j, TrialBalanceViewModel k)
        {
            j.CM1 = k.CM1;
            j.CM2 = k.CM2;
            j.CM3 = k.CM3;
            j.CM4 = k.CM4;
            j.CM5 = k.CM5;
            j.CM6 = k.CM6;
            j.CM7 = k.CM7;
            j.CM8 = k.CM8;
            j.CM9 = k.CM9;
            j.CM10 = k.CM10;
            j.CM11 = k.CM11;
            j.CM12 = k.CM12;
            j.DM1 = k.DM1;
            j.DM2 = k.DM2;
            j.DM3 = k.DM3;
            j.DM4 = k.DM4;
            j.DM5 = k.DM5;
            j.DM6 = k.DM6;
            j.DM7 = k.DM7;
            j.DM8 = k.DM8;
            j.DM9 = k.DM9;
            j.DM10 = k.DM10;
            j.DM11 = k.DM11;
            j.DM12 = k.DM12;
            j.BM1 = k.BM1;
            j.BM2 = k.BM2;
            j.BM3 = k.BM3;
            j.BM4 = k.BM4;
            j.BM5 = k.BM5;
            j.BM6 = k.BM6;
            j.BM7 = k.BM7;
            j.BM8 = k.BM8;
            j.BM9 = k.BM9;
            j.BM10 = k.BM10;
            j.BM11 = k.BM11;
            j.BM12 = k.BM12;
        }
        private static void TBQuarterDisplayReport(string date, List<TrialBalanceViewModel> data)
        {
            _ = DateTime.TryParse(date, out DateTime _toDate);
            for (var i = 1; i <= _toDate.Month; i += 3)
            {
                List<TrialBalanceViewModel> GLAcc = new();
                var __date = new DateTime(_toDate.Year, i, 1);
                var endQ = __date.Month + 2;
                var lastDayOfMonth = DateTime.DaysInMonth(_toDate.Year, endQ);
                var finDate = __date.GetQuarter();
                var _data = data
                    .Where(i =>
                        i.PostingDate >= __date &&
                        i.PostingDate <= new DateTime(_toDate.Year, endQ, lastDayOfMonth)
                    ).ToList();
                _data.ForEach(i => GLAcc.Add(i));
                if (finDate == 1)
                {
                    GLAcc.ForEach(busgla =>
                    {

                        busgla.DQ1 = GLAcc != null ? GLAcc.Sum(i => i.Debit) : 0;
                        busgla.CQ1 = GLAcc != null ? GLAcc.Sum(i => i.Credit) : 0;
                        busgla.BQ1 = busgla.DQ1 - busgla.CQ1;

                    });
                    GLAcc = new List<TrialBalanceViewModel>();
                }
                if (finDate == 2)
                {
                    GLAcc.ForEach(busgla =>
                    {

                        busgla.DQ2 = GLAcc != null ? GLAcc.Sum(i => i.Debit) : 0;
                        busgla.CQ2 = GLAcc != null ? GLAcc.Sum(i => i.Credit) : 0;
                        busgla.BQ2 = busgla.DQ2 - busgla.CQ2;

                    });
                    GLAcc = new List<TrialBalanceViewModel>();
                }
                if (finDate == 3)
                {
                    GLAcc.ForEach(busgla =>
                    {

                        busgla.DQ3 = GLAcc != null ? GLAcc.Sum(i => i.Debit) : 0;
                        busgla.CQ3 = GLAcc != null ? GLAcc.Sum(i => i.Credit) : 0;
                        busgla.BQ3 = busgla.DQ3 - busgla.CQ3;

                    });
                    GLAcc = new List<TrialBalanceViewModel>();
                }
                if (finDate == 4)
                {
                    GLAcc.ForEach(busgla =>
                    {

                        busgla.DQ4 = GLAcc != null ? GLAcc.Sum(i => i.Debit) : 0;
                        busgla.CQ4 = GLAcc != null ? GLAcc.Sum(i => i.Credit) : 0;
                        busgla.BQ4 = busgla.DQ4 - busgla.CQ4;

                    });
                    GLAcc = new List<TrialBalanceViewModel>();
                }
            }
        }
        private static void TBMonthlyDisplayReport(string toDate, List<TrialBalanceViewModel> busPartnersFilter)
        {
            _ = DateTime.TryParse(toDate, out DateTime _toDate);
            for (var i = 1; i <= _toDate.Month; i++)
            {
                List<TrialBalanceViewModel> GLAcc = new();
                var __date = new DateTime(_toDate.Year, i, 1);
                var lastDayOfMonth = DateTime.DaysInMonth(_toDate.Year, i);
                var _data = busPartnersFilter
                    .Where(d =>
                        d.PostingDate >= __date &&
                        d.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth)
                    ).ToList();
                _data.ForEach(i => GLAcc.Add(i));
                if (i == 1)
                {
                    GLAcc.ForEach(glacc =>
                    {
                        glacc.DM1 = GLAcc != null ? GLAcc.Sum(i => i.Debit) : 0;
                        glacc.CM1 = GLAcc != null ? GLAcc.Sum(i => i.Credit) : 0;
                        glacc.BM1 = glacc.DM1 - glacc.CM1;
                    });
                    GLAcc = new List<TrialBalanceViewModel>();
                }
                if (i == 2)
                {
                    GLAcc.ForEach(glacc =>
                    {
                        glacc.DM2 = GLAcc != null ? GLAcc.Sum(i => i.Debit) : 0;
                        glacc.CM2 = GLAcc != null ? GLAcc.Sum(i => i.Credit) : 0;
                        glacc.BM2 = glacc.DM2 - glacc.CM2;
                    });
                    GLAcc = new List<TrialBalanceViewModel>();
                }
                if (i == 3)
                {
                    GLAcc.ForEach(glacc =>
                    {
                        glacc.DM3 = GLAcc != null ? GLAcc.Sum(i => i.Debit) : 0;
                        glacc.CM3 = GLAcc != null ? GLAcc.Sum(i => i.Credit) : 0;
                        glacc.BM3 = glacc.DM3 - glacc.CM3;
                    });
                    GLAcc = new List<TrialBalanceViewModel>();
                }
                if (i == 4)
                {
                    GLAcc.ForEach(glacc =>
                    {
                        glacc.DM4 = GLAcc != null ? GLAcc.Sum(i => i.Debit) : 0;
                        glacc.CM4 = GLAcc != null ? GLAcc.Sum(i => i.Credit) : 0;
                        glacc.BM4 = glacc.DM4 - glacc.CM4;
                    });
                    GLAcc = new List<TrialBalanceViewModel>();
                }
                if (i == 5)
                {
                    GLAcc.ForEach(glacc =>
                    {
                        glacc.DM5 = GLAcc != null ? GLAcc.Sum(i => i.Debit) : 0;
                        glacc.CM5 = GLAcc != null ? GLAcc.Sum(i => i.Credit) : 0;
                        glacc.BM5 = glacc.DM5 - glacc.CM5;
                    });
                    GLAcc = new List<TrialBalanceViewModel>();
                }
                if (i == 6)
                {
                    GLAcc.ForEach(glacc =>
                    {
                        glacc.DM6 = GLAcc != null ? GLAcc.Sum(i => i.Debit) : 0;
                        glacc.CM6 = GLAcc != null ? GLAcc.Sum(i => i.Credit) : 0;
                        glacc.BM6 = glacc.DM6 - glacc.CM6;
                    });
                    GLAcc = new List<TrialBalanceViewModel>();
                }
                if (i == 7)
                {
                    GLAcc.ForEach(glacc =>
                    {
                        glacc.DM7 = GLAcc != null ? GLAcc.Sum(i => i.Debit) : 0;
                        glacc.CM7 = GLAcc != null ? GLAcc.Sum(i => i.Credit) : 0;
                        glacc.BM7 = glacc.DM7 - glacc.CM7;
                    });
                    GLAcc = new List<TrialBalanceViewModel>();
                }
                if (i == 8)
                {
                    GLAcc.ForEach(glacc =>
                    {
                        glacc.DM8 = GLAcc != null ? GLAcc.Sum(i => i.Debit) : 0;
                        glacc.CM8 = GLAcc != null ? GLAcc.Sum(i => i.Credit) : 0;
                        glacc.BM8 = glacc.DM8 - glacc.CM8;
                    });
                    GLAcc = new List<TrialBalanceViewModel>();
                }
                if (i == 9)
                {
                    GLAcc.ForEach(glacc =>
                    {
                        glacc.DM9 = GLAcc != null ? GLAcc.Sum(i => i.Debit) : 0;
                        glacc.CM9 = GLAcc != null ? GLAcc.Sum(i => i.Credit) : 0;
                        glacc.BM9 = glacc.DM9 - glacc.CM9;
                    });
                    GLAcc = new List<TrialBalanceViewModel>();
                }
                if (i == 10)
                {
                    GLAcc.ForEach(glacc =>
                    {
                        glacc.DM10 = GLAcc != null ? GLAcc.Sum(i => i.Debit) : 0;
                        glacc.CM10 = GLAcc != null ? GLAcc.Sum(i => i.Credit) : 0;
                        glacc.BM10 = glacc.DM10 - glacc.CM10;
                    });
                    GLAcc = new List<TrialBalanceViewModel>();
                }
                if (i == 11)
                {
                    GLAcc.ForEach(glacc =>
                    {
                        glacc.DM11 = GLAcc != null ? GLAcc.Sum(i => i.Debit) : 0;
                        glacc.CM11 = GLAcc != null ? GLAcc.Sum(i => i.Credit) : 0;
                        glacc.BM11 = glacc.DM11 - glacc.CM11;
                    });
                    GLAcc = new List<TrialBalanceViewModel>();
                }
                if (i == 12)
                {
                    GLAcc.ForEach(glacc =>
                    {
                        glacc.DM12 = GLAcc != null ? GLAcc.Sum(i => i.Debit) : 0;
                        glacc.CM12 = GLAcc != null ? GLAcc.Sum(i => i.Credit) : 0;
                        glacc.BM12 = glacc.DM12 - glacc.CM12;
                    });
                    GLAcc = new List<TrialBalanceViewModel>();
                }
            }
        }
        private ProfitAndLossReportViewModelForTB GetGLAccountsPLForTB(string fromDate, string toDate, int comID, int sysCurID, int typeDisplay, bool showZeroAcc)
        {
            List<GLAccountBalanceSheetreportViewModelForTB> Turnovers = new();
            List<GLAccountBalanceSheetreportViewModelForTB> CostOfSales = new();
            List<GLAccountBalanceSheetreportViewModelForTB> OperatingCosts = new();
            List<GLAccountBalanceSheetreportViewModelForTB> NonOperatingIncomeExpenditures = new();
            List<GLAccountBalanceSheetreportViewModelForTB> TaxationExtraordinaryItems = new();
            List<GLAccountBalanceSheetreportViewModelForTB> Assets = new();
            List<GLAccountBalanceSheetreportViewModelForTB> Liabilities = new();
            List<GLAccountBalanceSheetreportViewModelForTB> CapitalandReserves = new();

            List<AccountBalance> accountBalancesAssets = new();
            List<AccountBalance> accountBalancesLiabilities = new();
            List<AccountBalance> accountBalancesTurnovers = new();
            List<AccountBalance> accountBalancesCapitalandReserves = new();
            List<AccountBalance> accountBalancesCostofSales = new();
            List<AccountBalance> accountBalancesNonOperatingIncomeExpenditures = new();
            List<AccountBalance> accountBalancesOperatingCosts = new();
            List<AccountBalance> accountBalancesTaxationExtraordinaryItems = new();
            List<AccountBalance> accountBalancesAsset = new();
            List<AccountBalance> accountBalancesLiability = new();
            List<AccountBalance> accountBalancesCapitalAndReserve = new();

            bool AnnualReport = false;
            bool QuarterlyReport = false;
            bool MonthlyReport = false;
            bool PeriodicReport = false;
            var asset = _context.GLAccounts.FirstOrDefault(i => i.Code == ASSETCODE);
            var liability = _context.GLAccounts.FirstOrDefault(i => i.Code == LIABILIITYCODE);
            var capitalAndReserve = _context.GLAccounts.FirstOrDefault(i => i.Code == CAPITLANDRESERVECODE);
            var turnover = _context.GLAccounts.FirstOrDefault(i => i.Code == TURNOVERCODE);
            var costOfSale = _context.GLAccounts.FirstOrDefault(i => i.Code == COSTOFSALESCODE);
            var operatingCost = _context.GLAccounts.FirstOrDefault(i => i.Code == OPERATINGCOSTSCODE);
            var nonOIE = _context.GLAccounts.FirstOrDefault(i => i.Code == NONOPERATINGINCOMEEXPENDITURECODE);
            var taxEI = _context.GLAccounts.FirstOrDefault(i => i.Code == TAXATIONEXTRAORDINARYITEMSCODE);

            var curName = _context.Currency.Find(sysCurID);
            var displayCurr = _context.DisplayCurrencies.FirstOrDefault(c => c.AltCurrencyID == curName.ID) ?? new DisplayCurrency();
            var disformat = _context.Displays.FirstOrDefault(c => c.DisplayCurrencyID == displayCurr.AltCurrencyID) ?? new Display();
            _ = DateTime.TryParse(toDate, out DateTime _toDate);
            _ = DateTime.TryParse(fromDate, out DateTime _fromDate);
            var gLAccounts = (from glacc in _context.GLAccounts.Where(i => i.CompanyID == comID)
                              let _accbal = _context.AccountBalances.Where(i => i.PostingDate <= _toDate && i.PostingDate >= _fromDate && i.GLAID == glacc.ID).ToList()
                              select new GLAccountBalanceSheetreportViewModelForTB
                              {
                                  ID = glacc.ID,
                                  Code = glacc.Code,
                                  ExternalCode = glacc.ExternalCode,
                                  Name = glacc.Name,
                                  Level = glacc.Level,
                                  AccountType = glacc.AccountType,
                                  Balance = glacc.Balance,
                                  IsTitle = glacc.IsTitle,
                                  IsActive = glacc.IsActive,
                                  IsConfidential = glacc.IsConfidential,
                                  IsIndexed = glacc.IsIndexed,
                                  IsCashAccount = glacc.IsCashAccount,
                                  IsControlAccount = glacc.IsControlAccount,
                                  BlockManualPosting = glacc.BlockManualPosting,
                                  CashFlowRelavant = glacc.CashFlowRelavant,
                                  CurrencyID = glacc.CurrencyID,
                                  CurrencyName = curName.Description,
                                  ChangeLog = glacc.ChangeLog,
                                  ParentId = glacc.ParentId,
                                  CompanyID = glacc.CompanyID,
                                  AccountBalances = _accbal,
                                  ActiveAccountBalances = _accbal,
                                  MainParentID = glacc.MainParentId,

                              });

            var accs = gLAccounts.ToList();
            if (showZeroAcc)
            {
                accs = gLAccounts.ToList();
            }
            else
            {
                accs = gLAccounts.Where(i => (!i.IsActive && i.AccountBalances.Count == 0) || (i.IsActive && i.AccountBalances.Count != 0)).ToList();
            }

            foreach (var i in accs)
            {
                if (i.MainParentID == asset.ID)
                {
                    if (i.AccountBalances != null)
                    {
                        i.AccountBalances.ForEach(acc =>
                        {
                            accountBalancesAssets.Add(acc);
                        });
                    }
                    i.AccountBalances = accountBalancesAssets;
                    Assets.Add(i);
                }

                if (i.MainParentID == liability.ID)
                {
                    if (i.AccountBalances != null)
                    {
                        i.AccountBalances.ForEach(acc =>
                        {
                            accountBalancesLiabilities.Add(acc);
                        });
                    }
                    i.AccountBalances = accountBalancesLiabilities;
                    Liabilities.Add(i);
                }

                if (i.MainParentID == capitalAndReserve.ID)
                {
                    if (i.AccountBalances != null)
                    {
                        i.AccountBalances.ForEach(acc =>
                        {
                            accountBalancesCapitalandReserves.Add(acc);
                        });
                    }
                    i.AccountBalances = accountBalancesCapitalandReserves;
                    CapitalandReserves.Add(i);
                }

                if (i.MainParentID == turnover.ID)
                {
                    if (i.AccountBalances != null)
                    {
                        i.AccountBalances.ForEach(acc =>
                        {
                            accountBalancesTurnovers.Add(acc);
                        });
                    }
                    i.AccountBalances = accountBalancesTurnovers;
                    Turnovers.Add(i);
                }

                if (i.MainParentID == costOfSale.ID)
                {
                    if (i.AccountBalances != null)
                    {
                        i.AccountBalances.ForEach(acc =>
                        {
                            accountBalancesCostofSales.Add(acc);
                        });
                    }
                    i.AccountBalances = accountBalancesCostofSales;
                    CostOfSales.Add(i);
                }
                if (i.MainParentID == operatingCost.ID)
                {
                    if (i.AccountBalances != null)
                    {
                        i.AccountBalances.ForEach(acc =>
                        {
                            accountBalancesOperatingCosts.Add(acc);
                        });
                    }
                    i.AccountBalances = accountBalancesOperatingCosts;
                    OperatingCosts.Add(i);
                }
                if (i.MainParentID == nonOIE.ID)
                {
                    if (i.AccountBalances != null)
                    {
                        i.AccountBalances.ForEach(acc =>
                        {
                            accountBalancesNonOperatingIncomeExpenditures.Add(acc);
                        });

                    }
                    i.AccountBalances = accountBalancesNonOperatingIncomeExpenditures;
                    NonOperatingIncomeExpenditures.Add(i);
                }
                if (i.MainParentID == taxEI.ID)
                {
                    if (i.AccountBalances != null)
                    {
                        i.AccountBalances.ForEach(acc =>
                        {
                            accountBalancesTaxationExtraordinaryItems.Add(acc);
                        });
                    }
                    i.AccountBalances = accountBalancesTaxationExtraordinaryItems;
                    TaxationExtraordinaryItems.Add(i);
                }
            }


            /// TypeDisplayReport :::: 0: AnnualReport, 1: QuarterReport, 2: MonthlyReport, 3: PeriodicReport
            if (typeDisplay == 0)
            {
                AnnualReport = true;
                AddSumRecursivelyForTB(Assets.ToList(), "annual");
                SumLevelForTB(Assets);

                AddSumRecursivelyForTB(Liabilities.ToList(), "annual");
                SumLevelForTB(Liabilities);

                AddSumRecursivelyForTB(CapitalandReserves.ToList(), "annual");
                SumLevelForTB(CapitalandReserves);

                AddSumRecursivelyForTB(Turnovers.ToList(), "annual");
                SumLevelForTB(Turnovers);

                AddSumRecursivelyForTB(OperatingCosts.ToList(), "annual");
                SumLevelForTB(OperatingCosts);

                AddSumRecursivelyForTB(CostOfSales.ToList(), "annual");
                SumLevelForTB(CostOfSales);

                AddSumRecursivelyForTB(NonOperatingIncomeExpenditures.ToList(), "annual");
                SumLevelForTB(NonOperatingIncomeExpenditures);

                AddSumRecursivelyForTB(TaxationExtraordinaryItems.ToList(), "annual");
                SumLevelForTB(TaxationExtraordinaryItems);
                AddFirstGLAccForTB(Assets, asset, sysCurID, accountBalancesAssets, "annual");
                AddFirstGLAccForTB(Liabilities, liability, sysCurID, accountBalancesLiabilities, "annual");
                AddFirstGLAccForTB(CapitalandReserves, capitalAndReserve, sysCurID, accountBalancesCapitalandReserves, "annual");
                AddFirstGLAccForTB(Turnovers, turnover, sysCurID, accountBalancesTurnovers, "annual");
                AddFirstGLAccForTB(CostOfSales, costOfSale, sysCurID, accountBalancesCostofSales, "annual");
                AddFirstGLAccForTB(OperatingCosts, operatingCost, sysCurID, accountBalancesOperatingCosts, "annual");
                AddFirstGLAccForTB(NonOperatingIncomeExpenditures, nonOIE, sysCurID, accountBalancesNonOperatingIncomeExpenditures, "annual");
                AddFirstGLAccForTB(TaxationExtraordinaryItems, taxEI, sysCurID, accountBalancesTaxationExtraordinaryItems, "annual");
            }
            if (typeDisplay == 1)
            {
                QuarterlyReport = true;
                QuarterDisplayReportForTB(toDate, Assets, asset, sysCurID, accountBalancesAssets);
                QuarterDisplayReportForTB(toDate, Liabilities, liability, sysCurID, accountBalancesLiabilities);
                QuarterDisplayReportForTB(toDate, CapitalandReserves, capitalAndReserve, sysCurID, accountBalancesCapitalandReserves);
                QuarterDisplayReportForTB(toDate, Turnovers, turnover, sysCurID, accountBalancesTurnovers);
                QuarterDisplayReportForTB(toDate, OperatingCosts, operatingCost, sysCurID, accountBalancesOperatingCosts);
                QuarterDisplayReportForTB(toDate, CostOfSales, costOfSale, sysCurID, accountBalancesCostofSales);
                QuarterDisplayReportForTB(toDate, NonOperatingIncomeExpenditures, nonOIE, sysCurID, accountBalancesNonOperatingIncomeExpenditures);
                QuarterDisplayReportForTB(toDate, TaxationExtraordinaryItems, taxEI, sysCurID, accountBalancesTaxationExtraordinaryItems);
            }
            if (typeDisplay == 2)
            {
                MonthlyReport = true;
                MonthlyDisplayReportForTB(toDate, Assets, asset, sysCurID, accountBalancesAssets);
                MonthlyDisplayReportForTB(toDate, Liabilities, liability, sysCurID, accountBalancesLiabilities);
                MonthlyDisplayReportForTB(toDate, CapitalandReserves, capitalAndReserve, sysCurID, accountBalancesCapitalandReserves);
                MonthlyDisplayReportForTB(toDate, Turnovers, turnover, sysCurID, accountBalancesTurnovers);
                MonthlyDisplayReportForTB(toDate, OperatingCosts, operatingCost, sysCurID, accountBalancesOperatingCosts);
                MonthlyDisplayReportForTB(toDate, CostOfSales, costOfSale, sysCurID, accountBalancesCostofSales);
                MonthlyDisplayReportForTB(toDate, NonOperatingIncomeExpenditures, nonOIE, sysCurID, accountBalancesNonOperatingIncomeExpenditures);
                MonthlyDisplayReportForTB(toDate, TaxationExtraordinaryItems, taxEI, sysCurID, accountBalancesTaxationExtraordinaryItems);
            }
            if (typeDisplay == 3)
            {
                PeriodicReport = true;
            }

            var profitAndLostForTB = new ProfitAndLossReportViewModelForTB
            {
                AnnualReport = AnnualReport,
                QuarterlyReport = QuarterlyReport,
                MonthlyReport = MonthlyReport,
                PeriodicReport = PeriodicReport,
                ShowZeroAccount = showZeroAcc,
                Assets = Assets,
                Liabilities = Liabilities,
                CapitalandReserves = CapitalandReserves,
                Turnover = Turnovers,
                CostOfSales = CostOfSales,
                OperatingCosts = OperatingCosts,
                NonOperatingIncomeExpenditure = NonOperatingIncomeExpenditures,
                TaxationExtraordinaryItems = TaxationExtraordinaryItems,
            };
            return profitAndLostForTB;
        }
        private bool AddFirstGLAccForTB(List<GLAccountBalanceSheetreportViewModelForTB> listGl, dynamic Gl, int sysCurID, List<AccountBalance> accountBalances, string code)
        {
            var curName = _context.Currency.Find(sysCurID);
            var displayCurr = _context.DisplayCurrencies.FirstOrDefault(c => c.AltCurrencyID == curName.ID) ?? new DisplayCurrency();
            var disformat = _context.Displays.FirstOrDefault(c => c.DisplayCurrencyID == displayCurr.AltCurrencyID) ?? new Display();
            var sumCredit = accountBalances == null ? 0 : accountBalances.Sum(i => i.Credit);
            var sumDebit = accountBalances == null ? 0 : accountBalances.Sum(i => i.Debit);
            var MQsumCredit = accountBalances == null ? 0 : accountBalances.Sum(i => i.Credit);
            var MQsumDebit = accountBalances == null ? 0 : accountBalances.Sum(i => i.Debit);
            /// Vars for store sum /// 
            decimal dq1 = 0, dq2 = 0, dq3 = 0, dq4 = 0;
            decimal cq1 = 0, cq2 = 0, cq3 = 0, cq4 = 0;
            decimal bq1 = 0, bq2 = 0, bq3 = 0, bq4 = 0;
            decimal dm1 = 0, dm2 = 0, dm3 = 0, dm4 = 0, dm5 = 0, dm6 = 0,
                    dm7 = 0, dm8 = 0, dm9 = 0, dm10 = 0, dm11 = 0, dm12 = 0;
            decimal cm1 = 0, cm2 = 0, cm3 = 0, cm4 = 0, cm5 = 0, cm6 = 0,
                    cm7 = 0, cm8 = 0, cm9 = 0, cm10 = 0, cm11 = 0, cm12 = 0;
            decimal bm1 = 0, bm2 = 0, bm3 = 0, bm4 = 0, bm5 = 0, bm6 = 0,
                    bm7 = 0, bm8 = 0, bm9 = 0, bm10 = 0, bm11 = 0, bm12 = 0;
            // Quarter //
            if (code == "Q1")
            {
                dq1 = MQsumDebit;
                cq1 = MQsumCredit;
                bq1 = MQsumDebit - MQsumCredit;
            }
            if (code == "Q2")
            {
                dq2 = MQsumDebit;
                cq2 = MQsumCredit;
                bq2 = MQsumDebit - MQsumCredit;
            }
            if (code == "Q3")
            {
                dq3 = MQsumDebit;
                cq3 = MQsumCredit;
                bq3 = MQsumDebit - MQsumCredit;
            }
            if (code == "Q4")
            {
                dq4 = MQsumDebit;
                cq4 = MQsumCredit;
                bq4 = MQsumDebit - MQsumCredit;
            }
            //Monthly//
            if (code == "M1")
            {
                dm1 = MQsumDebit;
                cm1 = MQsumCredit;
                bm1 = MQsumDebit - MQsumCredit;
            }
            if (code == "M2")
            {
                dm2 = MQsumDebit;
                cm2 = MQsumCredit;
                bm2 = MQsumDebit - MQsumCredit;
            }
            if (code == "M3")
            {
                dm3 = MQsumDebit;
                cm3 = MQsumCredit;
                bm3 = MQsumDebit - MQsumCredit;
            }
            if (code == "M4")
            {
                dm4 = MQsumDebit;
                cm4 = MQsumCredit;
                bm4 = MQsumDebit - MQsumCredit;
            }
            if (code == "M5")
            {
                dm5 = MQsumDebit;
                cm5 = MQsumCredit;
                bm5 = MQsumDebit - MQsumCredit;
            }
            if (code == "M6")
            {
                dm6 = MQsumDebit;
                cm6 = MQsumCredit;
                bm6 = MQsumDebit - MQsumCredit;
            }
            if (code == "M7")
            {
                dm7 = MQsumDebit;
                cm7 = MQsumCredit;
                bm7 = MQsumDebit - MQsumCredit;
            }
            if (code == "M8")
            {
                dm8 = MQsumDebit;
                cm8 = MQsumCredit;
                bm8 = MQsumDebit - MQsumCredit;
            }
            if (code == "M9")
            {
                dm9 = MQsumDebit;
                cm9 = MQsumCredit;
                bm9 = MQsumDebit - MQsumCredit;
            }
            if (code == "M10")
            {
                dm10 = MQsumDebit;
                cm10 = MQsumCredit;
                bm10 = MQsumDebit - MQsumCredit;
            }
            if (code == "M11")
            {
                dm11 = MQsumDebit;
                cm11 = MQsumCredit;
                bm11 = MQsumDebit - MQsumCredit;
            }
            if (code == "M12")
            {
                dm12 = MQsumDebit;
                cm12 = MQsumCredit;
                bm12 = MQsumDebit - MQsumCredit;
            }
            //listGl..Where(i=> i.ID != Gl).ToList().ForEach
            listGl.Add(new GLAccountBalanceSheetreportViewModelForTB
            {
                ID = Gl.ID,
                MainParentID = Gl.MainParentId,
                Code = Gl.Code,
                ExternalCode = Gl.ExternalCode,
                Name = Gl.Name,
                Level = Gl.Level,
                AccountType = Gl.AccountType,
                Balance = sumDebit - sumCredit,
                IsTitle = Gl.IsTitle,
                IsActive = Gl.IsActive,
                IsConfidential = Gl.IsConfidential,
                IsIndexed = Gl.IsIndexed,
                IsCashAccount = Gl.IsCashAccount,
                IsControlAccount = Gl.IsControlAccount,
                BlockManualPosting = Gl.BlockManualPosting,
                CashFlowRelavant = Gl.CashFlowRelavant,
                CurrencyID = Gl.CurrencyID,
                CurrencyName = curName.Description,
                ChangeLog = Gl.ChangeLog,
                ParentId = Gl.ParentId,
                CompanyID = Gl.CompanyID,
                AccountBalances = accountBalances,
                Credit = sumCredit,
                Debit = sumDebit,
                /// Quarter ///
                DQ1 = dq1,
                DQ2 = dq2,
                DQ3 = dq3,
                DQ4 = dq4,
                CQ1 = cq1,
                CQ2 = cq2,
                CQ3 = cq3,
                CQ4 = cq4,
                BQ1 = bq1,
                BQ2 = bq2,
                BQ3 = bq3,
                BQ4 = bq4,
                ///// Monthly //
                CM1 = cm1,
                CM2 = cm2,
                CM3 = cm3,
                CM4 = cm4,
                CM5 = cm5,
                CM6 = cm6,
                CM7 = cm7,
                CM8 = cm8,
                CM9 = cm9,
                CM10 = cm10,
                CM11 = cm11,
                CM12 = cm12,
                DM1 = dm1,
                DM2 = dm2,
                DM3 = dm3,
                DM4 = dm4,
                DM5 = dm5,
                DM6 = dm6,
                DM7 = dm7,
                DM8 = dm8,
                DM9 = dm9,
                DM10 = dm10,
                DM11 = dm11,
                DM12 = dm12,
                BM1 = bm1,
                BM2 = bm2,
                BM3 = bm3,
                BM4 = bm4,
                BM5 = bm5,
                BM6 = bm6,
                BM7 = bm7,
                BM8 = bm8,
                BM9 = bm9,
                BM10 = bm10,
                BM11 = bm11,
                BM12 = bm12,
            });
            return true;
        }
        private CDBForTB AddSumRecursivelyForTB(List<GLAccountBalanceSheetreportViewModelForTB> activeGLs, string code)
        {
            activeGLs.ForEach(gl =>
            {
                activeGLs.Where(_gl => _gl.ParentId == gl.ID).ToList().ForEach(_gl =>
                {
                    if (_gl.IsTitle)
                    {
                        var Func = AddSumRecursivelyForTB(activeGLs.Where(ag => ag.ParentId == _gl.ID).ToList(), code);
                        //Annual//
                        _gl.Balance = Func.Balance;
                        _gl.Credit = Func.Credit;
                        _gl.Debit = Func.Debit;
                        gl.Balance += _gl.Balance;
                        gl.Debit += _gl.Debit;
                        gl.Credit += _gl.Credit;
                        //MonthLy//
                        MonthlyAddforTB(_gl, gl, Func);
                    }
                });
            });

            /// Vars for store sum /// 
            decimal dq1 = 0, dq2 = 0, dq3 = 0, dq4 = 0;
            decimal cq1 = 0, cq2 = 0, cq3 = 0, cq4 = 0;
            decimal bq1 = 0, bq2 = 0, bq3 = 0, bq4 = 0;
            decimal dm1 = 0, dm2 = 0, dm3 = 0, dm4 = 0, dm5 = 0, dm6 = 0,
                    dm7 = 0, dm8 = 0, dm9 = 0, dm10 = 0, dm11 = 0, dm12 = 0;
            decimal cm1 = 0, cm2 = 0, cm3 = 0, cm4 = 0, cm5 = 0, cm6 = 0,
                    cm7 = 0, cm8 = 0, cm9 = 0, cm10 = 0, cm11 = 0, cm12 = 0;
            decimal bm1 = 0, bm2 = 0, bm3 = 0, bm4 = 0, bm5 = 0, bm6 = 0,
                    bm7 = 0, bm8 = 0, bm9 = 0, bm10 = 0, bm11 = 0, bm12 = 0;


            decimal activeGLCredit = activeGLs.Where(ag => ag.IsActive).Count() < 0 ? 0
                : activeGLs.Where(ag => ag.IsActive).SelectMany(ag => ag.ActiveAccountBalances).Sum(ag => ag.Credit);

            decimal activeGLDebit = activeGLs.Where(ag => ag.IsActive).Count() < 0 ? 0
                : activeGLs.Where(ag => ag.IsActive).SelectMany(ag => ag.ActiveAccountBalances).Sum(ag => ag.Debit);

            decimal credit = activeGLCredit;
            decimal debit = activeGLDebit;
            decimal balance = debit - credit;
            //qaurter//
            if (code == "Q1")
            {
                dq1 = activeGLDebit;
                cq1 = activeGLCredit;
                bq1 = activeGLDebit - activeGLCredit;
            }
            if (code == "Q2")
            {
                dq2 = activeGLDebit;
                cq2 = activeGLCredit;
                bq2 = activeGLDebit - activeGLCredit;
            }
            if (code == "Q3")
            {
                dq3 = activeGLDebit;
                cq3 = activeGLCredit;
                bq3 = activeGLDebit - activeGLCredit;
            }
            if (code == "Q4")
            {
                dq4 = activeGLDebit;
                cq4 = activeGLCredit;
                bq4 = activeGLDebit - activeGLCredit;
            }

            //Monthly//
            if (code == "M1")
            {
                dm1 = activeGLDebit;
                cm1 = activeGLCredit;
                bm1 = activeGLDebit - activeGLCredit;
            }
            if (code == "M2")
            {
                dm2 = activeGLDebit;
                cm2 = activeGLCredit;
                bm2 = activeGLDebit - activeGLCredit;
            }
            if (code == "M3")
            {
                dm3 = activeGLDebit;
                cm3 = activeGLCredit;
                bm3 = activeGLDebit - activeGLCredit;
            }
            if (code == "M4")
            {
                dm4 = activeGLDebit;
                cm4 = activeGLCredit;
                bm4 = activeGLDebit - activeGLCredit;
            }
            if (code == "M5")
            {
                dm5 = activeGLDebit;
                cm5 = activeGLCredit;
                bm5 = activeGLDebit - activeGLCredit;
            }
            if (code == "M6")
            {
                dm6 = activeGLDebit;
                cm6 = activeGLCredit;
                bm6 = activeGLDebit - activeGLCredit;
            }
            if (code == "M7")
            {
                dm7 = activeGLDebit;
                cm7 = activeGLCredit;
                bm7 = activeGLDebit - activeGLCredit;
            }
            if (code == "M8")
            {
                dm8 = activeGLDebit;
                cm8 = activeGLCredit;
                bm8 = activeGLDebit - activeGLCredit;
            }
            if (code == "M9")
            {
                dm9 = activeGLDebit;
                cm9 = activeGLCredit;
                bm9 = activeGLDebit - activeGLCredit;
            }
            if (code == "M10")
            {
                dm10 = activeGLDebit;
                cm10 = activeGLCredit;
                bm10 = activeGLDebit - activeGLCredit;
            }
            if (code == "M11")
            {
                dm11 = activeGLDebit;
                cm11 = activeGLCredit;
                bm11 = activeGLDebit - activeGLCredit;
            }
            if (code == "M12")
            {
                dm12 = activeGLDebit;
                cm12 = activeGLCredit;
                bm12 = activeGLDebit - activeGLCredit;
            }
            var sumActive = new CDBForTB
            {
                Credit = credit,
                Debit = debit,
                Balance = balance,
                //Quarterly//
                DQ1 = dq1,
                CQ1 = cq1,
                BQ1 = bq1,
                DQ2 = dq2,
                CQ2 = cq2,
                BQ2 = bq2,
                DQ3 = dq3,
                CQ3 = cq3,
                BQ3 = bq3,
                DQ4 = dq4,
                CQ4 = cq4,
                BQ4 = bq4,
                DM1 = dm1,
                DM2 = dm2,
                DM3 = dm3,
                DM4 = dm4,
                DM5 = dm5,
                DM6 = dm6,
                DM7 = dm7,
                DM8 = dm8,
                DM9 = dm9,
                DM10 = dm10,
                DM11 = dm11,
                DM12 = dm12,
                CM1 = cm1,
                CM2 = cm2,
                CM3 = cm3,
                CM4 = cm4,
                CM5 = cm5,
                CM6 = cm6,
                CM7 = cm7,
                CM8 = cm8,
                CM9 = cm9,
                CM10 = cm10,
                CM11 = cm11,
                CM12 = cm12,
                BM1 = bm1,
                BM2 = bm2,
                BM3 = bm3,
                BM4 = bm4,
                BM5 = bm5,
                BM6 = bm6,
                BM7 = bm7,
                BM8 = bm8,
                BM9 = bm9,
                BM10 = bm10,
                BM11 = bm11,
                BM12 = bm12,
            };
            return sumActive;
        }
        private static void SumLevelForTB(List<GLAccountBalanceSheetreportViewModelForTB> items)
        {
            items.Where(i => i.Level == 2).ToList().ForEach(i =>
            {
                var credit = i.AccountBalances == null ? 0 : i.AccountBalances.Sum(i => i == null ? 0 : i.Credit);
                var debit = i.AccountBalances == null ? 0 : i.AccountBalances.Sum(i => i == null ? 0 : i.Debit);
                var balance = i.AccountBalances == null ? 0 : i.AccountBalances.Sum(i => i == null ? 0 : i.CumulativeBalance);
                i.Balance = balance;
                i.Credit = credit;
                i.Debit = debit;
            });
        }
        private void QuarterDisplayReportForTB(string date, List<GLAccountBalanceSheetreportViewModelForTB> data, dynamic Gl, int sysCurID, List<AccountBalance> accountBalances)
        {
            _ = DateTime.TryParse(date, out DateTime _toDate);
            for (var i = 1; i <= _toDate.Month; i += 3)
            {
                List<GLAccountBalanceSheetreportViewModelForTB> GLAcc = new();
                var __date = new DateTime(_toDate.Year, i, 1);
                var endQ = __date.Month + 2;
                var lastDayOfMonth = DateTime.DaysInMonth(_toDate.Year, endQ);
                var finDate = __date.GetQuarter();
                var title = data.Where(i => i.IsTitle).ToList();
                title.ForEach(i => GLAcc.Add(i));
                var _data = data
                    .Where(i =>
                        i.AccountBalances != null &&
                        i.IsActive
                    ).ToList();
                _data.ForEach(i => GLAcc.Add(i));
                if (finDate == 1)
                {
                    AddSumRecursivelyForTB(GLAcc, "Q1");
                    SumLevelForTB(GLAcc);
                    GLAcc.ForEach(i =>
                    {
                        if (i.Level == 2)
                        {
                            var credit = i.AccountBalances.Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, endQ, lastDayOfMonth) && acc.GLAID == acc.ID)
                                .Sum(i => i.Credit);
                            var debit = i.AccountBalances.Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, endQ, lastDayOfMonth) && acc.GLAID == acc.ID)
                                .Sum(i => i.Debit);
                            var balance = debit - credit;
                            i.DQ1 = debit;
                            i.CQ1 = credit;
                            i.BQ1 = balance;
                        }
                        else
                        {
                            var credit = (i.IsActive && i.AccountBalances != null) ?
                                i.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, endQ, lastDayOfMonth) && acc.GLAID == i.ID)
                                .Sum(acc => acc.Credit) : i.Credit;

                            var debit = (i.IsActive && i.AccountBalances != null) ?
                                i.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, endQ, lastDayOfMonth) && acc.GLAID == i.ID)
                                .Sum(acc => acc.Debit) : i.Debit;

                            var balance = debit - credit;
                            i.DQ1 = debit;
                            i.CQ1 = credit;
                            i.BQ1 = balance;
                        }

                    });
                    AddFirstGLAccForTB(data, Gl, sysCurID, accountBalances, "Q1");
                    GLAcc = new List<GLAccountBalanceSheetreportViewModelForTB>();
                }
                if (finDate == 2)
                {
                    AddSumRecursivelyForTB(GLAcc, "Q2");
                    SumLevelForTB(GLAcc);
                    GLAcc.ForEach(i =>
                    {
                        if (i.Level == 2)
                        {
                            var credit = i.AccountBalances.Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, endQ, lastDayOfMonth))
                                .Sum(i => i.Credit);
                            var debit = i.AccountBalances.Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, endQ, lastDayOfMonth))
                                .Sum(i => i.Debit);
                            var balance = debit - credit;
                            i.DQ2 = debit;
                            i.CQ2 = credit;
                            i.BQ2 = balance;
                        }
                        else
                        {
                            var credit = (i.IsActive && i.AccountBalances != null) ?
                                i.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, endQ, lastDayOfMonth) && acc.GLAID == i.ID)
                                .Sum(acc => acc.Credit) : i.Credit;

                            var debit = (i.IsActive && i.AccountBalances != null) ?
                                i.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, endQ, lastDayOfMonth) && acc.GLAID == i.ID)
                                .Sum(acc => acc.Debit) : i.Debit;

                            var balance = debit - credit;

                            i.DQ2 = debit;
                            i.CQ2 = credit;
                            i.BQ2 = balance;
                        }
                    });
                    AddFirstGLAccForTB(data, Gl, sysCurID, accountBalances, "Q2");
                    GLAcc = new List<GLAccountBalanceSheetreportViewModelForTB>();
                }
                if (finDate == 3)
                {
                    AddSumRecursivelyForTB(GLAcc, "Q3");
                    SumLevelForTB(GLAcc);
                    GLAcc.ForEach(i =>
                    {
                        if (i.Level == 2)
                        {
                            var credit = i.AccountBalances.Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, endQ, lastDayOfMonth))
                                .Sum(i => i.Credit);
                            var debit = i.AccountBalances.Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, endQ, lastDayOfMonth))
                                .Sum(i => i.Debit);
                            var balance = debit - credit;
                            i.DQ3 = debit;
                            i.CQ3 = credit;
                            i.BQ3 = balance;
                        }
                        else
                        {
                            var credit = (i.IsActive && i.AccountBalances != null) ?
                                i.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, endQ, lastDayOfMonth) && acc.GLAID == i.ID)
                                .Sum(acc => acc.Credit) : i.Credit;

                            var debit = (i.IsActive && i.AccountBalances != null) ?
                                i.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, endQ, lastDayOfMonth) && acc.GLAID == i.ID)
                                .Sum(acc => acc.Debit) : i.Debit;
                            var balance = debit - credit;

                            i.DQ3 = debit;
                            i.CQ3 = credit;
                            i.BQ3 = balance;
                        }
                    });
                    AddFirstGLAccForTB(data, Gl, sysCurID, accountBalances, "Q3");
                    GLAcc = new List<GLAccountBalanceSheetreportViewModelForTB>();
                }
                if (finDate == 4)
                {
                    AddSumRecursivelyForTB(GLAcc, "Q4");
                    SumLevelForTB(GLAcc);
                    GLAcc.ForEach(i =>
                    {
                        if (i.Level == 2)
                        {
                            var credit = i.AccountBalances.Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, endQ, lastDayOfMonth))
                                .Sum(i => i.Credit);
                            var debit = i.AccountBalances.Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, endQ, lastDayOfMonth))
                                .Sum(i => i.Debit);
                            var balance = debit - credit;
                            i.DQ4 = debit;
                            i.CQ4 = credit;
                            i.BQ4 = balance;
                        }
                        else
                        {
                            var credit = (i.IsActive && i.AccountBalances != null) ?
                                i.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, endQ, lastDayOfMonth) && acc.GLAID == i.ID)
                                .Sum(acc => acc.Credit) : i.Credit;

                            var debit = (i.IsActive && i.AccountBalances != null) ?
                                i.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, endQ, lastDayOfMonth) && acc.GLAID == i.ID)
                                .Sum(acc => acc.Debit) : i.Debit;
                            var balance = debit - credit;

                            i.DQ4 = debit;
                            i.CQ4 = credit;
                            i.BQ4 = balance;
                        }
                    });
                    AddFirstGLAccForTB(data, Gl, sysCurID, accountBalances, "Q4");
                    GLAcc = new List<GLAccountBalanceSheetreportViewModelForTB>();
                }
            }
        }
        private void MonthlyDisplayReportForTB(string date, List<GLAccountBalanceSheetreportViewModelForTB> data, dynamic Gl, int sysCurID, List<AccountBalance> accountBalances)
        {
            _ = DateTime.TryParse(date, out DateTime _toDate);
            for (var i = 1; i <= _toDate.Month; i++)
            {
                List<GLAccountBalanceSheetreportViewModelForTB> GLAcc = new();
                var __date = new DateTime(_toDate.Year, i, 1);
                var lastDayOfMonth = DateTime.DaysInMonth(_toDate.Year, i);
                var title = data.Where(i => i.IsTitle).ToList();
                title.ForEach(i => GLAcc.Add(i));
                var _data = data
                    .Where(d =>
                        d.AccountBalances != null &&
                        d.IsActive
                    ).ToList();
                _data.ForEach(i => GLAcc.Add(i));
                if (i == 1)
                {
                    AddSumRecursivelyForTB(GLAcc, "M1");
                    SumLevelForTB(GLAcc);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var credit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                            .Sum(i => i.Credit);
                            var debit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                                .Sum(i => i.Debit);
                            var balance = debit - credit;
                            glacc.DM1 = debit;
                            glacc.CM1 = credit;
                            glacc.BM1 = balance;
                        }
                        else
                        {
                            var credit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Credit) : glacc.Credit;

                            var debit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Debit) : glacc.Debit;
                            var balance = debit - credit;
                            glacc.DM1 = debit;
                            glacc.CM1 = credit;
                            glacc.BM1 = balance;
                        }
                    });
                    AddFirstGLAccForTB(data, Gl, sysCurID, accountBalances, "M1");
                    GLAcc = new List<GLAccountBalanceSheetreportViewModelForTB>();
                }
                if (i == 2)
                {
                    AddSumRecursivelyForTB(GLAcc, "M2");
                    SumLevelForTB(GLAcc);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var credit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                            .Sum(i => i.Credit);
                            var debit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                                .Sum(i => i.Debit);
                            var balance = debit - credit;
                            glacc.DM2 = debit;
                            glacc.CM2 = credit;
                            glacc.BM2 = balance;
                        }
                        else
                        {
                            var credit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Credit) : glacc.Credit;

                            var debit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Debit) : glacc.Debit;
                            var balance = debit - credit;
                            glacc.DM2 = debit;
                            glacc.CM2 = credit;
                            glacc.BM2 = balance;
                        }
                    });
                    AddFirstGLAccForTB(data, Gl, sysCurID, accountBalances, "M2");
                    GLAcc = new List<GLAccountBalanceSheetreportViewModelForTB>();
                }
                if (i == 3)
                {
                    AddSumRecursivelyForTB(GLAcc, "M3");
                    SumLevelForTB(GLAcc);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var credit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                            .Sum(i => i.Credit);
                            var debit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                                .Sum(i => i.Debit);
                            var balance = debit - credit;
                            glacc.DM3 = debit;
                            glacc.CM3 = credit;
                            glacc.BM3 = balance;
                        }
                        else
                        {
                            var credit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Credit) : glacc.Credit;

                            var debit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Debit) : glacc.Debit;
                            var balance = debit - credit;
                            glacc.DM3 = debit;
                            glacc.CM3 = credit;
                            glacc.BM3 = balance;
                        }
                    });
                    AddFirstGLAccForTB(data, Gl, sysCurID, accountBalances, "M3");
                    GLAcc = new List<GLAccountBalanceSheetreportViewModelForTB>();
                }
                if (i == 4)
                {
                    AddSumRecursivelyForTB(GLAcc, "M4");
                    SumLevelForTB(GLAcc);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var credit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                                .Sum(i => i.Credit);
                            var debit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                                .Sum(i => i.Debit);
                            var balance = debit - credit;
                            glacc.DM4 = debit;
                            glacc.CM4 = credit;
                            glacc.BM4 = balance;
                        }
                        else
                        {
                            var credit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Credit) : glacc.Credit;

                            var debit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Debit) : glacc.Debit;
                            var balance = debit - credit;
                            glacc.DM4 = debit;
                            glacc.CM4 = credit;
                            glacc.BM4 = balance;
                        }
                    });
                    AddFirstGLAccForTB(data, Gl, sysCurID, accountBalances, "M4");
                    GLAcc = new List<GLAccountBalanceSheetreportViewModelForTB>();
                }
                if (i == 5)
                {
                    AddSumRecursivelyForTB(GLAcc, "M5");
                    SumLevelForTB(GLAcc);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var credit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                            .Sum(i => i.Credit);
                            var debit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                                .Sum(i => i.Debit);
                            var balance = debit - credit;
                            glacc.DM5 = debit;
                            glacc.CM5 = credit;
                            glacc.BM5 = balance;
                        }
                        else
                        {
                            var credit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Credit) : glacc.Credit;

                            var debit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Debit) : glacc.Debit;
                            var balance = debit - credit;
                            glacc.DM5 = debit;
                            glacc.CM5 = credit;
                            glacc.BM5 = balance;
                        }
                    });
                    AddFirstGLAccForTB(data, Gl, sysCurID, accountBalances, "M5");
                    GLAcc = new List<GLAccountBalanceSheetreportViewModelForTB>();
                }
                if (i == 6)
                {
                    AddSumRecursivelyForTB(GLAcc, "M6");
                    SumLevelForTB(GLAcc);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var credit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                            .Sum(i => i.Credit);
                            var debit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                                .Sum(i => i.Debit);
                            var balance = debit - credit;
                            glacc.DM6 = debit;
                            glacc.CM6 = credit;
                            glacc.BM6 = balance;
                        }
                        else
                        {
                            var credit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Credit) : glacc.Credit;

                            var debit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Debit) : glacc.Debit;
                            var balance = debit - credit;
                            glacc.DM6 = debit;
                            glacc.CM6 = credit;
                            glacc.BM6 = balance;
                        }
                    });
                    AddFirstGLAccForTB(data, Gl, sysCurID, accountBalances, "M6");
                    GLAcc = new List<GLAccountBalanceSheetreportViewModelForTB>();
                }
                if (i == 7)
                {
                    AddSumRecursivelyForTB(GLAcc, "M7");
                    SumLevelForTB(GLAcc);
                    GLAcc.ForEach(glacc =>
                    {

                        if (glacc.Level == 2)
                        {
                            var credit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                            .Sum(i => i.Credit);
                            var debit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                                .Sum(i => i.Debit);
                            var balance = debit - credit;
                            glacc.DM7 = debit;
                            glacc.CM7 = credit;
                            glacc.BM7 = balance;
                        }
                        else
                        {
                            var credit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Credit) : glacc.Credit;

                            var debit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Debit) : glacc.Debit;
                            var balance = debit - credit;
                            glacc.DM7 = debit;
                            glacc.CM7 = credit;
                            glacc.BM7 = balance;
                        }
                    });
                    AddFirstGLAccForTB(data, Gl, sysCurID, accountBalances, "M7");
                    GLAcc = new List<GLAccountBalanceSheetreportViewModelForTB>();
                }
                if (i == 8)
                {
                    AddSumRecursivelyForTB(GLAcc, "M8");
                    SumLevelForTB(GLAcc);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var credit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                            .Sum(i => i.Credit);
                            var debit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                                .Sum(i => i.Debit);
                            var balance = debit - credit;
                            glacc.DM8 = debit;
                            glacc.CM8 = credit;
                            glacc.BM8 = balance;
                        }
                        else
                        {
                            var credit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Credit) : glacc.Credit;

                            var debit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Debit) : glacc.Debit;
                            var balance = debit - credit;
                            glacc.DM8 = debit;
                            glacc.CM8 = credit;
                            glacc.BM8 = balance;
                        }
                    });
                    AddFirstGLAccForTB(data, Gl, sysCurID, accountBalances, "M8");
                    GLAcc = new List<GLAccountBalanceSheetreportViewModelForTB>();
                }
                if (i == 9)
                {
                    AddSumRecursivelyForTB(GLAcc, "M9");
                    SumLevelForTB(GLAcc);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var credit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                            .Sum(i => i.Credit);
                            var debit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                                .Sum(i => i.Debit);
                            var balance = debit - credit;
                            glacc.DM9 = debit;
                            glacc.CM9 = credit;
                            glacc.BM9 = balance;
                        }
                        else
                        {
                            var credit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Credit) : glacc.Credit;

                            var debit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Debit) : glacc.Debit;
                            var balance = debit - credit;
                            glacc.DM9 = debit;
                            glacc.CM9 = credit;
                            glacc.BM9 = balance;
                        }
                    });
                    AddFirstGLAccForTB(data, Gl, sysCurID, accountBalances, "M9");
                    GLAcc = new List<GLAccountBalanceSheetreportViewModelForTB>();
                }
                if (i == 10)
                {
                    AddSumRecursivelyForTB(GLAcc, "M10");
                    SumLevelForTB(GLAcc);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var credit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                            .Sum(i => i.Credit);
                            var debit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                                .Sum(i => i.Debit);
                            var balance = debit - credit;
                            glacc.DM10 = debit;
                            glacc.CM10 = credit;
                            glacc.BM10 = balance;
                        }
                        else
                        {
                            var credit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Credit) : glacc.Credit;

                            var debit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Debit) : glacc.Debit;
                            var balance = debit - credit;
                            glacc.DM10 = debit;
                            glacc.CM10 = credit;
                            glacc.BM10 = balance;
                        }
                    });
                    AddFirstGLAccForTB(data, Gl, sysCurID, accountBalances, "M10");
                    GLAcc = new List<GLAccountBalanceSheetreportViewModelForTB>();
                }
                if (i == 11)
                {
                    AddSumRecursivelyForTB(GLAcc, "M11");
                    SumLevelForTB(GLAcc);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var credit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                            .Sum(i => i.Credit);
                            var debit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                                .Sum(i => i.Debit);
                            var balance = debit - credit;
                            glacc.DM11 = debit;
                            glacc.CM11 = credit;
                            glacc.BM11 = balance;
                        }
                        else
                        {
                            var credit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Credit) : glacc.Credit;

                            var debit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Debit) : glacc.Debit;
                            var balance = debit - credit;
                            glacc.DM11 = debit;
                            glacc.CM11 = credit;
                            glacc.BM11 = balance;
                        }
                    });
                    AddFirstGLAccForTB(data, Gl, sysCurID, accountBalances, "M11");
                    GLAcc = new List<GLAccountBalanceSheetreportViewModelForTB>();
                }
                if (i == 12)
                {
                    AddSumRecursivelyForTB(GLAcc, "M12");
                    SumLevelForTB(GLAcc);
                    GLAcc.ForEach(glacc =>
                    {
                        if (glacc.Level == 2)
                        {
                            var credit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                            .Sum(i => i.Credit);
                            var debit = glacc.AccountBalances.Where(accbal => accbal.PostingDate >= __date && accbal.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth))
                                .Sum(i => i.Debit);
                            var balance = debit - credit;
                            glacc.DM12 = debit;
                            glacc.CM12 = credit;
                            glacc.BM12 = balance;
                        }
                        else
                        {
                            var credit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Credit) : glacc.Credit;

                            var debit = (glacc.IsActive && glacc.AccountBalances != null) ?
                                glacc.AccountBalances
                                .Where(acc => acc.PostingDate >= __date && acc.PostingDate <= new DateTime(_toDate.Year, i, lastDayOfMonth) && acc.GLAID == glacc.ID)
                                .Sum(acc => acc.Debit) : glacc.Debit;
                            var balance = debit - credit;
                            glacc.DM12 = debit;
                            glacc.CM12 = credit;
                            glacc.BM12 = balance;
                        }
                    });
                    AddFirstGLAccForTB(data, Gl, sysCurID, accountBalances, "M12");
                    GLAcc = new List<GLAccountBalanceSheetreportViewModelForTB>();
                }
            }
        }
        private static void MonthlyAddforTB(GLAccountBalanceSheetreportViewModelForTB _gl, GLAccountBalanceSheetreportViewModelForTB gl, CDBForTB Func)
        {
            _gl.DQ1 = Func.DQ1;
            _gl.CQ1 = Func.CQ1;
            _gl.BQ1 = Func.BQ1;
            _gl.DQ2 = Func.DQ2;
            _gl.CQ2 = Func.CQ2;
            _gl.BQ2 = Func.BQ2;
            _gl.DQ3 = Func.DQ3;
            _gl.CQ3 = Func.CQ3;
            _gl.BQ3 = Func.BQ3;
            _gl.DQ4 = Func.DQ4;
            _gl.CQ4 = Func.CQ4;
            _gl.BQ4 = Func.BQ4;
            gl.DQ1 = _gl.DQ1;
            gl.CQ1 = _gl.CQ1;
            gl.BQ1 = _gl.BQ1;
            gl.DQ2 = _gl.DQ2;
            gl.CQ2 = _gl.CQ2;
            gl.BQ2 = _gl.BQ2;
            gl.DQ3 = _gl.DQ3;
            gl.CQ3 = _gl.CQ3;
            gl.BQ3 = _gl.BQ3;
            gl.DQ4 = _gl.DQ4;
            gl.CQ4 = _gl.CQ4;
            gl.BQ4 = _gl.BQ4;

            // Monthly //
            _gl.DM1 = Func.DM1;
            _gl.CM1 = Func.CM1;
            _gl.BM1 = Func.BM1;
            _gl.DM2 = Func.DM2;
            _gl.CM2 = Func.CM2;
            _gl.BM2 = Func.BM2;
            _gl.DM3 = Func.DM3;
            _gl.CM3 = Func.CM3;
            _gl.BM3 = Func.BM3;
            _gl.DM4 = Func.DM4;
            _gl.CM4 = Func.CM4;
            _gl.BM4 = Func.BM4;
            _gl.DM5 = Func.DM5;
            _gl.CM5 = Func.CM5;
            _gl.BM5 = Func.BM5;
            _gl.DM6 = Func.DM6;
            _gl.CM6 = Func.CM6;
            _gl.BM6 = Func.BM6;
            _gl.DM7 = Func.DM7;
            _gl.CM7 = Func.CM7;
            _gl.BM7 = Func.BM7;
            _gl.DM8 = Func.DM8;
            _gl.CM8 = Func.CM8;
            _gl.BM8 = Func.BM8;
            _gl.DM9 = Func.DM9;
            _gl.CM9 = Func.CM9;
            _gl.BM9 = Func.BM9;
            _gl.DM10 = Func.DM10;
            _gl.CM10 = Func.CM10;
            _gl.BM10 = Func.BM10;
            _gl.DM11 = Func.DM11;
            _gl.CM11 = Func.CM11;
            _gl.BM11 = Func.BM11;
            _gl.DM12 = Func.DM12;
            _gl.CM12 = Func.CM12;
            _gl.BM12 = Func.BM12;

            gl.DM1 = _gl.DM1;
            gl.CM1 = _gl.CM1;
            gl.BM1 = _gl.BM1;
            gl.DM2 = _gl.DM2;
            gl.CM2 = _gl.CM2;
            gl.BM2 = _gl.BM2;
            gl.DM3 = _gl.DM3;
            gl.CM3 = _gl.CM3;
            gl.BM3 = _gl.BM3;
            gl.DM4 = _gl.DM4;
            gl.CM4 = _gl.CM4;
            gl.BM4 = _gl.BM4;
            gl.DM5 = _gl.DM5;
            gl.CM5 = _gl.CM5;
            gl.BM5 = _gl.BM5;
            gl.DM6 = _gl.DM6;
            gl.CM6 = _gl.CM6;
            gl.BM6 = _gl.BM6;
            gl.DM7 = _gl.DM7;
            gl.CM7 = _gl.CM7;
            gl.BM7 = _gl.BM7;
            gl.DM8 = _gl.DM8;
            gl.CM8 = _gl.CM8;
            gl.BM8 = _gl.BM8;
            gl.DM9 = _gl.DM9;
            gl.CM9 = _gl.CM9;
            gl.BM9 = _gl.BM9;
            gl.DM10 = _gl.DM10;
            gl.CM10 = _gl.CM10;
            gl.BM10 = _gl.BM10;
            gl.DM11 = _gl.DM11;
            gl.CM11 = _gl.CM11;
            gl.BM11 = _gl.BM11;
            gl.DM12 = _gl.DM12;
            gl.CM12 = _gl.CM12;
            gl.BM12 = _gl.BM12;
        }

        public async Task<IEnumerable<SaleGrossProfitReport>> GetSaleGrossProfitReportAsync(string fromDate, string toDate, int userId, string timeFrom, string timeTo, Company company)
        {
            _ = DateTime.TryParse(toDate, out DateTime _toDate);
            _ = DateTime.TryParse(fromDate, out DateTime _fromDate);
            List<Receipt> receipts = new();
            List<SaleAR> sales = new();
            List<ReceiptMemo> receiptsCredit = new();
            List<SaleCreditMemo> salesCredit = new();
            var cur = _context.Currency.Find(company.SystemCurrencyID) ?? new Currency();
            var displayCurr = _context.DisplayCurrencies.FirstOrDefault(c => c.AltCurrencyID == cur.ID) ?? new DisplayCurrency();
            var disformat = _context.Displays.FirstOrDefault(c => c.DisplayCurrencyID == displayCurr.AltCurrencyID) ?? new Display();
            if ((toDate == null || fromDate == null) && userId == 0)
            {
                receipts = await _context.Receipt.ToListAsync();
                sales = await _context.SaleARs.ToListAsync();
                receiptsCredit = await _context.ReceiptMemo.ToListAsync();
                salesCredit = await _context.SaleCreditMemos.ToListAsync();
            }
            else if ((toDate == null || fromDate == null) && userId != 0)
            {
                receipts = await _context.Receipt.Where(i => i.UserOrderID == userId).ToListAsync();
                sales = await _context.SaleARs.Where(i => i.UserID == userId).ToListAsync();
                receiptsCredit = await _context.ReceiptMemo.Where(i => i.UserOrderID == userId).ToListAsync();
                salesCredit = await _context.SaleCreditMemos.Where(i => i.UserID == userId).ToListAsync();
            }
            else if (toDate != null && fromDate != null && userId == 0 && timeFrom == null && timeTo == null)
            {
                receipts = await _context.Receipt
                    .Where(i => i.DateIn >= _fromDate && i.DateIn <= _toDate)
                    .ToListAsync();
                sales = await _context.SaleARs
                    .Where(i => i.PostingDate >= _fromDate && i.PostingDate <= _toDate)
                    .ToListAsync();
                receiptsCredit = await _context.ReceiptMemo
                    .Where(i => i.DateIn >= _fromDate && i.DateIn <= _toDate && i.UserOrderID == userId)
                    .ToListAsync();
                salesCredit = await _context.SaleCreditMemos
                    .Where(i => i.PostingDate >= _fromDate && i.PostingDate <= _toDate && i.UserID == userId)
                    .ToListAsync();
            }
            else if (toDate != null && fromDate != null && userId != 0 && timeFrom == null && timeTo == null)
            {
                receipts = await _context.Receipt
                    .Where(i => i.DateIn >= _fromDate && i.DateIn <= _toDate && i.UserOrderID == userId)
                    .ToListAsync();
                sales = await _context.SaleARs
                    .Where(i => i.PostingDate >= _fromDate && i.PostingDate <= _toDate && i.UserID == userId)
                    .ToListAsync();
                receiptsCredit = await _context.ReceiptMemo
                    .Where(i => i.DateIn >= _fromDate && i.DateIn <= _toDate && i.UserOrderID == userId)
                    .ToListAsync();
                salesCredit = await _context.SaleCreditMemos
                    .Where(i => i.PostingDate >= _fromDate && i.PostingDate <= _toDate && i.UserID == userId)
                    .ToListAsync();
            }
            else if (toDate != null && fromDate != null && userId == 0 && timeFrom != null && timeTo != null)
            {
                DateTime dateFrom = DateTime.Parse(string.Format("{0} {1}", fromDate, timeFrom));
                DateTime dateTo = DateTime.Parse(string.Format("{0} {1}", toDate, timeTo));
                receipts = await _context.Receipt
                    .Where(i => Convert.ToDateTime(i.DateIn.ToString("MM-dd-yyyy") + " " + i.TimeIn) >= dateFrom && Convert.ToDateTime(i.DateIn.ToString("MM-dd-yyyy") + " " + i.TimeIn) <= dateTo)
                    .ToListAsync();
                sales = await _context.SaleARs
                    .Where(i => i.PostingDate >= _fromDate && i.PostingDate <= _toDate)
                    .ToListAsync();
                receiptsCredit = await _context.ReceiptMemo
                    .Where(i => Convert.ToDateTime(i.DateIn.ToString("MM-dd-yyyy") + " " + i.TimeIn) >= dateFrom && Convert.ToDateTime(i.DateIn.ToString("MM-dd-yyyy") + " " + i.TimeIn) <= dateTo)
                    .ToListAsync();
                salesCredit = await _context.SaleCreditMemos
                    .Where(i => i.PostingDate >= _fromDate && i.PostingDate <= _toDate)
                    .ToListAsync();
            }
            else if (toDate != null && fromDate != null && userId > 0 && timeFrom != null && timeTo != null)
            {
                DateTime dateFrom = DateTime.Parse(string.Format("{0} {1}", fromDate, timeFrom));
                DateTime dateTo = DateTime.Parse(string.Format("{0} {1}", toDate, timeTo));
                receipts = await _context.Receipt
                    .Where(i => i.UserOrderID == userId && Convert.ToDateTime(i.DateIn.ToString("MM-dd-yyyy") + " " + i.TimeIn) >= dateFrom && Convert.ToDateTime(i.DateIn.ToString("MM-dd-yyyy") + " " + i.TimeIn) <= dateTo)
                    .ToListAsync();
                sales = await _context.SaleARs
                    .Where(i => i.UserID == userId && i.PostingDate >= _fromDate && i.PostingDate <= _toDate)
                    .ToListAsync();
                receiptsCredit = await _context.ReceiptMemo
                    .Where(i => i.UserOrderID == userId && Convert.ToDateTime(i.DateIn.ToString("MM-dd-yyyy") + " " + i.TimeIn) >= dateFrom && Convert.ToDateTime(i.DateIn.ToString("MM-dd-yyyy") + " " + i.TimeIn) <= dateTo)
                    .ToListAsync();
                salesCredit = await _context.SaleCreditMemos
                    .Where(i => i.UserID == userId && i.PostingDate >= _fromDate && i.PostingDate <= _toDate)
                    .ToListAsync();
            }
            #region receipts data
            var receiptsData = (from rp in receipts
                                join rpd in _context.ReceiptDetail on rp.ReceiptID equals rpd.ReceiptID
                                join series in _context.Series on rp.SeriesID equals series.ID
                                join audi in _context.InventoryAudits.Where(i => i.Qty < 0) on rp.SeriesDID equals audi.SeriesDetailID
                                join item in _context.ItemMasterDatas on audi.ItemID equals item.ID
                                join uom in _context.UnitofMeasures on audi.UomID equals uom.ID
                                group new { rp, rpd, item, series, uom, audi } by new { rp.ReceiptID, item.ID, audi.Cost, rpd.UnitPrice } into g
                                let data = g.DefaultIfEmpty().Last()
                                let qty = data.audi.Qty * -1
                                let cost = data.audi.Cost * data.rp.ExchangeRate
                                let unitprice = data.rpd.UnitPrice * data.rp.ExchangeRate
                                let total = (unitprice - cost) * qty
                                let totalCost = cost * qty
                                let totalPrice = unitprice * qty
                                let disValue = (data.rpd.DiscountRate + data.rp.DiscountRate) * total / 100
                                let totalAfterDis = totalPrice - disValue
                                let grossprofit = total - disValue
                                let grossprofitPercentage = grossprofit * 100 / totalPrice
                                select new SaleGrossProfitReport
                                {
                                    PostingDate = data.rp.DateIn.ToShortDateString(),
                                    DateOut = data.rp.DateOut.ToShortDateString(),
                                    Code = data.item.Code,
                                    Cost = (decimal)cost,
                                    InvoiceNo = $"{data.series.Name}-{data.rp.ReceiptNo}",
                                    ItemID = data.item.ID,
                                    ItemName = data.item.KhmerName,
                                    ItemCode = data.item.Code,
                                    ManItemBy = data.item.ManItemBy,
                                    Price = (decimal)unitprice,
                                    Qty = (decimal)qty,
                                    Total = (decimal)total,
                                    TotalGrossProfit = 0,
                                    TotalItem = 0,
                                    UoMName = data.uom.Name,
                                    Discount = (decimal)disValue,
                                    GrossProfit = (decimal)grossprofit,
                                    TotalPrice = (decimal)totalPrice,
                                    TotalCost = (decimal)totalCost,
                                    DisRateM = data.rp.DiscountRate,
                                    DisRate = data.rpd.DiscountRate,
                                    TotalAfterDis = (decimal)totalAfterDis == 0 ? (decimal)totalPrice : (decimal)totalAfterDis,
                                    GrossProfitP = (decimal)grossprofitPercentage,
                                    ABID = data.audi.ID,
                                    TimeIn = data.rp.TimeIn,
                                    TimeOut = data.rp.TimeOut,
                                }).GroupBy(i => i.ABID).Select(i => i.FirstOrDefault()).ToList();
            #endregion

            #region receipts data credit
            var receiptsDataCredit = (from rp in receiptsCredit
                                      join rpd in _context.ReceiptDetailMemoKvms on rp.ID equals rpd.ReceiptMemoID
                                      join series in _context.Series on rp.SeriesID equals series.ID
                                      join audi in _context.InventoryAudits.Where(i => i.Qty > 0) on rp.SeriesDID equals audi.SeriesDetailID
                                      join item in _context.ItemMasterDatas on audi.ItemID equals item.ID

                                      join uom in _context.UnitofMeasures on audi.UomID equals uom.ID
                                      group new { rp, rpd, item, series, uom, audi } by new { RPID = rp.ID, item.ID, audi.Cost, rpd.UnitPrice } into g
                                      let data = g.DefaultIfEmpty().Last()
                                      let qty = data.audi.Qty
                                      let cost = data.audi.Cost * data.rp.ExchangeRate
                                      let unitprice = data.rpd.UnitPrice * data.rp.ExchangeRate
                                      let total = (unitprice - cost) * qty
                                      let totalCost = cost * qty
                                      let totalPrice = unitprice * qty
                                      let disValue = (data.rpd.DisRate + data.rp.DisRate) * total / 100
                                      let totalAfterDis = totalPrice - disValue
                                      let grossprofit = total - disValue
                                      let grossprofitPercentage = grossprofit * 100 / totalPrice
                                      select new SaleGrossProfitReport
                                      {
                                          PostingDate = data.rp.DateIn.ToShortDateString(),
                                          DateOut = data.rp.DateOut.ToShortDateString(),
                                          Code = data.item.Code,
                                          Cost = (decimal)cost * -1,
                                          InvoiceNo = $"{data.series.Name}-{data.rp.ReceiptNo}",
                                          ItemID = data.item.ID,
                                          ItemName = data.item.KhmerName,
                                          ItemCode = data.item.Code,
                                          ManItemBy = data.item.ManItemBy,
                                          Price = (decimal)unitprice * -1,
                                          Qty = (decimal)qty,
                                          Total = (decimal)total * -1,
                                          TotalGrossProfit = 0,
                                          TotalItem = 0,
                                          UoMName = data.uom.Name,
                                          Discount = (decimal)disValue * -1,
                                          GrossProfit = (decimal)grossprofit * -1,
                                          TotalPrice = (decimal)totalPrice * -1,
                                          TotalCost = (decimal)totalCost * -1,
                                          DisRateM = data.rp.DisRate * -1,
                                          DisRate = data.rpd.DisRate * -1,
                                          TotalAfterDis = (decimal)totalAfterDis == 0 ? (decimal)totalPrice * -1 : (decimal)totalAfterDis * -1,
                                          GrossProfitP = (decimal)grossprofitPercentage * -1,
                                          ABID = data.audi.ID,
                                          TimeIn = data.rp.TimeIn,
                                          TimeOut = data.rp.TimeOut,
                                      }).GroupBy(i => i.ABID).Select(i => i.FirstOrDefault()).ToList();
            #endregion

            #region sale data
            var saleData = (from ar in sales
                            join ard in _context.SaleARDetails on ar.SARID equals ard.SARID
                            join series in _context.Series on ar.SeriesID equals series.ID
                            join audi in _context.InventoryAudits.Where(i => i.Qty < 0) on ar.SeriesDID equals audi.SeriesDetailID
                            join item in _context.ItemMasterDatas on audi.ItemID equals item.ID
                            join uom in _context.UnitofMeasures on audi.UomID equals uom.ID
                            group new { ar, ard, item, series, uom, audi } by new { ar.SARID, item.ID, audi.Cost, ard.UnitPrice } into g
                            let data = g.DefaultIfEmpty().Last()
                            let qty = data.audi.Qty * -1
                            let cost = data.audi.Cost * data.ar.ExchangeRate
                            let unitprice = data.ard.UnitPrice * data.ar.ExchangeRate
                            let total = (unitprice - cost) * qty
                            let totalCost = cost * qty
                            let totalPrice = unitprice * qty
                            let disValue = (data.ard.DisRate + data.ar.DisRate) * total / 100
                            let grossprofit = total - disValue
                            let totalAfterDis = totalPrice - disValue
                            let grossprofitPercentage = grossprofit * 100 / totalPrice
                            select new SaleGrossProfitReport
                            {
                                Code = data.item.Code,
                                Cost = (decimal)cost,
                                InvoiceNo = $"{data.series.Name}-{data.ar.InvoiceNumber}",
                                ItemID = data.item.ID,
                                ItemName = data.item.KhmerName,
                                ItemCode = data.item.Code,
                                ManItemBy = data.item.ManItemBy,
                                PostingDate = data.ar.PostingDate.ToShortDateString(),
                                Price = (decimal)unitprice,
                                Qty = (decimal)qty,
                                Total = (decimal)total,
                                TotalGrossProfit = 0,
                                TotalItem = 0,
                                UoMName = data.uom.Name,
                                Discount = (decimal)disValue,
                                GrossProfit = (decimal)grossprofit,
                                TotalAfterDis = (decimal)totalAfterDis == 0 ? (decimal)totalPrice : (decimal)totalAfterDis,
                                TotalCost = (decimal)totalCost,
                                TotalPrice = (decimal)totalPrice,
                                DisRateM = data.ar.DisRate,
                                DisRate = data.ard.DisRate,
                                GrossProfitP = (decimal)grossprofitPercentage,
                                ABID = data.audi.ID,
                            }).GroupBy(i => i.ABID).Select(i => i.FirstOrDefault()).ToList();
            #endregion

            #region sale data credit
            var saleDataCredit = (from ar in salesCredit
                                  join ard in _context.SaleCreditMemoDetails on ar.SCMOID equals ard.SCMOID
                                  join series in _context.Series on ar.SeriesID equals series.ID
                                  join audi in _context.InventoryAudits.Where(i => i.Qty > 0) on ar.SeriesDID equals audi.SeriesDetailID
                                  join item in _context.ItemMasterDatas on audi.ItemID equals item.ID
                                  join uom in _context.UnitofMeasures on audi.UomID equals uom.ID
                                  group new { ar, ard, item, series, uom, audi } by new { ar.SCMOID, item.ID, audi.Cost, ard.UnitPrice } into g
                                  let data = g.DefaultIfEmpty().Last()
                                  let qty = data.audi.Qty
                                  let cost = data.audi.Cost * data.ar.ExchangeRate
                                  let unitprice = data.ard.UnitPrice * data.ar.ExchangeRate
                                  let total = (unitprice - cost) * qty
                                  let totalCost = cost * qty
                                  let totalPrice = unitprice * qty
                                  let disValue = (data.ard.DisRate + data.ar.DisRate) * total / 100
                                  let grossprofit = total - disValue
                                  let totalAfterDis = totalPrice - disValue
                                  let grossprofitPercentage = grossprofit * 100 / totalPrice
                                  select new SaleGrossProfitReport
                                  {
                                      Code = data.item.Code,
                                      Cost = (decimal)cost * -1,
                                      InvoiceNo = $"{data.series.Name}-{data.ar.InvoiceNumber}",
                                      ItemID = data.item.ID,
                                      ItemName = data.item.KhmerName,
                                      ItemCode = data.item.Code,
                                      ManItemBy = data.item.ManItemBy,
                                      PostingDate = data.ar.PostingDate.ToShortDateString(),
                                      Price = (decimal)unitprice * -1,
                                      Qty = (decimal)qty,
                                      Total = (decimal)total,
                                      TotalGrossProfit = 0,
                                      TotalItem = 0,
                                      UoMName = data.uom.Name,
                                      Discount = (decimal)disValue * -1,
                                      GrossProfit = (decimal)grossprofit * -1,
                                      TotalAfterDis = (decimal)totalAfterDis == 0 ? (decimal)totalPrice * -1 : (decimal)totalAfterDis * -1,
                                      TotalCost = (decimal)totalCost * -1,
                                      TotalPrice = (decimal)totalPrice * -1,
                                      DisRateM = data.ar.DisRate * -1,
                                      DisRate = data.ard.DisRate * -1,
                                      GrossProfitP = (decimal)grossprofitPercentage * -1,
                                      ABID = data.audi.ID,
                                  }).GroupBy(i => i.ABID).Select(i => i.FirstOrDefault()).ToList();
            #endregion

            #region
            List<SaleGrossProfitReport> all = new(receiptsData.Count + saleData.Count + receiptsDataCredit.Count + saleDataCredit.Count);
            all.AddRange(receiptsData);
            all.AddRange(saleData);
            all.AddRange(receiptsDataCredit);
            all.AddRange(saleDataCredit);
            var sumGroupData = (from _all in all
                                group _all by _all.ItemID into g
                                select new SaleGrossProfitReport
                                {
                                    ItemID = g.Key,
                                    TotalItem = g.Sum(i => i.Total),
                                    TotalItemCost = g.Sum(i => i.TotalCost),
                                    TotalItemPrice = g.Sum(i => i.TotalPrice),
                                    GrossProfitItem = g.Sum(i => i.GrossProfit),
                                    TotalAfterDisItem = g.Sum(i => i.TotalAfterDis),
                                    TotalDiscountItem = g.Sum(i => i.Discount),
                                    GrossProfitItemP = g.Sum(i => i.TotalPrice) == 0 ? 0 : g.Sum(i => i.GrossProfit) * 100 / g.Sum(i => i.TotalPrice),
                                }).ToList();
            foreach (var i in all)
            {
                foreach (var j in sumGroupData)
                {
                    if (i.ItemID == j.ItemID)
                    {
                        i.TotalItem = j.TotalItem;
                        i.GrossProfitItem = j.GrossProfitItem;
                        i.GrossProfitItemP = j.GrossProfitItemP;
                        i.TotalItemCost = j.TotalItemCost;
                        i.TotalItemPrice = j.TotalItemPrice;
                        i.TotalAfterDisItem = j.TotalAfterDisItem;
                        i.TotalDiscountItem = j.TotalDiscountItem;
                    }
                }
            }
            decimal totalAfterDisAllF = all.Sum(i => i.TotalAfterDis);
            decimal totalGrossProfit = all.Sum(i => i.GrossProfit);
            decimal totalDiscount = all.Sum(i => i.Discount);
            decimal totalAllCost = all.Sum(i => i.Cost * i.Qty);
            decimal totalAllPrice = all.Sum(i => i.Price * i.Qty);
            decimal totalGrossProfitP = all.Sum(i => i.TotalPrice) == 0 ? 0 : all.Sum(i => i.GrossProfit) * 100 / all.Sum(i => i.TotalPrice);
            var _data = (from _all in all
                         group _all by new { _all.InvoiceNo, _all.ItemID, _all.Cost, _all.Price } into g
                         let _g = g.FirstOrDefault()
                         select new SaleGrossProfitReport
                         {
                             ItemID = _g.ItemID,
                             InvoiceNo = _g.InvoiceNo,
                             Code = _g.Code,
                             CostF = $"{cur.Description} {_format.ToCurrency(_g.Cost, disformat.Amounts)}",
                             QtyF = _format.ToCurrency(g.Sum(u => u.Qty), disformat.Amounts),
                             TotalF = $"{cur.Description} {_format.ToCurrency(_g.Total, disformat.Amounts)}",
                             TotalItemF = $"{cur.Description} {_format.ToCurrency(g.Sum(i => i.TotalItem), disformat.Amounts)}",
                             TotalGrossProfitF = $"{cur.Description} {_format.ToCurrency(totalGrossProfit, disformat.Amounts)}",
                             PriceF = $"{cur.Description} {_format.ToCurrency(_g.Price, disformat.Amounts)}",
                             TotalItemCostF = $"{cur.Description} {_format.ToCurrency(_g.TotalItemCost, disformat.Amounts)}",
                             TotalItemPriceF = $"{cur.Description} {_format.ToCurrency(_g.TotalItemPrice, disformat.Amounts)}",
                             TotalCostF = $"{cur.Description} {_format.ToCurrency(_g.TotalCost, disformat.Amounts)}",
                             TotalPriceF = $"{cur.Description} {_format.ToCurrency(_g.TotalPrice, disformat.Amounts)}",
                             DiscountF = $"{cur.Description} {_format.ToCurrency(_g.Discount, disformat.Amounts)}",
                             GrossProfitF = $"{cur.Description} {_format.ToCurrency(_g.GrossProfit, disformat.Amounts)}",
                             TotalAfterDisF = $"{cur.Description} {_format.ToCurrency(_g.TotalAfterDis, disformat.Amounts)}",
                             TotalDiscountItemF = $"{cur.Description} {_format.ToCurrency(_g.TotalDiscountItem, disformat.Amounts)}",
                             TotalAfterDisItemF = $"{cur.Description} {_format.ToCurrency(_g.TotalAfterDisItem, disformat.Amounts)}",
                             TotalAfterDisAllF = $"{cur.Description} {_format.ToCurrency(totalAfterDisAllF, disformat.Amounts)}",
                             GrossProfitItemF = $"{cur.Description} {_format.ToCurrency(_g.GrossProfitItem, disformat.Amounts)}",
                             TotalDiscountF = $"{cur.Description} {_format.ToCurrency(totalDiscount, disformat.Amounts)}",
                             TotalAllCostF = $"{cur.Description} {_format.ToCurrency(totalAllCost, disformat.Amounts)}",
                             TotalAllPriceF = $"{cur.Description} {_format.ToCurrency(totalAllPrice, disformat.Amounts)}",
                             TotalGrossProfitPF = $"% {_format.ToCurrency(totalGrossProfitP, disformat.Amounts)}",
                             GrossProfitPF = $"% {_format.ToCurrency(_g.GrossProfitP, disformat.Amounts)}",
                             GrossProfitItemPF = $"% {_format.ToCurrency(_g.GrossProfitItemP, disformat.Amounts)}",

                             UoMName = _g.UoMName,
                             ItemName = _g.ItemName,
                             PostingDate = _g.PostingDate,
                             DateOut = _g.DateOut,
                             TimeIn = _g.TimeIn,
                             TimeOut = _g.TimeOut,
                         }).ToList();
            #endregion
            return await Task.FromResult(_data);
        }

        public List<TransactionJournalReport> GetTransactionJournalReport(string valuejournal, string fromDate, string toDate, Company company)
        {
            _ = DateTime.TryParse(toDate, out DateTime _toDate);
            _ = DateTime.TryParse(fromDate, out DateTime _fromDate);

            var currName = _context.Currency.Find(company.SystemCurrencyID);
            var displayCurr = _context.DisplayCurrencies.FirstOrDefault(c => c.AltCurrencyID == currName.ID) ?? new DisplayCurrency();
            var disformat = _context.Displays.FirstOrDefault(c => c.DisplayCurrencyID == displayCurr.AltCurrencyID) ?? new Display();
            var data = (from doc in _context.DocumentTypes.Where(i => i.Code == valuejournal)
                        join ab in _context.AccountBalances.Where(i => i.PostingDate >= _fromDate && i.PostingDate <= _toDate) on doc.ID equals ab.Origin
                        join gla in _context.GLAccounts.Where(i => i.CompanyID == company.ID) on ab.GLAID equals gla.ID
                        join je in _context.JournalEntries on ab.JEID equals je.ID
                        join series in _context.Series on doc.ID equals series.DocuTypeID
                        let usera = _context.UserAccounts.FirstOrDefault(i => ab.Creator == i.ID) ?? new Account.UserAccount()
                        let emp = _context.Employees.FirstOrDefault(i => i.ID == usera.EmployeeID) ?? new Employee()
                        let bp = _context.BusinessPartners.FirstOrDefault(i => ab.BPAcctID == i.ID) ?? new BusinessPartner()
                        let empName = emp.Name ?? ""
                        select new TransactionJournalReport
                        {
                            ID = ab.JEID,
                            Date = ab.PostingDate.ToString("MM/dd/yyyy"),
                            Series = series.Name,
                            Number = ab.OriginNo,
                            Type = doc.Code,
                            MasterRemarks = string.IsNullOrWhiteSpace(je.Remarks) ? "" : je.Remarks,
                            Remarks = ab.Remarks,
                            // Remarks = string.IsNullOrWhiteSpace(jed.Remarks) ? "" : jed.Remarks,
                            Trans = ab.OriginNo,
                            Creator = $"{usera.Username} - {empName}",
                            AccountBPCode = gla.Code,
                            AccountBPName = ab.BPAcctID > 0 ? bp.Name : gla.Name,
                            Debit = ab.Debit,
                            Credit = ab.Credit,
                            ABID = ab.ID,
                            SeriesID = series.ID,
                            GlID = gla.ID,
                            Efective = (int)ab.Effective
                        }).GroupBy(i => i.ABID).Select(i => i.LastOrDefault()).ToList();
            var subGroup = (from d in data
                            group d by new { d.ID, } into g
                            let dt = g.FirstOrDefault()
                            select new TransactionJournalReport
                            {
                                ABID = dt.ABID,
                                ID = dt.ID,

                                MasterRemarks = dt.MasterRemarks,
                                Number = dt.Number,
                                SeriesID = dt.SeriesID,
                                TotalGroupCredit = currName.Description + " " + _format.ToCurrency(g.Sum(s => s.Credit), disformat.Amounts),
                                TotalGroupDebit = currName.Description + " " + _format.ToCurrency(g.Sum(s => s.Debit), disformat.Amounts),
                            }).ToList();
            foreach (var i in data)
            {
                var subgroup = subGroup.FirstOrDefault(sb => sb.ID == i.ID && i.SeriesID == sb.SeriesID && sb.Number == i.Number);
                if (subgroup != null)
                {
                    i.TotalGroupCredit = subgroup.TotalGroupCredit;
                    i.TotalGroupDebit = subgroup.TotalGroupDebit;
                }
                i.TotalDebit = currName.Description + " " + _format.ToCurrency(data.Sum(s => s.Debit), disformat.Amounts);
                i.TotalCrebit = currName.Description + " " + _format.ToCurrency(data.Sum(s => s.Credit), disformat.Amounts);
                if (i.Efective == 1)
                {
                    i.DebitF = currName.Description + " " + _format.ToCurrency(i.Debit, disformat.Amounts);
                    i.CreditF = "";
                }
                else
                {
                    i.CreditF = currName.Description + " " + _format.ToCurrency(i.Credit, disformat.Amounts);
                    i.DebitF = "";
                }


                // i.TotalCrebit += Convert.ToDecimal(i.Credit);
            };

            return data.ToList();
        }
        public List<GlSaleARViews> GetGeneralLedgerReports(string dateFrom, string dateTo, Company company)
        {
            //_ = DateTime.TryParse(postingdate, out DateTime _postDate);
            _ = DateTime.TryParse(dateFrom, out DateTime _FromDate);
            _ = DateTime.TryParse(dateTo, out DateTime _ToDate);
            List<JournalEntryDetail> journalEntryDetails = new();
            List<JournalEntry> journalEntries = new();
            var sysCur = _context.Currency.Find(company.SystemCurrencyID) ?? new Currency();
            var displayCurr = _context.DisplayCurrencies.FirstOrDefault(c => c.AltCurrencyID == sysCur.ID) ?? new DisplayCurrency();
            var disformat = _context.Displays.FirstOrDefault(c => c.DisplayCurrencyID == displayCurr.AltCurrencyID) ?? new Display();
            var disCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == company.SystemCurrencyID) ?? new Display();
            if (dateFrom == null || dateTo == null)
            {
                journalEntries = _context.JournalEntries.ToList();
            }
            else
            {
                journalEntryDetails = _context.JournalEntryDetails.ToList();
                journalEntries = _context.JournalEntries.Where(x => x.PostingDate >= _FromDate && x.PostingDate <= _ToDate).ToList();
            }
            var genData = (from je in journalEntries
                           join jed in journalEntryDetails on je.ID equals jed.JEID
                           join ser in _context.Series on je.SeriesID equals ser.ID
                           join dc in _context.DocumentTypes on je.DouTypeID equals dc.ID
                           join user in _context.UserAccounts.Include(u => u.Employee) on je.Creator equals user.ID
                           let lgacc = _context.GLAccounts.FirstOrDefault(i => i.ID == jed.ItemID) ?? new GLAccount()
                           let cumulativeBalance = journalEntryDetails.Where(i => i.ID <= jed.ID && jed.ItemID == i.ItemID).Sum(i => i.Debit - i.Credit)
                           let empName = user.Employee != null ? user.Employee.Name : ""
                           select new GlSaleARViews
                           {
                               PostingDate = je.PostingDate.ToShortDateString(),
                               DocumentDate = je.DocumentDate.ToShortDateString(),
                               SeriesName = ser.Name,
                               DocType = dc.Code,
                               Receiptno = je.Number,
                               DebitCredit = jed.Credit <= 0 ? $"{sysCur.Description} {_format.ToCurrency(jed.Debit, disCur.Amounts)}" : jed.Debit <= 0 ? $"({sysCur.Description} {_format.ToCurrency(jed.Credit, disCur.Amounts)})" : "",
                               TransNo = je.TransNo,
                               Cumolativebalance = $"{sysCur.Description} {_format.ToCurrency(cumulativeBalance, disCur.Amounts)}",
                               AccountName = $"{lgacc.Code}-{lgacc.Name}",
                               FromDate = _FromDate.ToString("dd-MMMM-yyyy"),
                               Todate = _ToDate.ToString("dd-MMMM-yyyy"),
                               Creator = $"{user.Username} - {empName}"
                           }).ToList();
            return genData;
        }
    }
    public static class DateTimeExtensions
    {
        public static int GetQuarter(this DateTime date)
        {
            return (date.Month + 2) / 3;
        }

        public static int GetFinancialQuarter(this DateTime date)
        {
            return (date.AddMonths(-3).Month + 2) / 3;
        }
    }
}