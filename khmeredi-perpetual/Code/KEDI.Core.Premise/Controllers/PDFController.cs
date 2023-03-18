using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.ServicesClass.FinancailReportPrint;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Collections.Generic;
using System.Linq;
using KEDI.Core.Premise.Authorization;
using System.Security.Claims;
using CKBS.Models.Services.Account;
using Newtonsoft.Json;

namespace CKBS.Controllers
{
    [Route("pdf")]
    [Privilege]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PDFController : Controller
    {
        private readonly DataContext _context;
        private readonly IFinancialReports _finan;

        public PDFController(DataContext context, IFinancialReports financialReports)
        {
            _context = context;
            _finan = financialReports;
        }

        private int GetUserID()
        {
            _ = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int _id);
            return _id;
        }

        private Company GetCompany()
        {
            var com = (from us in _context.UserAccounts.Where(w => w.ID == GetUserID())
                       join co in _context.Company on us.CompanyID equals co.ID
                       select co
                       ).FirstOrDefault();
            return com;
        }

        //********** Balance Sheet *************//

        [Route("printbalancesheet")]
        public IActionResult PrintBalanceSheet(string date, int typeDisplay, bool showZeroAcc)
        {
            var company = _context.Company.Find(GetCompany().ID);
            var currencyName = _context.Currency.Find(GetCompany().SystemCurrencyID);
            var accbal = _finan.GetBalanceSheet(date, GetCompany().ID, GetCompany().SystemCurrencyID, typeDisplay, showZeroAcc);
        
            var asset = _finan.GetGategories("100000000000000", GetCompany().SystemCurrencyID);
            var liab = _finan.GetGategories("300000000000000", GetCompany().SystemCurrencyID);
            var cap = _finan.GetGategories("200000000000000", GetCompany().SystemCurrencyID);
            var model = new PrintBalance
            {
                Date = date,
                Model = accbal,
                Asset = asset,
                Liability = liab,
                CapReseve = cap,
                CompanyName = company.Name,
                CurrencyName = currencyName.Description,
            };
            return View(model);
        }

        //********** Profit And Loss *************//
        [Route("printprofitandloss")]
        public IActionResult PrintProfitAndLoss(string fromDate, string toDate, int typeDisplay, bool showZeroAcc,string branchs,string user)
        {
             List<MultiBrand> branch = JsonConvert.DeserializeObject<List<MultiBrand>>(branchs, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var company = _context.Company.Find(GetCompany().ID);
            var currencyName = _context.Currency.Find(GetCompany().SystemCurrencyID);
            var accbal = _finan.GetGLAccountsPL(fromDate, toDate, GetCompany().ID, GetCompany().SystemCurrencyID, typeDisplay, showZeroAcc, branch);
            var turnover = _finan.GetGategoriesPL("400000000000000", GetCompany().SystemCurrencyID);
            var costOfSales = _finan.GetGategoriesPL("500000000000000", GetCompany().SystemCurrencyID);
            var operatingCosts = _finan.GetGategoriesPL("600000000000000", GetCompany().SystemCurrencyID);
            var nonOperatingIncomeExpenditure = _finan.GetGategoriesPL("700000000000000", GetCompany().SystemCurrencyID);
            var taxationExtraordinaryItems = _finan.GetGategoriesPL("800000000000000", GetCompany().SystemCurrencyID);
            var model = new PrintProfitAddLoss
            {
                DateFrom = fromDate,
                DateTo = toDate,
                Model = accbal,
                Turnover = turnover,
                CostOfSales = costOfSales,
                OperatingCosts = operatingCosts,
                NonOperatingIncomeExpenditure = nonOperatingIncomeExpenditure,
                TaxationExtraordinaryItems = taxationExtraordinaryItems,
                CompanyName = company.Name,
                CurrencyName = currencyName.Description,
                 UserName   = user,
            };
            return View(model);
        }

        ///*************** Trial Balance ****************///
        [Route("printtrialbalance")]
        public IActionResult PrintTrialBalance(
            string VType, string CType,
            int fromId, int toId,
            string fromDate, string toDate,
            int displayType, bool showZeroAcc, bool showGla)
        {
            var company = _context.Company.Find(GetCompany().ID);
            var currencyName = _context.Currency.Find(GetCompany().SystemCurrencyID);
            var fromCode = _context.BusinessPartners.Find(fromId);
            var toCode = _context.BusinessPartners.Find(toId);
            var BPs = _finan.GetBusPartners(
                VType, CType, fromId, toId,
                fromDate, toDate, GetCompany().SystemCurrencyID,GetCompany().ID,
                displayType, showZeroAcc, showGla);
            var model = new PrintTrialBalance
            {
                CompanyName = company.Name,
                CType = CType,
                CurrencyName = currencyName.Description,
                DateFrom = fromDate,
                DateTo = toDate,
                FromID = fromCode != null ? fromCode.Code : "",
                ToID = toCode != null ? toCode.Code : "",
                Model = BPs,
                VType = VType,
            };
            return View(model);
        }
    }
}
